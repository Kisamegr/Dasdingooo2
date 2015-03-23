using UnityEngine;
using System.Collections;

public class GodCleaner : MonoBehaviour {

	void OnCollisionEnter2D(Collision2D coll) {
		if(coll.gameObject.transform.parent == null)
			Destroy(coll.gameObject);
		else
			Destroy(coll.gameObject.transform.parent.gameObject);
	}

	void OnTriggerEnter2D(Collider2D other)
	{
		if(other.gameObject.transform.parent == null)
			Destroy(other.gameObject);
		else
			Destroy(other.gameObject.transform.parent.gameObject);	
	
	}
}
