using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public GameObject pistol_bullet, machinegun_bullet;
	public GameObject grenade;
	public GameObject lightPistola, lightAmetralladora;
	public GameObject m9, ak47, cuchillo, granadaMano;
	public static int pistolAmmunition = 7;
	public static int machinegunAmmunition = 30;
	public static int grenadeAmmunition = 15;
	private int knifeAttacks = 0;
	public Transform muzzlePistola, muzzleAmetralladora;
	private static bool needReloadPistol, needReloadMachinegun;
	private static int currentPistolAmmo, currentMachinegunAmmo, currentGrenadeAmmo;
	private GUIStyle redStyle = new GUIStyle();
	private RaycastHit hit;
	private bool knifeAttack1, knifeAttack2, launching;
	public float knifeDamage = 5f, knifeRange = 2f, timeCuchillo1 = 0.5f, timeCuchillo2 = 1.5f;
	private int timenext = 3; //para la ametralladora
	private AudioSource[] audioAK47, audioPistol, audioGrenade, audioCuchillo;

	void Start() {
		currentPistolAmmo = pistolAmmunition;
		currentMachinegunAmmo = machinegunAmmunition;
		currentGrenadeAmmo = grenadeAmmunition;
		redStyle.alignment = TextAnchor.MiddleCenter;
		redStyle.normal.textColor = Color.red;
		knifeAttack1 = knifeAttack2 = launching = false;
		audioAK47 = ak47.GetComponents<AudioSource>();
		audioPistol = m9.GetComponents<AudioSource>();
		audioGrenade = granadaMano.GetComponents<AudioSource>();
		audioCuchillo = cuchillo.GetComponents<AudioSource>();
	}

	void Update () {
		if(PlayerLogic.GetWeapon() == 0) KnifeWeapon();
		else if(PlayerLogic.GetWeapon() == 1) PistolWeapon();
		else if(PlayerLogic.GetWeapon() == 2) MachinegunWeapon();
		else if(PlayerLogic.GetWeapon() == 3) GrenadeWeapon();
	}

	void KnifeWeapon() {
		if (!GameMaster.isGameOver()) {
			if (!(knifeAttack1 || knifeAttack2 || knifeAttacks >= 1)) {
				if (Input.GetMouseButtonDown(0))
					StartCoroutine(cuchillo1());
				if (Input.GetMouseButton(1))
					StartCoroutine(cuchillo2());
			}
			else if (knifeAttacks >= 1 && !Input.GetMouseButton(1)) {
				knifeAttack2 = false;
				PlayerLogic.getAnimationCuchillo().Play("ReposoCuchillo");
			}
		}
	}

	void PistolWeapon() {
		if (currentPistolAmmo <= 0)
			needReloadPistol = true;
		if (!GameMaster.isGameOver()) {
			if (!needReloadPistol) {
				if (Input.GetButtonDown("Fire1")) {
					audioPistol[0].Play(); //Disparo
					Instantiate(pistol_bullet, muzzlePistola.position, transform.rotation);
					StartCoroutine(fogonazoPistola());
					if (PlayerLogic.isApuntando())
						PlayerLogic.getAnimationPistol().Play("Retroceso_Apuntando");
					else
						PlayerLogic.getAnimationPistol().Play("Retroceso_Cadera");
					--currentPistolAmmo;
				}
			}
			else if (Input.GetButtonDown("Fire1"))
				audioPistol[2].Play(); //Gatillo vacio
			if (Input.GetKeyDown(KeyCode.R))
				reload(); //waitForReload("Pistola");
		}
	}

	void MachinegunWeapon() {
		if (currentMachinegunAmmo <= 0)
			needReloadMachinegun = true;
		if (!GameMaster.isGameOver()) {
			if (!needReloadMachinegun) {
				if (Input.GetMouseButton(0)) {
					if (timenext == 0) {
						audioAK47[0].Play(); //Disparo
						audioAK47[2].PlayDelayed(0.131f); //Casquillo
						audioAK47[2].volume = 0.2f;
						Instantiate(machinegun_bullet, muzzleAmetralladora.position, transform.rotation);
						StartCoroutine(fogonazoAmetralladora());
						if (PlayerLogic.isApuntando())
							PlayerLogic.getAnimationMachinegun().Play("Retroceso_ApuntandoM");
						else
							PlayerLogic.getAnimationMachinegun().Play("Retroceso_CaderaM");
						--currentMachinegunAmmo;
						timenext = 3;
					} else --timenext;
				}
			}
			else if (Input.GetMouseButton(0))
				audioAK47[3].Play(); //Gatillo vacio
			if (Input.GetKeyDown(KeyCode.R))
				reload(); //waitForReload("Ametralladora");
		}
	}

	void GrenadeWeapon() {
		if (currentGrenadeAmmo > 0 && !GameMaster.isGameOver() && !launching && Input.GetButtonDown("Fire1"))	
			StartCoroutine(launchGrenade());
	}

	void OnGUI() {
		if (PlayerLogic.GetWeapon () == 1) {
			GUI.Label (new Rect (1200, 10, Screen.width, Screen.height), "Munición: " + currentPistolAmmo);
			if (needReloadPistol)
				GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
				          "SIN MUNICIÓN! (R)", redStyle);
		}else if (PlayerLogic.GetWeapon () == 2) {
			GUI.Label (new Rect (1200, 10, Screen.width, Screen.height), "Munición: " + currentMachinegunAmmo);
			if (needReloadMachinegun)
				GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
				          "SIN MUNICIÓN! (R)", redStyle);
		}else if (PlayerLogic.GetWeapon () == 3) {
			GUI.Label (new Rect (1200, 10, Screen.width, Screen.height), "Munición: " + currentGrenadeAmmo);
	}
	}

	/* TODO IEnumerator waitForReload(string weapon) {
		yield return new WaitForSeconds(PlayerLogic.getAnimationWeapon().GetClip("Reload_" + weapon).length);
		reload();
	}*/

	public static void reload() {
		if (PlayerLogic.GetWeapon() == 1) {
			currentPistolAmmo = pistolAmmunition;
			needReloadPistol = false;
		} else if(PlayerLogic.GetWeapon() == 2) {
			currentMachinegunAmmo = machinegunAmmunition;
			needReloadMachinegun = false;
		}
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
		if (Random.value >= 0.5f)
			audioCuchillo[0].Play(); //Grito de aliento
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
		++knifeAttacks;
		PlayerLogic.getAnimationCuchillo().Play("AtaqueCuchillo2");
		yield return new WaitForSeconds(timeCuchillo2);
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, 0));
		if (knifeAttack2 && knifeAttacks <= 1) {
			if (Physics.Raycast(ray.origin, ray.direction, out hit)
			    && hit.collider.tag == "Enemy" && hit.distance <= knifeRange)
				hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = 2*knifeDamage, knockbackPower = 250, knockbackDirection = ray.direction });
			audioCuchillo[0].Play(); //Grito de aliento
			PlayerLogic.getAnimationCuchillo().Play("ReposoCuchillo");
		}
		--knifeAttacks;
		knifeAttack2 = false;
	}

	IEnumerator launchGrenade() {
		launching = true;
		--currentGrenadeAmmo;
		PlayerLogic.getAnimationGranada().Play("LanzarGranada");
		yield return new WaitForSeconds(0.7f);
		Instantiate(grenade, muzzlePistola.position, transform.rotation);
		yield return new WaitForSeconds(0.7f);
		PlayerLogic.getAnimationGranada().Play("GranadaEnMano");
		launching = false;
	}
	
	public static bool isReload() {
		if (PlayerLogic.GetWeapon() == 1) return needReloadPistol;
		if (PlayerLogic.GetWeapon() == 2) return needReloadMachinegun;
		else return false;
	}
}
