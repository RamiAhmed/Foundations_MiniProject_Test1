using UnityEngine;
using System.Collections;

public delegate Action.ActionState ActionFunction();

public class Action  {

	private ActionFunction CachedFunc;

	public enum ActionState {
		ACTION_WAITING,
		ACTION_RUNNING,
		ACTION_DONE,
		ACTION_CANCELLED,
		ACTION_ABORTED
	};

	public ActionState CurrentState = ActionState.ACTION_WAITING;


	public Action(ActionFunction action) {
		if (action == null)
			throw new System.ArgumentNullException("action", "ActionFunction supplied to Action cannot be null");

		this.CachedFunc = action;
	}

	public ActionState RunAction() {
		if (this.CachedFunc != null) {
			this.CurrentState = this.CachedFunc();
		}
		else {
			Debug.LogWarning("Action has no OnAction set");
			this.CurrentState = ActionState.ACTION_CANCELLED;
		}

		return this.CurrentState;
	}
}
