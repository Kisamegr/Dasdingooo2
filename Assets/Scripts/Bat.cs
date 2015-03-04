using UnityEngine;
using System.Collections;

public class Bat : MonoBehaviour {


    public Vector2 speed;


	// Use this for initialization
	void Start () {
        rigidbody2D.velocity = speed; 
	}
	
	// Update is called once per frame
	void Update () {
        //Elegkse gia destroy
	}
}
