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
	}

	void Update () {
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetButtonDown ("Fire1")) {
					Instantiate(bullet, muzzle.position, transform.rotation);
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
		GUI.Label(new Rect(1200, 10, Screen.width, Screen.height), "Munición: " + currentAmmo);
	}

	void reload() {
		//TODO Animation reloading
		currentAmmo = ammunition;
		needReload = false;
	}
}
