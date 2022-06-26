using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log(Camera.main.WorldToScreenPoint(transform.position));   
        Debug.Log(Camera.main.pixelWidth);   
    }
}
