using UnityEngine;

public class TaskModulePickupable : TaskModule
{

    [SerializeField] GameObject itemToSpawn;
    [SerializeField] GameObject spawner;

	protected override void OnActivate()
	{
		Instantiate(itemToSpawn, spawner.transform.position, Quaternion.identity);
	}

	protected override void OnDeactivate() { }

	protected override void SetPaused(bool pause) {	}
}
