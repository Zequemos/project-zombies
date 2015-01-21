using UnityEngine;
using System.Collections;

public class GoTo : MonoBehaviour {

	private GameObject objective;
	private NavMeshAgent navmesh;

	// Use this for initialization
	void Start () {
		navmesh = GetComponent<NavMeshAgent> ();
		objective = GameObject.FindWithTag ("Player");
	
	}
	
	// Update is called once per frame
	void Update () {
		navmesh.SetDestination (objective.transform.position);
	}

}

