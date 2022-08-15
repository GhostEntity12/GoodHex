using UnityEngine;

public class TaskModuleParticle : TaskModule
{
	ParticleSystem particle;
	void Start() => particle = GetComponent<ParticleSystem>();
	protected override void OnActivate() => particle.Play();
	protected override void OnDeactivate() => particle.Stop();

	protected override void SetPaused(bool pause)
	{
		if (active)
		{
			if (pause)
				particle.Pause();
			else
				particle.Play();
		}
	}
}
