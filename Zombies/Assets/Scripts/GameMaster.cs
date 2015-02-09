using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameMaster : MonoBehaviour {

	public int initialRound = 1;
	public float roundDelay = 5, randomSoundDelay = 40;
	public GameObject zombie1, zombie2, zombie3, zombie4, zombie5, zombie6, fatZombie;
	public GameObject pivot1, pivot2, pivot3, pivot4;
	private static GameObject player;
	private int zombiesToSpawn;
	private static int zombiesRemaining, round, bossCount, zombiesToPackage = 0, h, m, s, i, z;
	private static bool gameOver, isWaitingRound, pausedGame, isWaitingClock, pedirPaquete;
	private float roundDelayGUI = 0, randomSoundTime;
	private GUIStyle styleRound, packageStyle;
	public static AudioSource[] audioGM;
	private List<GameObject> pivots, zombies;

	void Start() {
		player = GameObject.FindGameObjectWithTag("Player");
		pivots = new List<GameObject>();
		zombies = new List<GameObject>();
		pivots.Add(pivot1);
		pivots.Add(pivot2);
		pivots.Add(pivot3);
		pivots.Add(pivot4);
		zombies.Add(zombie1);
		zombies.Add(zombie2);
		zombies.Add(zombie3);
		zombies.Add(zombie4);
		zombies.Add(zombie5);
		zombies.Add(zombie6);
		round = initialRound;
		zombiesRemaining = zombiesToSpawn = getZombiesPerRound(round);
		h = m = s = i = z = 0;
		bossCount = zombiesRemaining/20;
		randomSoundTime = randomSoundDelay;
		gameOver = isWaitingClock = isWaitingRound = pedirPaquete = pausedGame = false;
		audioGM = GetComponents<AudioSource>();
	}

	void Update () {
		if (!gameOver) {
			if (Input.GetKeyDown(KeyCode.P)) // Pausa
				pauseGame();
			else if (zombiesToSpawn > 0 && !pausedGame) {
				if (i == pivots.Count) i = 0;
				if (z == zombies.Count) z = 0;
				Instantiate(zombies[z], pivots[i].transform.position, Quaternion.identity);
				--zombiesToSpawn;
				if (bossCount > 0) {
					Instantiate(fatZombie, pivots[i].transform.position, Quaternion.identity);
					--bossCount; ++zombiesRemaining;
				}
				++i; ++z;
			}
			if (zombiesRemaining <= 0 && !isWaitingRound && !pausedGame) {
				isWaitingRound = true;
				roundDelayGUI = roundDelay;
			}
			if (!isWaitingClock && !pausedGame)
				StartCoroutine(playedTime());
			if (isWaitingRound && !pausedGame) {
				roundDelayGUI -= Time.deltaTime;
				if (roundDelayGUI <= 0)
					nextRound();
			}
			if (randomSoundTime <= 0) {
				audioGM[Random.Range(2, 5)].Play();
				randomSoundTime = randomSoundDelay;
			}
			else
				randomSoundTime -= Time.deltaTime;
		}
	}

	void OnGUI() {
		styleRound = new GUIStyle(GUI.skin.textField);
		styleRound.alignment = TextAnchor.MiddleCenter;
		styleRound.fontStyle = FontStyle.Bold;
		GUI.TextField(new Rect(10, 50, 80, 20), "ROUND " + round, styleRound);
		GUI.Label(new Rect(15, 80, Screen.width, Screen.height), "Zombies left: " + zombiesRemaining);
		//GUI.Label(new Rect(15, 90, Screen.width, Screen.height), "Tiempo sobrevivido: " + getClock());
		//GUI.Label(new Rect(15, 125, Screen.width, Screen.height), "Arma Actual: " + PlayerLogic.GetWeapon());
		//GUI.Label(new Rect(15, 150, Screen.width, Screen.height), "Stamina: " + PlayerLogic.GetStamina());
		if (isWaitingRound && roundDelayGUI >= 1)
			GUI.TextField (new Rect(400, 300, 500, 30), "Round " + (round + 1) +
			               " will start in... " + roundDelayGUI.ToString("#."), styleRound);
		/*if (pedirPaquete)
			GUI.TextField(new Rect(1000, 70, 300, 25), "PAQUETE DISPONIBLE: Munición (5), Vida (6)", packageStyle);*/
		if (pausedGame) {
			packageStyle = new GUIStyle(GUI.skin.textField);
			packageStyle.alignment = TextAnchor.MiddleCenter;
			packageStyle.fontStyle = FontStyle.Bold;
			packageStyle.normal.textColor = Color.cyan;
			GUI.TextField(new Rect(Screen.width/2 - 100, Screen.height/2 - 100, 200, 25),
			              "GAME PAUSED (P)", packageStyle);
		}
	}

	public static void zombieKilled(bool boss) {
		--zombiesRemaining;
		if (!pedirPaquete && !boss) {
			if (zombiesToPackage >= 49) {
				audioGM[5].Play();
				pedirPaquete = true;
			}
			++zombiesToPackage;
		}
	}

	public static int getZombiesToPackage(){
		return zombiesToPackage;
	}

	private void nextRound() {
		isWaitingRound = false;
		++round;
		zombiesRemaining = zombiesToSpawn = getZombiesPerRound(round);
		bossCount = zombiesRemaining/20;
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Dead"))
			Destroy(obj);
		player.SendMessage("checkUpgrades", round);
	}

	void restartGame() {
		setGameOver(false);
		round = initialRound;
		zombiesRemaining = zombiesToSpawn = getZombiesPerRound(round);
		bossCount = zombiesRemaining/20;
		foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
			Destroy(enemy);
		foreach (GameObject dead in GameObject.FindGameObjectsWithTag("Dead"))
			Destroy(dead);
		audioGM[0].Play(); //Alarma|Sirena
		audioGM[1].Play(); //Musica ambiental
		h = m = s = zombiesToPackage = 0;
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

	public static bool canPurchasePackage() {
		return pedirPaquete;
	}

	public static void packagePurchased() {
		pedirPaquete = false;
		zombiesToPackage = 0;
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

	public static int getRound() {
		return round;
	}

	public static bool isGameOver() {
		return gameOver;
	}

	public static void pauseGame() {
		pausedGame = !pausedGame;
		if (pausedGame) {
			foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
				enemy.SendMessage("StopGame", SendMessageOptions.DontRequireReceiver);
			player.SendMessage("stopPlayer", true);
			//TODO Menu de pausa
		}
		else { //Resume
			foreach (GameObject enemy in GameObject.FindGameObjectsWithTag("Enemy"))
				enemy.SendMessage("ResumeGame", SendMessageOptions.DontRequireReceiver);
			player.SendMessage("stopPlayer", false);
		}
	}
	
	public static bool isPausedGame() {
		return pausedGame;
	}
	
	public static void setGameOver(bool opt) {
		gameOver = opt;
	}
}