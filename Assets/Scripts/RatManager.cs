using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RatManager : Singleton<RatManager>
{
	/// <summary>
	/// Rat Data, persistant across levels
	/// </summary>
	[SerializeField] List<RatData> persistantRatData = new();

	[SerializeField] GameObject ratPrefab;

	[SerializeField] Transform[] spawnPointTransforms;
	Vector3[] spawnPoints;

	// Rat GameObjects - respawned between levels
	public readonly List<Rat> allRats = new();
	public readonly List<Rat> selectedRats = new();
	public bool HasSelectedRats => selectedRats.Count > 0;

	private void Start()
	{
		spawnPoints = spawnPointTransforms.Select(t => t.position).ToArray();
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			persistantRatData.Add(new());
		}
		//allRats = FindObjectsOfType<Rat>().ToList();
		SpawnRats();
	}

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

	public void SpawnRats()
	{
		for (int i = 0; i < persistantRatData.Count; i++)
		{
			RatData ratInfo = persistantRatData[i];
			Rat rat = Instantiate(ratPrefab, spawnPoints[i], Quaternion.identity).GetComponent<Rat>();
			rat.gameObject.name = ratInfo.Name;
			rat.AssignInfo(ratInfo);
			AddRat(rat);
		}
	}

	public void AddRat(Rat rat) => allRats.Add(rat);
	public void RemoveRat(Rat rat)
	{
		allRats.Remove(rat);
		selectedRats.Remove(rat);
	}

	public void SetRatDestinations(Vector3 destination, List<Rat> rats = null)
	{
		rats ??= selectedRats;
		foreach (Rat rat in rats)
		{
			TaskManager.Instance.UnassignRats(rat);
			rat.SetDestination(destination);
		}
	}
}
