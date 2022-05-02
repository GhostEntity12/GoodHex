using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

public class CircleSelector : MonoBehaviour
{
	int reticleSpriteIndex = 0;
	[field: SerializeField] public int ReticleSpriteIndex
    {
		get => reticleSpriteIndex;
		set
        {
			reticleMaterial.SetFloat("_Index", value);
        }
    }
	[SerializeField] float movementSpeed = 2f;
	float defaultSize = 3f;
	float circleSizeModifier = 1f;

	[SerializeField] float sizeChangeSpeed = 0.5f;

	Bounds[] levelBounds;
	Bounds currentBounds;

	Material reticleMaterial;

	private void Start()
	{
		reticleMaterial = GetComponent<Renderer>().material;
		levelBounds = GameObject.FindGameObjectsWithTag("Bounds").Select(b => b.GetComponent<Collider>().bounds).ToArray();
		// This should never happen dring the game - if this happens fix in editor.
		currentBounds = CurrentBounds(transform.position) ?? throw new System.Exception("Selector not within bounds");
	}

	// Update is called once per frame
	private void Update()
	{
		Vector3 inputVector = UniformInput.GetAnalogStick();

		// Move the circle, multiply by circleSizeModifier to speed up large circle
		Vector3 destPos = transform.position + circleSizeModifier * movementSpeed * Time.deltaTime * inputVector;
		Vector3 closestPoint = currentBounds.ClosestPoint(destPos); // Used in the case where it's out of bounds
		transform.position = InBoundingBox(destPos) ? destPos : new Vector3(closestPoint.x, 0, closestPoint.z);

		// Resizing circle
		transform.localScale += Input.GetAxisRaw("CircleSize") * sizeChangeSpeed * Time.deltaTime * Vector3.one;
		circleSizeModifier += Input.GetAxisRaw("CircleSize") * Time.deltaTime * sizeChangeSpeed;

		// Selecting characters
		if (Input.GetButtonDown("Select"))
		{
			// Select rats that are within defaultSize * circleSizeModifier
			RatManager.Instance.SelectRats(RatManager.Instance.AllRats.Where(r =>
				Vector3.Distance(transform.position, r.transform.position) < defaultSize * circleSizeModifier
			).ToList());
		}

		// Moving characters to position
		if (Input.GetButtonDown("Assign"))
		{
			if (NavMesh.SamplePosition(transform.position, out NavMeshHit nMHit, 200f, NavMesh.AllAreas))
			{
				RatManager.Instance.SelectedRats.ForEach(m => m.SetDestination(nMHit.position));
			}
		}
	}

	Bounds? CurrentBounds(Vector3 point)
	{
		foreach (Bounds bound in levelBounds)
		{
			if (bound.Contains(new Vector3(point.x, 0, point.z)))
			{
				return bound;
			}
		}
		return null;
	}

	bool InBoundingBox(Vector3 point)
	{
		foreach (Bounds bound in levelBounds)
		{
			if (bound.Contains(new Vector3(point.x, 0, point.z)))
			{
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