using System;
using System.Threading;
using WebSocketSharp;
using System.Threading.Tasks;
using System.Collections;
using UnityEngine;
using UnityEngine.Audio;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using UnityEngine.UI;

public class AutoDetectVoice : MonoBehaviour
{
    public string receivedText = "";
    public string language = "";
    public string processedText = "";

    protected bool can_record = false;
    protected bool microphoneActive = true;
    protected bool _autoDetectVoice = true;
    protected bool _isSetup = false;

    private bool _currentlyRecording = false;
    private bool _requestNeedsSending = false;
    private AudioClip _audioRecording = null;
    private bool _audioDetected = false;
    private bool _isWaitingForNextInput = false;
    WsClient socket;


    private bool _currentlyActive = false;
    private readonly float _amplitudeThreshold = 0.6f;

    [Header("Voice Input Settings")]
    [SerializeField, Tooltip("Time in seconds of detected silence before voice request is sent")]
    protected float _silenceTimer = 1.0f;

    [SerializeField, Tooltip("The minimum volume to detect voice input for"), Range(0.0f, 1.0f)]
    protected float _minimumSpeakingSampleValue = 0.5f;

    [Header("Audio Source")]
    [SerializeField] private AudioSource audioSource;
    public AudioMixerGroup _mixerGroupMicrophone;

    private float[] _samples = new float[512];
    public static float[] _freqBand = new float[8];
    public static float[] _bandBuffer = new float[8];
    public static float[] _bufferDecrease = new float[8];
    float[] _freqBandHighest = new float[8];
    public static float[] _audioBand = new float[8];
    public static float[] _audioBandBuffer = new float[8];
    private byte[] bytes;

    private bool _inRecognitionPhase = false;

    public static float _Amplitude, _AmplitudeBuffer;
    public float _AmplitudeHighest;
    public float _audioProfile;

    private int _micPrevPos = 0;
    private float _timeAtSilenceBegan = 0.0f;

    byte[] wavdata;
    public byte[] WavData
    {
        get { return wavdata; }
    }

    [SerializeField] private GameObject reactionImage;
    [SerializeField] private GameObject _KUBES;
    [SerializeField] private GameObject _RECOG;
    Integrity_Loader IL;

