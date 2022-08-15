using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseScript : MonoBehaviour
{
	[SerializeField] Canvas pauseScreen;

	public bool isPaused;

	void Update()
	{
		if (!GameManager.Instance.DialogueManager.DialogueActive && Input.GetKeyDown(KeyCode.Escape))
		{
			PauseGame(!isPaused);
		}
	}

	public void PauseGame(bool paused)
	{
		isPaused = paused;

		pauseScreen.gameObject.SetActive(paused);

		GameManager.Instance.SetPause(isPaused);
	}

	public void LoadMenu()
	{
		Time.timeScale = 1f;
		SceneManager.LoadScene("MainMenu");
	}
}
