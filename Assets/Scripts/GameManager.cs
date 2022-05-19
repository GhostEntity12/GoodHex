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

	public void Restart() => SceneManager.LoadScene(0);

	public void AllTasksComplete()
	{
		Debug.Log("All tasks complete");
	}
}
