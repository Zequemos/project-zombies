﻿using UnityEngine;
using System.Collections;

public class Bullet : MonoBehaviour {

	private RaycastHit hit;
	public int speed = 100;
	public float damage = 1;
	public float range = 100;
	public GameObject bloodPrefab;

	void Start() {
		StartCoroutine(clear());
		Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, 0));
		rigidbody.AddRelativeForce(new Vector3(-2, 0.5f, speed), ForceMode.VelocityChange);
		if (Physics.Raycast(ray, out hit, range)) {
			bool headshot = hit.collider.tag == "Headshot";
			if (headshot || hit.collider.tag == "Enemy") {
				hit.transform.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = headshot ? 2*damage : damage, knockbackPower = 300, knockbackDirection = ray.direction, boss = false });
				Vector3 posBlood = hit.transform.position;
				posBlood.Set(posBlood.x, posBlood.y + 1, posBlood.z);
				Instantiate(bloodPrefab, posBlood, Quaternion.identity);
				Destroy(gameObject);
			}
		}
		foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Dead"))
			Physics.IgnoreCollision(gameObject.collider, obj.collider);
	}
	
	void OnCollisionEnter() {
		Destroy(gameObject);
	}

	IEnumerator clear() {
		yield return new WaitForSeconds(range/speed);
		Destroy(gameObject);
	}
}