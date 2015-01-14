using UnityEngine;
using System.Collections;

public class EnemyLogic : MonoBehaviour {

	public float distanceAttack = 4;
	public float attackDelay = 2;
	public Transform Enemy_dead;
	private float life, damage;
	private bool isAttacking;
	private NavMeshAgent navmesh;
	private GameObject target;

	// Use this for initialization
	void Start () {
		isAttacking = false;
		life = GameMaster.getZombiesLife();
		damage = GameMaster.getZombiesDamage();
		navmesh = GetComponent<NavMeshAgent>();
		target = GameObject.FindWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (!GameMaster.isGameOver()) {
			if (!isAttacking)
				navmesh.SetDestination(target.transform.position);
			if (Vector3.Distance(transform.position, target.transform.position) <= distanceAttack) {
				if (!isAttacking) {
					//TODO Start animation
					StartCoroutine(strike());
				}
			}
			else if (isAttacking) {
				//TODO Abort animation
				isAttacking = false;
			}
		}
	}

	IEnumerator strike() {
		isAttacking = true;
		yield return new WaitForSeconds(attackDelay);
		if (isAttacking && Vector3.Distance(transform.position, target.transform.position) <= distanceAttack)
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
