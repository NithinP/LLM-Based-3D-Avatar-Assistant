import torch
from parler_tts import ParlerTTSForConditionalGeneration
from transformers import AutoTokenizer
import soundfile as sf
from datetime import datetime

class TTSGenerator:
    def __init__(self):

        self.device = "cuda:0" if torch.cuda.is_available() else "cpu"
        self.model = ParlerTTSForConditionalGeneration.from_pretrained("parler-tts-mini-multilingual").to(self.device)
        self.tokenizer = AutoTokenizer.from_pretrained("parler-tts-mini-multilingual")
        self.default_description = "Jenna's voice is expressive yet slightly moderate in delivery, with a very close recording that almost has no background noise"
        
    def generate_audio(self, prompt, output_filename=None):
        input_ids = self.tokenizer(self.default_description, return_tensors="pt").input_ids.to(self.device)
        prompt_input_ids = self.tokenizer(prompt, return_tensors="pt").input_ids.to(self.device)
        generation = self.model.generate(input_ids=input_ids, prompt_input_ids=prompt_input_ids)
        audio_arr = generation.cpu().numpy().squeeze()
        if output_filename is None:
            timestamp = datetime.now().strftime("%Y%m%d_%H%M%S")
            output_filename = f"parler_tts_{timestamp}.wav"

        sf.write(output_filename, audio_arr, self.model.config.sampling_rate)
        return output_filename

def main():
    tts = TTSGenerator()
    
    while True:
        print("\nEnter your prompt (or 'quit' to exit):")
        prompt = input().strip()
        
        if prompt.lower() == 'quit':
            break
            
        if not prompt:
            print("Prompt cannot be empty. Please try again.")
            continue
            
        try:
            output_file = tts.generate_audio(prompt)
            print(f"Audio generated successfully: {output_file}")
        except Exception as e:
            print(f"Error generating audio: {str(e)}")

if __name__ == "__main__":
    main()