using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class GameMaster : MonoBehaviour {

	public int zombiesToSpawn = 20;
	private int zombiesSpawned;
	public GameObject zombie;
	public GameObject pivot1;
	public GameObject pivot2;
	public GameObject pivot3;
	public GameObject pivot4;
	List<GameObject> pivots;



	// Use this for initialization
	void Start () {
		pivots = new List<GameObject> ();
		pivots.Add (pivot1);
		pivots.Add (pivot2);
		pivots.Add (pivot3);
		pivots.Add (pivot4);

	}
	
	// Update is called once per frame
	void Update () {

		int i = 0;
		while (zombiesToSpawn > 0){
			if(i == pivots.Count) i = 0;
			Instantiate(zombie,pivots[i].transform.position,Quaternion.identity);
			--zombiesToSpawn;
			++zombiesSpawned;
			++i;

		}

	
	}
	void OnGUI() {
		GUI.Label(new Rect(10, 10, 200, 20), "Zombies restantes: " + zombiesSpawned);

	}

}
