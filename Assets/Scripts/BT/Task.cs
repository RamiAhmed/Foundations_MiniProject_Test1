using UnityEngine;
using System;
using System.Collections;

public class Task : BTObject  {
	
	public Action Action { get; set; }
	public Condition Condition { get; set; }


	public Task Initialize(ActionFunction action, ConditionFunction condition, float priority, string name, bool bLooping, int counter) {
		if (action == null)
			throw new System.ArgumentNullException("action", "ActionFunction supplied to Task cannot be null");
		if (condition == null)
			throw new System.ArgumentNullException("condition", "ConditionFunction supplied to Task cannot be null; use an overload without the Condition parameter");

		this.Action = new Action(action);
		this.Condition = new Condition(condition);
		this.Priority = priority;

		if (!string.IsNullOrEmpty(name))
			this.BTName = name;

		this.Looping = bLooping;
		this.Counter = counter;

		return this;
	}

	public Task Initialize(ActionFunction action, ConditionFunction condition, float priority, string name) {
		return Initialize(action, condition, priority, name, false, 0);
	}

	public Task Initialize(ActionFunction action, ConditionFunction condition, float priority) {
		return Initialize(action, condition, priority, "");
	}

	public Task Initialize(ActionFunction action, ConditionFunction condition) {
		return Initialize(action, condition, 0.5f);
	}

	public Task Initialize(ActionFunction action, float priority, string name, bool bLooping, int counter) {
		if (action == null)
			throw new System.ArgumentNullException("action", "ActionFunction supplied to Task cannot be null");
		
		this.Action = new Action(action);
		this.Condition = null;
		this.Priority = priority;

		if (!string.IsNullOrEmpty(name))
			this.BTName = name;

		this.Looping = bLooping;
		this.Counter = counter;
		
		return this;
	}

	public Task Initialize(ActionFunction action, float priority, string name) {
		return Initialize(action, priority, name, false, 0);
	}

	public Task Initialize(ActionFunction action, float priority) {
		return Initialize(action, priority, "");
	}

	public Task Initialize(ActionFunction action) {
		return Initialize(action, 0.5f);
	}


	public override void StartObject() {
		if (this.Action != null) {
			if (this.CurrentState == TaskState.TASK_WAITING) {
				this.CurrentState = TaskState.TASK_RUNNING;

				if (this.Counter > 1 && !this.Looping)
					this.Looping = true;

				if (BTName == "BTObject")
					BTName = "Task";
				
				if (bPaused)
					bPaused = false;
			}
		}
	}

	public override bool RemoveSelf() {
		if (this.gameObject != null) {
			if (this.Looping && (this.Counter <= 1 || this.counterCount < this.Counter)) {
				//Debug.Log("Looping");
			}
			else {
				this.Condition = null;
				this.Action = null;

				Destroy(this, 0.1f);

				return true;
			}
		}

		return false;
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
		if (bPaused)
			return;

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
				if (this.Looping) {
					if (this.Counter <= 1 || this.counterCount < this.Counter) {
						this.counterCount++;
						this.CurrentState = TaskState.TASK_WAITING;

						this.Action.CurrentState = Action.ActionState.ACTION_WAITING;
					}
					else {
						this.Looping = false;
						this.bDoneRunning = true;
					}
				}
				else {
					this.bDoneRunning = true;
				}
			}
		}
	}

}
