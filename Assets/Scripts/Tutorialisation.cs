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

    [SerializeField] Transform mouseHole;
    [SerializeField] Transform mouseHoleExit;

    [SerializeField] Transform firstMoveSpherePos;
    [SerializeField] float firstMoveSphereRange;
    Rat r;

    [SerializeField] DummyTask dummy;

    float timer = 1f;

    bool paused;

    [SerializeField] float promptOffset = 0.25f;
    [SerializeField] SpriteRenderer mouseDownPrompt;
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
                        mouseDownPrompt.transform.position = GameManager.Instance.RatManager.allRats[0].transform.position + Vector3.up * promptOffset;
                        if (GameManager.Instance.RatManager.selectedRats.Count > 0)
                        {
                            movementState = InMovementState.Move;
                            r = GameManager.Instance.RatManager.selectedRats[0];
                        }
                        break;
                    case InMovementState.Move:
                        mouseDownPrompt.transform.position = firstMoveSpherePos.position + Vector3.up * promptOffset;
                        if (r && !r.Wandering && Vector3.Distance(r.transform.position, firstMoveSpherePos.position) < firstMoveSphereRange)
                        {
                            movementState = InMovementState.Deselect;
                        }
                        break;
                    case InMovementState.Deselect:
                        if (GameManager.Instance.RatManager.selectedRats.Count == 0)
                        {
                            GameManager.Instance.RatManager.SpawnRats(mouseHole.position)[0].SetDestination(mouseHoleExit.position);
                            IncrementState();
                        }
                        break;
                    default:
                        break;
                }
                break;
            case TutorialState.MovementTask:
                if (!r.Wandering && Vector3.Distance(r.transform.position, firstMoveSpherePos.position) < firstMoveSphereRange)
                {
                    IncrementState();
                }
                break;
            case TutorialState.Task:
                if (GameManager.Instance.RatManager.selectedRats.Count == 2 && !Input.GetMouseButton(0))
                {
                    dummy.SetState(BaseTask.State.Complete);
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
