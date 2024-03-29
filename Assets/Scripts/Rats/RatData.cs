using UnityEngine;

[System.Serializable]
public class RatData
{
	readonly string name;
	int speed = 1;
	int strength = 1;
	int patience = 4;

	public string Name => name;
	public float SpeedModifier => (1 + speed) / 30f;
	public int SpeedValue => speed;
	public int Strength => strength;
	public int PatienceValue => patience;
	public float WanderRadius => 1f - patience * 0.025f;
	public float PatienceDuration => 2f + patience * 0.25f;
	
	readonly System.Random rand = new();

	public Color lightColor;
	public Color darkColor;

	public RatData()
	{
		name = GameManager.names[rand.Next(0, GameManager.names.Length)];
		for (int i = 0; i < 12; i++)
		{
			IncreaseRandomStat();
		}

		int colIndex = GameManager.RatIndex();
		lightColor = GameManager.ratColors[0, colIndex];
		darkColor = GameManager.ratColors[1, colIndex];


	}

	public void IncreaseRandomStat()
	{
		int i = rand.Next(0, 2);
		switch (i)
		{
			case 0:
				IncreaseStrength();
				break;
			case 1:
				IncreaseSpeed();
				break;
			case 2:
				IncreasePatience();
				break;
			default:
				break;
		}
	}

	public void IncreaseStrength(int amount = 1) => strength += amount;

	public void IncreaseSpeed(int amount = 1) => speed += amount;
	public void IncreasePatience(int amount = 1) => patience += amount;
}