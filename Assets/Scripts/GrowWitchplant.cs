using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrowWitchplant : MonoBehaviour
{
	int state = -1;
	[SerializeField] Animator[] anims;
	[SerializeField] GreenhouseBar bar;
	[SerializeField] TextAsset finalDialogue;
	public void IncrementGrowthState()
	{
		state++;
		foreach (Animator anim in anims)
		{
			anim.SetInteger("GrowthStage", state);
		}

		if (state == 15)
		{
			GameManager.Instance.DialogueManager.QueueDialogue(finalDialogue, onEndAction: GameManager.Instance.AllTasksComplete);
			bar.SetActive(false);
		}
	}
	public void IncrementGrowthState(int count)
	{
		state += count;
		foreach (Animator anim in anims)
		{
			anim.SetInteger("GrowthStage", state);
		}

		if (state == 15)
		{
			GameManager.Instance.DialogueManager.QueueDialogue(finalDialogue, onEndAction: GameManager.Instance.AllTasksComplete);
			bar.SetActive(false);
		}
	}
}
