﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {
	
	public int round = 1;
	private int zombiesToSpawn;
	public float roundDelay = 5f;
	private static int zombiesRemaining, h, m, s;
	private static bool gameOver, isWaitingRound, isWaitingClock;
	private float roundDelayGUI;
	private GUIStyle styleRound;
	public GameObject zombie;
	public GameObject pivot1;
	public GameObject pivot2;
	public GameObject pivot3;
	public GameObject pivot4;
	List<GameObject> pivots;
	
	// Use this for initialization
	void Start () {
		pivots = new List<GameObject> ();
		pivots.Add (pivot1);
		pivots.Add (pivot2);
		pivots.Add (pivot3);
		pivots.Add (pivot4);
		zombiesRemaining = zombiesToSpawn = getZombiesPerRound(round);
		h = m = s = 0;
		roundDelayGUI = 0f;
		gameOver = isWaitingClock = isWaitingRound = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (!gameOver) {
			int i = 0;
			while (zombiesToSpawn > 0) {
				if (i == pivots.Count) i = 0;
				Instantiate(zombie, pivots[i].transform.position, Quaternion.identity);
				--zombiesToSpawn;
				++i;
			}
			if (zombiesRemaining == 0 && !isWaitingRound) {
				isWaitingRound = true;
				roundDelayGUI = roundDelay;
			}
			if (!isWaitingClock)
				StartCoroutine(playedTime());
			if (isWaitingRound) {
				roundDelayGUI -= Time.deltaTime;
				if (roundDelayGUI <= 0)
					nextRound();
			}
		}
	}

	void OnGUI() {
		styleRound = new GUIStyle (GUI.skin.textField);
		styleRound.alignment = TextAnchor.MiddleCenter;
		styleRound.fontStyle = FontStyle.Bold;
		GUI.TextField (new Rect (10, 10, 80, 20), "RONDA " + round, styleRound);
		GUI.Label(new Rect(15, 65, Screen.width, Screen.height), "Zombis restantes: " + zombiesRemaining);
		GUI.Label(new Rect(15, 90, Screen.width, Screen.height), "Tiempo sobrevivido: " + getClock());
		GUI.Label(new Rect(15, 125, Screen.width, Screen.height), "Arma Actual: " + PlayerLogic.GetWeapon());
		if (isWaitingRound && roundDelayGUI >= 1)
			GUI.TextField (new Rect(400, 300, 500, 30), "La RONDA " + (round + 1) +
			               " empezará en... " + roundDelayGUI.ToString("#."), styleRound);
	}

	public static void zombieKilled() {
		--zombiesRemaining;
	}

	private void nextRound() {
		isWaitingRound = false;
		++round;
		zombiesRemaining = zombiesToSpawn = getZombiesPerRound(round);
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Dead"))
			Destroy(obj);
	}

	void restartGame() {
		--round;
		setGameOver(false);
		round = 1;
		zombiesRemaining = zombiesToSpawn = getZombiesPerRound (round);
		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
			Destroy(enemy);
		foreach (GameObject dead in GameObject.FindGameObjectsWithTag("Dead"))
			Destroy(dead);
		h = m = s = 0;
	}

	IEnumerator playedTime() {
		isWaitingClock = true;
		yield return new WaitForSeconds(1);
		isWaitingClock = false;
		++s;
		if (s >= 60) {
			++m;
			s = 0;
			if (m >= 60) {
				++h;
				m = 0;
			}
		}
	}

	public static string getClock() {
		return h.ToString("D2") + ":" + m.ToString("D2") + ":" + s.ToString("D2");
	}

	public static int getZombiesPerRound (int round) {
		return round*round + 5;
	}
	
	// In seconds
	public static long getTime() {
		return h*3600 + m*60 + s;
	}

	public static bool isGameOver() {
		return gameOver;
	}

	public static void setGameOver(bool opt) {
		gameOver = opt;
	}
}
