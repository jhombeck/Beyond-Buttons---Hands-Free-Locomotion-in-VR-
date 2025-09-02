using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class CylindricSpawner : MonoBehaviour
{
    // Create Instance of LASTLY created SphereSpawner
    static CylindricSpawner instance;

    // Prefab that will be spawned for the Sphere 
    public GameObject sphere_spawn;
    //public List<GameObject> floor;

    // Player that will be moved when called
    public GameObject player;
    // Save Location Object for  easy Player Movement
    public GameObject player_move_location;

    public Vector3 scaling = new Vector3(1, 1, 1);

    // Object if Sphere is created
    public static GameObject root_cylindrical;

    public static float current_height = 0.02f;




    // Start is called before the first frame update
    void Awake()
    {
        instance = this;

    }
    private void Update()
    {
        FollowTarget(SceneManager.followHead);
    }

    private void FollowTarget(GameObject target)
    {
        if (!target || !root_cylindrical)
            return;
        if (root_cylindrical)
        {
            root_cylindrical.transform.position = new Vector3(target.transform.position.x, current_height, target.transform.position.z);
            root_cylindrical.transform.rotation = Quaternion.Euler(root_cylindrical.transform.rotation.eulerAngles.x, target.transform.rotation.eulerAngles.y, root_cylindrical.transform.rotation.eulerAngles.z);
        }
    }

    public static void InitSpawn(Vector3 position)
    {

        if (root_cylindrical)
        {

            Destroy(root_cylindrical);
        }

        instance.SpawnCylinder(position);

    }

    private void SpawnCylinder(Vector3 position)
    {
        root_cylindrical = Instantiate(sphere_spawn, position, Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0));
        root_cylindrical.transform.localScale = scaling;

        IsolineSetup.centerObjects = new List<GameObject>();
        IsolineSetup.centerObjects.Clear();

        IsolineSetup.centerObjects.Add(root_cylindrical);
        instance.player_move_location = root_cylindrical.transform.Find("Location")?.gameObject;

    }

    public static void BuildSlice(float angle = 0)
    {
        root_cylindrical.transform.GetChild(1).gameObject.SetActive(true);
        root_cylindrical.transform.GetChild(1).localRotation = Quaternion.Euler(0, angle, 0);

    }

    public static void MoveTo(float angle_flat, int distance_and_heigt, GameObject go)
    {
        // Get tens and ones digits
        int tensDigit = distance_and_heigt / 10;
        int onesDigit = Math.Abs(distance_and_heigt % 10);
        instance.player_move_location.transform.Rotate(0, angle_flat % 360, 0);
        instance.player_move_location.transform.Translate(0, tensDigit, onesDigit);
        instance.player_move_location.transform.rotation = Quaternion.Euler(new Vector3(0, instance.player_move_location.transform.rotation.y, instance.player_move_location.transform.rotation.z));

        // Now move Player 
        go.transform.position = instance.player_move_location.transform.position;
        go.transform.rotation = instance.player_move_location.transform.rotation;

        current_height = go.transform.position.y;


        instance.player_move_location.transform.localPosition = new Vector3(0, 0, 0);
        instance.player_move_location.transform.localRotation = Quaternion.Euler(0, 0, 0);



    }
}
