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
	private static bool needReloadPistol, needReloadMachinegun, reloading;
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
		knifeAttack1 = knifeAttack2 = launching = reloading = false;
		audioAK47 = ak47.GetComponents<AudioSource>();
		audioPistol = m9.GetComponents<AudioSource>();
		audioCuchillo = cuchillo.GetComponents<AudioSource>();
		currCargadoresPistol = cargadoresPistol;
		currCargadoresMachinegun = cargadoresMachinegun;
		knifeRange++;
	}

	void Update() {
		if (!PlayerLogic.isRunning() && !GameMaster.isPausedGame()) {
			switch (PlayerLogic.GetWeapon()) {
				case 0: KnifeWeapon(); break;
				case 1: PistolWeapon(); break;
				case 2: MachinegunWeapon(); break;
				case 3: GrenadeWeapon(); break;
			}
		}
	}

	private void KnifeWeapon() {
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

	private void PistolWeapon() {
		if (currentPistolAmmo <= 0)
			needReloadPistol = true;
		if (!GameMaster.isGameOver() && !reloading) {
			if (!needReloadPistol) {
				if (Input.GetButtonDown("Fire1")) {
					audioPistol[0].Play(); //Disparo
					Instantiate(pistol_bullet, muzzlePistola.position, transform.rotation);
					StartCoroutine(fogonazoPistola());
					--currentPistolAmmo;
				}
			}
			else if (Input.GetButtonDown("Fire1"))
				audioPistol[2].Play(); //Gatillo vacio
			if (Input.GetKeyDown(KeyCode.R) && !PlayerLogic.isApuntando())
				StartCoroutine(waitForReload(1));
		}
	}

	private void MachinegunWeapon() {
		if (currentMachinegunAmmo <= 0)
			needReloadMachinegun = true;
		if (!GameMaster.isGameOver() && !reloading) {
			if (!needReloadMachinegun) {
				if (Input.GetMouseButton(0)) {
					if (timenext == 0) {
						audioAK47[0].Play(); //Disparo
						audioAK47[2].PlayDelayed(0.131f); //Casquillo
						audioAK47[2].volume = 0.2f;
						Instantiate(machinegun_bullet, muzzleAmetralladora.position, transform.rotation);
						StartCoroutine(fogonazoAmetralladora());
						--currentMachinegunAmmo;
						timenext = 2;
					} else --timenext;
				}
				else if (Input.GetKeyDown(KeyCode.R) && !PlayerLogic.isApuntando())
					StartCoroutine(waitForReload(2));
			}
			else if (Input.GetMouseButton(0))
				audioAK47[3].Play(); //Gatillo vacio
			else if (Input.GetKeyDown(KeyCode.R) && !PlayerLogic.isApuntando())
				StartCoroutine(waitForReload(2));
		}
	}

	private void GrenadeWeapon() {
		if (currentGrenadeAmmo > 0 && !GameMaster.isGameOver() && !launching && Input.GetButtonDown("Fire1"))	
			StartCoroutine(launchGrenade());
	}

	void OnGUI() {
		if (PlayerLogic.GetWeapon () == 1) {
			GUI.Label(new Rect (1200, 10, Screen.width, Screen.height), "Ammo: " + currentPistolAmmo);
			GUI.Label(new Rect (1200, 30, Screen.width, Screen.height), "Magazines: " + currCargadoresPistol);
			if (needReloadPistol)
				GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
				          "RELOAD! (R)", redStyle);
		} else if (PlayerLogic.GetWeapon () == 2) {
			GUI.Label(new Rect (1200, 10, Screen.width, Screen.height), "Ammo: " + currentMachinegunAmmo);
			GUI.Label(new Rect (1200, 30, Screen.width, Screen.height), "Magazines: " + currCargadoresMachinegun);
			if (needReloadMachinegun)
				GUI.Label(new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
				          "RELOAD! (R)", redStyle);
		} else if (PlayerLogic.GetWeapon () == 3)
			GUI.Label (new Rect (1200, 10, Screen.width, Screen.height), "Ammo: " + currentGrenadeAmmo);
	}

	IEnumerator waitForReload(int weapon) {
		Animation anim = PlayerLogic.getAnimationWeapon();
		if (weapon == 1 && currCargadoresPistol > 0 && currentPistolAmmo != pistolAmmunition) {
			reloading = true;
			anim.Play("Reload_Pistola");
			audioPistol[1].Play(); //Reload
			yield return new WaitForSeconds(anim.GetClip("Reload_Pistola").length);
			reload(1);
			reloading = false;
		}
		else if (weapon == 2 && currCargadoresMachinegun > 0 && currentMachinegunAmmo != machinegunAmmunition) {
			reloading = true;
			anim.Play("Reload_Machinegun");
			audioAK47[1].Play(); //Reload
			yield return new WaitForSeconds(anim.GetClip("Reload_Machinegun").length);
			reload(2);
			reloading = false;
		}
	}

	public static void reload(int weapon) {
		if (weapon == 1 && currCargadoresPistol > 0) {
			currentPistolAmmo = pistolAmmunition;
			needReloadPistol = false;
			--currCargadoresPistol;
		} else if (weapon == 2 && currCargadoresMachinegun > 0) {
			currentMachinegunAmmo = machinegunAmmunition;
			needReloadMachinegun = false;
			--currCargadoresMachinegun;
		}
	}

	public static bool isAnimReloading() {
		return reloading;
	}

	public static int getStartAmmo(int weapon){
		if (weapon == 1) return pistolAmmunition;
		else if (weapon == 2) return machinegunAmmunition;
		else if (weapon == 3) return grenadeAmmunition;
		else return 0;
	}
	public static int getCurrentAmmo(int weapon){
		if (weapon == 1) return currentPistolAmmo;
		else if (weapon == 2) return currentMachinegunAmmo;
		else if (weapon == 3) return currentGrenadeAmmo;
		else return 0;
	}
	public static int getCurrentCargadores(int weapon){
		if (weapon == 1) return currCargadoresPistol;
		else if (weapon == 2) return currCargadoresMachinegun;
		else return 0;
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
		if (PlayerLogic.isApuntando())
			PlayerLogic.getAnimationPistol().Play("Apuntando");
		else
			PlayerLogic.getAnimationPistol().Play("Apuntar_Cadera");
		lightPistola.SetActive(false);
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
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, -1));
		if (Physics.Raycast(ray, out hit, knifeRange)) {
			bool headshot = hit.collider.tag == "Headshot";
			if (headshot || hit.collider.tag == "Enemy")
				hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = headshot ? 2*knifeDamage : knifeDamage, knockbackPower = 200, knockbackDirection = ray.direction, boss = false });
		}
		PlayerLogic.getAnimationCuchillo().Play("ReposoCuchillo");
		knifeAttack1 = false;
	}

	IEnumerator cuchillo2() {
		knifeAttack2 = true;
		++knifeAttacks;
		PlayerLogic.getAnimationCuchillo().Play("AtaqueCuchillo2");
		yield return new WaitForSeconds(timeCuchillo2);
		Ray ray = Camera.main.ScreenPointToRay (new Vector3 (Screen.width * 0.5f, Screen.height * 0.5f, -1));
		if (knifeAttack2 && knifeAttacks <= 1) {
			if (Physics.Raycast(ray, out hit, knifeRange)) {
				bool headshot = hit.collider.tag == "Headshot";
				if (headshot || hit.collider.tag == "Enemy")
					hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = headshot ? 4*knifeDamage : 2*knifeDamage, knockbackPower = 250, knockbackDirection = ray.direction, boss = false });
			}
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
