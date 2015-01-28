using UnityEngine;
using System.Collections;

public class FollowTarget : MonoBehaviour {

	public Transform target;
	public float smoothTime = 0.3f;
	private Transform thisTransform;
	private Vector3 transformAux;
	private Vector2 velocity;
	public float droneHeight;
	private Vector3 dronePos;

	void Start () {
		thisTransform = transform;
		transformAux.y = droneHeight;
	}
	
	// Update is called once per frame
	void Update () {
		if(velocity.x > 5f) velocity.x = 5f;
		transformAux.x = Mathf.SmoothDamp( thisTransform.position.x, target.position.x, ref velocity.x, smoothTime);
		transformAux.z = Mathf.SmoothDamp( thisTransform.position.z, target.position.z, ref velocity.y, smoothTime);
		thisTransform.position = transformAux;
	}	

}
