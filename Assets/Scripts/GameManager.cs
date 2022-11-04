using System;
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
		{new Color32(43, 61, 99, 255), new Color32(41, 78, 152, 255), new Color32(74, 110, 176, 255), new Color32(111, 144, 201, 255), new Color32(159, 127, 183, 255), new Color32(116, 87, 165, 255), new Color32(75, 68, 89, 255), new Color32(127, 164, 172, 255), new Color32(107, 69, 106, 255), new Color32(146, 54, 85, 255), new Color32(156, 182, 189, 255), new Color32(87, 46, 58, 255), new Color32(72, 66, 64, 255), new Color32(82, 85, 160, 255), new Color32(243, 230, 125, 255)},
		{new Color32(30, 31, 35, 255), new Color32(33, 40, 52, 255), new Color32(31, 80, 154, 255), new Color32(86, 81, 155, 255), new Color32(107, 69, 106, 255), new Color32(41, 78, 152, 255), new Color32(33, 40, 52, 255), new Color32(74, 110, 176, 255), new Color32(87, 46, 58, 255), new Color32(87, 46, 58, 255), new Color32(109, 146, 201, 255), new Color32(67, 22, 34, 255), new Color32(30, 28, 29, 255), new Color32(43, 61, 99, 255),  new Color32(219, 172, 79, 255)}
	};
	public static int RatIndex()
	{
		System.Random rand = new();
		return Mathf.FloorToInt((float)rand.NextDouble() / (0.99f / (ratColors.GetLength(1) - 1)));
		//return rand.NextDouble() switch
		//{
		//	<= 0.07f => 0,
		//	<= 0.14f => 1,
		//	<= 0.21f => 2,
		//	_ => 3,
		//};
	}
	static readonly string[] nonLevelScenes = new string[] { "MainMenu", "Controls", "Credits" };

	LevelMenus levelMenus;
	public ProgressBarManager ProgressBarManager { get; private set; }
	public RatManager RatManager { get; private set; }
	public TaskManager TaskManager { get; private set; }
	public DialogueManager DialogueManager { get; private set; }
	public Highlighter Highlighter { get; private set; }
	public BGMManager BGMManager { get; private set; }
	public Scorer Scorer { get; private set; }

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

	public ProgressBar CreateProgressBar() => ProgressBarManager.CreateProgressBar();

	public void AllTasksComplete()
	{
		Debug.Log("All tasks complete");
		Scorer.MarkLevelComplete();
		if (Scorer)
		{
			Debug.Log($"Final score: {Scorer.GetFinalScore()} stars");
		}
		BGMManager.StopAllTracks();
		levelMenus.TriggerVictoryScreen();
	}

	public void LevelFailed()
	{
		BGMManager.StopAllTracks();
		levelMenus.TriggerFailureScreen();
	}

	void OnLoadNewScene(Scene scene, LoadSceneMode mode)
	{
		if (Array.Exists(nonLevelScenes, e => e == scene.name)) return;

		mainCamera = Camera.main;

		Reticle = Instantiate(reticlePrefab);
		levelMenus = Instantiate(levelMenusPrefab);
		BGMManager = Instantiate(bgmManagerPrefab);
		ProgressBarManager = Instantiate(progressBarManagerPrefab);
		GameObject managers = new("Managers");
		RatManager = managers.AddComponent<RatManager>();
		TaskManager = managers.AddComponent<TaskManager>();
		RatManager.RatPrefab = ratPrefab;
		RatManager.SpawnRats();
		DialogueManager = FindObjectOfType<DialogueManager>();
		Highlighter = Instantiate(highlighterPrefab);
		Scorer = FindObjectOfType<Scorer>();
	}

	void OnDisable() => SceneManager.sceneLoaded -= OnLoadNewScene;

	public void SetPause(bool paused)
	{
		Pause(paused);
	}
}
