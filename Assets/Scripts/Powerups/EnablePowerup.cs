using UnityEngine;
using System.Collections;

public class EnablePowerup : MonoBehaviour {

	public GameObject powerupObject;

	private Game gameScript;

	void Start() {
		gameScript = GameObject.Find("_GAME").GetComponent <Game>();
	}


	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player") {
			Transform player = other.transform;

			Powerup old = player.GetComponentInChildren<Powerup>();
			Powerup p = powerupObject.GetComponent<Powerup>();

            if (p.pickupSound != null)
            {
                if (gameScript.save.isSoundOn())
                	AudioSource.PlayClipAtPoint(p.pickupSound,player.transform.position,0.3f);
            }

			gameScript.save.totalPowerups++;

			if(old==null || old.id != p.id) {

				GameObject power = (GameObject) Instantiate(powerupObject);

				power.transform.parent = player;
                Vector2 diff = player.position - player.collider2D.bounds.center;
                power.transform.position = player.collider2D.bounds.center;//new Vector3(player.position.x,player.position.y - 0.5f,0);
				power.GetComponent<Powerup>().player = player.gameObject;
				Destroy(gameObject);

			}
			else
				old.Refresh();



			
		}
	}
}
