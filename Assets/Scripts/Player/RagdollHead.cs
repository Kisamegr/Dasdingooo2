using UnityEngine;
using System.Collections;

public class RagdollHead : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		float angle = Vector3.Angle(Vector3.up,transform.up);

	}
}
