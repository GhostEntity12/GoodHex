using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioPauser : ComponentPauser
{
    AudioSource aS;
	protected override void Awake()
    {
        base.Awake();
        aS = GetComponent<AudioSource>();
    }
    protected override void Pause(bool paused)
    {
		base.Pause(paused);

		if (paused)
		{
			aS.Pause();
		}
		else
		{
			aS.UnPause();
		}
	}
}
