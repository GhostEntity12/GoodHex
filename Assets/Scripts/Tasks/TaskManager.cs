using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : Singleton<TaskManager>
{
	public readonly Dictionary<Rat, StandardTask> ratTasks = new();
	// Start is called before the first frame update
	void Start()
	{
		foreach (Rat rat in RatManager.Instance.allRats)
		{
			ratTasks.Add(rat, null);
		}
	}

	public List<Rat> AssignRatsToTask(List<Rat> rats, StandardTask task)
	{
		Queue<TaskPoint> availableSlots = new(task.slots.Where(s => s.Value == null).Select(s => s.Key));
		if (availableSlots.Count == 0) return null;

		// Might need some rework to make sure that rats cannot be assigned to the same task multiple times
		Queue<Rat> ratQueue = new(rats
			.Except(RatsOnTask(task)) // Exclude rats already on the task
			.OrderBy(r => Vector3.Distance(r.transform.position, task.transform.position)) // order by distance (ascending)
		);

		while (availableSlots.Count > 0 && ratQueue.Count > 0)
		{
			Rat r = ratQueue.Dequeue();
			TaskPoint s = availableSlots.Dequeue();

			AssignRats(task, r);
			task.slots[s] = r;
			r.SetDestination(s.taskPosition);
			r.Deselect();
		}
		return ratQueue.ToList();
	}

	public List<Rat> RatsOnTask(StandardTask task) => ratTasks.Where(p => p.Value == task).Select(p => p.Key).ToList();
	public void ClearRatsOnTask(StandardTask task)
	{
		foreach (var slot in task.slots.ToList())
		{
			task.slots[slot.Key] = null;
		}
		foreach (Rat r in RatsOnTask(task))
		{
			UnassignRats(r);
		}
	}
	public void UnassignRats(params Rat[] rats)
	{
		foreach (Rat rat in rats)
		{
			ratTasks[rat] = null;
		}
	}
	public void AssignRats(StandardTask task, params Rat[] rats)
	{
		foreach (Rat rat in rats)
		{
			ratTasks[rat] = task;
		}
	}

	public bool RatInPlace(Rat r)
	{
		TaskPoint tp = ratTasks[r].slots.Where(s => s.Value == r).Select(s => s.Key).FirstOrDefault();

		return tp != null && Vector3.Distance(tp.taskPosition, r.transform.position) < 0.1f;
	}
}
