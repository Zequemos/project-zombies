using UnityEngine;
using System.Collections;

public class Explosion : MonoBehaviour {

	public float damage = 20f;
	//public int damageTime = 20;
	public float range = 10f;
	
	void Start() {
		StartCoroutine(clear());
	}

	/*void Update() {
		if (damageTime > 0)
			--damageTime;
	}*/

	void OnTriggerEnter(Collider hit) {
        //if (damageTime > 0) {
			float dist = Vector3.Distance(transform.position, hit.transform.position);
			if (hit.gameObject.CompareTag("Enemy"))
			hit.gameObject.SendMessage("ApplyDamage", new KnockbackParameters{ dmg = damage - damage*(dist/range), knockbackPower = 500, knockbackDirection = -Vector3.forward });
			else if (hit.gameObject.CompareTag("Player"))
			hit.gameObject.SendMessage("ApplyDamage", damage - damage*(dist/range));
		//}
    }

	IEnumerator clear() {
		yield return new WaitForSeconds(4);
		Destroy(gameObject);
	}
}
