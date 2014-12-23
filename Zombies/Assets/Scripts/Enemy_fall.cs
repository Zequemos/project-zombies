using UnityEngine;
using System.Collections;

public class Enemy_fall : MonoBehaviour {

	// Use this for initialization
	void Start () {

		gameObject.rigidbody.AddForce (new Vector3 (0, 0, -10));
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
