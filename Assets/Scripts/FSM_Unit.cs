using UnityEngine;
using System.Collections;

public class FSM_Unit : Entity {

	private enum UnitState {
		WAITING,
		ATTACKING,
		FLEEING,
		DEAD
	};

	private UnitState currentState = UnitState.WAITING;

	// Use this for initialization
	protected override void Start () {
		base.Start();
	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();
	}

	/*
	public Entity FollowOther(Entity target) {
		Entity newTarget = null;
		if (target != null) {
			Entity nearestEnemy = GetNearestUnit(_gameController.enemies);
			if (target.attackTarget != null) {
				newTarget = target.attackTarget;	
			}
			else if (nearestEnemy != null && GetIsWithinAttackingRange(nearestEnemy)) {
				newTarget = nearestEnemy;
			}
			else {
				if (!GetIsWithinAttackingRange(target)) {
					MoveTo(target.transform);
				}
			}
		}
		
		return newTarget;
	}

	public void FleeFrom(Transform target) {
		if (target != null) {
			FleeFrom(target.position);
		}
	}
	
	public void FleeFrom(Vector3 target) {
		if (target.sqrMagnitude > 0f) {
			Vector3 direction = (this.transform.position - target).normalized * MovementSpeed;
			MoveTo(direction);
		}
	}*/
}
