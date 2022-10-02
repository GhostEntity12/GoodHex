using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using TMPro;

public class Tutorialisation : MonoBehaviour
{
	enum TutorialState { PreTutorial, HighlightMovement, MovementTask, Task, PostTutorial }
	//enum TutorialState { PreTutorial, HighlightSingle, MoveRat, Multiselect, Task, PostTutorial }
	enum InMovementState { Select, Move, Deselect }
	TutorialState state = TutorialState.PreTutorial;
	InMovementState movementState = InMovementState.Select;
	DialogueManager dm;

	[SerializeField] TextAsset[] dialogue;
	[SerializeField] StandardTask exampleTask;
	[SerializeField] StandardTask finishTask;

	[SerializeField] Transform mouseHole;
	[SerializeField] Transform mouseHoleExit;

	[SerializeField] Transform deselectPromptPosition;

	[SerializeField] Transform firstMoveSpherePos;
	[SerializeField] float firstMoveSphereRange;
	Rat r;

	[SerializeField] DummyTask dummy;

	float timer = 1f;

	bool paused;

	[SerializeField] float promptOffset = 0.25f;
	[SerializeField] List<GameObject> mouseDownPrompts;
	List<SpriteRenderer> mouseDownRenderers;
	List<TextMeshPro> holdText;
	[SerializeField] Sprite leftMouse;
	[SerializeField] Sprite rightMouse;

	private void Start()
	{
		dm = FindObjectOfType<DialogueManager>();

		GameManager.Pause += SetPaused;

		mouseDownRenderers = mouseDownPrompts.Select(p => p.transform.GetChild(0).GetComponent<SpriteRenderer>()).ToList();
		holdText = mouseDownPrompts.Select(r => r.GetComponentInChildren<TextMeshPro>()).ToList();
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
			case TutorialState.HighlightMovement:
				switch (movementState)
				{
					case InMovementState.Select:
						mouseDownPrompts[0].transform.position = GameManager.Instance.RatManager.allRats[0].transform.position + Vector3.up * promptOffset;
						holdText[0].enabled = true;
						if (GameManager.Instance.RatManager.selectedRats.Count > 0)
						{
							movementState = InMovementState.Move;
							r = GameManager.Instance.RatManager.selectedRats[0];
							mouseDownRenderers[0].sprite = rightMouse;
							holdText[0].enabled = false;
						}
						break;
					case InMovementState.Move:
						mouseDownPrompts[0].transform.position = firstMoveSpherePos.position + Vector3.up * promptOffset;
						if (r && !r.Wandering && Vector3.Distance(r.transform.position, firstMoveSpherePos.position) < firstMoveSphereRange)
						{
							movementState = InMovementState.Deselect;
							mouseDownRenderers[0].sprite = leftMouse;
						}
						break;
					case InMovementState.Deselect:
						mouseDownPrompts[0].transform.position = deselectPromptPosition.position + Vector3.up * promptOffset;
						holdText[0].enabled = false;
						if (GameManager.Instance.RatManager.selectedRats.Count == 0)
						{
							Rat r = GameManager.Instance.RatManager.SpawnRats(mouseHole.position)[0];
							Debug.Log(r);
							r.SetDestination(mouseHoleExit.position);
							dummy.SetState(BaseTask.State.Complete);
							mouseDownPrompts[1].gameObject.SetActive(true);
							holdText[0].enabled = true;
							holdText[1].enabled = true;
							IncrementState();
						}
						break;
					default:
						break;
				}
				break;
			case TutorialState.MovementTask:
				if (GameManager.Instance.RatManager.selectedRats.Count == 2)
				{
					holdText[0].enabled = false;
					holdText[1].enabled = false;
					mouseDownPrompts[0].gameObject.SetActive(false);
					mouseDownPrompts[1].gameObject.SetActive(true);
					mouseDownPrompts[1].transform.position = exampleTask.transform.position + Vector3.up * (promptOffset + 0.1f);
					mouseDownRenderers[1].sprite = rightMouse;
				}
				else
				{
					for (int i = 0; i < 2; i++)
					{
						bool showPrompt =
							!GameManager.Instance.RatManager.selectedRats.Contains(GameManager.Instance.RatManager.allRats[i]) &&
							!GameManager.Instance.RatManager.allRats[i].AssignedToTask;

						mouseDownPrompts[i].gameObject.SetActive(showPrompt);
						if (showPrompt)
						{
							mouseDownPrompts[i].transform.position = GameManager.Instance.RatManager.allRats[i].transform.position + Vector3.up * promptOffset;
						}
					}
				}

				if (exampleTask.IsComplete)
				{
					IncrementState();
					break;
				}
				break;
			case TutorialState.Task:
				mouseDownPrompts[0].gameObject.SetActive(true);
				mouseDownRenderers[0].sprite = rightMouse;
				mouseDownPrompts[1].gameObject.SetActive(false);
				mouseDownPrompts[0].transform.position = finishTask.transform.position + Vector3.up * (promptOffset + 0.1f) + Vector3.right * 0.5f;
				break;
			default:
				break;
		}
	}

	void IncrementState()
	{
		dm.QueueDialogue(dialogue[(int)state], onEndAction: () => state++);
		state++;
	}
	void SetPaused(bool paused) => this.paused = paused;
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}
}
