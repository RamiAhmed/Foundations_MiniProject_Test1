using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameController : MonoBehaviour {

	public enum GameState {
		WAITING,
		PLAYING,
		PAUSED,
		ENDING
	};

	public GameState CurrentState = GameState.WAITING;

	public float GameTime = 0f;

	public bool bGameEnded = false;

	public GameObject UnitParent;
	private int initialUnitCount = 0;

	void Start() {
		if (UnitParent == null) {
			Debug.LogError("Unit Parent game object has not been set on GameController");
		}
		else 
			initialUnitCount = UnitParent.transform.childCount;
	}

	// Update is called once per frame
	void Update () {
		if (CurrentState == GameState.PLAYING) {
			GameTime += Time.deltaTime;

			if (UnitParent.transform.childCount < initialUnitCount) {
				CurrentState = GameState.ENDING;
			}
		}
	}

	void OnGUI() {
		if (CurrentState == GameState.ENDING) {
			float endWidth = 250f;
			float endHeight = 80f;
			if (!GUI.skin.box.wordWrap)
				GUI.skin.box.wordWrap = true;

			GameObject winner = UnitParent.transform.GetChild(0).gameObject;
			string winnerName = winner.GetComponent<BT_Unit>() != null ? "BT Unit" : "FSM Unit";
			string winnerHP = winner.GetComponent<Entity>().CurrentHitPoints.ToString("F0");
			string winnerHPPercentage = (winner.GetComponent<Entity>().CurrentHitPoints / winner.GetComponent<Entity>().MaxHitPoints).ToString("F1") + "%";

			string endString = "The game has ended.\nThe winner was " + winnerName + ".\nThe winner has " + winnerHP + " HP (" + winnerHPPercentage + ") left.\n The fight took " + GameTime.ToString("F1") + " seconds.";
			GUI.Box(new Rect((Screen.width/2f) - (endWidth/2f), (Screen.height/2f) - (endHeight/2f), endWidth, endHeight), endString);
		}

		GUILayout.BeginArea(new Rect(5f, 5f, Screen.width-10f, 50f));
		GUILayout.BeginHorizontal();

		if (CurrentState != GameState.PLAYING) {
			if (GUILayout.Button("Play")) {
				CurrentState = GameState.PLAYING;
			}
		}
		else {
			if (GUILayout.Button("Pause")) {
				CurrentState = GameState.PAUSED;
			}
		}

		if (GUILayout.Button("Restart")) {
			Application.LoadLevel(Application.loadedLevelName);
		}

		if (GUILayout.Button("Exit")) {
			Application.Quit();
		}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		float width = 120f,
		     height = 30f;
		GUI.Box(new Rect(Screen.width-(width+5f), Screen.height-(height+5f), width, height), "State: " + CurrentState.ToString());

		GUI.Box(new Rect(5f, 50f, width, height), "Time: " + GameTime.ToString("F1"));
	}
}
