using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
	[SerializeField] Canvas pauseScreen;

	public bool isPaused;

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame(!isPaused);
		}
	}

	public void PauseGame(bool paused)
	{
		isPaused = paused;

		pauseScreen.gameObject.SetActive(paused);
		
		// TODO: Implement this at some point
		//GameManager.Instance.Pause(paused)
		Time.timeScale = paused ? 0f : 1f;
	}

	public void LoadMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}
}
