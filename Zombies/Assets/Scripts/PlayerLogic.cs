using UnityEngine;
using System.Collections;

public class PlayerLogic : MonoBehaviour
{
	public float maxHealth = 100;
	private bool restart = false;
	private float health;
	private GUIStyle healthStatus = new GUIStyle();
	private static int actualWeapon = 0;

	// Use this for initialization
	void Start () {
		health = maxHealth;
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
		}
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

