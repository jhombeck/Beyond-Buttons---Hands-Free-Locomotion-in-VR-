using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VolumeSpawner : MonoBehaviour
{

    public GameObject tile;
    public GameObject tile_tower;
    public GameObject player;
    public GameObject followTarget;


    public static GameObject root;
    public static GameObject root_tower;
    // Create instance to be able to Call InitSpawn from different class
    static VolumeSpawner instance;
    bool gridBuilded = false;



    public Vector3 dimentions = new Vector3(3, 3, 3);
    public Vector3 spacing = new Vector3(1.75f, 1.75f, 1.75f);
    public Vector3 scaling = new Vector3(1, 1, 1);
    private Vector3 center = new Vector3(0, 0, 0);
    GameObject[,,] grid;
    GameObject[] grid_tower;

    public static float grid_height = 0;




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
        if (!target || !gridBuilded)
            return;
        if (RootExists())
        {
            root.transform.position = new Vector3(target.transform.position.x, grid_height, target.transform.position.z);
            root.transform.rotation = Quaternion.Euler(root.transform.rotation.eulerAngles.x, target.transform.rotation.eulerAngles.y, root.transform.rotation.eulerAngles.z);
        }


    }
    public static void InitSpawn(Vector3 new_center)
    {
        // Create Parent Object
        if (GameObject.Find("Grid Parent"))
        {
            Destroy(root_tower);
            Destroy(root);

        }

        instance.center = new_center;
        root = new GameObject("Grid Parent");
        instance.center.y = 0;
        root.transform.position = instance.center;

        instance.grid = new GameObject[(int)instance.dimentions.x, (int)instance.dimentions.y, (int)instance.dimentions.z];
        instance.StartCoroutine(instance.CreateGrid());


    }

    public static void BuildTower(int location_number)
    {
        instance.StartCoroutine(instance.BuildTowerCo(location_number));
    }
    public static void MovePlayer(int location_number, GameObject player)
    {

        instance.StartCoroutine(instance.MovePlayerCo(location_number, player));
    }


    IEnumerator BuildTowerCo(int location_number)
    {

        yield return 0;
        // No Root, no tower to build
        if (!RootExists())
        {
            yield break;
        }
        // Create Parent Object
        if (GameObject.Find("Tower Parent"))
        {
            Destroy(GameObject.Find("Tower Parent"));
            Destroy(root_tower);
        }


        root_tower = new GameObject("Tower Parent");
        root_tower.transform.parent = root.transform;

        GameObject tower_foundation = Helper.GetGOFromID(instance.grid, location_number);
        if (tower_foundation != null)
        {
            root_tower.transform.position = tower_foundation.transform.position;
            root_tower.transform.rotation = Quaternion.Euler(0, root.transform.eulerAngles.y, 0);

            grid_tower = new GameObject[20];


            for (int i = 0; i < (grid_tower.Length - 1) / 2; i++)
            {

                grid_tower[i] = instance.CreateNewTowerGO(i, location_number);

            }
        }
        else
        {
            Debug.LogWarning("Object with number" + location_number + " not found");
        }
        root_tower.transform.localScale = scaling * 1.7f;

        yield return 0;
        yield return StartCoroutine(BuildTowerNegativeCo(location_number));
    }
    IEnumerator BuildTowerNegativeCo(int location_number)
    {

        yield return 0;
        // No Root, no tower to build
        if (!RootExists())
        {
            yield break;
        }
        // Create Parent Object
        if (GameObject.Find("Tower Parent"))
        {
            root_tower = GameObject.Find("Tower Parent");
        }
        else
        {
            Debug.Log("No Tower Found");
            yield break;
        }


        GameObject tower_foundation = Helper.GetGOFromID(instance.grid, location_number);
        if (tower_foundation != null)
        {
            root_tower.transform.position = tower_foundation.transform.position;
            root_tower.transform.rotation = Quaternion.Euler(0, root.transform.eulerAngles.y, 0);

            for (int i = 0; i < (grid_tower.Length / 2) - 1; i++)
            {

                grid_tower[9 + i] = instance.CreateNewTowerGO(-i - 2, location_number, true);

            }
        }
        else
        {
            Debug.LogWarning("Object with number" + location_number + " not found");
        }

    }

    IEnumerator MovePlayerCo(int location_number, GameObject player)
    {
        yield return 0;
        if (location_number >= 100 || location_number <= -100)
        {
            GameObject new_locoation = Helper.GetGOFromID(instance.grid_tower, location_number);
            if (new_locoation)
                player.transform.position = new_locoation.transform.position;
            else
            {

                yield return StartCoroutine(BuildTowerCo(Mathf.Abs(Helper.ReduceToTwoDigits(location_number))));
                yield return 0;
                new_locoation = Helper.GetGOFromID(instance.grid_tower, location_number);
                player.transform.position = new_locoation.transform.position;

            }
        }
        else
        {
            player.transform.position = Helper.GetGOFromID(instance.grid, location_number).transform.position;
        }
        // If player would be in the negative set to 0
        if (player.transform.position.y <= 0)
        {
            player.transform.position = new Vector3(player.transform.position.x, 0.01f, player.transform.position.z);
        }
        grid_height = player.transform.position.y;

        if (GameObject.Find("Tower Parent"))
        {
            Destroy(GameObject.Find("Tower Parent"));
            Destroy(root_tower);
        }

    }


    IEnumerator CreateGrid()
    {
        gridBuilded = false;
        yield return 0;
        for (int i = 0; i < dimentions.x; i++)
        {
            for (int j = 0; j < dimentions.y; j++)
            {
                for (int k = 0; k < dimentions.z; k++)
                {
                    grid[i, j, k] = CreateNewGO(i, j, k);

                }

            }

        }
        // Rotate acording to view direction
        root.transform.rotation = Quaternion.Euler(0, Camera.main.transform.eulerAngles.y, 0);
        // Translate the center to the click position 
        root.transform.position -= root.transform.right * (dimentions.x - 1) / 2 * spacing.x;
        // Calcualte the height of each position 
        SetHeightMinimalOffset();
        SetNewCenterPivotAndScale();

        FollowTarget(SceneManager.followHead);
        gridBuilded = true;
    }

    private void SetNewCenterPivotAndScale()
    {
        foreach (var go in grid)
        {

            float xForHieght = go.transform.position.x;
            float zForHieght = go.transform.position.z;
            go.transform.position = new Vector3(xForHieght, grid_height + 0.02f, zForHieght) - (root.transform.right * (dimentions.x - 1) / 2 * spacing.x);

        }

        root.transform.localScale = scaling;
    }

    private GameObject CreateNewGO(float i, float j, float k)
    {
        Vector3 pos = new Vector3(i, j, k);
        GameObject newObject = Instantiate(tile, root.transform);
        newObject.transform.position = center + Vector3.Scale(pos, spacing);

        string number = j.ToString() + k.ToString() + i.ToString();
        newObject.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = number;

        return newObject;
    }

    float offset = 0.5f;
    private GameObject CreateNewTowerGO(float j, int id, bool negative = false)
    {
        Vector3 pos = new Vector3(0, (j * spacing.y) + 0.7f + offset, 0);
        if (negative)
        {
            pos = new Vector3(0, ((j + 1) * spacing.y) + 0.7f - offset, 0);
        }
        GameObject newObject = Instantiate(tile_tower, root_tower.transform);
        newObject.transform.localPosition = pos;
        newObject.transform.localRotation = Quaternion.Euler(-90, 0, 0);


        string number = (j + 1).ToString() + id.ToString();
        newObject.transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Text>().text = number;

        return newObject;


    }
    private void SetHeightMinimalOffset()
    {

        foreach (var go in grid)
        {

            float xForHieght = go.transform.position.x;
            float zForHieght = go.transform.position.z;
            go.transform.position = new Vector3(xForHieght, 0.05f, zForHieght);

        }
    }

    public static bool RootExists()
    {
        if (VolumeSpawner.root)
        {
            return true;
        }
        else
        {
            Debug.LogWarning("Grid not created yet!");
            return false;
        }

    }

    public static bool RootTowerExists()
    {
        if (VolumeSpawner.root_tower)
        {
            return true;
        }
        else
        {
            Debug.LogWarning("Tower not created!");
            return false;
        }

    }

}
