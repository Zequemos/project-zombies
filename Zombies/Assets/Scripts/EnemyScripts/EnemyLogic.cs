using UnityEngine;
using System.Collections;

public class EnemyLogic : MonoBehaviour {

	public float distanceAttack = 4;
	public float attackDelay = 1;
	public float life = 8;
	public float damage = 5;
	public Transform Enemy_dead, hombro, zombieWithAnimation;
	private bool isAttacking;
	private NavMeshAgent navmesh;
	private GameObject target;
	private Animation animCaminar, animAtaque;
	
	void Start() {
		isAttacking = false;
		animCaminar = zombieWithAnimation.animation;
		animAtaque = hombro.animation;
		navmesh = GetComponent<NavMeshAgent>();
		target = GameObject.FindWithTag("Player");
		animCaminar.Play("Caminar");
	}

	void Update () {
		if (!GameMaster.isGameOver() && !isAttacking) {
			navmesh.SetDestination(target.transform.position);
			if (Vector3.Distance(transform.position, target.transform.position) <= distanceAttack)
				StartCoroutine(strike());
		}
	}

	IEnumerator strike() {
		isAttacking = true;
		animCaminar.Play("Stop");
		animAtaque.Play("Ataque_Zombie");
		yield return new WaitForSeconds(attackDelay);
		if (Vector3.Distance(transform.position, target.transform.position) <= distanceAttack)
			target.transform.gameObject.SendMessage("ApplyDamage", damage);
		else {
			animCaminar.Stop("Stop");
			animCaminar.Play("Caminar");
		}
		isAttacking = false;
	}

	void ApplyDamage(KnockbackParameters args) {
		life -= args.dmg;
		if (life <= 0) {
			Destroy(gameObject);
			Transform dead = (Transform) GameObject.Instantiate(Enemy_dead, transform.position, transform.rotation);
			Vector3 v3Force = args.knockbackPower*args.knockbackDirection;
			dead.rigidbody.AddForce(v3Force);
			GameMaster.zombieKilled();
		}
	}

	void StopGame() {
		animCaminar.Stop();
		animAtaque.Stop();
		navmesh.Stop();
	}
}
