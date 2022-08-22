using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToggler : MonoBehaviour
{

    public GameObject[] Cameras;

    int currentCam;

    // Start is called before the first frame update
    void Start()
    {
        currentCam = 0;
        setCam(currentCam);
    }

    public void setCam(int idx)
    {
        for (int i = 0; i < Cameras.Length; i++)
        {
            if (i == idx)
            {
                Cameras[i].SetActive(true);
            }
            else
            {
                Cameras[i].SetActive(false);
            }
        }
    }

    public void toggleCam()
    {
        Debug.Log("button clicked");
        currentCam++;
        if (currentCam > Cameras.Length - 1)
            currentCam = 0;
        setCam(currentCam);
    }
}