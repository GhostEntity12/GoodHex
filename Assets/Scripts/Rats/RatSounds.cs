using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class RatSounds : MonoBehaviour
{
    [SerializeField] Vector2 minMaxTime;
    float timer;

    [SerializeField] AudioClip[] selectSqueaks;
    [SerializeField] AudioClip[] assignSqueaks;
    [SerializeField] AudioClip[] randomSqueaks;

    AudioSource aS;

	private void Awake()
	{
        aS = GetComponent<AudioSource>();
        timer = Random.Range(minMaxTime.x, minMaxTime.y);
	}

	// Update is called once per frame
	void Update()
    {
        timer -= Time.deltaTime;
		if (timer <= 0)
		{
            timer = Random.Range(minMaxTime.x, minMaxTime.y);
            aS.PlayOneShot(randomSqueaks[Random.Range(0, randomSqueaks.Length)]);
        }
    }

    public void PlaySelected() => aS.PlayOneShot(selectSqueaks[Random.Range(0, selectSqueaks.Length)]);
    public void PlayAssigned() => aS.PlayOneShot(assignSqueaks[Random.Range(0, assignSqueaks.Length)]);
}
