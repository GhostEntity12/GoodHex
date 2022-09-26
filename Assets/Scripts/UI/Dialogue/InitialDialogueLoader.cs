using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InitialDialogueLoader : MonoBehaviour
{

	[SerializeField] TextAsset initialScene;
	[SerializeField] float timeToDialogue;
	float timer;

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime;
		if (timer > timeToDialogue)
		{
			if (initialScene)
			{
				GameManager.Instance.DialogueManager.QueueDialogue(initialScene);
			}
			Destroy(this);
		}
	}
}
