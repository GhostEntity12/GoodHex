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
		Queue<TaskPoint> availableSlots = new(task.TaskPoints.Where(p => p.rat == null));
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
		foreach (TaskPoint tp in task.TaskPoints)
		{
			UnassignRats(tp.rat);
			tp.rat = null;
		}
	}

	public void UnassignRats(params Rat[] rats)
	{
		foreach (Rat rat in rats)
		{
			if (ratTasks.ContainsKey(rat))
			{
				GetTaskPoint(rat).rat = null;
				ratTasks.Remove(rat);
			}
		}
	}

	void AssignRats(StandardTask task, TaskPoint slot, Rat rat)
	{
		// Register to dictionary
		ratTasks.Add(rat, task);
		// Assign to slot
		slot.rat = rat;

		// Set destination and remove from active selection
		rat.SetDestination(slot.taskPosition);
		rat.Deselect();
	}

	public TaskPoint GetTaskPoint(Rat r)
	{
		if (!ratTasks.ContainsKey(r)) return null;

		foreach (TaskPoint taskPoint in ratTasks[r].TaskPoints)
		{
			if (taskPoint.rat == r)
			{
				return taskPoint;
			}
		}
		return null;
	}
}
