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
			List<BTObject> taskList = new List<BTObject>();
			for (int i = 0; i < targets.Count; i++) {
				Task moveTask = this.gameObject.AddComponent<Task>();
				int index = i;
				moveTask.Initialize(() => {return ActionMove(index);});
				taskList.Add(moveTask as BTObject);
			}
			
			Sequence moveSequence = this.gameObject.AddComponent<Sequence>();
			moveSequence.Initialize(taskList);
			moveSequence.Looping = true;

			ActionQueue.Add(moveSequence);
		}
		else if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift)) {
			List<BTObject> countingList = new List<BTObject>();
			
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
			countSelector.Looping = true;
			countSelector.Counter = 3;
			
			ActionQueue.Add(countSelector);
		}
		else if (Input.GetKeyDown(KeyCode.KeypadEnter) || Input.GetKeyDown(KeyCode.Return)) {
			List<BTObject> sequenceList = new List<BTObject>();

			List<BTObject> taskList1 = new List<BTObject>();
			List<BTObject> taskList2 = new List<BTObject>();

			Task move1 = this.gameObject.AddComponent<Task>();
			move1.Initialize(
				() => {
					return ActionMove(0);
				}
			);
			taskList1.Add(move1 as BTObject);

			Task move2 = this.gameObject.AddComponent<Task>();
			move2.Initialize(
				() => {
					return ActionMove (1);
				}
			);
			taskList1.Add(move2 as BTObject);

			Sequence seq1 = this.gameObject.AddComponent<Sequence>();
			seq1.Initialize(taskList1);
			sequenceList.Add(seq1);


			Task move3 = this.gameObject.AddComponent<Task>();
			move3.Initialize(() => { return ActionMove(2); });
			taskList2.Add(move3 as BTObject);

			Task sayHello = this.gameObject.AddComponent<Task>();
			sayHello.Initialize(() => { Debug.Log("HELLO!!"); return Action.ActionState.ACTION_DONE; });
			taskList2.Add(sayHello as BTObject);

			Sequence seq2 = this.gameObject.AddComponent<Sequence>();
			seq2.Initialize(taskList2);
			sequenceList.Add(seq2);


			Sequence fullSeq = this.gameObject.AddComponent<Sequence>();
			fullSeq.Initialize(sequenceList);

			ActionQueue.Add(fullSeq);


		}

		if (ActionQueue.Count > 0) {
			if (!ActionQueue[0].bDoneRunning) {
				ActionQueue[0].StartObject();
			}
			else {
				if (ActionQueue[0].RemoveSelf())
					ActionQueue.RemoveAt(0);
			}
		}
	}

	public Action.ActionState ActionMove(int targetIndex) {
		//Debug.Log("Move to :  " + targetIndex);
		float movementSpeed = 2f;

		Vector3 target = targets[targetIndex];

		Vector3 direction = target - this.transform.position;
		float magnitude = direction.magnitude;
		Vector3 directionNormalized = direction / magnitude;

		this.transform.position += directionNormalized * movementSpeed * Time.deltaTime;

		if (Vector3.Distance(target, this.transform.position) <= movementSpeed/2f) {
			return Action.ActionState.ACTION_DONE;
		}
		else 
			return Action.ActionState.ACTION_RUNNING;
	}

}
