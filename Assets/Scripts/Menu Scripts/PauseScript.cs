using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    [SerializeField] Canvas pauseScreen;

    public static bool IsPaused;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            IsPaused = !IsPaused;
            PauseGame();
        }
    }

    void PauseGame()
    {
        if (IsPaused)
        {
            Time.timeScale = 0f;
            pauseScreen.gameObject.SetActive(true);
        }
        else
        {
            Time.timeScale = 1f;
            pauseScreen.gameObject.SetActive(false);
        }
    }

    public void UnpauseButton()
    {
        Time.timeScale = 1f;
        pauseScreen.gameObject.SetActive(false);
    }
}
