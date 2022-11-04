using UnityEngine;

public class GrowWitchplant : MonoBehaviour
{
	int state = -1;
	[SerializeField] Animator[] anims;
	[SerializeField] GreenhouseBar bar;
	[SerializeField] TextAsset finalDialogue;

	[SerializeField] Rune[] runes1;
	[SerializeField] Rune[] runes2;
	[SerializeField] AudioSource as1;
	[SerializeField] AudioSource as2;

	public void IncrementGrowthState()
	{
		state++;
		foreach (Animator anim in anims)
		{
			anim.SetInteger("GrowthStage", state);
		}

		if (state >= 15)
		{
			GameManager.Instance.DialogueManager.QueueDialogue(finalDialogue, onEndAction: GameManager.Instance.AllTasksComplete);
			bar.SetActive(false);
		}
		UpdateRunes();
	}
	public void IncrementGrowthState(int count)
	{
		for (int i = 0; i < count; i++)
		{
			IncrementGrowthState();
		}
	}

	void UpdateRunes()
	{
		if (state > 14)
		{
			ActivateRunes(6);
		}
		if (state > 12)
		{
			ActivateRunes(5);
		}
		if (state > 9)
		{
			ActivateRunes(4);
		}
		if (state > 7)
		{
			ActivateRunes(3);
		}
		if (state > 5)
		{
			ActivateRunes(2);
		}
		if (state > 2)
		{
			ActivateRunes(1);
		}
		if (state > 0)
		{
			ActivateRunes(0);
		}
	}

	void ActivateRunes(int index)
	{
		runes1[index].Enable();
		runes2[index].Enable();
		as1.Play();
		as2.Play();
	}
}
