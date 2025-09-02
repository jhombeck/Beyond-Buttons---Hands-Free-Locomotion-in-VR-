using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MicFeedbackColorChanger : MonoBehaviour
{

    public Color mic_active;
    public Color mic_inactive;

    public Text text;

    bool curent_state;
    // Start is called before the first frame update
    void Start()
    {
        curent_state = MicrophoneBehavior.IsMuted();
        UpdateColor();
    }

    // Update is called once per frame
    void Update()
    {
        text.text = MicrophoneStreamingBehavior.message;
        if (curent_state != MicrophoneBehavior.IsMuted()) {
            curent_state = MicrophoneBehavior.IsMuted();
            UpdateColor();
        }
    }

    private void UpdateColor()
    {
        if(!curent_state)
            this.GetComponent<Renderer>().material.color = mic_active;
        if (curent_state)
            this.GetComponent<Renderer>().material.color = mic_inactive;
    }
}
