using UnityEngine;

public class Scorer : MonoBehaviour
{
	public Threshold time, tasks, deaths;

	bool paused;
	bool levelComplete;

	private void Awake()
	{
		GameManager.Pause += Paused;
	}
	private void Update()
	{
		if (!paused && !levelComplete)
		{
			time.IncrementValue(Time.deltaTime);
		}
	}

	public void AddTask(int value) => tasks.IncrementValue(value);
	public void AddDeath() => deaths.IncrementValue(1);

	void Paused(bool paused) => this.paused = paused;

	public void MarkLevelComplete() => levelComplete = true;
	public int GetFinalScore() => Mathf.FloorToInt((time.GetStars() + tasks.GetStars() + deaths.GetStars()) / 3f);
}

[System.Serializable]
public class Threshold
{
	float value;
	[SerializeField] bool lowerBetter;
	[SerializeField] float threeStarThreshold;
	[SerializeField] float twoStarThreshold;
	[SerializeField] float oneStarThreshold;
	public void IncrementValue(float amount) => value += amount;

	public int GetStars()
	{
		if (lowerBetter)
		{
			return value switch
			{
				float v when v <= threeStarThreshold => 3,
				float v when v <= twoStarThreshold => 2,
				float v when v <= oneStarThreshold => 1,
				_ => 0
			};
		}
		else
		{
			return value switch
			{
				float v when v >= threeStarThreshold => 3,
				float v when v >= twoStarThreshold => 2,
				float v when v >= oneStarThreshold => 1,
				_ => 0
			};
		}
	}
}
