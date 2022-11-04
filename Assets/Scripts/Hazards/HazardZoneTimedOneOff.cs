using UnityEngine;

public class HazardZoneTimedOneOff : Hazard
{
	[SerializeField] float inactiveTime;
	float timer;

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (timer >= inactiveTime)
		{
			Activate();
			timer = 0;
		}
	}
}


