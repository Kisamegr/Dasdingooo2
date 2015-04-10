using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    public int minPoints;

    public int maxPoints;

	public float followSpeed = 200;

    public AudioClip pickupSound;

    public Player player = null;

    public Game game = null;

	private bool follow;

	private Vector3 followForce;

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

		follow = false;
	}
	
	// Update is called once per frame
	void Update () {
	
		if(follow) {
			followForce = player.transform.position - transform.position;
			rigidbody2D.AddForce(followForce.normalized * followSpeed);
		}
	}


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {

            float points = minPoints + (maxPoints - minPoints) * player.rigidbody2D.velocity.x/player.maxSpeed;

            game.coinsCollected++;
			game.score.AddCoin();
			game.score.AddCoinScore(points);

			if(game.save.isSoundOn())
            	AudioSource.PlayClipAtPoint(pickupSound, transform.position);

            Destroy(gameObject);
        }

    }

	public void FollowPlayer() {
		follow = true;
		rigidbody2D.isKinematic = false;
	}
}
