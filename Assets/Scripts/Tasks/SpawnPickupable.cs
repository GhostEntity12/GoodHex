using UnityEngine;

public class SpawnPickupable : MonoBehaviour
{

	[SerializeField] GameObject itemToSpawn;
	[SerializeField] GameObject spawner;

	[SerializeField] Assignable[] destinations;

	public void Spawn()
	{
		Debug.Log("Spawning");
		BaseTask t = Instantiate(itemToSpawn, spawner.transform.position, Quaternion.identity).GetComponent<BaseTask>();
		foreach (Assignable destination in destinations)
		{
			destination.SetRequiredTasks(t);
		}
	}
}
