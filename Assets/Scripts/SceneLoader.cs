using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
	int sceneToLoad = -1;
	[SerializeField] CanvasGroup fade;
	[SerializeField] float fadeTime = 1.5f;
	float timer = 0f;
	AudioSource aS;

	[SerializeField] bool idleEnabled;
	[SerializeField] float idleTime;
	[SerializeField] int idleScene;
	[SerializeField] KeyCode idleKey = KeyCode.F12;
 	private float idleCounter = 0.0f;
	[SerializeField] bool idleScreen;

	private void Awake()
	{
		aS = GetComponent<AudioSource>();
	}

	private void Update()
	{
		if (sceneToLoad >= 0)
		{
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
			if (Input.anyKey)
			{
				idleCounter = 0.0f;  // reset counter  
			}
			else
			{
				idleCounter += Time.deltaTime; // increment counter
			}

			if (idleCounter > idleTime || Input.GetKeyDown(idleKey))
			{
				//Debug.Log("idleTime = idleCounter");
				LoadScene(idleScene);
			}
		}

		if(idleScreen == true)
        {
			if(Input.anyKey)
            {
				LoadScene(0);
            }
        }
	}

	public void PlaySoundClip(AudioClip clip) => aS.PlayOneShot(clip);

	public void LoadNextScene()
	{
		int nextScene = SceneManager.GetActiveScene().buildIndex + 1;
		sceneToLoad = nextScene >= SceneManager.sceneCountInBuildSettings ? 0 : SceneManager.GetActiveScene().buildIndex + 1;

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
