using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
	[SerializeField] Canvas pauseScreen;

	public bool isPaused;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			isPaused = !isPaused;
			PauseGame(isPaused);
		}
	}

	public void PauseGame(bool paused)
	{
		isPaused = paused;
		Time.timeScale = paused ? 0f : 1f;
		pauseScreen.gameObject.SetActive(paused);
	}

	public void LoadMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}
}
