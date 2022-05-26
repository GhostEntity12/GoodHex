using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
	string sceneToLoad = null;
	[SerializeField] CanvasGroup fade;
	[SerializeField] float fadeTime = 1.5f;
	float timer = 0f;
	private void Update()
	{
		if (sceneToLoad != null)
		{
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

	public void Quit()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
