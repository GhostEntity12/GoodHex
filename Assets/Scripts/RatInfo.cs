using UnityEngine;

[System.Serializable]
public class RatData
{
	string name;
	int speed = 1;
	int strength = 1;
	int patience = 1;

	public string Name => name;
	public float SpeedModifier => 1 + speed / 10f;
	public int SpeedValue => speed;
	public int Strength => strength;
	public int PatienceValue => patience;
	public float WanderRadius => 5f - patience * 0.25f;
	public float PatienceDuration => 2f + patience * 0.25f;

	public RatData()
	{
		name = GameManager.names[Random.Range(0, GameManager.names.Length)];
		for (int i = 0; i < 12; i++)
		{
			IncreaseRandomStat();
		}
	}

	public void IncreaseRandomStat()
	{
		int i = Random.Range(0, 2);
		switch (i)
		{
			case 0:
				IncreaseStrength();
				break;
			case 1:
				IncreaseSpeed();
				break;
			default:
				break;
		}
	}

	public void IncreaseStrength(int amount = 1) => strength += amount;

	public void IncreaseSpeed(int amount = 1) => speed += amount;
}