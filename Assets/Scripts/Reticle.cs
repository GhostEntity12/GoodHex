using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Reticle : MonoBehaviour
{
	Camera c;
	Animator anim;
	SpriteRenderer graphic;

	float clampedMouseWheelInput;
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
		targetColor = RatManager.Instance.HasSelectedRats && hoverTask
			? taskColor
			: RatManager.Instance.HasSelectedRats ? ratsColor : defaultColor;

		graphic.color = ExtensionMethods.ColorMoveTowards(graphic.color, targetColor, colorChangeSpeed * Time.deltaTime);

		// Mouse wheel to set size
		clampedMouseWheelInput += Input.mouseScrollDelta.y;
		clampedMouseWheelInput = Mathf.Clamp(clampedMouseWheelInput, 0, 4);
		circleSize = clampedMouseWheelInput / 2 + 1;
		transform.localScale = new Vector3(circleSize, 1, circleSize); // Reset Y scale to prevent lifting

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
			return;
		}

		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
		 * LOGIC FOR SELECTION
		 * 1) if no rats are selected, focus rats
		 * 2) if rats are selected and clicked task, send rats to task
		 * 3) if rats are selected and clicked on selected rats, move to point
		 * 4) if rats are selected and clicked on NOT selected rats, select rats
		/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

		// Task checking/setting
		Collider taskCollider = Physics.OverlapSphere(transform.position, 0.5f, 1 << 7).FirstOrDefault();
		if (!taskCollider)
		{
			hoverTask = null;
		}
		else
		{
			Task t = taskCollider.GetComponent<Task>();
			if (t.Available)
			{
				hoverTask = t;
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			// Get all unselected rats within the circle
			List<Rat> unselectedRats =
				RatManager.Instance.allRats
				.Where(r =>
					Vector3.Distance(transform.position, r.transform.position) < 1.5f * circleSize &&
					!RatManager.Instance.selectedRats.Contains(r))
				.ToList();

			if (hoverTask && hoverTask.Available) // Assign to task
			{
				List<Rat> remainingRats = hoverTask.AssignRats(RatManager.Instance.selectedRats);
				// Clear selected rats
				RatManager.Instance.selectedRats.Clear();
				// Select the rats without tasks
				RatManager.Instance.SelectRats(remainingRats);
				// Set their destinations
				RatManager.Instance.SetRatDestinations(transform.position);
				// Reselect the assigned rats for consistency
				// This currently selects all rats on the task, rework?
				RatManager.Instance.SelectRats(hoverTask.AssignedRats);
			}
			else if (unselectedRats.Count == 0) // Set destination
			{
				RatManager.Instance.SetRatDestinations(transform.position);
				RatManager.Instance.selectedRats.ForEach(r => r.UnsetTask());
			}
			else if (unselectedRats.Count > 0) // Select rats
			{
				RatManager.Instance.SelectRats(unselectedRats);
				anim.SetBool("Active", true);
			}
		}
		if (Input.GetMouseButtonDown(1)) // Deselect
		{
			RatManager.Instance.ClearRats();
			anim.SetBool("Active", false);
		}
	}
}
