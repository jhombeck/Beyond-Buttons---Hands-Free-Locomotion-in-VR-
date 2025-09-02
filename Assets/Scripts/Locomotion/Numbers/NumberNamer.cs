using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NumberNamer : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        this.transform.GetChild(0).GetChild(0).GetComponent<Text>().text = this.name;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
