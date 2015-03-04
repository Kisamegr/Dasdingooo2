using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{
    //
    public GameObject player;
    //    
    public float hookSpeed;
    //
    public float extraForce = 2f;
    //
    private Vector3 hookVector;
    //
    private float hookLength;

	public float hookAngle;
    //
    private Vector2 extraForceDirection;
    //
    private GameObject connectedRigidbody;
    //
    public Vector2 connectedAnchorTransformOffset;
    //
    private bool hit;
    //
    private Vector3 hitPoint;
    //
    LineRenderer line;

	private Vector3 lineStart;

    // Use this for initialization
    void Start()
    {
        //Time.timeScale = 0.2f;
        hit = false;
        hookLength = 0;
        line = (LineRenderer)gameObject.GetComponent<LineRenderer>();
		lineStart = new Vector3(player.GetComponent<DistanceJoint2D>().anchor.x,player.GetComponent<DistanceJoint2D>().anchor.y,0);

        hookVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * hookAngle), Mathf.Sin(Mathf.Deg2Rad * hookAngle), 0);

        transform.RotateAround(transform.position, Vector3.forward, hookAngle);


        float forceAngle = hookAngle - 90;
        forceAngle *= Mathf.Deg2Rad;

        extraForceDirection = new Vector2(Mathf.Cos(forceAngle), Mathf.Sin(forceAngle));
        extraForceDirection.Normalize();

	}

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
			transform.position = hitPoint;
			//transform.RotateAround(transform.position,transform.forward,Vector3.Angle() );
			transform.rotation = Quaternion.Euler( new Vector3(0,0,Vector3.Angle(hitPoint - player.transform.position,Vector3.right)));
			//Debug.Log(transform.rotation);


        }
        else
        {

            hookLength += Time.deltaTime * hookSpeed;

            transform.position = player.transform.position + hookLength * hookVector;
        }

        line.SetPosition(0,  player.transform.position + lineStart);
        
        //Leptomereies
        if (hit)
        {
            transform.position = connectedRigidbody.transform.position + new Vector3(connectedAnchorTransformOffset.x,connectedAnchorTransformOffset.y,0);
            line.SetPosition(1, transform.position);
        }
        else
        {
            line.SetPosition(1, transform.position);
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {

        if (other.tag == "Ceiling" || other.tag == "Platform")
        {
            if (hit) return;
            //transform.rotation = Quaternion.EulerAngles(new Vector3(0, 0, 90));
          

            hit = true;
            connectedRigidbody = other.gameObject;

            rigidbody2D.velocity = Vector2.zero;

            DistanceJoint2D joint = (DistanceJoint2D)player.GetComponent<DistanceJoint2D>();


            float xColPoint = (transform.position.x - other.transform.position.x);
            float yColPoint = -other.bounds.extents.y + (other.bounds.center.y - other.transform.position.y);


            connectedAnchorTransformOffset = new Vector2(xColPoint, yColPoint);



            xColPoint /= other.transform.localScale.x;
            yColPoint /= other.transform.localScale.y;


            Vector2 connectedAnchor = new Vector2(xColPoint, yColPoint);


            
            //Vector2 anchor = Vector2.zero;
            
            joint.enabled = true;
            joint.connectedBody = other.rigidbody2D;
            //joint.anchor = anchor;
            joint.connectedAnchor = connectedAnchor;
            //joint.maxDistanceOnly = false;
            joint.distance = Vector2.Distance(player.transform.position, transform.position);


                
            hitPoint = new Vector3(other.transform.position.x + xColPoint, other.transform.position.y + yColPoint, 0);

            player.rigidbody2D.AddForce(extraForceDirection * extraForce, ForceMode2D.Impulse);
			player.GetComponent<Player>().hooked = true;
			player.GetComponent<Player>().shotHook = false;


            player.rigidbody2D.gravityScale = 0;
           
        }

    }

}
