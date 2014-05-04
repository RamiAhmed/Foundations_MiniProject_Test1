using UnityEngine;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public class Selector : BTObject {

	public List<Task> TaskSelectors { get; set; }
	
	private Task currentTask = null;

	public bool Result { get; set; }

	//public bool bIsSelectorDone = false;

	public void Initialize(List<Task> taskSelectors) {
		this.TaskSelectors = taskSelectors;
		this.TaskSelectors = this.TaskSelectors.OrderByDescending( x => x.Priority ).ToList();

		this.currentTask = this.TaskSelectors[0];
		this.Result = false;
	}
	
	public override void StartObject() {
		if (this.TaskSelectors == null || this.TaskSelectors.Count <= 0) {
			Debug.LogWarning("Selector has no Task Selector list set");
		}
		else {
			this.currentTask.StartObject();
		}
	}

	public override void RemoveSelf() {
		if (this.gameObject != null) {
			while (this.TaskSelectors.Count > 0) {
				this.TaskSelectors[0].RemoveSelf();
				this.TaskSelectors.RemoveAt(0);
			}

			Destroy(this,0.1f);
		}
	}

	private void Update() {
		if (!this.bDoneRunning && this.currentTask.bDoneRunning) {
			if (this.currentTask.CurrentState == Task.TaskState.TASK_ABORTED || this.currentTask.CurrentState == Task.TaskState.TASK_CANCELLED) {
				this.currentTask = getNextTask();

				if (this.currentTask == null) {
					this.Result = false;
					this.bDoneRunning = true;
				}
				else {
					this.currentTask.StartObject();
				}
			}
			else if (this.currentTask.CurrentState == Task.TaskState.TASK_DONE) {
				this.Result = true;
				this.bDoneRunning = true;
			}
		}
	}
	
	private Task getNextTask() {
		if (this.currentTask == null)
			return null;
		else {
			int index = this.TaskSelectors.IndexOf(this.currentTask) + 1;
			return (index < this.TaskSelectors.Count) ? this.TaskSelectors[index] : null;
		}
	}
}
