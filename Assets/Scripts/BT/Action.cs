using UnityEngine;
using System.Collections;

public delegate Action.ActionState ActionFunction();

public class Action  {

	public event ActionFunction OnAction;

	public enum ActionState {
		ACTION_WAITING,
		ACTION_RUNNING,
		ACTION_DONE,
		ACTION_CANCELLED,
		ACTION_ABORTED
	}

	public ActionState CurrentState = ActionState.ACTION_WAITING;


	public Action(ActionFunction action) {
		this.AddAction(action);
	}

	public void AddAction(ActionFunction action) {
		this.OnAction += action;
	}
	public void RemoveAction(ActionFunction action) {
		this.OnAction -= action;
	}

	public ActionState RunAction() {
		if (this.OnAction != null) {
			this.CurrentState = this.OnAction();
		}
		else {
			Debug.LogWarning("Action has no OnAction set");
			this.CurrentState = ActionState.ACTION_CANCELLED;
		}

		return this.CurrentState;
	}
}
