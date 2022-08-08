using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public readonly Dictionary<Rat, StandardTask> ratTasks = new();

	/// <summary>
	/// Sets the rats to a task in the <Rat, Task> Dictionary
	/// </summary>
	/// <param name="rats"></param>
	/// <param name="task"></param>
	/// <returns></returns>
	public List<Rat> AssignRatsToTask(List<Rat> rats, StandardTask task)
	{
		// Get a list of the available slots
		Queue<TaskPoint> availableSlots = new(task.ratSlots.Where(s => s.Value == null).Select(s => s.Key));
		if (availableSlots.Count == 0) return null;

		// Get a list of viable rats from the provided list
		Queue<Rat> ratQueue = new(rats
			.Except(RatsOnTask(task)) // Exclude rats already on the task
			.OrderBy(r => Vector3.Distance(r.transform.position, task.transform.position)) // order by distance (ascending)
		);

		// While there are both rats and slots available, assign
		while (availableSlots.Count > 0 && ratQueue.Count > 0)
		{
			AssignRats(task, availableSlots.Dequeue(), ratQueue.Dequeue());
		}
		return ratQueue.ToList();
	}

	public List<Rat> RatsOnTask(StandardTask task) => ratTasks.Where(p => p.Value == task).Select(p => p.Key).ToList();
	public void ClearRatsOnTask(StandardTask task)
	{
		task.ratSlots.Clear();
		UnassignRats(RatsOnTask(task).ToArray());
	}

	public void UnassignRats(params Rat[] rats)
	{
		foreach (Rat rat in rats)
		{
			if (ratTasks.ContainsKey(rat))
			{
				ratTasks.Remove(rat);
			}
		}
	}

	void AssignRats(StandardTask task, TaskPoint slot, Rat rat)
	{
		// Register to dictionary
		ratTasks.Add(rat, task);
		// Assign to slot
		task.ratSlots[slot] = rat;

		// Set destination and remove from active selection
		rat.SetDestination(slot.taskPosition);
		rat.Deselect();
	}

	/// <summary>
	/// Whether the rat is in the acceptable range of the task point
	/// </summary>
	/// <param name="r"></param>
	/// <returns></returns>
	public bool RatInPlace(Rat r)
	{
		// Return false if rat is not assigned to a task
		if (!ratTasks.ContainsKey(r)) return false;

		// This is a little messy, rework?
		TaskPoint tp = ratTasks[r].ratSlots.Where(s => s.Value == r).Select(s => s.Key).FirstOrDefault();

		return tp != null && Vector3.Distance(tp.taskPosition, r.transform.position) < 0.1f;
	}

	public TaskPoint GetTaskPoint(Rat r)
	{
		if (!ratTasks.ContainsKey(r)) return null;

		//TaskPoint tp = ratTasks[r].

		return tp;
	}
}
