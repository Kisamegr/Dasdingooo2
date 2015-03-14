using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour {

    public int minPoints;

    public int maxPoints;


	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Player")
        {
            Player player = other.GetComponent<Player>();

            float points = minPoints + (maxPoints - minPoints) * player.rigidbody2D.velocity.x/player.maxSpeed;
            Destroy(gameObject);
        }

    }
}
