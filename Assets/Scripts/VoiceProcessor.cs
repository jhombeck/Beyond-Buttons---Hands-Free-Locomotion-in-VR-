using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static SceneManager;






public class VoiceProcessor : MonoBehaviour
{
    //======================
    //Sphere based Functions
    //======================

    // Display sphere as 2D 
    public void ShowSphere2D()
    {

        SphereSpawner.InitSpawn(SceneManager.followHead.transform.position);

    }

    // Create the 3Dslice at the  angle of flat_radius

    //flat_radius float[-180:180]
    public void BuildSphereSlice(float flat_radius)
    {

        if (!SphereSpawner.root)
        {
            Debug.LogWarning("Sphere not created yet!");
            return;
        }
        SphereSpawner.BuildSlice(flat_radius);

    }


    // Display sphere as 3D with slice at the angle of flat_radius 

    // flat_radius float[-180:180]
    public void ShowSphere3D(float flat_radius)
    {

        ShowSphere2D();
        BuildSphereSlice(flat_radius);

    }

    // Move player to sphere in 2D Coordiante System.
    // flat_radius = the radius on the ground the player should be moved to
    // distance = the distance the player wants to move from the center of the circle

    // could also be represeted with as MoveToSphere3D() with height_radius = 0

    // flat_radius float[-180:180], distance float[0:50]
    public void MovePlayerToSphere2D(float flat_radius, float distance)
    {


        if (!SphereSpawner.root)
        {
            Debug.LogWarning("Sphere not created yet!");
            return;
        }

        SphereSpawner.MoveTo2D(flat_radius, distance, SceneManager.player);
    }

    // Move player to sphere in 3D Coordiante System.
    // flat_radius = the angle on the ground the player should be moved to
    // height_radius = the 3D angle that should move the player into the air
    // distance = the distance the player wants to move from the center of the circle

    // flat_radius float[-180:180], height_radius float[-90:90], distance float[0:50]
    public void MovePlayerToSphere3D(float flat_radius, float height_radius, float distance)
    {

        if (!SphereSpawner.root)
        {
            Debug.LogWarning("Sphere not created yet!");
            return;
        }

        SphereSpawner.MoveTo3D(flat_radius, height_radius, distance, SceneManager.player);

    }


    //======================
    //Grid based Functions
    //======================

    // Display grid as 2D 
    public void ShowGrid2D()
    {

        VolumeSpawner.InitSpawn(SceneManager.followHead.transform.position);


    }


    // Create a 3D tower at the location of the grid with the number grid_number

    // grid_number int[0:99]
    public void BuildGridTower(int grid_number)
    {

        if (!VolumeSpawner.RootExists())
        {
            ShowGrid2D();
        }
        VolumeSpawner.BuildTower(grid_number);
    }


    // Display grid in 3D with a tower at the location of the grid with the number grid_number

    public void ShowGrid3D(int grid_number)
    {

        ShowGrid2D();
        BuildGridTower(grid_number);



    }

    // Move player to grid with the number grid_number.
    // grid_number can be 2D with two digits 22
    // or 3D with 3 digits 322 

    //grid_number int[-999:999]
    public void MovePlayerGrid(int grid_number)
    {


        if (!VolumeSpawner.RootExists())
        {
            ShowGrid2D();
        }
        if (!VolumeSpawner.RootTowerExists())
        {
            if (grid_number >= 100)
            {
                int grid_number_2D = Helper.ReduceToTwoDigits(grid_number);
                BuildGridTower(grid_number_2D);
            }
            if (grid_number <= -100)
            {
                int grid_number_2D = Mathf.Abs(Helper.ReduceToTwoDigits(grid_number));
                BuildGridTower(grid_number_2D);
            }

        }

        VolumeSpawner.MovePlayer(grid_number, SceneManager.player);


    }

    //======================
    //Cylindrical manipulation 
    //======================
    public void BuildSphereSliceCylindirc(float flat_radius)
    {

        if (!CylindricSpawner.root_cylindrical)
        {
            Debug.LogWarning("Cylinder not created yet!");
            return;
        }
        CylindricSpawner.BuildSlice(flat_radius);

    }
    public void ShowCylindrical2D()
    {
        CylindricSpawner.InitSpawn(SceneManager.followHead.transform.position);
    }
    public void MovePlayerToCylindric(float flat_radius, int distance)
    {


        if (!CylindricSpawner.root_cylindrical)
        {
            Debug.LogWarning("Sphere not created yet!");
            return;
        }

        CylindricSpawner.MoveTo(flat_radius, distance, SceneManager.player);
    }


    // Undo the last command
    public void Undo()
    {
    }




}
