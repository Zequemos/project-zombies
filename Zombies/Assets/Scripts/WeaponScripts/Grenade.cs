using UnityEngine;
using System.Collections;

public class Grenade : MonoBehaviour {
	
	public float destroyDelay = 1;
	public GameObject explosion;

	// Use this for initialization
	void Start() {
		gameObject.rigidbody.AddRelativeForce(new Vector3 (0,200,700));
		StartCoroutine(clear());
	}
	
	IEnumerator clear() {
		yield return new WaitForSeconds(destroyDelay);
		Instantiate(explosion, transform.position, Quaternion.identity);
		Destroy(gameObject);
	}
}
