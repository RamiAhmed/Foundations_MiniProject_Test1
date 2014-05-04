using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public abstract class BTObject : MonoBehaviour {

	public enum TaskState {
		TASK_WAITING,
		TASK_RUNNING,
		TASK_DONE,
		TASK_CANCELLED,
		TASK_ABORTED
	}

	public float Priority = 0.5f;
	
	public TaskState CurrentState = TaskState.TASK_WAITING;

	public bool bDoneRunning = false;


	public abstract void StartObject();

	public abstract void RemoveSelf();

}
