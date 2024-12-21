import asyncio
import websockets
import speech_recognition as sr
import whisper
import os
from subprocess import run, PIPE

def check_ffmpeg():
    """Check if FFmpeg is installed and accessible"""
    try:
        run(['ffmpeg', '-version'], stdout=PIPE, stderr=PIPE)
        return True
    except FileNotFoundError:
        return False

async def receive_audio_stream(websocket):
    global model
    print("Unity connected")
    
    # Check FFmpeg installation first
    if not check_ffmpeg():
        print("Error: FFmpeg not found. Please install FFmpeg and add it to your system PATH")
        return
    
    try:
        while True:
            data = await websocket.recv()
            if not data:
                break
                
            # Ensure the audio file path is absolute
            audio_file_path = os.path.abspath("received_audio.wav")
            
            try:
                # Write received audio data
                with open(audio_file_path, "wb") as wav_file:
                    wav_file.write(data if isinstance(data, bytes) else data.encode())
                
                # Verify file exists before processing
                if not os.path.exists(audio_file_path):
                    print(f"Error: Audio file not created at {audio_file_path}")
                    continue
                    
                # Load and process audio
                audio = whisper.load_audio(audio_file_path)
                audio = whisper.pad_or_trim(audio)
                mel = whisper.log_mel_spectrogram(audio).to(model.device)
                _, probs = model.detect_language(mel)
                detected_language = max(probs, key=probs.get)
                print(f"Detected language: {detected_language}")
                
                # Process speech
                await recognize_speech(audio_file_path, websocket, detected_language)
                
            except Exception as e:
                print(f"Error processing audio: {str(e)}")
                
            finally:
                # Clean up file if it exists
                if os.path.exists(audio_file_path):
                    try:
                        os.remove(audio_file_path)
                    except Exception as e:
                        print(f"Error removing audio file: {str(e)}")
                        
    except websockets.exceptions.ConnectionClosedOK:
        print("WebSocket connection closed by the client")
    except Exception as e:
        print(f"Unexpected error in receive_audio_stream: {str(e)}")

async def recognize_speech(file_path, websocket, detected_language):
    recognizer = sr.Recognizer()
    with sr.AudioFile(file_path) as source:
        audio_data = recognizer.record(source)
        try:
            if detected_language == 'en':
                text = recognizer.recognize_google(audio_data)
            elif detected_language == 'ja':
                text = recognizer.recognize_google(audio_data, language="ja-JP")
            elif detected_language == 'zh':
                text = recognizer.recognize_google(audio_data, language="zh-CN")
            elif detected_language == 'es':
                text = recognizer.recognize_google(audio_data, language="es-ES")
            elif detected_language == 'ru':
                text = recognizer.recognize_google(audio_data,language = "ru-RU")
            else:
                text = recognizer.recognize_google(audio_data)
            print("Recognized text:", text)
            text_without_apostrophe = text.replace("'", "")
            text_with_lang = f"{text_without_apostrophe} [{detected_language}]"
            await websocket.send(text_with_lang)

        except sr.UnknownValueError:
            error_message = "please say again"
            print(error_message)
            await websocket.send(error_message)
        except sr.RequestError as e:
            print("Error during speech recognition:", e)

async def main():
    global model
    model = whisper.load_model("base")
    server = await websockets.serve(receive_audio_stream, '0.0.0.0', 9002)
    print("WebSocket server started on port 9002")
    
    await server.wait_closed()

if __name__ == "__main__":
    asyncio.run(main())
