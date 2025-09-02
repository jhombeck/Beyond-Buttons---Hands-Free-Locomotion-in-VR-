using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IsolineSetup : MonoBehaviour
{
    Renderer _Rend;
    Material _Material;
    public  List<GameObject> centerObjects_init;
    public static List<GameObject> centerObjects;
    // Start is called before the first frame update
    void Start()
    {
        centerObjects = centerObjects_init;
        _Rend = gameObject.GetComponent<Renderer>();
        _Material = GetComponent<Renderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        // Setup for multiple reference points
        Vector4[] CenterPositionArray = new Vector4[centerObjects.Count];
        for (int i = 0; i < centerObjects.Count; i++)
        {
            CenterPositionArray[i] = centerObjects[i].transform.position;
        }

        // Update Uniforms
        _Material.SetInt("_CenterPointAmount", centerObjects.Count);
        _Material.SetVectorArray("_CenterPositionArray", CenterPositionArray);
    }
}
