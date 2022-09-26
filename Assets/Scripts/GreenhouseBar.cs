using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GreenhouseBar : MonoBehaviour
{
	bool paused;
	float Percentage => (value + 1) / 2f;

	[SerializeField] float time = 30f;
	float value = 0;
	bool failed;

	bool gettingSun = true;

	[SerializeField] Image slider;
	[SerializeField] Image indicator;
	[SerializeField] CanvasGroup vingnetteWater;
	[SerializeField] CanvasGroup vingnetteSun;
	float sliderWidth;
	[SerializeField] float buffer = 20;
	bool active;
	float vignetteAppearPercent;

	private void Start()
	{
		GameManager.Pause += OnPaused;
		sliderWidth = slider.rectTransform.sizeDelta.x - buffer;
	}

	// Update is called once per frame
	void Update()
	{
		if ((!failed || !paused) && active)
		{
			switch (value)
			{
				case -1:
					Debug.Log("Lost: Not enough sun");
					failed = true;
					GameManager.Instance.LevelFailed();
					return;
				case 1:
					Debug.Log("Lost: Not enough water");
					failed = true;
					GameManager.Instance.LevelFailed();
					return;
				default:
					value += Time.deltaTime / time * (gettingSun ? 1 : -1);
					break;
			}
			value = Mathf.Clamp(value, -1, 1);
			indicator.rectTransform.anchoredPosition = new(Percentage * sliderWidth + buffer / 2, 0);
			vingnetteWater.alpha = Mathf.Clamp01((value - (1 - vignetteAppearPercent) * (1 / vignetteAppearPercent)));
			vingnetteSun.alpha = Mathf.Clamp01((value + (1 - vignetteAppearPercent) * (1 / vignetteAppearPercent)));
		}
	}

	public void SetActive(bool active)
	{
		slider.enabled = active;
		indicator.enabled = active;
		this.active = active;
	}
	public void ToggleState() => gettingSun = !gettingSun;
	public void SetState(bool setSun)
	{
		Debug.Log("Setting State: " + setSun);
		gettingSun = setSun;
	}

	void OnPaused(bool paused) => this.paused = paused;
	void OnDestroy() => GameManager.Pause -= OnPaused;
}
