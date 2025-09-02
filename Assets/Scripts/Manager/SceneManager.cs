using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.XR.WSA.Input;
using Valve.VR;
using static SceneManager;


public enum TechniqueOptions
{
    Cartesian,
    Cylindrical,
    Polar,
    fallback,
}
public enum TaskOptions
{
    Navigation,
    fallback,
}


public class SceneManager : MonoBehaviour
{

    public MicrophoneStreamingBehavior mic_streaming;
    public VoiceProcessor VP;


    public TechniqueOptions _locomotionTechnique = TechniqueOptions.Cartesian;
    private TaskOptions _evaluationTask = TaskOptions.Navigation; // For Demo only Navigation Works

    public GameObject _player;
    public static GameObject player;

    public GameObject _followHead;
    public static GameObject followHead;

    public MicrophoneBehavior mic;


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {

            mic.StartRecording();

        }

        if (Input.GetKeyUp(KeyCode.Space))
        {
            mic.StopRecording();
        }

        // Mute And Unmute Mic
        if (SteamVR_Actions._default.InteractUI.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            mic.StartRecording();

        }
        if (SteamVR_Actions._default.InteractUI.GetStateUp(SteamVR_Input_Sources.LeftHand))
        {
            mic.StopRecording();
        }

    }

    void Start()
    {
        player = _player;
        followHead = _followHead;
        init();
    }

    private void init()
    {
        // Some astetic settings
        VolumeSpawner.grid_height = 0.0f;
        SphereSpawner.current_height = 0.02f;
        CylindricSpawner.current_height = 0.02f;
        // Sometime 2 player exist, filter and fix that
        Filter2Player();
        EnableVis();
        Debug.Log("init DONE");

    }
    // Enable all aid and settings nedes for locomotion technique 
    private void EnableVis()
    {

        mic_streaming.SetScalarNames(_locomotionTechnique.ToString() + "," + _evaluationTask.ToString());
        switch (_locomotionTechnique)
        {
            case TechniqueOptions.Cartesian:
                VP.ShowGrid2D();
                break;

            case TechniqueOptions.Polar:
                VP.ShowSphere2D();
                VP.BuildSphereSlice(0);
                break;

            case TechniqueOptions.Cylindrical:
                VP.ShowCylindrical2D();
                VP.BuildSphereSliceCylindirc(0);
                break;
        }

    }

    private void Filter2Player()
    {
        List<GameObject> goList = new List<GameObject>();
        foreach (GameObject go in GameObject.FindObjectsOfType(typeof(GameObject)))
        {
            if (go.name == "Player")
                goList.Add(go);
        }

        if (goList.Count > 1)
            goList[0].SetActive(false);

    }






}
