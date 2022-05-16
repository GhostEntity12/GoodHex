using TMPro;
using UnityEngine;

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
	/// Transform to rotate
	/// </summary>
	[SerializeField] RectTransform clockDisplay;

	[Header("Debug")]
	[SerializeField] TextMeshProUGUI text;
	[SerializeField] TextMeshProUGUI eod;

	// Update is called once per frame
	void Update()
	{
		// Calculate the percentage of time that has passed in the day
		float timePercent = Mathf.Clamp01(time / dayLength);
		if (timePercent <= 1f)
		{
			time += Time.deltaTime;

			// Rotate the clock sprite
			// TODO: replace angles
			clockDisplay.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(45, -230, timePercent));

			// Calculating the time in 24h time
			int time24 = Mathf.FloorToInt(Mathf.Lerp(activeHours.x, activeHours.y, timePercent));

			// Display the time in 12h time (has a bunch of handling for nighttime levels)
			text.text = $"{((time24 + 23) % 12) + 1} {((time24 % 24) < 12 ? "AM" : "PM")}";
		}

		if (timePercent == 1)
		{
			// TODO: Rewrite at some point
			// End of day
			eod.enabled = true;
			Time.timeScale = 0;
		}
	}
}
