using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowWitchplant : MonoBehaviour
{
	int state;
	Animator anim;
	private void Start()
	{
		anim = GetComponent<Animator>();
	}
	public void IncrementGrowthState()
	{
		state++;
		if (state is 1 or 3 or 6 or 9 or 12 or 14)
		{
			anim.SetTrigger("Grow");
		}

		if (state == 15)
		{
			GameManager.Instance.AllTasksComplete();
		}
	}
}
