using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public GameObject pistol_bullet, machinegun_bullet;
	public GameObject grenade;
	public GameObject lightPistola, lightAmetralladora;
	public static int ammunition = 100;
	public Transform muzzlePistola, muzzleAmetralladora;
	private static bool needReload;
	private static int currentAmmo;
	private GUIStyle redStyle = new GUIStyle();
	private RaycastHit hit;
	public float knifeDamage = 5f;
	public float knifeRange = 2f;
	private int timenext = 2; //para la ametralladora. Old: 7

	void Start() {
		currentAmmo = ammunition;
		redStyle.alignment = TextAnchor.MiddleCenter;
		redStyle.normal.textColor = Color.red;
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

	void PistolWeapon() {
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetButtonDown ("Fire1")) {
					Instantiate(pistol_bullet, muzzlePistola.position, transform.rotation);
					StartCoroutine(fogonazoPistola());
					if (PlayerLogic.isApuntando())
						PlayerLogic.getAnimationWeapon().Play("Retroceso_Apuntando");
					else
						PlayerLogic.getAnimationWeapon().Play("Retroceso_Cadera");
					--currentAmmo;
				}
			}
			if (Input.GetKeyDown(KeyCode.R))
				reload(); //waitForReload("Pistola");
		}
	}

	void MachinegunWeapon() {
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetMouseButton(0)) {
					if (timenext == 0) {
						Instantiate(machinegun_bullet, muzzleAmetralladora.position, transform.rotation);
						StartCoroutine(fogonazoAmetralladora());
						if (PlayerLogic.isApuntando())
							PlayerLogic.getAnimationWeapon().Play("Retroceso_Apuntando");
						else
							PlayerLogic.getAnimationWeapon().Play("Retroceso_Cadera");
						--currentAmmo;
						timenext = 2;
					} else --timenext;
				}
			}
			if (Input.GetKeyDown(KeyCode.R))
				reload(); //waitForReload("Ametralladora");
		}
	}

	void GrenadeWeapon() {
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetButtonDown ("Fire1")) {
					Instantiate(grenade, muzzlePistola.position, transform.rotation);								
					--currentAmmo;
				}
			}			
		}
	}

	void OnGUI() {
		if (needReload)
			GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
			           "SIN MUNICIÓN! (R)", redStyle);
		/*else
			GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20), "+", redStyle);*/
		GUI.Label(new Rect(1200, 10, Screen.width, Screen.height), "Munición: " + currentAmmo);
	}

	/* TODO IEnumerator waitForReload(string weapon) {
		yield return new WaitForSeconds(PlayerLogic.getAnimationWeapon().GetClip("Reload_" + weapon).length);
		reload();
	}*/

	public static void reload() {
		currentAmmo = ammunition; //TODO cambiar segun el arma
		needReload = false; //TODO cambiar segun el arma
	}

	IEnumerator fogonazoPistola() {
		lightPistola.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		lightPistola.SetActive(false);
	}

	IEnumerator fogonazoAmetralladora() {
		lightAmetralladora.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		lightAmetralladora.SetActive(false);
	}
}
