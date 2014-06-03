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

	private BT_Unit _btUnitRef;

	// Use this for initialization
	protected override void Start () {
		base.Start();

		_btUnitRef = GameObject.FindGameObjectWithTag("BT_Unit").GetComponent<BT_Unit>();
		if (_btUnitRef == null)
			Debug.LogError("Could not find BT Unit");

	}
	
	// Update is called once per frame
	protected override void Update () {
		base.Update();

		if (_gameController.CurrentState == GameController.GameState.PLAYING) {
			if (currentState == UnitState.WAITING) {

				if (_btUnitRef != null && !_btUnitRef.IsDead) {
					this.attackTarget = _btUnitRef as Entity;
					this.currentState = UnitState.ATTACKING;
					//Debug.Log(this.Name + " engages " + _btUnitRef.Name);
				}

			}
			else if (currentState == UnitState.ATTACKING) {
				if (attackTarget != null && !attackTarget.IsDead) {
					if (GetShouldFlee()) {
						this.currentState = UnitState.FLEEING;
					}
					else if (GetIsWithinAttackingRange(attackTarget)) {
						Attack(attackTarget);
					}
					else if (GetIsWithinPerceptionRange(attackTarget)) {
						this.MoveTo(attackTarget.transform);
					}
					else {
						Debug.LogWarning(attackTarget.Name + " is not in range for " + this.Name);
					}
				}
				else {
					this.currentState = UnitState.WAITING;
				}
			}
			else if (currentState == UnitState.FLEEING) {
				if (attackTarget != null && !attackTarget.IsDead) {
					if (GetShouldFlee() && GetIsWithinPerceptionRange(attackTarget)) {
						Flee();
					}
					else {
						currentState = UnitState.WAITING;
					}
				}
				else {
					this.currentState = UnitState.WAITING;
				}
			}
			else if (currentState == UnitState.DEAD) {
				if (!IsDead)
					IsDead = true;
			}
		}
	}

	void LateUpdate() {
		if (IsDead) 
			Destroy(this.gameObject, 0.1f);
	}
}
