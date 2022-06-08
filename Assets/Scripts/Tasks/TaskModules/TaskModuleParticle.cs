using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskModuleParticle : TaskModule
{
    ParticleSystem particle;
	void Start() => particle = GetComponent<ParticleSystem>();
	public override void OnActivate() => particle.Play();
	public override void OnDeactivate() => particle.Stop();
}
