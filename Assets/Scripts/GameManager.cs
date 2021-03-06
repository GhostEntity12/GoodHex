using System;
using System.Collections.Generic;
using System.Linq;
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
	public RatManager RatManager { get; private set; }
	public TaskManager TaskManager { get; private set; }

	public Camera mainCamera;
	
	[Header("Prefabs")]
	[SerializeField] ProgressBarManager progressBarManagerPrefab;
	[SerializeField] LevelMenus levelMenusPrefab;
	[SerializeField] Reticle reticlePrefab;
	[SerializeField] BGMManager bgmManagerPrefab;
	[SerializeField] Rat ratPrefab;
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
		GameObject managers = new("Managers");
		RatManager = managers.AddComponent<RatManager>();
		TaskManager = managers.AddComponent<TaskManager>();
		RatManager.RatPrefab = ratPrefab;
		RatManager.SpawnRats(GameObject.FindGameObjectsWithTag("SpawnPoints").Select(t => t.transform.position).ToArray());
	}

	void OnDisable() => SceneManager.sceneLoaded -= OnLoadNewScene; 
}
