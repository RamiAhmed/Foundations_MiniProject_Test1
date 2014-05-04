﻿using UnityEngine;
using System;
using System.Collections;

public class Task : BTObject  {
	
	public Action Action { get; set; }
	public Condition Condition { get; set; }

	public void Initialize(GetAction action, GetCondition condition, float priority) {
		this.Action = new Action(action);
		this.Condition = new Condition(condition);
		this.Priority = priority;
	}

	public void Initialize(GetAction action, GetCondition condition) {
		this.Action = new Action(action);
		this.Condition = new Condition(condition);
	}

	public void Initialize(GetAction action, float priority) {
		this.Action = new Action(action);
		this.Condition = null;
		this.Priority = priority;
	}

	public void Initialize(GetAction action) {
		this.Action = new Action(action);
		this.Condition = null;
	}


	public override void StartObject() {
		if (this.Action != null) {
			if (this.CurrentState == TaskState.TASK_WAITING) {
				this.CurrentState = TaskState.TASK_RUNNING;
			}
		}
	}

	public override void RemoveSelf() {
		if (this.gameObject != null) {
			this.Condition = null;
			this.Action = null;

			Destroy(this, 0.1f);
		}
	}

	private void runAction() {
		Action.ActionState actionState = this.Action.RunAction();

		if (actionState == Action.ActionState.ACTION_DONE) 
			this.CurrentState = TaskState.TASK_DONE;
		else if (actionState == Action.ActionState.ACTION_ABORTED) 
			this.CurrentState = TaskState.TASK_ABORTED;
		else if (actionState == Action.ActionState.ACTION_CANCELLED)
			this.CurrentState = TaskState.TASK_CANCELLED;

	}

	private void Update() {
		if (this.CurrentState == TaskState.TASK_RUNNING) {
			if (this.Action != null) {
				if (this.Condition != null) {
					if (this.Condition.GetIsConditionTrue()) {
						runAction();
					}
					else {
						this.CurrentState = TaskState.TASK_ABORTED;
					}
				}
				else {
					runAction();
				}
			}
			else {
				this.CurrentState = TaskState.TASK_CANCELLED;
				Debug.LogWarning("Task has no Action set");
			}
		}
		else if (this.CurrentState != TaskState.TASK_WAITING) {
			if (!this.bDoneRunning) {
				this.bDoneRunning = true;
			}
		}
	}

}
