using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BT_Unit : Entity {

	public List<BTObject> ActionQueue = new List<BTObject>();

	private FSM_Unit _fsmUnitRef;

	// Use this for initialization
	protected override void Start () {
		base.Start();

		_fsmUnitRef = GameObject.FindGameObjectWithTag("FSM_Unit").GetComponent<FSM_Unit>();
		if (_fsmUnitRef == null)
			Debug.LogError("Could not find FSM Unit");


		Task moveTask = this.gameObject.AddComponent<Task>().Initialize(() => {
			if (_fsmUnitRef == null) 
				return Action.ActionState.ACTION_ABORTED;
			else if (GetIsWithinAttackingRange(_fsmUnitRef as Entity))
				return Action.ActionState.ACTION_DONE;
			else {
				MoveTo(_fsmUnitRef.transform);
				return Action.ActionState.ACTION_RUNNING;
			}
		},
		() => {
			return GetIsWithinPerceptionRange(_fsmUnitRef as Entity) && !GetIsWithinAttackingRange(_fsmUnitRef as Entity);
		}, 0.1f, "Move Task");


		Task attackTask = this.gameObject.AddComponent<Task>().Initialize(() => {
			if (_fsmUnitRef.IsDead)
				return Action.ActionState.ACTION_DONE;
			else if (_fsmUnitRef == null)
				return Action.ActionState.ACTION_ABORTED;
			else {
				Attack(_fsmUnitRef as Entity);
				return Action.ActionState.ACTION_RUNNING;
			}
		},
		() => {
			return GetIsWithinAttackingRange(_fsmUnitRef as Entity);
		}, 0.7f, "Attack Task");


		/*Task fleeTask = this.gameObject.AddComponent<Task>().Initialize(() => {
			if (_fsmUnitRef == null) 
				return Action.ActionState.ACTION_ABORTED;			
			else if (GetIsWithinPerceptionRange(_fsmUnitRef as Entity)) {
				Flee();
				return Action.ActionState.ACTION_RUNNING;
			}
			else 
				return Action.ActionState.ACTION_DONE;
		},
		() => {
			return GetIsWithinPerceptionRange(_fsmUnitRef as Entity) && this.GetShouldFlee();
		}, 1f, "Flee Task");*/

		List<BTObject> taskList = new List<BTObject>();
		taskList.Add(moveTask);
		taskList.Add(attackTask);
		//taskList.Add(fleeTask);

		Selector topSelector = this.gameObject.AddComponent<Selector>();
		topSelector.Initialize(taskList, "BT Unit Selector", true);

		ActionQueue.Add(topSelector);
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();

		if (_gameController.CurrentState == GameController.GameState.PLAYING) {
			if (ActionQueue.Count > 0) {
				if (!ActionQueue[0].bDoneRunning) {
					ActionQueue[0].StartObject();
				}
				/*else {
					if (ActionQueue[0].RemoveSelf())
						ActionQueue.RemoveAt(0);
				}*/
			}
		}
	}

	void LateUpdate() {
		if (IsDead) 
			Destroy(this.gameObject, 0.1f);
	}

}
