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

	public Canvas progressCanvas;
	[Header("Prefabs")]
	public GameObject progressBarPrefab;
	public Camera mainCamera;
	public Reticle Reticle { get; private set; }
	public float CanvasScale => progressCanvas.transform.localScale.x;
	
	protected override void Awake()
	{
		base.Awake();
		mainCamera = Camera.main;

		Reticle = FindObjectOfType<Reticle>();
	}
	public void Restart() => LoadSceneManager.LoadScene(0);

	public void AllTasksComplete()
	{
		Debug.Log("All tasks complete");
		LoadSceneManager.LoadScene("VictoryScreen");
	}
}
