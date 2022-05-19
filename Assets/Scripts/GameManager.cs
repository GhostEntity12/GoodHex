using UnityEngine;

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

	public Canvas mainCanvas;
	[Header("Prefabs")]
	public GameObject progressBarPrefab;
	public Camera mainCamera;
	
	protected override void Awake()
	{
		base.Awake();
		mainCamera = Camera.main;
	}
	public void Restart() => SceneManager.LoadScene(0);

	public void AllTasksComplete()
	{
		Debug.Log("All tasks complete");
	}
}
