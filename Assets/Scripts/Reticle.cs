using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reticle : MonoBehaviour
{
	Camera c;
	Animator anim;
	SpriteRenderer graphic;

	float clampedMouseWheelInput = 2;
	float circleSize = 1f;

	Task hoverTask;

	[Header("Colors")]
	[SerializeField] Color defaultColor;
	[SerializeField] Color ratsColor;
	[SerializeField] Color taskColor;
	[SerializeField] float colorChangeSpeed = 10f;
	Color targetColor;

	void Awake()
	{
		c = Camera.main;

		anim = GetComponentInChildren<Animator>();
		graphic = GetComponentInChildren<SpriteRenderer>();
	}

	void Update()
	{
		SetSize();
		SetPosition();

		SetColor();

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
		 * LOGIC FOR SELECTION
		 * 1) if no rats are selected, focus rats
		 * 2) if rats are selected and clicked task, send rats to task
		 * 3) if rats are selected and clicked on selected rats, move to point
		 * 4) if rats are selected and clicked on NOT selected rats, select rats
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */


		if (Input.GetMouseButton(0))
		{

			// Get all unselected rats within the circle

			List<Rat> unselectedRats = 
				RatManager.Instance.allRats
				.Where(r =>
					Vector3.Distance(transform.position, r.transform.position) < 1.5f * circleSize &&
					!RatManager.Instance.selectedRats.Contains(r))
				.ToList();
			if (unselectedRats.Count > 0) // Select rats
			{
				RatManager.Instance.SelectRats(unselectedRats);
				anim.SetBool("Active", true);
			}
			else if (Input.GetMouseButtonDown(0)) // Deselect
			{
				RatManager.Instance.ClearRats();
				anim.SetBool("Active", false);
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			if (hoverTask) // Assign to task
			{
				List<Rat> remainingRats = hoverTask.AssignRats(RatManager.Instance.selectedRats);
				// Clear selected rats
				RatManager.Instance.ClearRats();
				// Select the rats without tasks
				RatManager.Instance.SelectRats(remainingRats);
				// Set their destinations
				RatManager.Instance.SetRatDestinations(transform.position);
				RatManager.Instance.ClearRats();
				//// Reselect the assigned rats for consistency
				//// This currently selects all rats on the task, rework?
				//RatManager.Instance.SelectRats(hoverTask.AssignedRats);
				anim.SetBool("Active", false);
			}
			else
			{
				RatManager.Instance.SetRatDestinations(transform.position);
				RatManager.Instance.selectedRats.ForEach(r => r.UnsetTask());
			}
		}
	}

	void SetColor()
	{
		targetColor =
			RatManager.Instance.HasSelectedRats
			? hoverTask && hoverTask.TaskState == Task.State.Unlocked
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

	public void SetTask(Task task) => hoverTask = task;
}
