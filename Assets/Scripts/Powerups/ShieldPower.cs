using UnityEngine;
using System.Collections;

public class ShieldPower : Powerup {
	

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Ground") {
			player.GetComponent<Player>().cancelHook();
			player.rigidbody2D.velocity = new Vector2(player.rigidbody2D.velocity.x, -player.rigidbody2D.velocity.y*1.2f);
			Destroy(gameObject);
		}

		if(other.tag == "Enemy") {
			other.GetComponent<Enemy>().Death();
			Destroy(gameObject);
		}
	}

	protected override void Power ()
	{
		//Do nothing...
	}
	
	protected override void PowerEnded ()
	{
		//Do nothing...
	}
	public override void Refresh ()
	{
		//Do nothing...
	}
}
