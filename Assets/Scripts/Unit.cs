using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Unit : MonoBehaviour {


	public List<BTObject> ActionQueue = new List<BTObject>();

	public List<Vector3> targets = new List<Vector3>();


	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Space)) {
			List<Task> taskList = new List<Task>();
			for (int i = 0; i < targets.Count; i++) {
				Task moveTask = this.gameObject.AddComponent<Task>();
				int index = i;
				moveTask.Initialize(() => {return ActionMove(index);});
				taskList.Add(moveTask);
			}
			
			Sequence moveSequence = this.gameObject.AddComponent<Sequence>();
			moveSequence.Initialize(taskList);

			ActionQueue.Add(moveSequence);
		}
		else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			List<Task> countingList = new List<Task>();
			
			for (int i = 0; i < 100; i++) {
				Task countTask = this.gameObject.AddComponent<Task>();
				int index = i;
				countTask.Initialize(
					() => {
						Debug.Log("Counting: " + index.ToString());
						return Action.ActionState.ACTION_DONE;
					},
					() => {
						return (index == Mathf.RoundToInt(Time.time));
					}
				);
				countingList.Add(countTask);
			}
			
			Selector countSelector = this.gameObject.AddComponent<Selector>();
			countSelector.Initialize(countingList);
			
			ActionQueue.Add(countSelector);
		}
		/*
		if (Random.value < 0.01f) {
			Task moveTask = this.gameObject.AddComponent<Task>();
			//moveTask.Initialize((() => {return ActionMove(Random.Range(0,3));}), Random.value);
			moveTask.Initialize(ActionMove, Random.value);

			ActionQueue.Add(moveTask);
		}*/

		if (ActionQueue.Count > 0) {
			if (!ActionQueue[0].bDoneRunning) {
				ActionQueue[0].StartObject();
			}
			else {
				ActionQueue[0].RemoveSelf();
				ActionQueue.RemoveAt(0);
			}
		}
	}

	public Action.ActionState ActionMove(int targetIndex) {
		//Debug.Log("Move to :  " + targetIndex);
		float movementSpeed = 2f;

		Vector3 target = targets[targetIndex];

		Vector3 dir = target - this.transform.position;
		float magnitude = dir.magnitude;
		Vector3 dir_norm = dir / magnitude;

		this.transform.position += dir_norm * movementSpeed * Time.deltaTime;

		if (Vector3.Distance(target, this.transform.position) <= movementSpeed/2f) {
			return Action.ActionState.ACTION_DONE;
		}
		else 
			return Action.ActionState.ACTION_RUNNING;
	}

}
