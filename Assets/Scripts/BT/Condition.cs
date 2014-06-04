using UnityEngine;
using System.Collections;

public delegate bool ConditionFunction();

public class Condition {

	private ConditionFunction CachedFunc;

	public bool Result { get; set; }


	public Condition(ConditionFunction condition) {
		if (condition == null)
			throw new System.ArgumentNullException("condition", "ConditionFunction in Condition cannot be null");

		this.CachedFunc = condition;
	}

	public bool GetIsConditionTrue() {
		if (this.CachedFunc != null) {
			this.Result = this.CachedFunc();
		}

		return this.Result;
	}

}
