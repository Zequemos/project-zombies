using UnityEngine;
using System.Collections;

public class GoTo : MonoBehaviour {

	public Transform objective;
	private NavMeshAgent navmesh;

	// Use this for initialization
	void Start () {
		navmesh = GetComponent<NavMeshAgent> ();
	
	}
	
	// Update is called once per frame
	void Update () {
		navmesh.SetDestination (objective.position);
	}

	void OnGUI() {
		GUI.Label(new Rect(10, 10, 100, 20), "Hello World!");
	}
}

