using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {


	public float forcePower;
	private Vector2 halfSize;
	public Vector3 originalPosition;

	private Vector2 pos;
	private float diff;
	private float angle;

	void Start () {
		originalPosition = transform.position;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		pos = transform.position;
		diff = originalPosition.y - pos.y;


		rigidbody2D.AddForceAtPosition(-Physics.gravity * rigidbody2D.mass * diff * forcePower , new Vector2(pos.x - halfSize.x, pos.y));

	}
}
