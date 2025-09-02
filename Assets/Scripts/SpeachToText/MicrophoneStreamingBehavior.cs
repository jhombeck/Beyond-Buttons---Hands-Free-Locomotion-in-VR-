using NativeWebSocket;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using UnityEngine;
// post request 
using UnityEngine.Networking;

public class MicrophoneStreamingBehavior : MonoBehaviour
{
    WebSocket websocket;

    string websocket_url = "ws://localhost:8080/ws";


    string _language_assistant_url = "http://localhost:8080/perceive_text?input_string=";
    string _scalar_field_names_url = "http://localhost:8080/set_scalar_field_names?scalar_field_names=";

    // get all possible functions 
    ActionSpaceHandler action_space_handler;

    public string current_text = "";
    public static string message;



    private void Awake()
    {
        action_space_handler = new ActionSpaceHandler();
    }


    // Start is called before the first frame update
    async void Start()
    {
        // solution found here: https://developers.deepgram.com/blog/2022/03/deepgram-unity-tutorial/
        websocket = new WebSocket(websocket_url);

        websocket.OnOpen += () =>
        {
            Debug.Log("CONNECTED to service_speech_to_text!");
        };

        websocket.OnError += (e) =>
        {
            Debug.Log("Error: " + e);

        };

        websocket.OnClose += (e) =>
        {
            Debug.Log("Connection to service_speech_to_text CLOSED!");

        };

        websocket.OnMessage += (bytes) =>
        {
            // check if bytes contain a result string or are empty
            if (bytes.Length > 3)
            {
                // getting the message as a string
                message = System.Text.Encoding.UTF8.GetString(bytes);
                Debug.Log("RESULT: " + message);

                // CALL LANGUAGE ASSISTANT
                StartCoroutine(send_get_request(message));
            }

        };

        await websocket.Connect();

    }

    // Update is called once per frame
    void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        websocket.DispatchMessageQueue();
#endif
    }

    // Close websocket once the application is closed
    private async void OnApplicationQuit()
    {
        await websocket.Close();
    }

    // send audio
    public async void ProcessAudio(byte[] audio)
    {
        if (websocket.State == WebSocketState.Open)
        {
            await websocket.Send(audio);
        }
    }

    IEnumerator send_get_request(string result_text)
    {
        string url = _language_assistant_url + UnityWebRequest.EscapeURL(result_text);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) //-> works for Unity version 2019
        {
            Debug.Log("GET request failed!");
            Debug.Log(www.error);
        }
        else
        {
            string output_string = www.downloadHandler.text;
            Debug.Log(output_string);
            // parse json_result 
            // get function name and arguments
            (string functionName, List<string> arguments) = action_space_handler.ParseOutputString(output_string);
            // call function by name with arguments
            action_space_handler.CallFunctionByName(functionName, arguments);
        }
        // prevent memory leak
        www.Dispose();
    }

    // change the locomotion mode based on the current task 
    public IEnumerator set_locomotion_mode(string new_locomotion_mode)
    {
        string url = "http://localhost:2702/set_locomotion_mode?locomotion_mode=" + UnityWebRequest.EscapeURL(new_locomotion_mode);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) //-> works for Unity version 2019
        {
            Debug.Log("GET request failed!");
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("GET request successful for setting locomotion mode!");
            string output_string = www.downloadHandler.text;
            Debug.Log(output_string);
        }
        // prevent memory leak
        www.Dispose();
    }
    public void SetScalarNames(string names)
    {
        StartCoroutine(set_scalar_field_names(names));
    }
    public IEnumerator set_scalar_field_names(string scalar_field_names)
    {
        string url = _scalar_field_names_url + UnityWebRequest.EscapeURL(scalar_field_names);
        UnityWebRequest www = UnityWebRequest.Get(url);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError) //-> works for Unity version 2019
        {
            Debug.Log("GET request failed!");
            Debug.Log(www.error);
        }
        else
        {
            Debug.Log("GET request successful for setting scalar values!");
            string output_string = www.downloadHandler.text;
            Debug.Log(output_string);
        }
        // prevent memory leak
        www.Dispose();
    }
}


// ACTION SPACE HANDLER 
public class ActionSpaceHandler
{

