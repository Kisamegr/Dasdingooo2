using UnityEngine;
using System.Collections;

public class EnablePowerup : MonoBehaviour {

	public GameObject powerupObject;


	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.tag == "Player") {
			Transform player = other.transform;
			GameObject power = (GameObject) Instantiate(powerupObject);
			power.transform.parent = player;
			power.transform.position = new Vector3(player.position.x,player.position.y - 0.5f,0);
			power.GetComponent<Powerup>().player = player.gameObject;
			Destroy(gameObject);
		}
	}
}
