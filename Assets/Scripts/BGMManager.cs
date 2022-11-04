using UnityEngine;

public class BGMManager : MonoBehaviour
{
	[SerializeField] AudioSource parent;
	[SerializeField] AudioSource[] children;

	float childVolume;


	private void Start()
	{
		foreach (AudioSource audioSource in children)
		{
			audioSource.volume = 0;
		}
	}
	// Update is called once per frame
	void Update()
	{
		// Set the children's timesamples to the parent's timesamples
		foreach (AudioSource child in children)
		{
			// Handling the problem that the parent is ever so slightly longer than some of the children
			child.timeSamples = Mathf.Min(parent.timeSamples, child.clip.samples - 1);
		}

		foreach (AudioSource child in children)
		{
			child.volume = parent.volume / childVolume;
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
	public void SetChildVolume(float t) => childVolume = t;
}
