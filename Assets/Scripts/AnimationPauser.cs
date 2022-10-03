using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationPauser : ComponentPauser
{
	Animator anim;
	protected override void Awake()
	{
		base.Awake();
		anim = GetComponent<Animator>();
	}

	protected override void Pause(bool paused)
	{
		base.Pause(paused);
		anim.speed = paused ? 0 : 1;
	}
}
