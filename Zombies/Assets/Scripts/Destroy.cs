using UnityEngine;
using System.Collections;

public class Destroy : MonoBehaviour {
	
	public float destroyDelay;

	// Use this for initialization
	void Start () {
		StartCoroutine(clear());	
	}
	
	IEnumerator clear() {
			yield return new WaitForSeconds(destroyDelay);
			Destroy(gameObject);
		}
}
