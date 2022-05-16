using System.Collections.Generic;
using UnityEngine;

public class RatManager : Singleton<RatManager>
{
	/// <summary>
	/// Rat Data, persistant across levels
	/// </summary>
	[SerializeField] List<RatData> persistantRatData = new();

	[SerializeField] GameObject ratPrefab;

	// Rat GameObjects - respawned between levels
    List<Rat> allRats = new();
    List<Rat> selectedRats = new();
    public List<Rat> AllRats => allRats;
    public List<Rat> SelectedRats => selectedRats;
    public bool HasSelectedRats => selectedRats.Count > 0;

	private void Start()
	{
		for (int i = 0; i < 5; i++)
		{
			persistantRatData.Add(new());
		}
		//allRats = FindObjectsOfType<Rat>().ToList();
		SpawnRats();
	}

	public void ClearRats()
	{
		selectedRats.ForEach(r => r.Deselect());
		selectedRats.Clear();
	}

	public void SelectRats(List<Rat> ratsToSelect)
	{
		ClearRats();
		ratsToSelect.ForEach(r => r.Select());
        selectedRats.AddRange(ratsToSelect);
	}

	public void SpawnRats()
	{
		foreach (RatData ratInfo in persistantRatData)
		{
			Rat rat = Instantiate(ratPrefab).GetComponent<Rat>();
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
			rat.SetDestination(destination);
		}
	}
}
