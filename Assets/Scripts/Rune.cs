using UnityEngine;

public class Rune : MonoBehaviour
{
	Renderer r;
	bool active;
	bool finishedLerp;
	float timer;
	[SerializeField] float lerpSpeed = 0.5f;

	private void Start()
	{
		r = GetComponent<Renderer>();
	}

	private void Update()
	{
		if (!active) return;

		if (finishedLerp)
		{
			r.material.SetFloat("_Intensity", (Mathf.Sin(Time.time) / 2) + 6);
		}
		else
		{
			timer += Time.deltaTime / lerpSpeed;
			r.material.SetFloat("_Saturation", Mathf.Lerp(0, 1, timer));
			r.material.SetFloat("_Intensity", Mathf.Lerp(0, 6, timer));
			if (timer >= 1)
			{
				finishedLerp = true;
			}
		}
	}

	public void Enable() => active = true;
}
