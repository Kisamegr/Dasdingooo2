using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour {


    public Vector2 speed;


	// Use this for initialization
	void Start () {
        GetComponent<Rigidbody2D>().velocity = speed; 
	}

}
