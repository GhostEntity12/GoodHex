using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CircleSelector : MonoBehaviour
{
	[Header("Movement")]
	[SerializeField] float movementSpeed = 2f;
	[SerializeField] float sizeChangeSpeed = 0.5f;
	const float DefaultSize = 3f;
	float circleSizeModifier = 1f;

	Bounds[] levelBounds;
	Bounds currentBounds;

	Collider hoverTask;

	Animator anim;

	[Header("Colors")]
	[SerializeField] Color defaultColor;
	[SerializeField] Color ratsColor;
	[SerializeField] Color taskColor;
	[SerializeField] float colorChangeSpeed = 0.1f;
	Color targetColor;
	SpriteRenderer sprite;

	private void Start()
	{
		anim = GetComponentInChildren<Animator>();

		sprite = GetComponentInChildren<SpriteRenderer>();
		sprite.color = defaultColor;
		targetColor = defaultColor;

		levelBounds = GameObject.FindGameObjectsWithTag("Bounds").Select(b => b.GetComponent<Collider>().bounds).ToArray();
		// This should never happen dring the game - if this happens fix in editor.
		currentBounds = CurrentBounds(transform.position, ref levelBounds) ?? throw new System.Exception("Selector not within bounds");
	}

	// Update is called once per frame
	private void Update()
	{
		hoverTask = Physics.OverlapSphere(transform.position, 0.5f, 1 << 7).FirstOrDefault();

		targetColor = RatManager.Instance.HasSelectedRats && hoverTask
			? taskColor
			: RatManager.Instance.HasSelectedRats ? ratsColor : defaultColor;

		sprite.color = ExtensionMethods.ColorMoveTowards(sprite.color, targetColor, colorChangeSpeed * Time.deltaTime);
		Vector3 inputVector = UniformInput.GetAnalogStick();

		// Move the circle, multiply by circleSizeModifier to speed up large circle
		Vector3 destPos = transform.position + circleSizeModifier * movementSpeed * Time.deltaTime * inputVector;
		Vector3 closestPoint = currentBounds.ClosestPoint(destPos); // Used in the case where it's out of bounds
		transform.position = InBoundingBox(destPos, ref levelBounds) ? destPos : new Vector3(closestPoint.x, 0, closestPoint.z);

		// Resizing circle
		circleSizeModifier += Input.GetAxisRaw("CircleSize") * Time.deltaTime * sizeChangeSpeed;
		Vector3 scale = transform.localScale + Input.GetAxisRaw("CircleSize") * sizeChangeSpeed * Time.deltaTime * Vector3.one;
		transform.localScale = new Vector3(scale.x, 1, scale.z); // Reset Y scale to prevent lifting

		// Selecting characters
		if (Input.GetButtonDown("Select"))
		{
			Debug.Log("Selecting");
			// Select rats that are within DefaultSize * circleSizeModifier
			RatManager.Instance.SelectRats(RatManager.Instance.allRats.Where(r =>
				Vector3.Distance(transform.position, r.transform.position) < DefaultSize * circleSizeModifier
			).ToList());

			if (RatManager.Instance.selectedRats.Count > 0)
			{
				anim.SetBool("Active", true);
			}
		}

		// Moving characters to position
		if (Input.GetButtonDown("Assign") && // Button press
			NavMesh.SamplePosition(transform.position, out NavMeshHit nMHit, 200f, NavMesh.AllAreas)) // Closest point on Navmesh
		{
			List<Rat> ratsToAssign = new(RatManager.Instance.selectedRats);
			if (hoverTask) // Assign to task
			{
				hoverTask.GetComponent<Task>().AssignRats(ratsToAssign);
			}

			Debug.Log("Assigning");
			ratsToAssign.ForEach(m => m.SetDestination(nMHit.position));

			if (RatManager.Instance.selectedRats.Count > 0)
			{
				anim.SetBool("Active", false);
			}
		}
	}

	/// <summary>
	/// Gets the first bounds the given point is in
	/// </summary>
	/// <param name="point"></param>
	/// <param name="bounds">The bounds array to check</param>
	/// <returns></returns>
	Bounds? CurrentBounds(Vector3 point, ref Bounds[] bounds) => bounds.FirstOrDefault(b => b.Contains(new Vector3(point.x, 0, point.z)));

	/// <summary>
	/// Returns whether a given point is in any of a array of bounds
	/// </summary>
	/// <param name="point"></param>
	/// <param name="bounds">The bounds array to check</param>
	/// <returns></returns>
	bool InBoundingBox(Vector3 point, ref Bounds[] bounds)
	{
		foreach (Bounds bound in bounds)
		{
			if (bound.Contains(new Vector3(point.x, 0, point.z)))
			{
				currentBounds = bound;
				return true;
			}
		}
		return false;
	}
}


/// <summary>
///  Via https://amorten.com/blog/2017/mapping-square-input-to-circle-in-unity/, modified to use Vector3
/// </summary>
public static class UniformInput
{
	/// <summary>
	/// Remaps the horizontal/vertical input to a perfect circle instead of a square.
	/// This prevents the issue of characters speeding up when moving diagonally, but maintains the analog stick sensivity.
	/// </summary>
	public static Vector3 GetAnalogStick(string horizontal = "Horizontal", string vertical = "Vertical")
	{
		// apply some error margin, because the analog stick typically does not
		// reach the corner entirely
		const float error = 1.1f;

		// clamp input with error margin
		var input = new Vector3(
			Mathf.Clamp(Input.GetAxisRaw(horizontal) * error, -1f, 1f),
			0,
			Mathf.Clamp(Input.GetAxisRaw(vertical) * error, -1f, 1f)
		);

		// map square input to circle, to maintain uniform speed in all
		// directions
		return new Vector3(
			input.x * Mathf.Sqrt(1 - input.z * input.z * 0.5f),
			0,
			input.z * Mathf.Sqrt(1 - input.x * input.x * 0.5f)
		);
	}
}