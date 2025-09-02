using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class MicrophoneBehavior : MonoBehaviour
{
    AudioSource _audioSource;
    int lastPosition, currentPosition;
    static bool _isMuted = false;

    MicrophoneStreamingBehavior websocket;

    bool _use_streaming = false;

    int _sampling_rate = 16000;

    int _max_recording_length = 10;


    private AudioClip clip;
    private byte[] bytes;
    private bool recording;

    private void Awake()
    {
        // load websocket 
        websocket = GameObject.Find("WebsocketObject").GetComponent<MicrophoneStreamingBehavior>();
    }

    // Start is called before the first frame update
    void Start()
    {
        _audioSource = GetComponent<AudioSource>();

        Debug.Log("Sample Rate: " + AudioSettings.outputSampleRate);

        // Select correct microphone
        foreach (var x in Microphone.devices)
        {
            Debug.Log("Mics:" + x.ToString());
        }

        //select correct sampling rate
        int sampling_rate = 16000;

        if (Microphone.devices.Length > 0)
        {
            // Select main microphone source
            _audioSource.clip = Microphone.Start(Microphone.devices[0], true, 10, sampling_rate);
        }
        else
        {
            Debug.Log("This will crash!");
        }
        Debug.Log("Selected Audio Source : " + _audioSource.name);
        Debug.Log("Active Audio Source : " + _audioSource.isActiveAndEnabled);
        Debug.Log("To Sting Audio Source : " + Microphone.devices[0]);

        _audioSource.Play();


    }

    public static void MuteMic(bool mute)
    {
        if (mute)
        {
            _isMuted = true;
            Debug.Log("Mic Muted");
        }
        if (!mute)
        {
            _isMuted = false;
            Debug.Log("Mic Listening");
        }

    }

    public static bool IsMuted()
    {
        return _isMuted;
    }

    // Update is called once per frame
    void Update()
    {

        if (_use_streaming)
        {
            // STREAMING CASE
            if ((currentPosition = Microphone.GetPosition(null)) > 0)
            {
                if (lastPosition > currentPosition)
                    lastPosition = 0;

                if (currentPosition - lastPosition > 0)
                {
                    float[] samples = new float[(currentPosition - lastPosition) * _audioSource.clip.channels];
                    _audioSource.clip.GetData(samples, lastPosition);

                    short[] samplesAsShorts = new short[(currentPosition - lastPosition) * _audioSource.clip.channels];
                    for (int i = 0; i < samples.Length; i++)
                    {
                        if (!_isMuted)
                        {
                            samplesAsShorts[i] = f32_to_i16(samples[i]);
                        }
                        else
                        {
                            samplesAsShorts[i] = (short)0;
                        }
                    }

                    var samplesAsBytes = new byte[samplesAsShorts.Length * 2];
                    System.Buffer.BlockCopy(samplesAsShorts, 0, samplesAsBytes, 0, samplesAsBytes.Length);
                    websocket.ProcessAudio(samplesAsBytes);

                    if (!GetComponent<AudioSource>().isPlaying)
                        GetComponent<AudioSource>().Play();
                    lastPosition = currentPosition;
                }
            }
        }
        else
        {
            // START STOP BUTTON CASE
            // check if we are recording longer than max recording length
            if (recording && Microphone.GetPosition(null) >= clip.samples)
            {
                StopRecording();
            }
        }

    }

    short f32_to_i16(float sample)
    {
        sample = sample * 32768;
        if (sample > 32767)
        {
            return 32767;
        }
        if (sample < -32768)
        {
            return -32768;
        }
        return (short)sample;
    }

    private byte[] EncodeAsWAV(float[] samples, int frequency, int channels)
    {
        using (var memoryStream = new MemoryStream(44 + (samples.Length * 2)))
        {
            using (var writer = new BinaryWriter(memoryStream))
            {
                writer.Write("RIFF".ToCharArray());
                writer.Write(36 + (samples.Length * 2));
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

    public void StartRecording()
    {
        Debug.Log("Recording started");
        clip = Microphone.Start(null, false, _max_recording_length, _sampling_rate);
        recording = true;
    }

    public void StopRecording()
    {
        Debug.Log("Recording stopped");
        var position = Microphone.GetPosition(null);
        Microphone.End(null);
        var samples = new float[position * clip.channels];
        clip.GetData(samples, 0);
        bytes = EncodeAsWAV(samples, clip.frequency, clip.channels);
        recording = false;
        websocket.ProcessAudio(bytes);
    }

}
