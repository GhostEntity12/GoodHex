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
    [SerializeField] SpriteRenderer mouseDownPrompt1;
    [SerializeField] SpriteRenderer mouseDownPrompt2;
    [SerializeField] TextMeshPro holdText;
    [SerializeField] Sprite leftMouse;
    [SerializeField] Sprite rightMouse;

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
            case TutorialState.HighlightMovement:
                switch (movementState)
                {
                    case InMovementState.Select:
                        mouseDownPrompt1.transform.position = GameManager.Instance.RatManager.allRats[0].transform.position + Vector3.up * promptOffset;
                        if (GameManager.Instance.RatManager.selectedRats.Count > 0)
                        {
                            movementState = InMovementState.Move;
                            r = GameManager.Instance.RatManager.selectedRats[0];
                        }
                        break;
                    case InMovementState.Move:
                        mouseDownPrompt1.transform.position = firstMoveSpherePos.position + Vector3.up * promptOffset;
                        if (r && !r.Wandering && Vector3.Distance(r.transform.position, firstMoveSpherePos.position) < firstMoveSphereRange)
                        {
                            movementState = InMovementState.Deselect;
                        }
                        break;
                    case InMovementState.Deselect:
                        mouseDownPrompt1.transform.position = deselectPromptPosition.position + Vector3.up * promptOffset;
                        if (GameManager.Instance.RatManager.selectedRats.Count == 0)
                        {
                            GameManager.Instance.RatManager.SpawnRats(mouseHole.position)[0].SetDestination(mouseHoleExit.position);
                            dummy.SetState(BaseTask.State.Complete);
                            IncrementState();
                            mouseDownPrompt2.gameObject.SetActive(true);
                        }
                        break;
                    default:
                        break;
                }
                break;
            case TutorialState.MovementTask:
                if (!GameManager.Instance.RatManager.selectedRats.Contains(GameManager.Instance.RatManager.allRats[0]) && !GameManager.Instance.RatManager.allRats[0].AssignedToTask)
                {
                    mouseDownPrompt1.transform.position = GameManager.Instance.RatManager.allRats[0].transform.position + Vector3.up * promptOffset;
                }
                else
                {
                    mouseDownPrompt1.gameObject.SetActive(false);
                }
                if (!GameManager.Instance.RatManager.selectedRats.Contains(GameManager.Instance.RatManager.allRats[1]) && !GameManager.Instance.RatManager.allRats[1].AssignedToTask)
                {
                    mouseDownPrompt2.transform.position = GameManager.Instance.RatManager.allRats[1].transform.position + Vector3.up * promptOffset;
                }
                else
                {
                    mouseDownPrompt2.gameObject.SetActive(false);
                }

                if (GameManager.Instance.RatManager.selectedRats.Count == 2)
                {
                    mouseDownPrompt2.transform.position = exampleTask.transform.position + Vector3.up * promptOffset;
                    mouseDownPrompt2.sprite = rightMouse;
                }
                if (exampleTask.IsComplete)
                {
                    IncrementState();
                }
                break;
            case TutorialState.Task:
                mouseDownPrompt1.transform.position = finishTask.transform.position + Vector3.up * promptOffset;
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
