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

	readonly Queue<RatData> deadRats = new();

	readonly float respawnTime = 5f;
	float respawnTimer;

	Vector3[] spawnPoints;
	Queue<Vector3> spawnPointBag;

	private void Start()
	{
		spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoints").Select(t => t.transform.position).ToArray();
	}

	private void Update()
	{
		if (deadRats.Count > 0)
		{
			respawnTimer -= Time.deltaTime;
			if (respawnTimer <= 0)
			{
				respawnTimer = respawnTime;

				SpawnRat();
			}
		}
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

	public Rat SpawnRat(Vector3 position, RatData data = null)
	{
		data ??= new();
		Rat rat = Instantiate(RatPrefab, position, Quaternion.identity).GetComponent<Rat>();
		rat.gameObject.name = data.Name;
		rat.AssignInfo(data);
		rat.SetColor();
		AddRat(rat);
		return rat;
	}

	public Rat SpawnRat(RatData data = null)
	{
		data ??= new();
		Rat rat = Instantiate(RatPrefab, GetSpawnPoint(), Quaternion.identity).GetComponent<Rat>();
		rat.gameObject.name = data.Name;
		rat.AssignInfo(data);
		rat.SetColor();
		AddRat(rat);
		return rat;
	}

	public List<Rat> SpawnRats(params RatData[] data)
	{
		List<Rat> spawnedRats = new();
		for (int i = 0; i < spawnPoints.Length; i++)
		{
			spawnedRats.Add(SpawnRat(data[i]));
		}
		return spawnedRats;
	}

	public List<Rat> SpawnRats() => SpawnRats(new RatData[spawnPoints.Length]);

	void AddRat(Rat rat) => allRats.Add(rat);

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
			GameManager.Instance.TaskManager.UnassignRats(rat);
			rat.SetDestination(destination);
		}
	}

	Vector3 GetSpawnPoint()
	{
		if (spawnPointBag.Count == 0)
		{
			spawnPoints.Shuffle();
			spawnPointBag = new Queue<Vector3>(spawnPoints);
		}
		return spawnPointBag.Dequeue();
	}

	public void QueueRatForRespawn(RatData data) => deadRats.Enqueue(data);
}
