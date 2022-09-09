using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(AudioSource))]
public class Reticle : MonoBehaviour
{
	Camera c;
	Animator anim;
	SpriteRenderer graphic;
	AudioSource aS;

	float clampedMouseWheelInput = 2;
	float circleSize = 1f;

	ProgressTask hoverTask;

	[SerializeField] AudioClip selectClip;

	[Header("Lerping")]
	[SerializeField] float lerpSpeed;
	[SerializeField] float rotationAngle;

	[Header("Colors")]
	[SerializeField] Color defaultColor;
	[SerializeField] Color ratsColor;
	[SerializeField] Color taskColor;
	[SerializeField] float colorChangeSpeed = 10f;
	Color targetColor;

	readonly List<Rat> ratsInHold = new();

	RatManager ratManager;
	PickUp pickUp;

	Vector3 reticlePosition;

	bool mouseLocked = false;

	bool paused;

	float pointerDownTimer;

	public float requiredHoldTime;

	void Start()
	{
		c = Camera.main;
		aS = GetComponent<AudioSource>();

		anim = GetComponentInChildren<Animator>();
		graphic = GetComponentInChildren<SpriteRenderer>();
		ratManager = GameManager.Instance.RatManager;
		GameManager.Pause += SetPaused;
	}

	void Update()
	{
		if (paused)
		{
			SetColor(new(0, 0, 0, 0));
			return;
		}
		

		if (Physics.Raycast(c.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 << 8 | 1 << 10) && hit.normal.y > 0)
        {
			graphic.enabled = true;
			if (!mouseLocked)
			{
				reticlePosition = hit.point;
			}
			else
			{
				if (Vector3.Distance(hit.point, reticlePosition) > circleSize /2)
                {
					mouseLocked = false;
					anim.SetBool("Active", false);
					pointerDownTimer = 0;
				}
			}
        }
        else
        {
			graphic.enabled = false;
        }

		SetSize();
		SetPosition();
		SetColor();

		SelectDeselect();
		Assign();
	}

	void SetColor()
	{
		bool unselectedRats =
					ratManager.allRats
					.Where(r =>
						Vector3.Distance(transform.position, r.transform.position) < 0.25f * circleSize &&
						!ratManager.selectedRats.Contains(r))
					.Any();

		targetColor =
			(mouseLocked && unselectedRats) || ratManager.HasSelectedRats
			? hoverTask && hoverTask.TaskState == BaseTask.State.Unlocked
				? taskColor
				: ratsColor
			: defaultColor;

		graphic.color = ExtensionMethods.ColorMoveTowards(graphic.color, targetColor, colorChangeSpeed * Time.deltaTime);
	}

	void SetColor(Color color) => graphic.color = ExtensionMethods.ColorMoveTowards(graphic.color, color, colorChangeSpeed * Time.deltaTime);

	void SetSize()
	{
		// Mouse wheel to set size
		clampedMouseWheelInput = Mathf.Clamp(clampedMouseWheelInput + Input.mouseScrollDelta.y, 0, 4);
		circleSize = clampedMouseWheelInput / 2 + 1;
		transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(circleSize, 1, circleSize), lerpSpeed); // Reset Y scale to prevent lifting
	}

	void SetPosition()
	{
		// Raycast to set reticle position
		// 1 << 8 is Surface, hit.normal is > 0 when it is vaguely upwards
		if (Physics.Raycast(c.transform.position, reticlePosition - c.transform.position, out RaycastHit hit, Mathf.Infinity, 1 << 8) && hit.normal.y > 0)
		{
			transform.position = hit.point;
		}
	}

	void SelectDeselect()
	{
		if (!(Physics.Raycast(c.transform.position, reticlePosition - c.transform.position, out RaycastHit hit, Mathf.Infinity, 1 << 8) && hit.normal.y > 0)) return;
		
		if (Input.GetMouseButtonDown(0))
		{
			mouseLocked = true;
		}

		if (mouseLocked)
		{
		//Debug.Log("Button down time = "+pointerDownTimer);
		pointerDownTimer += Time.deltaTime;
		anim.SetBool("Active", true);

		if (pointerDownTimer >= requiredHoldTime)
        {
			// Get all unselected rats within the circle
			List<Rat> unselectedRats =
				ratManager.allRats
				.Where(r =>
					Vector3.Distance(transform.position, r.transform.position) < 0.25f * circleSize &&
					!ratManager.selectedRats.Contains(r))
				.ToList();

			if (unselectedRats.Count > 0) // Select rats
			{
				if (ratManager.selectedRats.Count == 0)
				{
					aS.PlayOneShot(selectClip);
				}
				ratManager.SelectRats(unselectedRats);
				ratsInHold.AddRange(unselectedRats);
			}
			mouseLocked = false;
		}
	}
		// Mouse release - Paintbrush select
		if (Input.GetMouseButtonUp(0))
		{
			//Debug.Log("Button up time = " + pointerDownTimer);
			mouseLocked = false;
			pointerDownTimer = 0;

			if (ratsInHold.Count == 0) // Deselect
			{
				ratManager.ClearRats();
				anim.SetBool("Active", false);
			}
			ratsInHold.Clear();
		}
	}

    void Assign()
	{
		if (Input.GetMouseButtonDown(1))
		{
			//PickUp code
			if (Physics.Raycast(c.transform.position, reticlePosition - c.transform.position, out RaycastHit hitPickup, Mathf.Infinity, 1 << 10))
			{
				if (hitPickup.transform.TryGetComponent(out Pickupable pickupable))
				{
					Rat closestRat = ratManager.selectedRats.OrderBy(r => Vector3.Distance(r.transform.position, hitPickup.transform.position)).FirstOrDefault();
					ratManager.SetRatDestinations(transform.position);
					pickupable.AssignRat(closestRat);
				}
			}

			else if (hoverTask && hoverTask.TaskState == BaseTask.State.Locked)
            {
				ratManager.SetRatDestinations(transform.position);
				ratManager.ClearRats();
			}

			else if (hoverTask && hoverTask.TaskState == BaseTask.State.Unlocked) // Assign to task
			{
				List<Rat> remainingRats = GameManager.Instance.TaskManager.AssignRatsToTask(ratManager.selectedRats, hoverTask);
				// Clear selected rats
				ratManager.ClearRats();
				// Select the rats without tasks
				ratManager.SelectRats(remainingRats);
				// Set their destinations
				ratManager.SetRatDestinations(transform.position);
				ratManager.ClearRats();
				anim.SetBool("Active", false);
			}
			else if (Physics.Raycast(c.transform.position, reticlePosition - c.transform.position, out RaycastHit hit, Mathf.Infinity, 1 << 8) && hit.normal.y > 0)
			{
				ratManager.SetRatDestinations(transform.position);
				GameManager.Instance.TaskManager.UnassignRats(ratManager.selectedRats.ToArray());
			}
		}
	}

	public void SetTask(ProgressTask task) => hoverTask = task;

	void SetPaused(bool paused) => this.paused = paused;
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}
}
