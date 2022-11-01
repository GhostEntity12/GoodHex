using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RatManager : MonoBehaviour
{
	public Rat RatPrefab { private get; set; }

	// Rat GameObjects - respawned between levels
	public readonly List<Rat> allRats = new();
	public readonly List<Rat> selectedRats = new();
	public bool HasSelectedRats => selectedRats.Count > 0;
	public int deadRats = 0;

	public void ClearRats()
	{
		selectedRats.ToList().ForEach(r => r.Deselect());
		selectedRats.Clear();
	}

	public void SelectRats(List<Rat> ratsToSelect)
	{
		if (ratsToSelect == null || ratsToSelect.Count == 0) return;
		ratsToSelect.ForEach(r => r.Select());
		selectedRats.AddRange(ratsToSelect);
	}

	public List<Rat> SpawnRats(params Vector3[] spawnPoints)
	{
		List<Rat> spawnedRats = new();
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			RatData ratInfo = new();
			Rat rat = Instantiate(RatPrefab, spawnPoints[i], Quaternion.identity).GetComponent<Rat>();
			rat.gameObject.name = ratInfo.Name;
			rat.AssignInfo(ratInfo);
			spawnedRats.Add(rat);
			rat.SetColor();
			AddRat(rat);
		}
		
		return spawnedRats;
	}

	public void RespawnCheck(params Vector3[] spawnPoints) {

		List<Rat> spawnedRats = new();
		while (deadRats > 0)
		{
			for (int i = 0; i < spawnPoints.Length; i++)
			{
				RatData ratInfo = new();
				Rat rat = Instantiate(RatPrefab, spawnPoints[i], Quaternion.identity).GetComponent<Rat>();
				rat.gameObject.name = ratInfo.Name;
				rat.AssignInfo(ratInfo);
				spawnedRats.Add(rat);
				rat.SetColor();
				AddRat(rat);
				deadRats -= 1;
			}
		}
	
	}

	public void AddRat(Rat rat) => allRats.Add(rat);

	public void RemoveRat(Rat rat)
	{
		allRats.Remove(rat);
		selectedRats.Remove(rat);
		deadRats += 1;
	}

	public void SetRatDestinations(Vector3 destination, List<Rat> rats = null)
	{
		rats ??= selectedRats;
		foreach (Rat rat in rats)
		{
			GameManager.Instance.TaskManager.UnassignRats(rat);
			rat.SetDestination(destination);
		}
	}
}
