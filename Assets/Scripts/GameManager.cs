using System;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

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

	public static Color[,] ratColors = {
		{ new Color32(41, 78, 152, 255), new Color32(111, 144, 201, 255), new Color32(116, 87, 165, 255), new Color32(243, 230, 125, 255)},
		{ new Color32(33, 40, 52, 255), new Color32(86, 91, 155, 255), new Color32(41, 78, 152, 255), new Color32(219, 172, 79, 255)}
	};
	public static int RatIndex()
	{
		System.Random rand = new();
		return rand.NextDouble() switch
		{
			<= 0.33f => 0,
			<= 0.66f => 1,
			<= 0.99f => 2,
			_ => 3,
		};
	}
	static readonly string[] nonLevelScenes = new string[] { "MainMenu", "Controls", "Credits" };

	LevelMenus levelMenus;
	BGMManager bgmManager;
	ProgressBarManager progressBarManager;
	public RatManager RatManager { get; private set; }
	public TaskManager TaskManager { get; private set; }
	public DialogueManager DialogueManager { get; private set; }
	public Highlighter Highlighter { get; private set; }
	public Camera mainCamera;

	[Header("Prefabs")]
	[SerializeField] ProgressBarManager progressBarManagerPrefab;
	[SerializeField] LevelMenus levelMenusPrefab;
	[SerializeField] Reticle reticlePrefab;
	[SerializeField] BGMManager bgmManagerPrefab;
	[SerializeField] Rat ratPrefab;
	[SerializeField] Highlighter highlighterPrefab;
	public Reticle Reticle { get; private set; }

	public bool IsPaused { get; private set; }
	public static event Action<bool> Pause;

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
		DialogueManager = FindObjectOfType<DialogueManager>();
		Highlighter = Instantiate(highlighterPrefab);
	}

	void OnDisable() => SceneManager.sceneLoaded -= OnLoadNewScene;

	public void SetPause(bool paused) => Pause(paused);
}
