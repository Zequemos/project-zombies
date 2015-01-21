using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public int damage = 20;
	public int damageTime = 20;
	
	void Start() {
		StartCoroutine(clear());
	}

	void Update() {
		if (damageTime > 0)
			--damageTime;
	}

	void OnTriggerEnter(Collider hit) {
        if (damageTime > 0) {
			if (hit.gameObject.CompareTag("Enemy"))
				hit.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = damage, knockbackPower = 300, knockbackDirection = Vector3.forward });
			else if (hit.gameObject.CompareTag("Player"))
				hit.gameObject.SendMessage("ApplyDamage", damage);
		}
    }

	IEnumerator clear() {
		yield return new WaitForSeconds(4);
		Destroy(gameObject);
	}
}
