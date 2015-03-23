using UnityEngine;
using System.Collections;

public class PlayerPart : MonoBehaviour {

	// Use this for initialization
	void Start () {
		rigidbody2D.AddTorque(Random.Range(-30,30) * 5);
	}

}
