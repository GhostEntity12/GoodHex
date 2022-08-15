using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class TaskModuleAudio : TaskModule
{
	[SerializeField] AudioClip clip;
	AudioSource source;
	private void Start() => source = GetComponent<AudioSource>();

	protected override void OnActivate() => source.PlayOneShot(clip);
	protected override void OnDeactivate() { }
	protected override void SetPaused(bool pause) { }
}
