using UnityEngine;
[System.Serializable]
public enum MenuState { Null, Play, Instructions, Credits };
public class MenuListener : MonoBehaviour
{
	public MenuState state;
	[SerializeField] Renderer leftPage;
	[SerializeField] Renderer centerPage;
	[SerializeField] Renderer rightPage;

	[SerializeField] Material[] pages;
	[SerializeField] GameObject[] clickAreas;

	public void SetState(int i)
	{
		state = (MenuState)i;
		Debug.Log("State set to " + state);
		Material m = state switch
		{
			MenuState.Play => pages[0],
			MenuState.Instructions => pages[1],
			MenuState.Credits => pages[2],
			_ => throw new System.Exception("Unmanaged Stage")
		};
		Material[] ms = centerPage.materials;
		ms[0] = m;
		centerPage.materials = ms;
		rightPage.material = m;
		foreach (GameObject ca in clickAreas)
		{
			ca.SetActive(false);
		}

	}
	public void OnPageFlipped()
	{
		Material m = state switch
		{
			MenuState.Play => pages[0],
			MenuState.Instructions => pages[1],
			MenuState.Credits => pages[2],
			_ => throw new System.Exception("Unmanaged Stage")
		};

		leftPage.material = m;
		Material[] ms = centerPage.materials;
		ms[1] = m;
		centerPage.materials = ms;

		clickAreas[(int)state - 1].SetActive(true);
	}
}

