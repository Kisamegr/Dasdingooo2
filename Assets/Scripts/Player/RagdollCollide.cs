using UnityEngine;
using System.Collections;

public class RagdollCollide : MonoBehaviour {

	public GameObject[] ragdollParts;

	// Use this for initialization
	void Start () {
	
	}
	
	
	void OnCollisionEnter2D(Collision2D coll) {
		if(coll.gameObject.tag == "Menu") {

			foreach(GameObject part in ragdollParts) {
				part.GetComponent<HingeJoint2D>().enabled = false;
				//part.rigidbody2D.velocity *= -1;
			}

		}
	}
}
