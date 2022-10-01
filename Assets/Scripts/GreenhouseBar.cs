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

	[SerializeField] TweenedElement slider;
	[SerializeField] Image indicator;
	[SerializeField] CanvasGroup vingnetteWater;
	[SerializeField] CanvasGroup vingnetteSun;
	float sliderWidth;
	[SerializeField] float buffer = 20;
	bool active;
	[SerializeField] float vignetteAppearPercent = 0.3f;
	[SerializeField] float vignetteHoldPercent = 0.2f;

	private void Start()
	{
		GameManager.Pause += OnPaused;
		slider.SetCachesAndPosition(new(0, 200));
		sliderWidth = slider.rectTransform.sizeDelta.x;
	}

	// Update is called once per frame
	void Update()
	{
		//if (!active && )
		//{

		//}
		if (!failed && !paused && active)
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
			indicator.rectTransform.anchoredPosition = new(Mathf.Lerp(buffer / 2, sliderWidth - (buffer / 2), Percentage), 0);
			vingnetteSun.alpha = Mathf.Clamp01(((value + vignetteHoldPercent) - (1 - vignetteAppearPercent)) * (1 / vignetteAppearPercent));
			vingnetteWater.alpha = Mathf.Clamp01(-((value - vignetteHoldPercent) - (-1 + vignetteAppearPercent)) * (1 / vignetteAppearPercent));
		}
	}

	public void TriggerWitchPlantDialogue(TextAsset dialogue)
	{
		GameManager.Instance.DialogueManager.QueueDialogue(dialogue, onEndAction: () => SetActive(true));
	}

	public void SetActive(bool active)
	{
		slider.SlideElement(TweenedElement.ScreenState.Onscreen, null, LeanTweenType.easeOutBack);
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
