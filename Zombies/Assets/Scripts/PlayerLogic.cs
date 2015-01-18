using UnityEngine;
using System.Collections;

public class PlayerLogic : MonoBehaviour
{
	public float maxHealth = 100f, maxStamina = 100f,
		staminaPerSecond = 10f, staminaCostPerSecond = 20f;
	private bool restart = false, running = false;
	private float health, speed, runningSpeed;
	private GUIStyle healthStatus = new GUIStyle();
	private static int actualWeapon = 0;
	private static float stamina;

	// Use this for initialization
	void Start () {
		health = maxHealth;
		stamina = maxStamina;
		speed = gameObject.GetComponent<CharacterMotor>().movement.maxForwardSpeed;
		runningSpeed = (int)(speed*1.5f);
		healthStatus.normal.textColor = Color.green;
		healthStatus.fontStyle = FontStyle.Bold;
	}

	// Update is called once per frame
	void Update () {
		if(Input.GetKey(KeyCode.Alpha1)) {
			actualWeapon = 0;
		} else if (Input.GetKey(KeyCode.Alpha2)) {
			actualWeapon = 1;
		} else if (Input.GetKey(KeyCode.Alpha3)) {
			actualWeapon = 2;
		} else if (Input.GetKey(KeyCode.Alpha4)) {
			actualWeapon = 3;
		} else if (Input.GetKey(KeyCode.LeftShift)) {
			if (stamina > 0f) {
				if (!running) {
					changeMovementSpeed(runningSpeed);
					running = true;
				}
				else
					stamina = max(stamina - Time.deltaTime*staminaCostPerSecond, 0f);
			}
			else if (running) {
				changeMovementSpeed(speed);
				running = false;
			}
		} else if (running) {
			changeMovementSpeed(speed);
			running = false;
		} else if (stamina < maxStamina)
			stamina = min(stamina + Time.deltaTime*staminaPerSecond, maxStamina);

		if (health <= 0) {
			if (restart) {
				if (Input.GetKeyDown(KeyCode.Return))
					restartGame();
			}
			else {
				health = 0;
				GameMaster.setGameOver(true);
			}
		}
	}

	public static int GetWeapon () {
		return actualWeapon;
	}

	public static int GetStamina() {
		return (int)stamina;
	}

	private void changeMovementSpeed(float speed) {
		gameObject.GetComponent<CharacterMotor>().movement.maxForwardSpeed =
			gameObject.GetComponent<CharacterMotor>().movement.maxSidewaysSpeed =
				gameObject.GetComponent<CharacterMotor>().movement.maxBackwardsSpeed
					= speed;
	}

	public static float max(float n1, float n2) {
		return n1 >= n2 ? n1 : n2;
	}

	public static float min(float n1, float n2) {
		return n1 <= n2 ? n1 : n2;
	}

	void ApplyDamage(float dmg) {
		if (health > 0)
			health -= dmg;
	}

	void OnGUI() {
		GUI.Label(new Rect(15, 40, Screen.width, Screen.height),
		          "Vida: " + (health > 0 ? health.ToString("#.##") : "0") + " / " + maxHealth, healthStatus);
		if (GameMaster.isGameOver()) {
			GUIStyle style = new GUIStyle(GUI.skin.textField);
			style.alignment = TextAnchor.MiddleCenter;
			style.fontStyle = FontStyle.Bold;
			style.normal.textColor = Color.red;
			GUI.TextField (new Rect (0, 0, Screen.width, Screen.height), "GAME OVER\nPulsa ENTER para reiniciar. ", style);
			restart = true;
		}
	}

	void restartGame() {
		restart = false;
		health = maxHealth;
		gameObject.transform.position = new Vector3(0, 1, 0);
		GameObject.FindWithTag("GameController").SendMessage("restartGame");
	}
}

