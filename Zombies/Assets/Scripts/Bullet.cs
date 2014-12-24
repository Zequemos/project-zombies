using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	private RaycastHit hit;
	private Camera cam;
	public int speed = 100;
	public float damage = 1;
	public float range = 100;
	
	// Use this for initialization
	void Start() {
		StartCoroutine(clear());
		Ray ray = cam.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
		rigidbody.AddRelativeForce(new Vector3(-2, 0.5f, speed), ForceMode.VelocityChange);
		if (Physics.Raycast (ray.origin, ray.direction, out hit)) {
			if (hit.collider.tag == "Enemy" && hit.distance <= range) {
				hit.transform.gameObject.SendMessage("ApplyDamage", damage);
				StartCoroutine(fastClear());
			}
		}
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Dead"))
			Physics.IgnoreCollision(gameObject.collider, obj.collider);
	}
	
	void OnCollisionEnter() {
		Destroy(gameObject);
	}

	IEnumerator clear() {
		yield return new WaitForSeconds(range/speed);
		Destroy(gameObject);
	}

	IEnumerator fastClear() {
		yield return new WaitForSeconds(0.1f);
		Destroy(gameObject);
	}

	void setCamera(Camera cameraToSet) {
		cam = cameraToSet;
	}
}