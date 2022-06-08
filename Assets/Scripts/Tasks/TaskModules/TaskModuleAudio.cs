using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))]
public class TaskModuleAudio : TaskModule
{
	[SerializeField] AudioClip clip;
	AudioSource source;
	private void Start() => source = GetComponent<AudioSource>();

	public override void OnActivate() => source.PlayOneShot(clip);

	public override void OnDeactivate() { }
}
