using UnityEngine;

public class HazardZoneTimed : Hazard
{
	[SerializeField] float activeTime;
	[SerializeField] float inactiveTime;
	float timer;
	bool active = false;

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (active)
		{
			if (Activate())
			{
				timer = activeTime;
			}
			if (timer >= activeTime)
			{
				active = false;
				timer = 0;
			}
		}
		else if (timer >= inactiveTime)
		{
			active = true;
			timer = 0;
		}
	}
}






