using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
	[SerializeField] Image[] timeStars;
	[SerializeField] Image[] taskStars;
	[SerializeField] Image[] deathStars;
	[SerializeField] Sprite[] trophies;
	[SerializeField] Image[] totalStars;
	private void OnEnable()
	{
		Debug.Log("Calculating");
		Scorer s = GameManager.Instance.Scorer;
		if (s)
		{
			SetStars(s.time.GetStars(), timeStars);
			SetStars(s.tasks.GetStars(), taskStars);
			SetStars(s.deaths.GetStars(), deathStars);
			SetStars(s.GetFinalScore(), totalStars);
		}
		else
		{
			gameObject.SetActive(false);
		}
	}

	void SetStars(int activeStars, Image[] starImages)
	{
		starImages[0].sprite = trophies[activeStars];
		starImages[0].gameObject.SetActive(true);
		// For three trophies
		//for (int i = 0; i < totalStars.Length; i++)
		//{
		//	starImages[i].gameObject.SetActive(i < activeStars);
		//}
	}
}
