using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public GameObject pistol_bullet, machinegun_bullet;
	public GameObject grenade;
	public GameObject lightPistola, lightAmetralladora;
	public GameObject m9, ak47, cuchillo, granadaMano;
	public static int pistolAmmunition = 14;
	public static int machinegunAmmunition = 60;
	public static int grenadeAmmunition = 5;
	public static int cargadoresPistol = 10;
	public static int cargadoresMachinegun = 10;
	private int knifeAttacks = 0;
	public Transform muzzlePistola, muzzleAmetralladora;
	private static bool needReloadPistol, needReloadMachinegun;
	private static int currentPistolAmmo, currentMachinegunAmmo, currentGrenadeAmmo,
	currCargadoresPistol, currCargadoresMachinegun;
	private GUIStyle redStyle = new GUIStyle();
	private RaycastHit hit;
	private bool knifeAttack1, knifeAttack2, launching;
	public float knifeDamage = 5f, knifeRange = 2f, timeCuchillo1 = 0.5f, timeCuchillo2 = 1.5f;
	private int timenext = 2; //para la ametralladora
	private AudioSource[] audioAK47, audioPistol, audioCuchillo;

	void Start() {
		currentPistolAmmo = pistolAmmunition;
		currentMachinegunAmmo = machinegunAmmunition;
		currentGrenadeAmmo = grenadeAmmunition;
		redStyle.alignment = TextAnchor.MiddleCenter;
		redStyle.normal.textColor = Color.red;
		knifeAttack1 = knifeAttack2 = launching = false;
		audioAK47 = ak47.GetComponents<AudioSource>();
		audioPistol = m9.GetComponents<AudioSource>();
		audioCuchillo = cuchillo.GetComponents<AudioSource>();
		currCargadoresPistol = cargadoresPistol;
		currCargadoresMachinegun = cargadoresMachinegun;
	}

	void Update () {
		if (!PlayerLogic.isRunning()) {
			if (PlayerLogic.GetWeapon() == 0) KnifeWeapon();
			else if (PlayerLogic.GetWeapon() == 1) PistolWeapon();
			else if (PlayerLogic.GetWeapon() == 2) MachinegunWeapon();
			else if (PlayerLogic.GetWeapon() == 3) GrenadeWeapon();
		}
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
			if (Input.GetKeyDown(KeyCode.R) && !PlayerLogic.isApuntando())
				reload(1); //waitForReload("Pistola");
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
			if (Input.GetKeyDown(KeyCode.R) && !PlayerLogic.isApuntando())
				reload(2); //waitForReload("Ametralladora");
		}
	}

	void GrenadeWeapon() {
		if (currentGrenadeAmmo > 0 && !GameMaster.isGameOver() && !launching && Input.GetButtonDown("Fire1"))	
			StartCoroutine(launchGrenade());
	}

	void OnGUI() {
		if (PlayerLogic.GetWeapon () == 1) {
			GUI.Label(new Rect (1200, 10, Screen.width, Screen.height), "Munición: " + currentPistolAmmo);
			GUI.Label(new Rect (1200, 30, Screen.width, Screen.height), "Cargadores: " + currCargadoresPistol);
			if (needReloadPistol)
				GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
				          "SIN MUNICIÓN! (R)", redStyle);
		} else if (PlayerLogic.GetWeapon () == 2) {
			GUI.Label(new Rect (1200, 10, Screen.width, Screen.height), "Munición: " + currentMachinegunAmmo);
			GUI.Label(new Rect (1200, 30, Screen.width, Screen.height), "Cargadores: " + currCargadoresMachinegun);
			if (needReloadMachinegun)
				GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
				          "SIN MUNICIÓN! (R)", redStyle);
		} else if (PlayerLogic.GetWeapon () == 3)
			GUI.Label (new Rect (1200, 10, Screen.width, Screen.height), "Munición: " + currentGrenadeAmmo);
	}

	/* TODO IEnumerator waitForReload(int weapon) {
	 *  if (weapon == 1 && currCargadoresPistol > 0) {
	 * 		yield return new WaitForSeconds(PlayerLogic.getAnimationWeapon().GetClip("Reload_Pistola").length);
	 *  	reload(1);
	 *  }
	 *  else if (weapon == 2 && currCargadoresMachinegun > 0) {
	 * 		yield return new WaitForSeconds(PlayerLogic.getAnimationWeapon().GetClip("Reload_M").length);
	 * 		reload(2);
	 * 	}
	}*/

	public static void reload(int weapon) {
		if (weapon == 1 && currentPistolAmmo != pistolAmmunition && currCargadoresPistol > 0) {
			currentPistolAmmo = pistolAmmunition;
			needReloadPistol = false;
			--currCargadoresPistol;
		} else if (weapon == 2 && currentMachinegunAmmo != machinegunAmmunition && currCargadoresMachinegun > 0) {
			currentMachinegunAmmo = machinegunAmmunition;
			needReloadMachinegun = false;
			--currCargadoresMachinegun;
		}
	}

	public static void reset() {
		currentPistolAmmo = pistolAmmunition;
		needReloadPistol = false;
		currentMachinegunAmmo = machinegunAmmunition;
		needReloadMachinegun = false;
		currCargadoresPistol = cargadoresPistol;
		currCargadoresMachinegun = cargadoresMachinegun;
		currentGrenadeAmmo = grenadeAmmunition;
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
				hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = knifeDamage, knockbackPower = 200, knockbackDirection = ray.direction, boss = false });
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
				hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = 2*knifeDamage, knockbackPower = 250, knockbackDirection = ray.direction, boss = false });
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
		if (PlayerLogic.GetWeapon() == 3) return currentGrenadeAmmo == 0;
		return false;
	}
}
