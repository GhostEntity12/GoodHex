using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public readonly Dictionary<Rat, Assignable> ratTasks = new();

	public List<Rat> AssignRats(List<Rat> rats, ProgressTask task)
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
			RegisterRat(task, availableSlots.Dequeue(), ratQueue.Dequeue());
		}
		return ratQueue.ToList();
	}

	/// <summary>
	/// Sets the rats to a task in the <Rat, Task> Dictionary
	/// </summary>
	/// <param name="rats"></param>
	/// <param name="task"></param>
	/// <returns></returns>
	public void AssignRats(List<Rat> rats, RatWarp warp)
	{
		foreach (Rat rat in rats)
		{
			RegisterRat(warp, warp.TaskPoints[0], rat);
		}
	}

	public List<Rat> RatsOnTask(Assignable task) => ratTasks.Where(p => p.Value == task).Select(p => p.Key).ToList();
	public void ClearRatsOnTask(Assignable task)
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
				if (ratTasks[rat] is ProgressTask)
				{
					GetTaskPoint(rat).rat = null;
				}
				ratTasks.Remove(rat);
			}
		}
	}

	void RegisterRat(Assignable task, TaskPoint slot, Rat rat)
	{
		if (ratTasks.ContainsKey(rat) && task != ratTasks[rat])
		{
			UnassignRats(rat);
		}
		
		// Register to dictionary
		ratTasks.Add(rat, task);

		if (task is ProgressTask)
		{
			// Assign to slot
			slot.rat = rat;
		}

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

	public float GetDistanceToTask(Rat r)
	{
		if (ratTasks.ContainsKey(r))
		{
			return Vector3.Distance(r.transform.position, ratTasks[r].transform.position);
		}
		else return -1;
	}
}
