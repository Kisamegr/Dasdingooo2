using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour {


    public Vector2 speed;
	public Rigidbody2D rigidbody2d;

	// Use this for initialization
	void Start () {
		rigidbody2d.velocity = speed; 


	}


}