    private float _sustainedTime = 0f;
    private float _requiredSustainTime =1f;
    private float _resetTime = 0.05f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        socket = FindObjectOfType<WsClient>();
        IL = FindObjectOfType<Integrity_Loader>();
        AudioProfile(_audioProfile);
        can_record = true;
        if (can_record)
        {
            SetupService(_autoDetectVoice);
        }
        reactionImage.SetActive(false);
        _KUBES.SetActive(false);
    }

    void Update()
    {
        if (can_record && !_isSetup)
        {
            Debug.Log("Starting Service");
            SetupService(_autoDetectVoice);
        }

        if (_autoDetectVoice && microphoneActive)
        {
            GetSpectrumAudioSource();
            makefrequencybands();
            createAudioBands();
            BandBuffer();
            DetectAudio();
            GetAmplitude();
        }
        if (_AmplitudeBuffer > _amplitudeThreshold && IL != null && IL.End_Flag)
        {
            _sustainedTime += Time.deltaTime;
            if (_sustainedTime >= _requiredSustainTime && !_currentlyActive)
            {
                _currentlyActive = true;
                reactionImage.SetActive(true);
                _KUBES.SetActive(true);
            }
        }
        else
        {
            _sustainedTime -= Time.deltaTime;
            if (_sustainedTime <= 0 && _currentlyActive && IL != null && IL.End_Flag)
            {
                _currentlyActive = false;
                if (!_inRecognitionPhase)
                {
                    reactionImage.SetActive(false);
                    _KUBES.SetActive(false);
                }
            }
        }
        _sustainedTime = Mathf.Clamp(_sustainedTime, 0f, _requiredSustainTime + 0.1f);
    }

    #region Spectrum Analyzer
    void GetSpectrumAudioSource()
    {
        audioSource.GetSpectrumData(512, 0, FFTWindow.Blackman);
    }
    void makefrequencybands()
    {
        int count = 0;

        for (int i = 0; i < 8; i++)
        {
            float average = 0;
            int samplecount = (int)Mathf.Pow(2, i) * 2;

            if (i == 7)
            {
                samplecount += 2;
            }
            for (int j = 0; j < samplecount; j++)
            {
                average += _samples[count] * (count + 1);
                count++;
            }

            average /= count;
            _freqBand[i] = average * 10;
        }
    }

    void BandBuffer()
    {
        for (int g = 0; g < 8; ++g)
        {
            if (_freqBand[g] > _bandBuffer[g])
            {
                _bandBuffer[g] = _freqBand[g];
                _bufferDecrease[g] = 0.005f;
            }
            if (_freqBand[g] < _bandBuffer[g])
            {
                _bandBuffer[g] -= _bufferDecrease[g];
                _bufferDecrease[g] *= 1.2f;
            }
        }
    }

    void createAudioBands()
    {
        for (int i = 0; i < 8; i++)
        {
            if (_freqBand[i] > _freqBandHighest[i])
            {
                _freqBandHighest[i] = _freqBand[i];
            }
            _audioBand[i] = (_freqBand[i] / _freqBandHighest[i]);
            _audioBandBuffer[i] = (_bandBuffer[i] / _freqBandHighest[i]);
        }
    }

    void GetAmplitude()
    {
        float _CurrentAmplitude = 0;
        float _CurrentAmplitudeBuffer = 0;
        for (int i = 0; i < 8; i++)
        {
            _CurrentAmplitude += _audioBand[i];
            _CurrentAmplitudeBuffer += _audioBandBuffer[i];
        }
        if (_CurrentAmplitude > _AmplitudeHighest)
        {
            _AmplitudeHighest = _CurrentAmplitude;
        }

        _Amplitude = _CurrentAmplitude / _AmplitudeHighest;
        _AmplitudeBuffer = _CurrentAmplitudeBuffer / _AmplitudeHighest;
    }

    void AudioProfile(float audioProfile)
    {
        for (int i = 0; i < 8; i++)
        {
            _freqBandHighest[i] = audioProfile;
        }
    }
    #endregion

    public void SetupService(bool autoDetectVoice)
    {
        _autoDetectVoice = autoDetectVoice;
        if (can_record && autoDetectVoice)
        {
            _isSetup = true;
            ToggleActivelyRecording(true);
        }
    }

    private void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            StopMicrophoneCapture();
        }
        else if (_autoDetectVoice)
        {
            StartMicrophoneCapture();
        }
    }

    #region Microphone / Voice Input Helpers
    public void SaveAudioToWav()
    {
        if (_audioRecording == null)
        {
            Debug.LogError("No audio recording available to save.");
            return;
        }

        float[] samples = new float[_audioRecording.samples * _audioRecording.channels];
        _audioRecording.GetData(samples, 0);

        wavdata = EncodeAsWAV(samples, _audioRecording.frequency, _audioRecording.channels);

    }

    public void ToggleActivelyRecording(bool enable)
    {
        if (!can_record)
            return;

        if (enable)
        {
            StartMicrophoneCapture();
        }
        else
        {
            StopMicrophoneCapture();
        }
    }

    private void StartMicrophoneCapture()
    {
        if (can_record && !_currentlyRecording)
        {
            microphoneActive = true;
            _currentlyRecording = true;
            _audioRecording = Microphone.Start(Microphone.devices[0], true, 10, AudioSettings.outputSampleRate);
            audioSource.clip = _audioRecording;
            audioSource.outputAudioMixerGroup = _mixerGroupMicrophone;
            audioSource.Play();
        }
    }

    private void StopMicrophoneCapture()
    {
        Microphone.End(Microphone.devices[0]);
        _currentlyRecording = false;
    }

    private async Task DetectAudio()
    {
        if (_isWaitingForNextInput)
        {
            return;
        }

        FillSamples(_micPrevPos);
        float maxVolume = 0.0f;
        for (int i = _micPrevPos + 1; i < Microphone.GetPosition(Microphone.devices[0]); ++i)
        {
            if (i >= _samples.Length)
                Debug.LogError("Index out of bounds: " + i);
            if (_samples[i] > maxVolume)
            {
                maxVolume = _samples[i];
            }
        }

        if (maxVolume > _minimumSpeakingSampleValue)
        {
            if (!_audioDetected && IL != null && IL.End_Flag)
            {
                if (!_requestNeedsSending)
                {
                    Debug.Log("User Started talking");
                }
                _audioDetected = true;
                _requestNeedsSending = true;
            }
        }
        else
        {
            if (_audioDetected)
            {
                _timeAtSilenceBegan = Time.time;
                _audioDetected = false;
            }
            else if (_requestNeedsSending && IL != null && IL.End_Flag)
            {
                if (Time.time - _timeAtSilenceBegan > _silenceTimer)
                {
                    Debug.Log("Sending Audio to recognizer...");
                    _KUBES.SetActive(false);
                    reactionImage.SetActive(true);
                    _RECOG.SetActive(true);
                    _inRecognitionPhase = true;
                    Invoke("DeactivateUI", 5f);
                    _audioDetected = false;
                    _requestNeedsSending = false;
                    SaveAudioToWav();
                    SendRecording(WavData);
                    ClearSamples();
                    StartCoroutine(WaitForNextInput(5f));
                }
            }
        }
        _micPrevPos = Microphone.GetPosition(Microphone.devices[0]);
    }
    private void DeactivateUI()
    {
        reactionImage.SetActive(false);
        _RECOG.SetActive(false);
        _inRecognitionPhase = false;
    }
    private IEnumerator WaitForNextInput(float delay)
    {
        _isWaitingForNextInput = true;
        yield return new WaitForSeconds(delay);
        _isWaitingForNextInput = false;
        Debug.Log("Ready to accept next input.");
        ClearSamples();
    }

    void FillSamples(int micPosition)
    {
        _samples = new float[_audioRecording.samples];
        _audioRecording.GetData(_samples, micPosition);
    }

    void ClearSamples()
    {
        for (int i = 0; i < _samples.Length; ++i)
        {
            _samples[i] = 0.0f;
        }
        _audioRecording.SetData(_samples, 0);
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + samples.Length * 2))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + samples.Length * 2);
                writer.Write("WAVE".ToCharArray());
                writer.Write("fmt ".ToCharArray());
                writer.Write(16);
                writer.Write((ushort)1);
                writer.Write((ushort)channels);
                writer.Write(frequency);
                writer.Write(frequency * channels * 2);
                writer.Write((ushort)(channels * 2));
                writer.Write((ushort)16);
                writer.Write("data".ToCharArray());
                writer.Write(samples.Length * 2);

                foreach (var sample in samples)
                {
                    writer.Write((short)(sample * short.MaxValue));
                }
            }
            return memoryStream.ToArray();
        }
    }
    #endregion
    public async Task<string> SendRecording(byte[] audioBytes)
    {
        CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        cancellationTokenSource.CancelAfter(7000);
        string response = null;

        try
        {
            using (var ws = new WebSocket("ws://127.0.0.1:9002"))
            {
                ws.Connect();
                Debug.Log("STT Connection Opened");
                ws.Send(audioBytes);

                TaskCompletionSource<bool> messageReceived = new TaskCompletionSource<bool>();

                ws.OnMessage += (sender, e) =>
                {
                    response = e.Data;
                    string pattern = @"\[([^\[\]]*?)\]$";

                    Match match = Regex.Match(response, pattern);

                    if (match.Success)
                    {
                        language = match.Groups[1].Value;
                        receivedText = response.Substring(0, match.Index).Trim();
                        processedText = receivedText.ToLower();
                    }
                    else
                    {
                        receivedText = response;
                    }

                    messageReceived.SetResult(true);
                };

                await Task.WhenAny(messageReceived.Task, Task.Delay(-1, cancellationTokenSource.Token));
                ws.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogError("Error: " + e.Message);
        }

        return receivedText;
    }
}
