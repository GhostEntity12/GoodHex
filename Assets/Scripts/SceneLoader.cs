using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	int sceneToLoad = -1;
	[SerializeField] CanvasGroup fade;
	[SerializeField] float fadeTime = 1.5f;
	float timer = 0f;
	AudioSource aS;

	private void Awake()
	{
		aS = GetComponent<AudioSource>();
		Debug.Log(SceneUtility.GetScenePathByBuildIndex(10));
	}

	private void Update()
	{
		if (sceneToLoad > 0)
		{
			// Temp fix. Avoid Timescale?
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

	public void PlaySoundClip(AudioClip clip) => aS.PlayOneShot(clip);

	public void LoadNextScene()
	{
		int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
		
		sceneToLoad = nextScene > SceneManager.sceneCountInBuildSettings ? 0 : SceneManager.GetActiveScene().buildIndex + 1;
	}

	public void LoadScene(int i) => sceneToLoad = i;
	public void LoadMenuScene() => sceneToLoad = 0;
	public void LoadCurrentScene() => sceneToLoad = SceneManager.GetActiveScene().buildIndex;

	public void Quit()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
