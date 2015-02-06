using UnityEngine;
using System.Collections;

public class PlayerLogic : MonoBehaviour
{
	public float maxHealth = 100f, maxStamina = 100f,
	staminaPerSecond = 10f, staminaCostPerSecond = 20f, runningSpeedMult = 1.5f;
	public static int rondaPistola = 2, rondaMachinegun = 7, rondaGranada = 12;
	public GameObject m9, ak47, cuchillo, granadaMano;
	private GameObject granada;
	private static Animation animationAmetralladora, animationPistola, animationCuchillo, animationGranada;
	private static bool apuntando, running;
	private bool restart, animApuntando;
	private float sensitivityX, sensitivityYCam, sensitivityXCam;
	private GUIStyle healthStatus;
	private static int actualWeapon = 0;
	private static float stamina, speed, health;
	public Texture redTexture; //esto es para que se dibuje sangre cuando te dañan
	private bool drawTexture = false;
	public float drawTime;
	
	void Start() {
		restart = running = apuntando = animApuntando = false;
		health = maxHealth;
		stamina = maxStamina;
		animationPistola = m9.animation;
		animationAmetralladora = ak47.animation;
		animationCuchillo = cuchillo.animation;
		animationGranada = granadaMano.animation;
		granada = granadaMano.transform.Find("FragFBX").gameObject;
		speed = gameObject.GetComponent<CharacterMotor>().movement.maxForwardSpeed;
		healthStatus = new GUIStyle();
		healthStatus.normal.textColor = Color.green;
		healthStatus.fontStyle = FontStyle.Bold;
		sensitivityX = GetComponent<MouseLook>().sensitivityX;
		sensitivityXCam = Camera.main.GetComponent<MouseLook>().sensitivityX;
		sensitivityYCam = Camera.main.GetComponent<MouseLook>().sensitivityY;
	}
	
	void Update() {
		if (GameMaster.isPausedGame()) return;
		else if (health <= 0) {
			if (restart) {
				if (Input.GetKeyDown(KeyCode.Return))
					restartGame();
			}
			else {
				health = 0;
				audio.Play();
				GameMaster.setGameOver(true);
			}
		}
		else {
			if (!apuntando) {
				if (!running) {
					if (Input.GetKey(KeyCode.Alpha1)) //Cuchillo
						changeWeaponTo(0);
					else if (Input.GetKey(KeyCode.Alpha2) && isUnlocked(1, GameMaster.getRound())) //Pistola
						changeWeaponTo(1);
					else if (Input.GetKey(KeyCode.Alpha3) && isUnlocked(2, GameMaster.getRound())) //Ametralladora
						changeWeaponTo(2);
					else if (Input.GetKey(KeyCode.Alpha4) && isUnlocked(3, GameMaster.getRound())) //Granada
						changeWeaponTo(3);
					else if (Input.GetKey(KeyCode.Alpha5) && GameMaster.canPurchasePackage()) //Paquete de Munición
						StartCoroutine(purchasePackage(1));
					else if (Input.GetKey(KeyCode.Alpha6) && GameMaster.canPurchasePackage()) //Paquete de Vida
						StartCoroutine(purchasePackage(2));
					else if (Input.GetKey(KeyCode.LeftShift) && !Shoot.isAnimReloading()) { //Correr
						if (stamina > 0f) {
							changeMovementSpeed(runningSpeedMult);
							StartCoroutine(correr());
							running = true;
						}
					}
					else if (Input.GetMouseButton(1) && !Shoot.isAnimReloading()) { //Apuntar
						if (actualWeapon == 1 || actualWeapon == 2)
							StartCoroutine(apuntar());
					}
					if (stamina < maxStamina)
						stamina = min(stamina + Time.deltaTime*staminaPerSecond, maxStamina);
				} else if (stamina <= 0f || !Input.GetKey(KeyCode.LeftShift)) {
					running = false;
					changeMovementSpeed(1);
					if (actualWeapon == 1 || actualWeapon == 2) {
						Animation animationWeapon = actualWeapon == 2 ? animationAmetralladora : animationPistola;
						animationWeapon.Play(actualWeapon == 1 ? "Apuntar_Cadera" : "Apuntar_CaderaM");
					}
				} else if (stamina > 0f)
					stamina = max(stamina - Time.deltaTime*staminaCostPerSecond, 0f);
				if (actualWeapon == 3) {
					if (Shoot.isReload())
						granada.SetActive(false);
					else if (!granada.activeSelf)
						granada.SetActive(true);
				}
			}
			else if (!Input.GetMouseButton(1)) {
				animApuntando = apuntando = false;
				changeMovementSpeed(1);
				sensitivity(false);
				Animation animationWeapon = actualWeapon == 2 ? animationAmetralladora : animationPistola;
				animationWeapon.Stop();
				animationWeapon.Play(actualWeapon == 1 ? "Apuntar_Cadera" : "Apuntar_CaderaM");
			}
			if (stamina < maxStamina)
				stamina = min(stamina + Time.deltaTime*staminaPerSecond, maxStamina);
		}
	}
	
