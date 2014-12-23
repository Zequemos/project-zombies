using UnityEngine;
using System.Collections;

public class EnemyLogic : MonoBehaviour {

	public int life = 1;
	public Transform Enemy_dead;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	void ApplyDamage(){
		-- life;
		if (life <= 0){

			--GameMaster.zombiesSpawned;
			Instantiate(Enemy_dead, transform.position, Quaternion.identity);
			Destroy (gameObject);

		} 
	}
}
