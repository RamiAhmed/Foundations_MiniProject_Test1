using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public int Counter = 0;

	// Use this for initialization
	void Start () {
		/*
		Action hello = new Action();
		hello.OnAction += sayHello;

		Task helloTask = new Task(hello, null);
		Task hello2 = new Task(hello, null);
		Task hello3 = new Task(hello, null);

		List<Task> taskList = new List<Task>();
		taskList.Add(helloTask);
		taskList.Add(hello2);
		taskList.Add(hello3);

		Sequence manyHelloes = new Sequence(taskList);

		manyHelloes.RunSequence();
		*/	
		/*
		Action sayHello = new Action(say);

		Condition on0 = new Condition(isCounter0);
		Condition on1 = new Condition(isCounter1);
		Condition on2 = new Condition(isCounter2);
		Condition on4 = new Condition(isCounter4);

		Task task0 = new Task(sayHello, on0);
		Task task1 = new Task(sayHello, on1);
		Task task2 = new Task(sayHello, on2);
		Task task4 = new Task(sayHello, on4);

		List<Task> taskList = new List<Task>();
		taskList.Add(task0);
		taskList.Add(task1);
		taskList.Add(task2);
		taskList.Add(task4);

		Selector sayingHello = new Selector(taskList);
		sayingHello.RunSelector();
*/
	}

	private bool isCounter0() {
		return Counter == 0;
	}

	private bool isCounter1() {
		return Counter == 1;
	}

	private bool isCounter2() {
		return Counter == 2;
	}

	private bool isCounter4() {
		return Counter == 4;
	}

	private bool say() {
		Debug.Log("Hello " + Counter.ToString());
		return true;
	}

	private bool sayHello() {
		Debug.Log("Hello world");
		return true;
	}

	private bool sayHelloTen() {
		Debug.Log("Hello ten!");
		return true;
	}

	private bool GetIsCounter10() {
		return this.Counter == 10;
	}
	
	// Update is called once per frame
	void Update () {
		/*
		Action hello = new Action();
		hello.OnAction += sayHelloTen;

		Condition isCounter10 = new Condition();
		isCounter10.OnCondition += GetIsCounter10;

		Task helloOn10 = new Task(hello, isCounter10);
		helloOn10.RunTask();
	*/
		Counter = Mathf.RoundToInt(Time.time);

		//Debug.Log("Time: " + Counter);
	}
}
