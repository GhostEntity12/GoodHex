using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskModuleAnimation : TaskModule
{
	Animator anim;
	// Start is called before the first frame update
	void Start() => anim = GetComponent<Animator>();
	public override void OnActivate() => anim.SetBool("active", true);

	public override void OnDeactivate() => anim.SetBool("active", false);
}
