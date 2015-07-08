using UnityEngine;
using System.Collections;

public class AirBalloon : Enemy {

	public float forcePower;

	private Transform player;
	private Vector3 force;

	// Use this for initialization
	void Start () {
		base.Start();

		player = GameObject.Find("Player").transform;
	}

	void FixedUpdate() {
		force = player.position - transform.position;

		if(force.x < 0)
			GetComponent<Rigidbody2D>().AddForce(force.normalized * forcePower);
	}
	
	public override void Death() {
		GetComponent<Rigidbody2D>().gravityScale = 1;
		GetComponent<Rigidbody2D>().AddTorque(50);
		GetComponent<Collider2D>().enabled = false;
		transform.FindChild("sting").GetComponent<Collider2D>().enabled = false;

		StartCoroutine(Kill(8));
	}
}
