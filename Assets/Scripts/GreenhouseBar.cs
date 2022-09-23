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
	float sliderWidth;
	[SerializeField] float buffer = 20;
	bool active;

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
		}
	}

	public void Activate()
	{
		slider.enabled = true;
		indicator.enabled = true;
		active = true;
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
