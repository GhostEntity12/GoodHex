using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	int sceneToLoad = -1;
	[SerializeField] CanvasGroup fade;
	[SerializeField] float fadeTime = 1.5f;
	float timer = 0f;
	AudioSource aS;

	[SerializeField] public bool idleEnabled;
	[SerializeField] public float idleTime;
	[SerializeField] public int idleScene;
	private float idleCounter = 0.0f;

	private void Awake()
	{
		aS = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (sceneToLoad >= 0)
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
		//idle check
		if (idleEnabled == true)
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
				//Debug.Log("idleTime = idleCounter");
				LoadScene(idleScene);
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
