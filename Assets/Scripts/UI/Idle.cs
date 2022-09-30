using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Idle : MonoBehaviour
{
    [SerializeField] public float idleTime;

    private float idleCounter = 0.0f;

    [SerializeField] public bool idleEnabled;

    // Update is called once per frame
    void Update()
    {
        if(idleEnabled == true)
        {
            Debug.Log("idle Counter = " + idleCounter);
            if (Input.anyKey)
            {
                idleCounter = 0.0f;  // reset counter  
            }
            else
            {
                idleCounter += Time.deltaTime; // increment counter
            }

            if (idleCounter > idleTime)
            {
                Debug.Log("idleTime = idleCounter");
                //SceneLoader.LoadMenuScene();
                SceneManager.LoadScene("MainMenu");
            }
        }
    }
}
