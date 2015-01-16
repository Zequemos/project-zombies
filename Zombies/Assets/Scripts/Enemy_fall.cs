using UnityEngine;
using System.Collections;

public class Enemy_fall : MonoBehaviour {

	public int destroyDelay = 180;
	public int rigidBodyDelay = 10;

	// Use this for initialization
	void Start () {
		ignoreCollisions();
		Vector3 v3Force = -1000 * transform.forward;
		gameObject.rigidbody.AddForce(v3Force);
		StartCoroutine(clear());
		StartCoroutine(noRigidBody());
	}

	IEnumerator clear() {
		yield return new WaitForSeconds(destroyDelay);
		Destroy(gameObject);
	}

	IEnumerator noRigidBody() {
		yield return new WaitForSeconds(rigidBodyDelay);
		Destroy(rigidbody);
	}

	void ignoreCollisions() {
		Physics.IgnoreCollision(gameObject.collider,
		                        GameObject.FindGameObjectWithTag("Player").collider);
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
			Physics.IgnoreCollision(gameObject.collider, obj.collider);
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Dead")) {
			if (obj != this.gameObject)
				Physics.IgnoreCollision(gameObject.collider, obj.collider);
		}
	}
}