	public static int GetWeapon() {
		return actualWeapon;
	}
	
	public static int GetStamina() {
		return (int)stamina;
	}
	
	IEnumerator apuntar() {
		animApuntando = true;
		Animation animationWeapon = actualWeapon == 2 ? animationAmetralladora : animationPistola;
		string clipName = actualWeapon == 1 ? "Apuntar" : "ApuntarM";
		animationWeapon.Play(clipName);
		yield return new WaitForSeconds(animationWeapon.GetClip(clipName).length);
		if (animApuntando) {
			apuntando = true;
			animationWeapon.Play(actualWeapon == 1 ? "Apuntando" : "ApuntandoM");
			changeMovementSpeed(0.5f);
			sensitivity(true);
		}
		else {
			animationWeapon.Stop();
			animationWeapon.Play(actualWeapon == 1 ? "Apuntar_Cadera" : "Apuntar_CaderaM");
			changeMovementSpeed(1);
			sensitivity(false);
		}
	}
	
	public void changeMovementSpeed(float multiplier) {
		gameObject.GetComponent<CharacterMotor>().movement.maxForwardSpeed =
			gameObject.GetComponent<CharacterMotor>().movement.maxSidewaysSpeed =
				gameObject.GetComponent<CharacterMotor>().movement.maxBackwardsSpeed
					= speed*multiplier;
	}
	
	public static float max(float n1, float n2) {
		return n1 >= n2 ? n1 : n2;
	}
	
	public static float min(float n1, float n2) {
		return n1 <= n2 ? n1 : n2;
	}
	
	void ApplyDamage(float dmg) {
		if (health > 0) {
			drawTexture = true;
			health -= dmg;
		}
	}
	
	void OnGUI() {
		/*GUI.Label(new Rect(15, 40, Screen.width, Screen.height),
		          "Vida: " + (health > 0 ? health.ToString("#.##") : "0") + " / " + maxHealth, healthStatus);*/
		if (drawTexture) {
			StartCoroutine(drawingTexture());
			GUI.DrawTexture (new Rect (0, 0, Screen.width, Screen.height), redTexture);
		}
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
		Shoot.reset();
		GameObject.FindWithTag("GameController").SendMessage("restartGame");
	}
	
	IEnumerator drawingTexture(){
		yield return new WaitForSeconds(drawTime);
		drawTexture = false;
	}
	
	public static Animation getAnimationWeapon() {
		switch (actualWeapon) {
		case 0:
			return animationCuchillo;
		case 2:
			return animationAmetralladora;
		case 3:
			return animationGranada;
		default:
			return animationPistola;
		}
	}

