using UnityEngine;
using System.Collections;

public delegate bool ConditionFunction();

public class Condition {
	
	public event ConditionFunction OnCondition;

	public bool Result { get; set; }


	public Condition(ConditionFunction condition) {
		this.AddCondition(condition);
	}

	public void AddCondition(ConditionFunction condition) {
		this.OnCondition += condition;
	}

	public void RemoveCondition(ConditionFunction condition) {
		this.OnCondition -= condition;
	}

	public bool GetIsConditionTrue() {
		if (this.OnCondition != null) {
			this.Result = this.OnCondition();
		}

		return this.Result;
	}

}
