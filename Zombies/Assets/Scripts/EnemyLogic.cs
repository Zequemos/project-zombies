using UnityEngine;
using System.Collections;

public class EnemyLogic : MonoBehaviour {

	public float distanceAttack = 4;
	public float attackDelay = 1;
	public float life = 8;
	public float damage = 5;
	public Transform Enemy_dead;
	private bool isAttacking;
	private NavMeshAgent navmesh;
	private GameObject target;

	// Use this for initialization
	void Start () {
		isAttacking = false;
		navmesh = GetComponent<NavMeshAgent>();
		target = GameObject.FindWithTag("Player");
	}

	void Update () {
		if (!GameMaster.isGameOver() && !isAttacking) {
			navmesh.SetDestination(target.transform.position);
			if (Vector3.Distance(transform.position, target.transform.position) <= distanceAttack) {
				//TODO Start animation
				StartCoroutine(strike());
			}
		}
	}

	IEnumerator strike() {
		isAttacking = true;
		yield return new WaitForSeconds(attackDelay);
		if (Vector3.Distance(transform.position, target.transform.position) <= distanceAttack)
			target.transform.gameObject.SendMessage("ApplyDamage", damage);
		isAttacking = false;
	}

	void ApplyDamage(float dmg) {
		life -= dmg;
		if (life <= 0) {
			Destroy(gameObject);
			Instantiate(Enemy_dead, transform.position, transform.rotation);
			GameMaster.zombieKilled();
		}
	}
}