	public void checkUpgrades(int ronda) {
		//TODO Añadir mensajes de desbloqueo
		bool upgraded = false;
		if (ronda == rondaPistola) {
			changeWeaponTo(1);
			upgraded = true;
		}
		else if (ronda == rondaMachinegun) {
			changeWeaponTo(2);
			upgraded = true;
		}
		else if (ronda == rondaGranada) {
			changeWeaponTo(3);
			upgraded = true;
		}
		if (upgraded)
			GameMaster.audioGM[5].Play();
	}

	public bool isUnlocked(int arma, int ronda) {
		switch (arma) {
		case 1:
			return ronda >= rondaPistola;
		case 2:
			return ronda >= rondaMachinegun;
		case 3:
			return ronda >= rondaGranada;
		default:
			return true;
		}
	}

	private void changeWeaponTo(int weapon) {
		switch (weapon) {
		case 0:
			//TODO animacion cambio de arma
			m9.SetActive(false);
			ak47.SetActive(false);
			granadaMano.SetActive(false);
			cuchillo.SetActive(true);
			break;
		case 1:
			//TODO animacion cambio de arma
			ak47.SetActive(false);
			cuchillo.SetActive(false);
			granadaMano.SetActive(false);
			m9.SetActive(true);
			break;
		case 2:
			//TODO animacion cambio de arma
			cuchillo.SetActive(false);
			m9.SetActive(false);
			granadaMano.SetActive(false);
			ak47.SetActive(true);
			break;
		case 3:
			//TODO animacion cambio de arma
			cuchillo.SetActive(false);
			m9.SetActive(false);
			ak47.SetActive(false);
			granadaMano.SetActive(true);
			break;
		}
		actualWeapon = weapon;
	}

	private IEnumerator correr() {
		Animation animationWeapon = actualWeapon == 2 ? animationAmetralladora : animationPistola;
		animationWeapon.Play(actualWeapon == 2 ? "CorrerM" : "CorrerPistola");
		yield return new WaitForSeconds(animationWeapon.GetClip(actualWeapon == 2 ? "CorrerM" : "CorrerPistola").length);
		if (running)
			animationWeapon.Play(actualWeapon == 2 ? "CorriendoM" : "CorriendoPistola");
	}

	private IEnumerator purchasePackage(int package) {
		GameMaster.packagePurchased();
		//TODO Añadir mensajes de audio
		yield return new WaitForSeconds(20);
		if (!GameMaster.isGameOver()) {
			if (package == 1) //Municion
				Shoot.reset();
			else //Vida
				health = maxHealth;
		}
	}

	private void sensitivity(bool apuntando) {
		if (apuntando) {
			GetComponent<MouseLook>().sensitivityX = 7;
			Camera.main.GetComponent<MouseLook>().sensitivityX = 5;
			Camera.main.GetComponent<MouseLook>().sensitivityY = 5;
		} else {
			GetComponent<MouseLook>().sensitivityX = sensitivityX;
			Camera.main.GetComponent<MouseLook>().sensitivityY = sensitivityYCam;
			Camera.main.GetComponent<MouseLook>().sensitivityX = sensitivityXCam;
		}
	}

	public void stopPlayer(bool pause) {
		if (pause)
			gameObject.GetComponent<CharacterMotor>().enabled =
				gameObject.GetComponent<MouseLook>().enabled =
				Camera.main.GetComponent<MouseLook>().enabled = false;
		else //Resume
			gameObject.GetComponent<CharacterMotor>().enabled =
				gameObject.GetComponent<MouseLook>().enabled =
				Camera.main.GetComponent<MouseLook>().enabled = true;
	}

	public static float getHealth(){
		return health;
	}

	public static float getStamina(){
		return stamina;
	}

	public static Animation getAnimationPistol() {
		return animationPistola;
	}
	
	public static Animation getAnimationMachinegun() {
		return animationAmetralladora;
	}
	
	public static Animation getAnimationCuchillo() {
		return animationCuchillo;
	}
	
	public static Animation getAnimationGranada() {
		return animationGranada;
	}
	
	public static bool isApuntando() {
		return apuntando;
	}

	public static bool isRunning() {
		return running;
	}
}