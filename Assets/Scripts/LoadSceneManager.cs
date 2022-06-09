public static class LoadSceneManager 
{
	public static void LoadScene(int index) => UnityEngine.SceneManagement.SceneManager.LoadScene(index);
	public static void LoadScene(string name) => UnityEngine.SceneManagement.SceneManager.LoadScene(name);
}
