using UnityEngine;

public class LevelMenus : MonoBehaviour
{
	[SerializeField] GameObject pauseCanvas;
	[SerializeField] GameObject victoryCanvas;
	[SerializeField] AudioClip victorySting;
	[SerializeField] GameObject failureCanvas;
	[SerializeField] AudioClip failureSting;
	SceneLoader sl;

	private void Awake()
	{
		sl = GetComponent<SceneLoader>();
	}

	public void TriggerPauseCanvas()
	{
		pauseCanvas.SetActive(true);
	}

	public void TriggerVictoryScreen()
	{
		//sl.PlaySoundClip(victorySting);
		victoryCanvas.SetActive(true);
	}

	public void TriggerFailureScreen()
	{
		//sl.PlaySoundClip(failureSting);
		failureCanvas.SetActive(true);
	}
}
