using UnityEngine;

public class InitialDialogueLoader : MonoBehaviour
{

	[SerializeField] TextAsset initialScene;
	[SerializeField] float timeToDialogue;
	float timer;
	bool paused;

	private void Start()
	{
		GameManager.Pause += Paused;
	}
	// Update is called once per frame
	void Update()
	{
		if (paused) return;

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
	void Paused(bool paused)
	{
		this.paused = paused;
	}
}
