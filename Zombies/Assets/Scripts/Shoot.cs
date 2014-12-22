using UnityEngine;
using System.Collections;

public class Shoot : MonoBehaviour {

	private RaycastHit hit;
	
	public float damage = 1;
	void Start(){
	//Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
	}
	void Update () {		
		if (Input.GetButtonDown ("Fire1")) {	
		Ray ray = camera.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));		
			if (Physics.Raycast (ray.origin, ray.direction, out hit)) {							
				if (hit.collider.tag == "Enemy") {										
					hit.transform.gameObject.SendMessage ("ApplyDamage", damage);								
				}						
			}				
		}
	}

	void OnGUI(){
		GUI.Label (new Rect(Screen.width/2 - 10, Screen.height/2 - 10, 20, 20), "+");
	}
}
