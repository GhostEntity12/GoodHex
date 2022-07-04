using TMPro;
using UnityEngine;
using UnityEngine.UI;
public class Clock : MonoBehaviour
{
	float time = 0;
	/// <summary>
	/// Day length in seconds
	/// </summary>
	[SerializeField] float dayLength = 60;
	/// <summary>
	/// Starting and ending times
	/// </summary>
	[SerializeField] Vector2Int activeHours;
	/// <summary>
	/// Hand transform to rotate
	/// </summary>
	[SerializeField] RectTransform clockHand;
	/// <summary>
	/// Image to track time spent
	/// </summary>
	[SerializeField] Image clockFill;
	/// <summary>
	/// Bool to check if audio is playing
	/// </summary>
	private bool isPlaying = false;

	[SerializeField] Canvas gameOverCanvas;

	[SerializeField] TextMeshProUGUI text;
	[SerializeField] BGMManager bgmManager;

	// Update is called once per frame
	void Update()
	{
		// Calculate the percentage of time that has passed in the day
		float timePercent = Mathf.Clamp01(time / dayLength);
		if (timePercent <= 1f)
		{
			time += Time.deltaTime;

			clockFill.fillAmount = timePercent;

			// Rotate the clock sprite
			clockHand.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-360, 0, timePercent));

			// Calculating the time in 24h time
			int time24 = Mathf.FloorToInt(Mathf.Lerp(activeHours.x, activeHours.y, timePercent));

			// Display the time in 12h time (has a bunch of handling for nighttime levels)
			text.text = $"{((time24 + 23) % 12) + 1} {((time24 % 24) < 12 ? "AM" : "PM")}";

			if (timePercent >= 0.8f) {
				bgmManager.TriggerEndMusic();
			}
		}

		if (timePercent == 1)
		{
			// TODO: Rewrite at some point
			// End of day
			//eod.enabled = true;
			//LoadSceneManager.LoadScene("GameOver");
			bgmManager.StopAllTracks();
			gameOverCanvas.gameObject.SetActive(true);
		}
	}
}
