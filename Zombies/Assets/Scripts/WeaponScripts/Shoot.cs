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
	private bool knifeAttack1, knifeAttack2, launching;
	public float knifeDamage = 5f, knifeRange = 2f, timeCuchillo1 = 0.5f, timeCuchillo2 = 1.5f;
	private int timenext = 3; //para la ametralladora

	void Start() {
		currentAmmo = ammunition;
		redStyle.alignment = TextAnchor.MiddleCenter;
		redStyle.normal.textColor = Color.red;
		knifeAttack1 = knifeAttack2 = launching = false;
	}

	void Update () {
		if(PlayerLogic.GetWeapon() == 0) KnifeWeapon();
		else if(PlayerLogic.GetWeapon() == 1) PistolWeapon();
		else if(PlayerLogic.GetWeapon() == 2) MachinegunWeapon();
		else if(PlayerLogic.GetWeapon() == 3) GrenadeWeapon();
	}

	void KnifeWeapon() {
		if (!GameMaster.isGameOver()) {
			if (!(knifeAttack1 || knifeAttack2)) {
				if (Input.GetButtonDown("Fire1"))
					StartCoroutine(cuchillo1());
				if (Input.GetMouseButton(1))
					StartCoroutine(cuchillo2());
			}
			else if (knifeAttack2 && !Input.GetMouseButton(1)) {
				PlayerLogic.getAnimationCuchillo().Play("ReposoCuchillo");
				knifeAttack2 = false;
			}
		}
	}

	void PistolWeapon() {
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetButtonDown("Fire1")) {
					Instantiate(pistol_bullet, muzzlePistola.position, transform.rotation);
					StartCoroutine(fogonazoPistola());
					if (PlayerLogic.isApuntando())
						PlayerLogic.getAnimationPistol().Play("Retroceso_Apuntando");
					else
						PlayerLogic.getAnimationPistol().Play("Retroceso_Cadera");
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
							PlayerLogic.getAnimationMachinegun().Play("Retroceso_ApuntandoM");
						else
							PlayerLogic.getAnimationMachinegun().Play("Retroceso_CaderaM");
						--currentAmmo;
						timenext = 3;
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
				if (!launching && Input.GetButtonDown("Fire1"))
					StartCoroutine(launchGrenade());
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
		if (PlayerLogic.isApuntando())
			PlayerLogic.getAnimationPistol().Play("Apuntando");
		else
			PlayerLogic.getAnimationPistol().Play("Apuntar_Cadera");
	}

	IEnumerator fogonazoAmetralladora() {
		lightAmetralladora.SetActive(true);
		yield return new WaitForSeconds(0.1f);
		if (PlayerLogic.isApuntando())
			PlayerLogic.getAnimationMachinegun().Play("ApuntandoM");
		else
			PlayerLogic.getAnimationMachinegun().Play("Apuntar_CaderaM");
		lightAmetralladora.SetActive(false);
	}

	IEnumerator cuchillo1() {
		knifeAttack1 = true;
		PlayerLogic.getAnimationCuchillo().Play("AtaqueCuchillo1");
		yield return new WaitForSeconds(timeCuchillo1);
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0));
		if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
			if (hit.collider.tag == "Enemy" && hit.distance <= knifeRange)
				hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = knifeDamage, knockbackPower = 200, knockbackDirection = ray.direction });
		}
		PlayerLogic.getAnimationCuchillo().Play("ReposoCuchillo");
		knifeAttack1 = false;
	}

	IEnumerator cuchillo2() {
		knifeAttack2 = true;
		PlayerLogic.getAnimationCuchillo().Play("AtaqueCuchillo2");
		yield return new WaitForSeconds(timeCuchillo2);
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0));
		if (knifeAttack2 && Physics.Raycast(ray.origin, ray.direction, out hit)) {
			if (hit.collider.tag == "Enemy" && hit.distance <= knifeRange)
				hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = 2*knifeDamage, knockbackPower = 250, knockbackDirection = ray.direction });
		}
		PlayerLogic.getAnimationCuchillo().Play("ReposoCuchillo");
		knifeAttack2 = false;
	}

	IEnumerator launchGrenade() {
		launching = true;
		--currentAmmo;
		PlayerLogic.getAnimationGranada().Play("LanzarGranada");
		yield return new WaitForSeconds(0.7f);
		Instantiate(grenade, muzzlePistola.position, transform.rotation);
		PlayerLogic.getAnimationGranada().Play("GranadaEnMano");
		launching = false;
	}
	
	public static bool isReload() {
		return needReload;
	}
}
