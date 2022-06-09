using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuScript : MonoBehaviour
{
	string sceneToLoad = null;
	[SerializeField] CanvasGroup fade;
	[SerializeField] float fadeTime = 1.5f;
	float timer = 0f;
	static string previousScene;
	
	private void Update()
	{
		if (sceneToLoad != null)
		{
			Time.timeScale = 1f;
			timer += Time.deltaTime;
			fade.alpha = timer / fadeTime;
			if (timer >= fadeTime)
			{
				fade.blocksRaycasts = true;
				SceneManager.LoadScene(sceneToLoad);
			}
		}
	}

	public void LoadScene(string sceneName) => sceneToLoad = sceneName;

	public void LoadGameOver() {
		previousScene = SceneManager.GetActiveScene().name;
		sceneToLoad = "GameOver";
	}

	public void LoadVictory() {
		previousScene = SceneManager.GetActiveScene().name;
		sceneToLoad = "VictoryScreen";
	}

	public void LoadPreviousScene() => sceneToLoad = previousScene;

	public void Quit()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
