using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public GameObject bullet;
	public int ammunition = 100;
	public Transform muzzle;
	private bool needReload;
	private int currentAmmo;
	private GUIStyle crossStyle = new GUIStyle();

	void Start() {
		currentAmmo = ammunition;
		crossStyle.alignment = TextAnchor.MiddleCenter;
		crossStyle.normal.textColor = Color.red;
	}

	void Update () {
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetButtonDown ("Fire1")) {
					GameObject shot = Instantiate(bullet, muzzle.position, transform.rotation) as GameObject;
					shot.SendMessage("setCamera", camera);
					--currentAmmo;
				}
			}
			else if (Input.GetKeyDown(KeyCode.R))
				reload();
		}
	}

	void OnGUI() {
		if (needReload)
			GUI.Label (new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
			           "SIN MUNICIÓN! (R)", crossStyle);
		else
			GUI.Label (new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20), "+", crossStyle);
		GUI.Label(new Rect(1200, 10, Screen.width, Screen.height), "Munición: " + currentAmmo);
	}

	void reload() {
		//TODO Animation reloading
		currentAmmo = ammunition;
		needReload = false;
	}
}
