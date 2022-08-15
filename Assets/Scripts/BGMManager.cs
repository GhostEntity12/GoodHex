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
		if (endMusicActive)
		{   
			// Set the children's timesamples to the parent's timesamples
			foreach (AudioSource child in children)
			{
				// Handling the problem that the parent is ever so slightly longer than some of the children
				child.timeSamples = Mathf.Min(parent.timeSamples, child.clip.samples - 1);
			}

			// Fading in the volume of the children
			if (children[0].volume < parent.volume)
			{
				time += Time.deltaTime;

				foreach (AudioSource child in children)
				{
					child.volume = parent.volume * childMusicRampTime * time;
					child.volume = Mathf.Clamp(child.volume, 0, parent.volume);
				}
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