    // Invoke correct function from the parsed string (from server)
    public void CallFunctionByName(string functionName, List<string> arguments)
    {
        // call function by name with parameters
        Type thisType = typeof(ActionSpaceHandler);
        MethodInfo method = thisType.GetMethod(functionName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);



        if (method != null)
        {

            method.Invoke(this, arguments.ToArray());

        }
        else
        {

            Debug.LogError($"Function '{functionName}' does not exist!");
        }
    }
    public (string, List<string>) ParseOutputString(string output_string)
    {
        // OUTPUT STRING LOOKS LIKE THIS: method: method_name, params: ['param1', 'param2', 'param3']

        // split the string into two parts by the first comma (,) in the string seen from the left
        int index = output_string.IndexOf(',');
        // GET FUNCTION NAME
        string functionName = output_string.Substring(output_string.IndexOf(':') + 1, index - output_string.IndexOf(':') - 1).Trim().ToLower();
        // GET ARGUMENTS
        string paramsSubstring = output_string.Substring(index + 1).Split(':')[1].Trim();

        // get all parameters out of the paramsSubstring by matching all strings between ' and ' or all numbers
        List<string> arguments = new List<string>();
        MatchCollection matches = Regex.Matches(paramsSubstring, @"'([^'\\]*(?:\\.[^'\\]*)*)'|(-?\d+(?:\.\d+)?)\b");

        foreach (Match match in matches)
        {
            if (match.Groups[1].Success)
            {
                arguments.Add(match.Groups[1].Value);
            }
            else if (match.Groups[2].Success)
            {
                arguments.Add(match.Groups[2].Value);
            }
        }

        return (functionName, arguments);
    }


    // ---------------------------------------------------------------- 
    // LIST OF FUNCTIONS TO BE CALLED

    private VoiceProcessor VP = new VoiceProcessor();

    public void rotate_x(string degrees)
    {

        string methodName = MethodInfo.GetCurrentMethod().Name;
        Debug.Log(methodName + " called.");

    }
    public void build_grid_tower(string grid_number)
    {
        VP.BuildGridTower(int.Parse(grid_number));
        // build tower on grid number
        Debug.Log($"Build tower on grid number: {grid_number}");

    }

    public void move_player_grid(string grid_number)
    {

        VP.MovePlayerGrid(int.Parse(grid_number));
        // move player to grid number
        Debug.Log($"Move player to grid number: {int.Parse(grid_number)}");

    }


    public void build_sphere_slice(string longitude_angle)
    {
        VP.BuildSphereSlice(float.Parse(longitude_angle));
        // build sphere slice with latitude angle
        Debug.Log($"Build sphere slice with latitude angle: {longitude_angle}");

    }

    public void move_player_to_sphere_2d(string longitude_angle, string distance)
    {
        VP.MovePlayerToSphere2D(float.Parse(longitude_angle), float.Parse(distance));
        // move player to sphere 2d
        Debug.Log($"Move player to sphere 2d: {longitude_angle}, {distance}");

    }

    public void move_player_to_sphere_3d(string longitude_angle, string latitude_angle, string distance)
    {
        VP.MovePlayerToSphere3D(float.Parse(longitude_angle), float.Parse(latitude_angle), float.Parse(distance));
        // move player to sphere 3d
        Debug.Log($"Move player to sphere 3d: {longitude_angle}, {latitude_angle}, {distance}");
    }


    public void select_object(string object_name)
    {
        // select object
        Debug.Log($"Select object: {object_name}");
    }


    //=======================================
    // Cylindric commands
    // ======================================
    public void build_cylindric_slice(string longitude_angle)
    {
        VP.BuildSphereSliceCylindirc(float.Parse(longitude_angle));
        // build sphere slice with latitude angle
        Debug.Log($"Build sphere slice with latitude angle: {longitude_angle}");

    }
    public void move_player_to_cylindric(string longitude_angle, string distance_and_heigt)
    {
        VP.MovePlayerToCylindric(float.Parse(longitude_angle), int.Parse(distance_and_heigt));
        // move player to sphere 2d
        Debug.Log($"Move player to cylindric: {longitude_angle}, {distance_and_heigt}");

    }

    // FALLBACK FUNCTION
    public void no_relevant_function()
    {
        Debug.Log("No Relevant Function Found.");
        // function that does nothing
    }


    public void do_nothing()
    {
        // function that does nothing
    }
    // ---------------------------------------------------------------- 
}