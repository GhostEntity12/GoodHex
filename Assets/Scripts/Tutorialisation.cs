using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

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
	Material magicCircleMaterial;
	float magicCircleOpacity;
	bool magicCircleActive = false;
	private void Start()
	{
		dm = FindObjectOfType<DialogueManager>();

		GameManager.Pause += SetPaused;

		mouseDownRenderers = mouseDownPrompts.Select(p => p.transform.GetChild(0).GetComponent<SpriteRenderer>()).ToList();
		holdText = mouseDownPrompts.Select(r => r.GetComponentInChildren<TextMeshPro>()).ToList();
		magicCircleMaterial = firstMoveSpherePos.GetComponent<Renderer>().material;
	}

	void Update()
	{
		if (paused) return;
		UpdateMagicCircle();

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
				mouseDownPrompts[0].SetActive(true);
				switch (movementState)
				{
					case InMovementState.Select:
						mouseDownPrompts[0].transform.position = GameManager.Instance.RatManager.allRats[0].transform.position + Vector3.up * promptOffset;
						holdText[0].text = "Select\n)Hold(";
						if (GameManager.Instance.RatManager.selectedRats.Count > 0)
						{
							r = GameManager.Instance.RatManager.selectedRats[0];
							mouseDownRenderers[0].sprite = rightMouse;
							movementState = InMovementState.Move;
						}
						break;
					case InMovementState.Move:
						magicCircleActive = true;
						holdText[0].text = "Move";
						mouseDownPrompts[0].transform.position = firstMoveSpherePos.position + Vector3.up * promptOffset;
						if (r && !r.Wandering && Vector3.Distance(r.transform.position, firstMoveSpherePos.position) < firstMoveSphereRange)
						{
							movementState = InMovementState.Deselect;
							mouseDownRenderers[0].sprite = leftMouse;
						}
						break;
					case InMovementState.Deselect:
						magicCircleActive = false;
						holdText[0].text = "Deselect";
						mouseDownPrompts[0].transform.position = deselectPromptPosition.position + Vector3.up * promptOffset;
						if (GameManager.Instance.RatManager.selectedRats.Count == 0)
						{
							Rat r = GameManager.Instance.RatManager.SpawnRat(mouseHole.position);
							r.SetDestination(mouseHoleExit.position);
							dummy.SetState(BaseTask.State.Complete);
							mouseDownPrompts[1].SetActive(true);
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
					holdText[1].text = "Assign";
					mouseDownPrompts[0].SetActive(false);
					mouseDownPrompts[1].SetActive(true);
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

						holdText[i].text = "Select\n)Hold(";
						mouseDownPrompts[i].SetActive(showPrompt);
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
				holdText[0].text = "Assign";
				mouseDownPrompts[0].SetActive(true);
				mouseDownRenderers[0].sprite = rightMouse;
				mouseDownPrompts[1].SetActive(false);
				mouseDownPrompts[0].transform.position = finishTask.transform.position + Vector3.up * (promptOffset + 0.2f) + Vector3.right * 0.25f;
				break;
			default:
				break;
		}
	}

	void UpdateMagicCircle()
	{
		if (magicCircleActive)
		{
			magicCircleOpacity = Mathf.Clamp01(magicCircleOpacity + (Time.deltaTime * 3));
		}
		else
		{
			magicCircleOpacity = Mathf.Clamp01(magicCircleOpacity - (Time.deltaTime * 3));

		}
		magicCircleMaterial.SetFloat("_Opacity", magicCircleOpacity);

		if (magicCircleActive)
		{
			firstMoveSpherePos.Rotate(Vector3.up, 10f * Time.deltaTime, Space.World);
			firstMoveSpherePos.localScale = (0.05f * Mathf.Sin(Time.time * 2) * Vector3.one) + Vector3.one;
		}
	}

	void IncrementState()
	{
		mouseDownPrompts[0].SetActive(false);
		mouseDownPrompts[1].SetActive(false);
		dm.QueueDialogue(dialogue[(int)state], onEndAction: () =>
		{
			state++;
		});
	}
	void SetPaused(bool paused) => this.paused = paused;
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}
}
