using UnityEngine;
using System.Collections;

public class Platform : MonoBehaviour {


	public float forcePower;
	private Vector2 halfSize;
	public Vector3 originalPosition;

	private Vector2 pos;
	private float diff;
	private float angle;
    private Collider2D playerCollider;
    private Collider2D platformCollider;

	void Start () {
		originalPosition = transform.position;
        playerCollider = GameObject.Find("Player").collider2D;
        platformCollider = collider2D;
	}

    void Update()
    {
        //Physics2D.IgnoreCollision(platformCollider, playerCollider,platformCollider.bounds.center.y + transform.localScale.y*platformCollider.bounds.extents.y - 0.01f < playerCollider.bounds.min.y);
    }
	
	// Update is called once per frame
	void FixedUpdate () {
	
		pos = transform.position;
		diff = originalPosition.y - pos.y;


		rigidbody2D.AddForceAtPosition(-Physics.gravity * rigidbody2D.mass * diff * forcePower , new Vector2(pos.x - halfSize.x, pos.y));

	}
}
