using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class Reticle : MonoBehaviour
{
	Camera c;

	[Header("Colors")]
	[SerializeField] Color defaultColor;
	[SerializeField] Color ratsColor;
	[SerializeField] Color taskColor;
	[SerializeField] float colorChangeSpeed = 0.1f;
	Color targetColor;

	Animator anim;
	SpriteRenderer graphic;

	Task hoverTask;

	float clampedMouseWheelInput;

	float circleSize = 1f;

	// Start is called before the first frame update
	void Start()
	{
		c = Camera.main;

		anim = GetComponentInChildren<Animator>();
		graphic = GetComponentInChildren<SpriteRenderer>();
	}

	// Update is called once per frame
	void Update()
	{
		// Mouse wheel to set size
		clampedMouseWheelInput += Input.mouseScrollDelta.y;
		clampedMouseWheelInput = Mathf.Clamp(clampedMouseWheelInput, 0, 4);
		circleSize = clampedMouseWheelInput / 2 + 1;
		transform.localScale = new Vector3(circleSize, 1, circleSize); // Reset Y scale to prevent lifting

		// Raycast to set reticle position
		// 1 << 8 is Surface, hit.normal is >0 when it is vaguely upwards
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


		if (Input.GetMouseButtonDown(0))
		{
			Collider c = Physics.OverlapSphere(transform.position, 0.5f, 1 << 7).FirstOrDefault();

			hoverTask = c ? c.GetComponent<Task>() : null;

			Collider[] cols = Physics.OverlapSphere(transform.position, 1.5f * circleSize);//, 1 << 6); // For some reason this stops working randomly?
			List<Rat> rats = cols.Select(c => c.GetComponent<Rat>()).Where(r => r != null).ToList();
			List<Rat> unselectedRats = rats.Where(r => !RatManager.Instance.SelectedRats.Contains(r)).ToList();
			Debug.Log($"CSize {1.5f * circleSize}, Colliders: {cols.Length}, Rats: {rats.Count}, Unselected {unselectedRats.Count}");

			if (hoverTask)
			{
				hoverTask.AssignRats(ref rats);
				RatManager.Instance.ClearRats();
				RatManager.Instance.SelectRats(rats);
				RatManager.Instance.SetRatDestinations(transform.position);
			}
			else if (unselectedRats.Count == 0)
			{
				RatManager.Instance.SetRatDestinations(transform.position);
			}
			else if (unselectedRats.Count > 0)
			{
				RatManager.Instance.SelectRats(unselectedRats);
				anim.SetBool("Active", true);
			}
		}
		if (Input.GetMouseButtonDown(1))
		{
			RatManager.Instance.ClearRats();
			anim.SetBool("Active", false);
		}
	}
}
