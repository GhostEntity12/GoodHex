using UnityEngine;

public class Tutorialisation : MonoBehaviour
{
	// enum TutorialState { PreTutorial, HighlightMovement, MovementTask, Task, PostTutorial }
	enum TutorialState { PreTutorial, HighlightSingle, MoveRat, Multiselect, Task, PostTutorial }
    TutorialState state = TutorialState.PreTutorial;
	DialogueManager dm;

	[SerializeField] TextAsset[] dialogue;
	[SerializeField] StandardTask exampleTask;

	[SerializeField] Transform mouseHole;
	[SerializeField] Transform mouseHoleExit;

	[SerializeField] Transform firstMoveSpherePos;
	[SerializeField] float firstMoveSphereRange;
	Rat r;

	[SerializeField] DummyTask dummy;

	float timer = 1f;

	bool paused;

	private void Start()
	{
		dm = FindObjectOfType<DialogueManager>();

		GameManager.Pause += SetPaused;
	}

	void Update()
    {
		if (paused) return;

		switch (state)
		{
			case TutorialState.PreTutorial:
				timer -= Time.deltaTime;
				if (timer <= 0)
				{
					IncrementState();
				}
				break;
			case TutorialState.HighlightSingle:
				if (GameManager.Instance.RatManager.HasSelectedRats)
				{
					r = GameManager.Instance.RatManager.selectedRats[0];
					IncrementState();
				}
				break;
			case TutorialState.MoveRat:
				if (!r.Wandering && Vector3.Distance(r.transform.position, firstMoveSpherePos.position) < firstMoveSphereRange)
				{
					IncrementState();
					GameManager.Instance.RatManager.SpawnRats(mouseHole.position)[0].SetDestination(mouseHoleExit.position);
				}
				break;
			case TutorialState.Multiselect:
				if (GameManager.Instance.RatManager.selectedRats.Count == 2 && !Input.GetMouseButton(0))
				{
					dummy.SetState(BaseTask.State.Complete);
					IncrementState();
				}
				break;
			case TutorialState.Task:
				if (exampleTask.TaskState == BaseTask.State.Complete)
				{
					IncrementState();
				}
				break;
			default:
				break;
		}
	}

	void IncrementState()
	{
		dm.QueueDialogue(dialogue[(int)state]);
		state++;
	}
	void SetPaused(bool paused) => this.paused = paused;
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}
}
