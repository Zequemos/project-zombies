using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public GameObject bullet;
	public int ammunition = 100;
	public Transform muzzle;
	private bool needReload;
	private int currentAmmo;
	private GUIStyle crossStyle = new GUIStyle();
	private RaycastHit hit;
	public float knifeDamage = 5f;
	public float knifeRange = 2f;

	void Start() {
		currentAmmo = ammunition;
	}

	void Update () {
		if(PlayerLogic.GetWeapon() == 0) KnifeWeapon();
		else if(PlayerLogic.GetWeapon() == 1) PistolWeapon();
		else if(PlayerLogic.GetWeapon() == 2) MachinegunWeapon();
		else if(PlayerLogic.GetWeapon() == 3) GrenadeWeapon();
	}

	void KnifeWeapon() {
		if (!GameMaster.isGameOver ()) {
			if (Input.GetButtonDown ("Fire1")) {
				Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0));
				if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
					if (hit.collider.tag == "Enemy" && hit.distance <= knifeRange)
						hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = knifeDamage, knockbackPower = 200, knockbackDirection = ray.direction });
				}
			}
		}
	}

	void PistolWeapon(){
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

	void MachinegunWeapon() {
		//TODO
	}

	void GrenadeWeapon() {
		//TODO
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
