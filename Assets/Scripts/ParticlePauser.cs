using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticlePauser : ComponentPauser
{
	ParticleSystem ps;
	bool active;
	protected override void Awake()
	{
		base.Awake();
		ps = GetComponent<ParticleSystem>();
	}
	protected override void Pause(bool paused)
	{
		base.Pause(paused);
		if (paused)
		{
			active = ps.isPlaying;
		}

		if (active)
		{
			if (paused)
			{
				ps.Pause();
			}
			else
			{
				ps.Play();
			}
		}
	}
}
