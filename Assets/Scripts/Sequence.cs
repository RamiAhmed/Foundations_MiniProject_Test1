using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Sequence : BTObject {

	public List<BTObject> TaskSequence { get; set; }

	private BTObject currentTask = null;


	public void Initialize(List<BTObject> taskSequence) {
		this.TaskSequence = taskSequence;
		this.TaskSequence = this.TaskSequence.OrderByDescending( x => x.Priority ).ToList();

		this.currentTask = this.TaskSequence[0];
	}

	public override void StartObject() {
		if (this.TaskSequence == null || this.TaskSequence.Count <= 0) {
			Debug.LogWarning("Sequence has no Task Sequence list set");
			this.CurrentState = TaskState.TASK_CANCELLED;
		}
		else {
			if (this.CurrentState == TaskState.TASK_WAITING) { 
				this.CurrentState = TaskState.TASK_RUNNING;
				this.currentTask.StartObject();
			}
		}
	}

	public override void RemoveSelf() {
		if (this.gameObject != null) {
			while (this.TaskSequence.Count > 0) {
				this.TaskSequence[0].RemoveSelf();
				this.TaskSequence.RemoveAt(0);
			}

			Destroy(this, 0.1f);
		}
	}

	private void Update() {
		if (this.CurrentState == TaskState.TASK_RUNNING) {
			if (this.currentTask.CurrentState == TaskState.TASK_DONE) {
				this.currentTask = getNextTask();

				if (this.currentTask == null) {
					this.CurrentState = TaskState.TASK_DONE;
				}
				else {
					this.currentTask.StartObject();
				}
			}
			else if (this.currentTask.CurrentState == Task.TaskState.TASK_ABORTED || this.currentTask.CurrentState == Task.TaskState.TASK_CANCELLED) {
				this.CurrentState = this.currentTask.CurrentState;
			}
		}
		else if (this.CurrentState != TaskState.TASK_WAITING) {
			if (!this.bDoneRunning) {
				this.bDoneRunning = true;
			}
		}
	}

	private BTObject getNextTask() {
		if (this.currentTask == null)
			return null;
		else {
			int index = this.TaskSequence.IndexOf(this.currentTask) + 1;
			return (index < this.TaskSequence.Count) ? this.TaskSequence[index] : null;
		}
	}
}
