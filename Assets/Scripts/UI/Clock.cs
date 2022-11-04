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

	[SerializeField] TextMeshProUGUI text;

	// Update is called once per frame
	void Update()
	{
		// Calculate the percentage of time that has passed in the day
		float levelProgress = Mathf.Clamp01(time / dayLength);
		if (levelProgress <= 1f)
		{
			time += Time.deltaTime;

			clockFill.fillAmount = levelProgress;

			// Rotate the clock sprite
			clockHand.localRotation = Quaternion.Euler(0, 0, Mathf.Lerp(-360, 0, levelProgress));

			// Calculating the time in 24h time
			int time24 = Mathf.FloorToInt(Mathf.Lerp(activeHours.x, activeHours.y, levelProgress));

			// Display the time in 12h time (has a bunch of handling for nighttime levels)
			text.text = $"{((time24 + 23) % 12) + 1} {((time24 % 24) < 12 ? "AM" : "PM")}";
		}

		if (levelProgress == 1)
		{
			GameManager.Instance.LevelFailed();
		}
	}
}
