using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowWitchplant : MonoBehaviour
{
	int state = -1;
	[SerializeField] Animator[] anims;
	[SerializeField] GreenhouseBar bar;
	public void IncrementGrowthState()
	{
		state++;
		foreach (Animator anim in anims)
		{
			anim.SetInteger("GrowthStage", state);
		}

		if (state == 15)
		{
			GameManager.Instance.AllTasksComplete();
			bar.SetActive(false);
		}
	}
}
