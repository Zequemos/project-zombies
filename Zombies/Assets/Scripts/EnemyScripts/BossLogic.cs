using UnityEngine;
using System.Collections;

public class BossLogic : MonoBehaviour {

	public float distanceAttack = 4;
	public float attackDelay = 1;
	public float life = 20;
	public Transform Enemy_dead, bossWithAnimation;
	public GameObject explosion;
	private bool isAttacking = false, grito = false;
	private NavMeshAgent navmesh;
	private GameObject target;

	void Start() {
		navmesh = GetComponent<NavMeshAgent>();
		animation.Play("Correr");
		target = GameObject.FindWithTag("Player");
	}
	
	void Update () {
		if (!GameMaster.isGameOver() && !isAttacking) {
			navmesh.SetDestination(target.transform.position);
			float dist = Vector3.Distance(transform.position, target.transform.position);
			if (!grito && dist <= (distanceAttack + 20)) {
				audio.Play(); //Grito
				grito = true;
				navmesh.speed += 3;
			}
			else if (dist <= distanceAttack) {
				animation.Play("BossParado");
				StartCoroutine(explode());
			}
		}
	}
	
	IEnumerator explode() {
		isAttacking = true;
		yield return new WaitForSeconds(attackDelay);
		if (isAttacking) {
			Instantiate(explosion, transform.position, transform.rotation);
			explosion.audio.Play();
			Destroy(gameObject);
			GameMaster.zombieKilled(true);
		}
	}
	
	void ApplyDamage(KnockbackParameters args) {
		life -= args.dmg;
		if (life <= 0) {
			isAttacking = false;
			Destroy(gameObject);
			Transform dead = (Transform) GameObject.Instantiate(Enemy_dead, transform.position, transform.rotation);
			Vector3 v3Force = args.knockbackPower*args.knockbackDirection;
			dead.rigidbody.AddForce(v3Force);
			GameMaster.zombieKilled(false);
		}
	}

	void StopGame() {
		animation.Stop();
		navmesh.Stop();
	}
}
