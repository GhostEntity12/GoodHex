using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class GameManager : Singleton<GameManager>
{
	public static string[] names = new string[] {
		"Jerry",
		"Cheddar",
		"Gouda",
		"Brie",
		"Cam",
		"Whiskers",
		"Dr. Squeaks",
		"Geronimo",
		"Moz",
		"Roque",
		"Rick",
		"Gru",
	};

	static readonly string[] nonLevelScenes = new string[] { "MainMenu", "Controls", "Credits" };

	LevelMenus levelMenus;
	BGMManager bgmManager;
	ProgressBarManager progressBarManager;

	public Camera mainCamera;
	
	[Header("Prefabs")]
	[SerializeField] ProgressBarManager progressBarManagerPrefab;
	[SerializeField] LevelMenus levelMenusPrefab;
	[SerializeField] Reticle reticlePrefab;
	[SerializeField] BGMManager bgmManagerPrefab;
	public Reticle Reticle { get; private set; }

	protected override void Awake()
	{
		base.Awake();
		SceneManager.sceneLoaded += OnLoadNewScene;
	}

	public void Restart() => LoadSceneManager.LoadScene(0);

	public ProgressBar CreateProgressBar() => progressBarManager.CreateProgressBar();
	public void TriggerEndMusic() => bgmManager.TriggerEndMusic();

	public void AllTasksComplete()
	{
		Debug.Log("All tasks complete");
		bgmManager.StopAllTracks();
		levelMenus.TriggerVictoryScreen();
	}

	public void LevelFailed()
	{
		bgmManager.StopAllTracks();
		levelMenus.TriggerFailureScreen();
	}

	void OnLoadNewScene(Scene scene, LoadSceneMode mode)
	{
		if (Array.Exists(nonLevelScenes, e => e == scene.name)) return;

		mainCamera = Camera.main;

		Reticle = Instantiate(reticlePrefab);
		levelMenus = Instantiate(levelMenusPrefab);
		bgmManager = Instantiate(bgmManagerPrefab);
		progressBarManager = Instantiate(progressBarManagerPrefab);
	}

	void OnDisable() => SceneManager.sceneLoaded -= OnLoadNewScene; 
}
