using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BGMManager : MonoBehaviour
{
	[SerializeField] AudioSource parent;
	[SerializeField] AudioSource[] children;
	[SerializeField] float childMusicRampTime = 0.1f;

	bool endMusicActive;
	float time;

	// Update is called once per frame
	void Update()
	{
		foreach (AudioSource child in children)
		{
			child.timeSamples = parent.timeSamples;
		}

		if (endMusicActive && children[0].volume < parent.volume)
		{
			time += Time.deltaTime;

			foreach (AudioSource child in children)
			{
				child.volume = parent.volume * childMusicRampTime * time;
				child.volume = Mathf.Clamp(child.volume, 0, parent.volume);
			}
		}
	}

	public void StopAllTracks()
	{
		foreach (AudioSource child in children)
		{
			child.Stop();
		}
		parent.Stop();
	}
	public void TriggerEndMusic() => endMusicActive = true;
}
