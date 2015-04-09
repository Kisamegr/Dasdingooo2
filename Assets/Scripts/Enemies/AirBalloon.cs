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
			rigidbody2D.AddForce(force.normalized * forcePower);
	}
	
	public override void Death() {
		rigidbody2D.gravityScale = 1;
		rigidbody2D.AddTorque(50);
		collider2D.enabled = false;
		transform.FindChild("sting").collider2D.enabled = false;

		StartCoroutine(Kill(8));
	}
}
