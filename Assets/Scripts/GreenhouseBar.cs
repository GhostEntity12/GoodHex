using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GreenhouseBar : MonoBehaviour
{
	bool paused;
	float Percentage => (value + 1) / 2f;

	[SerializeField] float time = 30f;
	float value = 0;
	bool failed;

	[SerializeField] bool gettingSun;

	[SerializeField] Image slider;
	[SerializeField] Image indicator;
	float sliderWidth;
	[SerializeField] float buffer = 20;

	private void Start()
	{
		GameManager.Pause += OnPaused;
		sliderWidth = slider.rectTransform.sizeDelta.x - buffer;
	}

	// Update is called once per frame
	void Update()
	{
		Debug.Log(Percentage);
		if (!failed || !paused)
		{
			switch (value)
			{
				case -1:
					Debug.Log("Lost: Not enough sun");
					failed = true;
					return;
				case 1:
					Debug.Log("Lost: Not enough water");
					failed = true;
					return;
				default:
					value += Time.deltaTime / time * (gettingSun ? 1 : -1);
					break;
			}
			value = Mathf.Clamp(value, -1, 1);
			indicator.rectTransform.anchoredPosition = new(Percentage * sliderWidth + buffer / 2, 0);
		}
	}

	public void ToggleState() => gettingSun = !gettingSun;

	void OnPaused(bool paused) => this.paused = paused;
	void OnDestroy() => GameManager.Pause -= OnPaused;
}
