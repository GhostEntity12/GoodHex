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

	public Vector2 reticleSize = new(1, 3);
	float clampedMouseWheelInput = 2;
	float circleSize = 1f;

	Assignable hoveredAssignable;

	[SerializeField] AudioClip selectClip;

	[Header("Particle System")]
	[SerializeField] ParticleSystem particleDrawIn;
	[SerializeField] ParticleSystem particleRelease;
	[SerializeField] ParticleSystem particleReleaseTask;

	[Header("Pulse")]
	[SerializeField] Transform pulse;
	[SerializeField] float pulseLength = 0.1f;
	Material pulseMat;
	bool doPulse;
	float pulseTimer;

	[Header("Lerping")]
	[SerializeField] float lerpSpeed;
	[SerializeField] float rotationSpeed;
	[SerializeField] float rotationAngle;

	[Header("Colors")]
	[SerializeField] Color defaultColor;
	[SerializeField] Color ratsColor;
	[SerializeField] Color taskColor;
	[SerializeField] float colorChangeSpeed = 10f;
	Color targetColor;

	readonly List<Rat> ratsInHold = new();

	RatManager ratManager;

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
		pulse.transform.parent = null;
		pulseMat = pulse.GetComponentInChildren<Renderer>().material;
	}

	void Update()
	{
		if (paused)
		{
			SetColor(new(0, 0, 0, 0));
			return;
		}


		if (Physics.Raycast(c.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, 1 << 8 | 1 << 10))
		{
			if (!mouseLocked)
			{
				reticlePosition = hit.point;
			}
			else
			{
				if (Vector3.Distance(hit.point, reticlePosition) > circleSize / 2)
				{
					particleDrawIn.Stop();
					mouseLocked = false;
					anim.SetBool("Active", false);
					pointerDownTimer = 0;
				}
			}
			graphic.enabled = hit.normal.y > 0;
		}
		else
		{
			graphic.enabled = false;
			particleDrawIn.Stop();
		}

		SetSize();
		SetPosition();
		SetColor();

		if (hit.normal.y > 0)
		{
			SelectDeselect();
		}
		Assign();

		if (doPulse)
		{
			pulseTimer += Time.deltaTime / pulseLength;
			if (pulseTimer >= pulseLength)
			{
				doPulse = false;
				pulseTimer = 0;
			}
			float percent = pulseTimer / pulseLength;
			pulseMat.SetFloat("_PulseValue", Mathf.PingPong(percent * 2, 1));
		}
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
			? hoveredAssignable && hoveredAssignable.TaskState == BaseTask.State.Unlocked
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
		circleSize = Mathf.Lerp(reticleSize.x, reticleSize.y, clampedMouseWheelInput / 4f);
		transform.localScale = Vector3.MoveTowards(transform.localScale, new Vector3(circleSize, 1, circleSize), lerpSpeed * Time.deltaTime); // Reset Y scale to prevent lifting
		transform.localRotation = Quaternion.RotateTowards(transform.localRotation, Quaternion.Euler(0, rotationAngle * circleSize, 0), rotationSpeed * Time.deltaTime);
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
			particleDrawIn.Play();
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
					aS.PlayOneShot(selectClip);
					doPulse = true;
					pulse.position = transform.position;
					pulse.localScale = new Vector3(circleSize, 3 / 4f * circleSize, circleSize);
					ratManager.SelectRats(unselectedRats);
					ratsInHold.AddRange(unselectedRats);
					particleRelease.Play();
				}
				mouseLocked = false;
				particleDrawIn.Stop();
			}
		}
		// Mouse release
		if (Input.GetMouseButtonUp(0))
		{
			//Debug.Log("Button up time = " + pointerDownTimer);
			mouseLocked = false;
			particleDrawIn.Stop();
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
			if (hoveredAssignable && hoveredAssignable is not DeliveryTask && hoveredAssignable.TaskState == BaseTask.State.Unlocked) // Assign to task
			{
				int startRats = GameManager.Instance.RatManager.selectedRats.Count;
				List<Rat> remainingRats = GameManager.Instance.TaskManager.AssignRats(hoveredAssignable, ratManager.selectedRats.ToArray());
				if (remainingRats != null && startRats - remainingRats.Count > 0)
				{
					particleReleaseTask.Play();
				}
				// Clear selected rats
				ratManager.ClearRats();
				// Select the rats without tasks
				ratManager.SelectRats(remainingRats);
				// Set their destinations
				ratManager.SetRatDestinations(transform.position);
				ratManager.ClearRats();
				anim.SetBool("Active", false);
				GameManager.Instance.Highlighter.StopHighlight();
			}
			else if (hoveredAssignable && hoveredAssignable is DeliveryTask dt && dt.TaskState == BaseTask.State.Unlocked)
			{
				List<Rat> remainingRats = GameManager.Instance.TaskManager.AssignRats(dt, GameManager.Instance.RatManager.selectedRats.ToArray());
				// Clear selected rats
				ratManager.ClearRats();
				// Select the rats without tasks
				ratManager.SelectRats(remainingRats);
				ratManager.SetRatDestinations(transform.position);
				ratManager.ClearRats();
				GameManager.Instance.Highlighter.StopHighlight();
			}
			else if (Physics.Raycast(c.transform.position, reticlePosition - c.transform.position, out RaycastHit hit, Mathf.Infinity, 1 << 8) && hit.normal.y > 0)
			{
				ratManager.SetRatDestinations(transform.position);
				GameManager.Instance.TaskManager.UnassignRats(ratManager.selectedRats.ToArray());
			}
		}
	}

	public void SetAssignable(Assignable assignable) => hoveredAssignable = assignable;

	void SetPaused(bool paused) => this.paused = paused;
	private void OnDestroy()
	{
		GameManager.Pause -= SetPaused;
	}
}
