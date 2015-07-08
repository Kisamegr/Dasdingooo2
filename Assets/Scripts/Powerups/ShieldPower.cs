using UnityEngine;
using System.Collections;

public class ShieldPower : Powerup {

    public AudioClip shieldBreakSound;
	public GameObject brokenShield;

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Ground") {
			player.GetComponent<Player>().cancelHook();
			player.GetComponent<Rigidbody2D>().velocity = new Vector2(player.GetComponent<Rigidbody2D>().velocity.x, -player.GetComponent<Rigidbody2D>().velocity.y*1.2f);
            if (GameObject.Find("_GAME").GetComponent<Game>().save.isSoundOn())
            {
                AudioSource.PlayClipAtPoint(shieldBreakSound, transform.position, 0.7f);
            }

			Instantiate(brokenShield,transform.position,Quaternion.identity);

			Destroy(gameObject);
		}

		if(other.tag == "Enemy") {
			other.GetComponent<Enemy>().Death();
            if (GameObject.Find("_GAME").GetComponent<Game>().save.isSoundOn())
            {
                AudioSource.PlayClipAtPoint(shieldBreakSound, transform.position, 0.7f);
            }

			Instantiate(brokenShield,transform.position,Quaternion.identity);

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
