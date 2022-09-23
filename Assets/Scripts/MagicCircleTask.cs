using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCircleTask : MonoBehaviour
{
	bool doAnimation;
	Material magicCircleMat;
	float timer;
	[SerializeField] Transform magicOrb;
	[SerializeField] float orbGrowSpeed;
	[SerializeField] Renderer magicCircle;
	[SerializeField] ParticleSystem ps;

	private void Start()
	{
		magicCircleMat = magicCircle.material;
	}

	// Update is called once per frame
	void Update()
	{
		timer += Time.deltaTime * (doAnimation ? 1 : -1);
		timer = Mathf.Clamp(timer, 0, orbGrowSpeed);
		magicOrb.localScale = (Mathf.Clamp01(Mathf.Lerp(0, 1, timer / orbGrowSpeed)) + 0.07f * Mathf.Sin(Time.time * (doAnimation ? 5 : 0))) * Vector3.one;
		magicCircleMat.SetFloat("_EmissionStrength", doAnimation ? 1 + Mathf.Sin(Time.time * 2.5f) : 1.5f);
	}

	public void SetAnimState(bool active)
	{
		doAnimation = active;
		if (active)
		{
			ps.Play();
		}
		else
		{
			ps.Stop();
		}
	}
}
