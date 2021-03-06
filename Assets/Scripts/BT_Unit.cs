﻿using UnityEngine;
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


		Task findTargetTask = this.gameObject.AddComponent<Task>().Initialize(
		() => {
			this.attackTarget = _fsmUnitRef as Entity;
			if (attackTarget != null) {
				//Debug.Log(this.Name + " found target: " + attackTarget.Name);
				return Action.ActionState.ACTION_DONE;
			}
			else
				return Action.ActionState.ACTION_ABORTED;
		},
		() => {
			return this.attackTarget == null;
		}, 1f, "Find Target Task");


		Task moveTask = this.gameObject.AddComponent<Task>().Initialize(
		() => {
			if (attackTarget == null) 
				return Action.ActionState.ACTION_ABORTED;
			else if (GetIsWithinAttackingRange(attackTarget))
				return Action.ActionState.ACTION_DONE;
			else if (_gameController.CurrentState != GameController.GameState.PLAYING) 
				return Action.ActionState.ACTION_CANCELLED;
			else {
				MoveTo(_fsmUnitRef.transform);
				return Action.ActionState.ACTION_RUNNING;
			}
		},
		() => {
			return GetIsWithinPerceptionRange(attackTarget) && !GetIsWithinAttackingRange(attackTarget);
		}, 0.8f, "Move Task");


		Task attackTask = this.gameObject.AddComponent<Task>().Initialize(
		() => {
			if (attackTarget == null)
				return Action.ActionState.ACTION_ABORTED;
			else if (attackTarget.IsDead)
				return Action.ActionState.ACTION_DONE;
			else if (_gameController.CurrentState != GameController.GameState.PLAYING) 
				return Action.ActionState.ACTION_CANCELLED;
			else {
				Attack(attackTarget);
				return Action.ActionState.ACTION_RUNNING;
			}
		},
		() => {
			return GetIsWithinAttackingRange(attackTarget) && !this.GetShouldFlee();
		}, 0.5f, "Attack Task");


		Task fleeTask = this.gameObject.AddComponent<Task>().Initialize(
		() => {
			if (attackTarget == null) 
				return Action.ActionState.ACTION_ABORTED;			
			else if (_gameController.CurrentState != GameController.GameState.PLAYING) 
				return Action.ActionState.ACTION_CANCELLED;
			else if (GetIsWithinPerceptionRange(attackTarget)) {
				Flee();
				return Action.ActionState.ACTION_RUNNING;
			}
			else 
				return Action.ActionState.ACTION_DONE;
		},
		() => {
			return GetIsWithinPerceptionRange(attackTarget) && this.GetShouldFlee();
		}, 0.7f, "Flee Task");


		Selector topSelector = this.gameObject.AddComponent<Selector>();
		topSelector.Initialize(fleeTask, attackTask, moveTask, findTargetTask);
		topSelector.BTName = "BT Unit Selector";
		topSelector.Looping = true;

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
		else {
			StopMoving();
			StopAllAnimations();
		}
	}

	void LateUpdate() {
		if (IsDead) 
			Destroy(this.gameObject, 0.1f);
	}

}
