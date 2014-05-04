using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Selector : BTObject {

	public List<BTObject> TaskSelectors { get; set; }
	
	private BTObject currentTask = null;


	public void Initialize(List<BTObject> taskSelectors) {
		this.TaskSelectors = taskSelectors;
		this.TaskSelectors = this.TaskSelectors.OrderByDescending( x => x.Priority ).ToList();

		this.currentTask = this.TaskSelectors[0];
	}
	
	public override void StartObject() {
		if (this.TaskSelectors == null || this.TaskSelectors.Count <= 0) {
			Debug.LogWarning("Selector has no Task Selector list set");
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
			while (this.TaskSelectors.Count > 0) {
				this.TaskSelectors[0].RemoveSelf();
				this.TaskSelectors.RemoveAt(0);
			}

			Destroy(this, 0.1f);
		}
	}

	private void Update() {
		if (this.CurrentState == TaskState.TASK_RUNNING) {
			if (this.currentTask.CurrentState == Task.TaskState.TASK_ABORTED || this.currentTask.CurrentState == Task.TaskState.TASK_CANCELLED) {
				this.currentTask = getNextTask();

				if (this.currentTask == null) {
					this.CurrentState = this.currentTask.CurrentState;
				}
				else {
					this.currentTask.StartObject();
				}
			}
			else if (this.currentTask.CurrentState == Task.TaskState.TASK_DONE) {
				this.CurrentState = TaskState.TASK_DONE;
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
			int index = this.TaskSelectors.IndexOf(this.currentTask) + 1;
			return (index < this.TaskSelectors.Count) ? this.TaskSelectors[index] : null;
		}
	}
}
