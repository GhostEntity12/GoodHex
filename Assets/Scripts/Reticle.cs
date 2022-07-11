using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Reticle : MonoBehaviour
{
	Camera c;
	Animator anim;
	SpriteRenderer graphic;
	AudioSource aS;

	float clampedMouseWheelInput = 2;
	float circleSize = 1f;

	StandardTask hoverTask;

	[SerializeField] AudioClip selectClip;

	[Header("Colors")]
	[SerializeField] Color defaultColor;
	[SerializeField] Color ratsColor;
	[SerializeField] Color taskColor;
	[SerializeField] float colorChangeSpeed = 10f;
	Color targetColor;

	readonly List<Rat> ratsInHold = new();

	void Awake()
	{
		c = Camera.main;
		aS = GetComponent<AudioSource>();

		anim = GetComponentInChildren<Animator>();
		graphic = GetComponentInChildren<SpriteRenderer>();
	}

	void Update()
	{
		SetSize();
		SetPosition();
		SetColor();

		SelectDeselect();
		Assign();
	}

	void SetColor()
	{
		targetColor =
			RatManager.Instance.HasSelectedRats
			? hoverTask && hoverTask.TaskState == BaseTask.State.Unlocked
				? taskColor
				: ratsColor
			: defaultColor;

		graphic.color = ExtensionMethods.ColorMoveTowards(graphic.color, targetColor, colorChangeSpeed * Time.deltaTime);
	}

	void SetSize()
	{
		// Mouse wheel to set size
		clampedMouseWheelInput = Mathf.Clamp(clampedMouseWheelInput + Input.mouseScrollDelta.y, 0, 4);
		circleSize = clampedMouseWheelInput / 2 + 1;
		transform.localScale = new Vector3(circleSize, 1, circleSize); // Reset Y scale to prevent lifting
	}

	void SetPosition()
	{
		// Raycast to set reticle position
		// 1 << 8 is Surface, hit.normal is > 0 when it is vaguely upwards
		if (Physics.Raycast(c.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 << 8) && hit.normal.y > 0)
		{
			graphic.enabled = true;
			transform.position = hit.point;
		}
		else
		{
			graphic.enabled = false;
		}
	}

	void SelectDeselect()
	{
		if (Input.GetMouseButton(0))
		{
			// Get all unselected rats within the circle
			List<Rat> unselectedRats =
				RatManager.Instance.allRats
				.Where(r =>
					Vector3.Distance(transform.position, r.transform.position) < 0.25f * circleSize &&
					!RatManager.Instance.selectedRats.Contains(r))
				.ToList();

			if (unselectedRats.Count > 0) // Select rats
			{
				if (RatManager.Instance.selectedRats.Count == 0)
				{
					aS.PlayOneShot(selectClip);
				}
				RatManager.Instance.SelectRats(unselectedRats);
				ratsInHold.AddRange(unselectedRats);
				anim.SetBool("Active", true);
			}
		}
		// Mouse release - Paintbrush select
		if (Input.GetMouseButtonUp(0))
		{
			if (ratsInHold.Count == 0) // Deselect
			{
				RatManager.Instance.ClearRats();
				anim.SetBool("Active", false);
			}
			ratsInHold.Clear();
		}
	}

	void Assign()
	{
		if (Input.GetMouseButtonDown(1))
		{
			if (hoverTask) // Assign to task
			{
				List<Rat> remainingRats = TaskManager.Instance.AssignRatsToTask(RatManager.Instance.selectedRats, hoverTask);
				// Clear selected rats
				RatManager.Instance.ClearRats();
				// Select the rats without tasks
				RatManager.Instance.SelectRats(remainingRats);
				// Set their destinations
				RatManager.Instance.SetRatDestinations(transform.position);
				RatManager.Instance.ClearRats();
				anim.SetBool("Active", false);
			}
			else
			{
				RatManager.Instance.SetRatDestinations(transform.position);
				RatManager.Instance.selectedRats.ForEach(r => TaskManager.Instance.UnassignRats(r));
			}
		}
	}

	public void SetTask(StandardTask task) => hoverTask = task;
}
