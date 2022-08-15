using UnityEngine;

public class TaskModuleAnimation : TaskModule
{
	Animator anim;
	// Start is called before the first frame update
	void Start() => anim = GetComponent<Animator>();
	protected override void OnActivate() => anim.SetBool("active", true);
	protected override void OnDeactivate() => anim.SetBool("active", false);

	protected override void SetPaused(bool paused) => anim.speed = paused ? 0 : 1;
}
