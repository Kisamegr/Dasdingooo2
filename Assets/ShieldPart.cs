using UnityEngine;
using System.Collections;

public class ShieldPart : MonoBehaviour {

	public float torqueForce;

	private float startTime;

	// Use this for initialization
	void Start () {
		GetComponent<Rigidbody2D>().AddTorque(Random.Range (50,50 +torqueForce));

		startTime = Time.time;

		GetComponent<Rigidbody2D>().velocity = new Vector2(GameObject.Find("Player").GetComponent<Rigidbody2D>().velocity.x * Random.Range(0.5f,1.25f),0);
	}

	void Update() {
		if(Time.time - startTime > 10)
			Destroy(gameObject);
	}
	

}
