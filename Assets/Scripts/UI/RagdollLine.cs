using UnityEngine;
using System.Collections;

public class RagdollLine : MonoBehaviour {

	public float distMin;
	public float distMax;
	public Transform lineStart;
	private Vector3 lineEnd;

	private LineRenderer line;
	private DistanceJoint2D joint;

	// Use this for initialization
	void Start () {
		line = (LineRenderer)gameObject.GetComponent<LineRenderer>();
		joint = (DistanceJoint2D)gameObject.GetComponent<DistanceJoint2D>();
		//lineStart = transform.position;

		joint.distance = Random.Range(distMin,distMax);

		lineEnd = gameObject.GetComponent<DistanceJoint2D>().connectedBody.transform.position;

	}
	
	// Update is called once per frame
	void Update () {
		//lineStart = new Vector3(transform.position.x ,transform.position.y  + yOffset,-1);
		//lineStart = new Vector3(joint..x, joint.anchor.y, -1);

		line.SetPosition(0,  lineStart.position);
		line.SetPosition(1,  lineEnd);

	}
}
