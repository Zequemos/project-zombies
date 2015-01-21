using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {
	
	public GameObject bullet;
	public GameObject grenade;
	public GameObject light;
	public int ammunition = 100;
	public Transform muzzle;
	private bool needReload;
	private int currentAmmo;
	private GUIStyle crossStyle = new GUIStyle();
	private RaycastHit hit;
	public float knifeDamage = 5f;
	public float knifeRange = 2f;
	private int timenext = 7; //para la ametralladora

	void Start() {
		currentAmmo = ammunition;
		/***/crossStyle.alignment = TextAnchor.MiddleCenter;
		/***/crossStyle.normal.textColor = Color.red;
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
					StartCoroutine(fogonazo());	
					--currentAmmo;
				}
			}
			else if (Input.GetKeyDown(KeyCode.R))
				reload();
		}
	}

	void MachinegunWeapon() {
		if(timenext != 0)
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetButton ("Fire1")) {
					if(timenext == 0){
						Instantiate(bullet, muzzle.position, transform.rotation);
						StartCoroutine(fogonazo());						
						--currentAmmo;
						timenext = 7;
					} else --timenext;
					
				}
			}
			else if (Input.GetKeyDown(KeyCode.R))
				reload();
		}
	}

	void GrenadeWeapon() {
		if (currentAmmo <= 0)
			needReload = true;
		if (!GameMaster.isGameOver()) {
			if (!needReload) {
				if (Input.GetButtonDown ("Fire1")) {
					Instantiate(grenade, muzzle.position, transform.rotation);									
					--currentAmmo;
				}
			}			
		}
	}

	void OnGUI() {
		if (needReload)
			GUI.Label (new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20),
			           "SIN MUNICIÓN! (R)", crossStyle);
		/***/else
		/***/	GUI.Label (new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20), "+", crossStyle);
		GUI.Label(new Rect(1200, 10, Screen.width, Screen.height), "Munición: " + currentAmmo);
	}

	void reload() {
		//TODO Animation reloading
		currentAmmo = ammunition;
		needReload = false;
	}
	IEnumerator fogonazo() {
		light.SetActive (true);
		yield return new WaitForSeconds(0.1f);
		light.SetActive (false);
	}
}
