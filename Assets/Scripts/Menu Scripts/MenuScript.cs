using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuScript : MonoBehaviour
{
	public void LoadScene(string sceneName) => StartCoroutine(DelayLoadScene(sceneName));
	private IEnumerator DelayLoadScene(string sceneName) {
		yield return new WaitForSeconds(1);
		SceneManager.LoadScene(sceneName);
	}
	public void Quit()
	{
		Application.Quit();
#if UNITY_EDITOR
		UnityEditor.EditorApplication.isPlaying = false;
#endif
	}
}
