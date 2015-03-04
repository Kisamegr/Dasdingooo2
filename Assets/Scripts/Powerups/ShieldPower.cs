using UnityEngine;
using System.Collections;

public class ShieldPower : Powerup {
	

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Ground") {
			player.rigidbody2D.velocity = new Vector2(player.rigidbody2D.velocity.x, -player.rigidbody2D.velocity.y);
			Destroy(gameObject);
		}

		if(other.tag == "Enemy") {
			Destroy(other.gameObject);
			Destroy(gameObject);
		}
	}

	protected override void Power ()
	{
	}
	
	protected override void PowerEnded ()
	{
	}
}
