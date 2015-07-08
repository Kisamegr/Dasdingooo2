using UnityEngine;
using System.Collections;

public class SwingCog : Enemy {

	public float angle;
	public float minRadius;
	public float maxRadius;
	private float radius;

	private Transform stat;
	private Transform cog;
	private DistanceJoint2D joint;
	private LineRenderer line;
	// Use this for initialization
	void Start () {
		base.Start();

		stat = transform.parent.transform.FindChild("Stat");
		cog =  transform;
		joint = cog.GetComponent<DistanceJoint2D>();
		line = cog.GetComponent<LineRenderer>();

		radius = Mathf.Clamp( gameScript.stageTop - gameScript.player.position.y - 5, minRadius, maxRadius) ;

		stat.position = new Vector3(transform.position.x,gameScript.stageTop,0);
		cog.position = new Vector3(transform.position.x + radius * Mathf.Sin(angle * Mathf.Deg2Rad), gameScript.stageTop -  Mathf.Cos(angle*Mathf.Deg2Rad)*radius,0);


		joint.connectedBody = stat.GetComponent<Rigidbody2D>();
		joint.connectedAnchor = Vector3.zero;
		joint.distance = radius;



	}
	
	// Update is called once per frame
	void Update () {
		base.Update();

		line.SetPosition(0,cog.position);
		line.SetPosition(1,stat.position);
	}


	public override void Death ()
	{
		line.enabled = false;
		joint.enabled = false;
		GetComponent<Collider2D>().enabled = false;

		StartCoroutine(Kill(8));
	} 
}
