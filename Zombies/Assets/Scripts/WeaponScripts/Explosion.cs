using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {
	
	public float damage = 20f;
	public float range = 10f;
	public bool isBossExplosion = false;
	
	void Start() {
		StartCoroutine(clear());
	}

	void OnTriggerEnter(Collider hit) {
        //if (damageTime > 0) {
			float dist = Vector3.Distance(transform.position, hit.transform.position);
			if (dist < range) {
				if (hit.gameObject.CompareTag("Player")) {
					hit.gameObject.SendMessage("ApplyDamage", damage - damage*(dist/range));
					if (isBossExplosion)
						StartCoroutine(bossSlowDown(hit.gameObject));
				}
				else if (hit.gameObject.CompareTag("Enemy"))
					hit.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = damage - damage*(dist/range), knockbackPower = 500, knockbackDirection = -Vector3.forward, boss = isBossExplosion });
			}
		//}
    }

	IEnumerator bossSlowDown(GameObject player) {
		player.SendMessage("changeMovementSpeed", 0.5f);
		yield return new WaitForSeconds(3);
		player.SendMessage("changeMovementSpeed", 1);
	}

	IEnumerator clear() {
		yield return new WaitForSeconds(4);
		Destroy(gameObject);
	}
}
