using UnityEngine.SceneManagement;

public static class LoadSceneManager 
{

	public static void LoadScene(int index) => SceneManager.LoadScene(index);
	public static void LoadScene(string name) => SceneManager.LoadScene(name);
}
