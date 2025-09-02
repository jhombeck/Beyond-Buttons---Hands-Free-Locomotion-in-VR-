using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[RequireComponent(typeof(AudioSource))]
public class AgentVoiceAudioSourceBehavior : MonoBehaviour
{

    AudioSource _agent_voice_audio_source;
    AudioClip current_voice_audio_clip;

    Button get_audio_clip_button;

    private void Awake()
    {
        _agent_voice_audio_source = GetComponent<AudioSource>();
        get_audio_clip_button = GameObject.Find("GetAudioClipButton").GetComponent<Button>();
        get_audio_clip_button.onClick.AddListener(PlayAudioClipFromWebAPI);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    async void PlayAudioClipFromWebAPI()
    {
        // await get audio clip from web api and save it into local audio clip variable
        await GetAudioClip();
        // play audio clip in audio source
        _agent_voice_audio_source.clip = current_voice_audio_clip;
        _agent_voice_audio_source.Play();
    }

    // Example found here: https://docs.unity3d.com/ScriptReference/Networking.UnityWebRequestMultimedia.GetAudioClip.html
    private async Task GetAudioClip()
    {
        string text_to_be_said = "Hello from Unity."; // url encoding is automatically handled by the unity web request class!!!
        string url_string = "http://localhost:2701/tts/" + text_to_be_said;
        UnityWebRequest request = UnityWebRequestMultimedia.GetAudioClip(url_string, AudioType.WAV);
        request.SendWebRequest();
        // wait for task response to come back
        while (!request.isDone)
        {
            await Task.Yield();
        }
        current_voice_audio_clip = DownloadHandlerAudioClip.GetContent(request);

    }


}
