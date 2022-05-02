using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class RatManager : Singleton<RatManager>
{
	/// <summary>
	/// Rat Data, persistant across levels
	/// </summary>
	List<RatData> persistantRatData = new();

	[SerializeField] GameObject ratPrefab;

	// Rat GameObjects - respawned between levels
    List<Rat> allRats = new();
    List<Rat> selectedRats = new();
    public List<Rat> AllRats => allRats;
    public List<Rat> SelectedRats => selectedRats;

	private void Start()
	{
		allRats = FindObjectsOfType<Rat>().ToList();
	}

	public void SelectRats(List<Rat> ratsToSelect)
	{
        selectedRats.Clear();
        selectedRats.AddRange(ratsToSelect);
	}

	public void SpawnRats()
	{
		foreach (RatData ratInfo in persistantRatData)
		{
			((GameObject)Instantiate(ratPrefab)).GetComponent<Rat>().Setup(ratInfo);
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
