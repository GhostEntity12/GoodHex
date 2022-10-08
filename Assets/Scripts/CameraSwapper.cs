using UnityEngine;
using UnityEngine.UI;

public class CameraSwapper : MonoBehaviour
{
	Camera c;

	int index;
	bool swapping;
	bool fadingOut;

	[SerializeField] Transform[] cameraPositions;
	[SerializeField] Sprite[] swapImageSprites;
	[SerializeField] Image swapImage;

	[SerializeField] CanvasGroup fade;
	[SerializeField] float fadeDuration;

	void Start()
	{
		c = Camera.main;
		c.transform.SetPositionAndRotation(cameraPositions[index].position, cameraPositions[index].rotation);
	}

	private void Update()
	{
		if (swapping)
		{
			if (fadingOut)
			{
				fade.alpha += Time.deltaTime / fadeDuration;
				if (fade.alpha == 1)
				{
					fadingOut = false;
					c.transform.SetPositionAndRotation(cameraPositions[index].position, cameraPositions[index].rotation);
					swapImage.sprite = swapImageSprites[index];
					GameManager.Instance.ProgressBarManager.UpdatePositions();
					GameManager.Instance.RatManager.ClearRats();
				}
			}
			else
			{
				fade.alpha -= Time.deltaTime / fadeDuration;
				if (fade.alpha == 0)
				{
					swapping = false;
				}
			}
		}
		else
		{

			if (Input.GetKeyDown(KeyCode.PageDown))
			{
				Decrement();
			}
			if (Input.GetKeyDown(KeyCode.PageUp))
			{
				Increment();
			}
		}
	}

	public void WhenButtonClicked()
	{
		if (!swapping)
		{
			Increment();
		}
	}

	void Increment()
	{
		index = (index + 1 + cameraPositions.Length) % cameraPositions.Length;
		swapping = fadingOut = true;
	}

	void Decrement()
	{
		index = (index - 1 + cameraPositions.Length) % cameraPositions.Length;
		swapping = fadingOut = true;
	}
}
