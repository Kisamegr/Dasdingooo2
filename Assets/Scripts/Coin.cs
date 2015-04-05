using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    public int minPoints;

    public int maxPoints;

    public AudioClip pickupSound;

    public Player player = null;

    public Game game = null;


	// Use this for initialization
	void Start () {
        if (game == null)
        {
            game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        }
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            float points = minPoints + (maxPoints - minPoints) * player.rigidbody2D.velocity.x/player.maxSpeed;

            game.coinsCollected++;
                

            AudioSource.PlayClipAtPoint(pickupSound, transform.position);
            Destroy(gameObject);
        }

    }
}
