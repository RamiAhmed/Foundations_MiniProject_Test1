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

	// Update is called once per frame
	void Update () {
		if (CurrentState == GameState.PLAYING) {
			GameTime += Time.deltaTime;
		}
		else if (CurrentState == GameState.ENDING) {
			Application.Quit();
		}
	}

	void OnGUI() {
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
			CurrentState = GameState.ENDING;
		}

		GUILayout.EndHorizontal();
		GUILayout.EndArea();

		float width = 120f,
		     height = 30f;
		GUI.Box(new Rect(Screen.width-(width+5f), Screen.height-(height+5f), width, height), "State: " + CurrentState.ToString());

		GUI.Box(new Rect(5f, 50f, width, height), "Time: " + GameTime.ToString("F1"));
	}
}
