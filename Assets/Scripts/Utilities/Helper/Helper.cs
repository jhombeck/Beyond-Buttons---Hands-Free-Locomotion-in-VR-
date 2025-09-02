using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Helper : MonoBehaviour
{
    public static GameObject GetGOFromID(GameObject[,,] grid, int location_number)
    {

        foreach (var go in grid)
        {
            if (location_number == GetIDFromGO(go))
            {
                Debug.Log("Found at" + go.transform.position);
                return go;

            }
        }
        return null;
    }
    public static GameObject GetGOFromID(GameObject[] grid, int location_number)
    {

        foreach (var go in grid)
        {
            if (go)
            {
                if (location_number == GetIDFromGO(go.transform.GetChild(0).gameObject))
                {
                    Debug.Log("Found at" + go.transform.position);
                    return go;

                }
            }
        }
        return null;
    }
    public static int GetIDFromGO(GameObject teleport_location)
    {

        return int.Parse(teleport_location.transform.GetChild(0).GetChild(0).gameObject.GetComponent<Text>().text);

    }

    public static float GetHeightAtPosition(float x, float z)
    {

        float y = -100;
        RaycastHit[] hits = Physics.RaycastAll(new Vector3(x, 100f, z), Vector3.down, 1000);
        foreach (RaycastHit hit in hits)
        {
            if (hit.collider != null && !hit.collider.isTrigger)
            {
                y = Mathf.Max(y, hit.point.y);
            }
        }

        return y;
    }

    public static void SetHeigt(GameObject go)
    {

        float xForHieght = go.transform.position.x;
        float zForHieght = go.transform.position.z;
        go.transform.position = new Vector3(xForHieght, GetHeightAtPosition(xForHieght, zForHieght), zForHieght);

    }

    public static Vector3 GetIntersection()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            return hit.point;
        }
        return new Vector3(float.NaN, float.NaN, float.NaN);

    }


    public static int ReduceToTwoDigits(int value)
    {

        int sum = 0;

        for (int i = 1; i < 100; i *= 10)
        {

            sum += value % 10 * i;
            value /= 10;

        }
        return sum;

    }




}
