using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraSwapper : MonoBehaviour
{
	Camera c;

	int index;
	bool swapping;
	bool fadingOut;

	[SerializeField] Transform[] cameraPositions;
	
	[SerializeField] CanvasGroup fade;
	[SerializeField] float fadeDuration;

	void Start()
	{
		c = Camera.main;
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
		/*else
		{

			if (Input.GetKeyDown(KeyCode.PageDown))
			{
				Decrement();
			}
			if (Input.GetKeyDown(KeyCode.PageUp))
			{
				Increment();
			}
		}*/
	}

	public void whenButtonClicked()
    {
		Increment();
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
