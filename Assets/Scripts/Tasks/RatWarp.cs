using System.Collections.Generic;
using UnityEngine;

public class RatWarp : Assignable
{
	[SerializeField] Transform warpPosition;
	[SerializeReference] AudioClip[] sounds;
	protected new void Start()
	{
		base.Start();
		TaskState = State.Unlocked;
		OnUnlock();
	}

	void Update()
	{
		if (paused) return;

		if (GameManager.Instance.TaskManager.RatsOnTask(this).Count > 0)
		{
			List<Rat> ratsToDeselect = new();
			foreach (Rat rat in GameManager.Instance.TaskManager.RatsOnTask(this))
			{
				if (rat.ArrivedAtTask())
				{
					ratsToDeselect.Add(rat);
					rat.NavAgent.Warp(warpPosition.position);
					Debug.Log("Playing");
					source.PlayOneShot(sounds[Random.Range(0, sounds.Length)]);
				}
			}
			GameManager.Instance.TaskManager.UnassignRats(ratsToDeselect.ToArray());
		}
	}

	protected override void OnActivate() { base.OnActivate(); }
	protected override void OnComplete() { base.OnComplete(); }
	protected override void OnUnlock() { base.OnUnlock(); }
}
