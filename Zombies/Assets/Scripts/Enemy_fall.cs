using UnityEngine;
using System.Collections;

public class Enemy_fall : MonoBehaviour {

	public float destroyDelay = 180;

	// Use this for initialization
	void Start () {
		Physics.IgnoreCollision(gameObject.collider,
		                        GameObject.FindGameObjectWithTag("Player").collider);
		gameObject.rigidbody.AddForce(new Vector3 (0, 0, -10));
		StartCoroutine(clear());
	}
	
	// Update is called once per frame
	void Update () {
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Enemy"))
			Physics.IgnoreCollision(gameObject.collider, obj.collider);
	}

	IEnumerator clear() {
		yield return new WaitForSeconds(destroyDelay);
		Destroy(gameObject);
	}
}
