using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Sequence : BTObject {

	public List<Task> TaskSequence { get; set; }

	private Task currentTask = null;

	public bool Result { get; set; }

	//public bool bIsSequenceDone = false;

	public void Initialize(List<Task> taskSequence) {
		this.TaskSequence = taskSequence;
		this.TaskSequence = this.TaskSequence.OrderByDescending( x => x.Priority ).ToList();

		this.currentTask = this.TaskSequence[0];
		this.Result = false;
	}

	public override void StartObject() {
		if (this.TaskSequence == null || this.TaskSequence.Count <= 0) {
			Debug.LogWarning("Sequence has no Task Sequence list set");
		}
		else {
			this.currentTask.StartObject();
		}
	}

	public override void RemoveSelf() {
		if (this.gameObject != null) {
			while (this.TaskSequence.Count > 0) {
				this.TaskSequence[0].RemoveSelf();
				this.TaskSequence.RemoveAt(0);
			}

			Destroy(this,0.1f);
		}
	}

	private void Update() {
		if (!this.bDoneRunning && this.currentTask.bDoneRunning) {
			if (this.currentTask.CurrentState == Task.TaskState.TASK_DONE) {
				this.currentTask = getNextTask();

				if (this.currentTask == null) {
					this.Result = true;
					this.bDoneRunning = true;
				}
				else {
					this.currentTask.StartObject();
				}
			}
			else if (this.currentTask.CurrentState == Task.TaskState.TASK_ABORTED || this.currentTask.CurrentState == Task.TaskState.TASK_CANCELLED) {
				this.Result = false;
				this.bDoneRunning = true;
			}
		}
	}

	private Task getNextTask() {
		if (this.currentTask == null)
			return null;
		else {
			int index = this.TaskSequence.IndexOf(this.currentTask) + 1;
			return (index < this.TaskSequence.Count) ? this.TaskSequence[index] : null;
		}
	}
}
