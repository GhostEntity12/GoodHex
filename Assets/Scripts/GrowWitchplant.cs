using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowWitchplant : MonoBehaviour
{
	int state;
	[SerializeField] Animator[] anims;
	[SerializeField] GreenhouseBar bar;
	public void IncrementGrowthState()
	{
		state++;
		if (state is 1 or 3 or 6 or 9 or 12 or 14)
		{
			foreach (Animator anim in anims)
			{
				anim.SetTrigger("Grow");
			}
		}

		if (state == 15)
		{
			GameManager.Instance.AllTasksComplete();
			bar.SetActive(false);
		}
	}
}
