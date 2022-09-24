using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class TaskManager : MonoBehaviour
{
	public readonly Dictionary<Rat, Assignable> ratTasks = new();

	public List<Rat> AssignRats(Assignable assignable, params Rat[] rats)
	{
		switch (assignable)
		{
			case ProgressTask pt:
				return AssignRats(pt, rats);
			case RatWarp rw:
				AssignRats(rw, rats);
				return new();
			case Pickupable p:
				return AssignRats(p, rats);
			default:
				return rats.ToList();
		}
	}

	public List<Rat> AssignRats(ProgressTask task, params Rat[] rats)
	{
		//if (task.Locked) return rats.ToList();

		// Get a list of the available slots
		Queue<TaskPoint> availableSlots = new(task.TaskPoints.Where(p => p.rat == null));
		if (availableSlots.Count == 0) return null;

		// Get a list of viable rats from the provided list
		Queue<Rat> ratQueue = new(rats
			.Except(RatsOnTask(task)) // Exclude rats already on the task or holding items
			.OrderBy(r => Vector3.Distance(r.transform.position, task.transform.position)) // order by distance (ascending)
		);
		List<Rat> heldItemRats = new();
		// While there are both rats and slots available, assign
		while (availableSlots.Count > 0 && ratQueue.Count > 0)
		{
			Rat r = ratQueue.Dequeue();
			if (!r.IsHoldingItem || (r.heldItem.ItemId == task.TriggerId))
			{
				RegisterRat(task, availableSlots.Dequeue(), r);
			}
			else
			{
				heldItemRats.Add(r);
			}
		}
		List<Rat> unassigned = ratQueue.ToList();
		unassigned.AddRange(heldItemRats);
		return unassigned;
	}

	public void AssignRats(RatWarp warp, params Rat[] rats)
	{
		foreach (Rat rat in rats)
		{
			RegisterRat(warp, warp.TaskPoints[0], rat);
		}
	}

	public List<Rat> AssignRats(Pickupable pickupable, params Rat[] rats)
	{
		TaskPoint tp = pickupable.TaskPoints[0];
		if (tp.rat) return null;

		// Get a list of viable rats from the provided list
		Queue<Rat> ratQueue = new(rats
			.Except(RatsOnTask(pickupable)) // Exclude rats already on the task
			.OrderBy(r => Vector3.Distance(r.transform.position, pickupable.transform.position)));

		List<Rat> heldItemRats = new();
		while (ratQueue.Count > 0)
		{
			Rat r = ratQueue.Dequeue();
			if (r.IsHoldingItem)
			{
				heldItemRats.Add(r);
			}
			else
			{
				RegisterRat(pickupable, tp, r);
				break;
			}
		}
		List<Rat> unassigned = ratQueue.ToList();
		unassigned.AddRange(heldItemRats);
		return unassigned;
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
			if (!rat) continue;
			if (ratTasks.ContainsKey(rat))
			{
				if (ratTasks[rat] is not RatWarp)
				{
					GetTaskPoint(rat).rat = null;
				}
				ratTasks.Remove(rat);
			}
		}
	}

	void RegisterRat(Assignable task, TaskPoint slot, Rat rat)
	{
		if (Random.value < 0.5f)
		{
			rat.SetEmote(RatEmotes.Emotes.Idea);
		}

		if (Random.value > 0.75f)
		{
			rat.SetEmote(RatEmotes.Emotes.Angry);
		}
		if (ratTasks.ContainsKey(rat) && task != ratTasks[rat])
		{
			UnassignRats(rat);
		}

		// Register to dictionary
		ratTasks.Add(rat, task);

		if (task is ProgressTask || task is Pickupable)
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
			Vector3 point;
			Assignable a = ratTasks[r];

			point = a switch
			{
				RatWarp rw => rw.TaskPoints[0].taskPosition,
				ProgressTask pt => GetTaskPoint(r).taskPosition,
				Pickupable p => GetTaskPoint(r).taskPosition,
				_ => throw new System.Exception("Unexpected type")
			};
			return Vector3.Distance(r.transform.position, point);
		}
		else return -1;
	}
}
