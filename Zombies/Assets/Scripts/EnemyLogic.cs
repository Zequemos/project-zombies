using UnityEngine;
using System.Collections;

public class EnemyLogic : MonoBehaviour {

	public int life = 1;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void ApplyDamage(){
		-- life;
		if (life <= 0) Destroy (gameObject);
	}
}
