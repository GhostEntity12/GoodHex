using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField] Canvas pauseScreen;
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            PauseGame();
        }
    }

    void PauseGame()
    {
        Time.timeScale = 0f;
        pauseScreen.gameObject.SetActive(true);
    }

    public void UnpauseGame()
    {
        Time.timeScale = 1f;
        pauseScreen.gameObject.SetActive(false);
    }
}
