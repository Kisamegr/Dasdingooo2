using UnityEngine;
using System.Collections;

public class Hook : MonoBehaviour
{

    private int collisionLayermask;
    //
    public GameObject player;
    //    
    private float hookSpeed;
    //
    public float initialHookSpeed;
    //
    public float finalHookSpeed;
    //

    private float extraForce = 2f;

    public float initialExtraForce;

    public float finalExtraForce;
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
    private LineRenderer line;

    private Vector3 lineStart;

    private Game game;

    public AudioClip hookShootSound;

    public AudioClip hookHitSound;

    // Use this for initialization
    void Start()
    {
        //Time.timeScale = 0.2f;
        game = GameObject.Find("_GAME").GetComponent<Game>();
        if (game.save.isSoundOn())
        {
            AudioSource.PlayClipAtPoint(hookShootSound, transform.position, 0.15f);
        }

        hit = false;
        hookLength = 0;
        line = (LineRenderer)gameObject.GetComponent<LineRenderer>();
        lineStart = new Vector3(player.GetComponent<DistanceJoint2D>().anchor.x, player.GetComponent<DistanceJoint2D>().anchor.y, 0);

        hookVector = new Vector3(Mathf.Cos(Mathf.Deg2Rad * hookAngle), Mathf.Sin(Mathf.Deg2Rad * hookAngle), 0);

        transform.RotateAround(transform.position, Vector3.forward, hookAngle);


        float forceAngle = hookAngle - 90;
        forceAngle *= Mathf.Deg2Rad;
        extraForceDirection = new Vector2(Mathf.Cos(forceAngle), Mathf.Sin(forceAngle));
        extraForceDirection.Normalize();
        collisionLayermask = 1 << LayerMask.NameToLayer("Platform") | 1 << LayerMask.NameToLayer("Ceiling");

    }

    // Update is called once per frame
    void Update()
    {

        if (game.NormalizedDiffuclty < 1)
        {
            hookSpeed = initialHookSpeed + game.NormalizedDiffuclty * (finalHookSpeed - initialHookSpeed);
        }



        if (hit)
        {
            transform.position = hitPoint;
            //transform.RotateAround(transform.position,transform.forward,Vector3.Angle() );
            transform.rotation = Quaternion.Euler(new Vector3(0, 0, Vector3.Angle(hitPoint - player.transform.position, Vector3.right)));
            //Debug.Log(transform.rotation);


        }
        else
        {
            float distance = hookLength;
            hookLength += Time.deltaTime * hookSpeed;

            distance = hookLength - distance;

            //Raycast before moving the hook 

            RaycastHit2D hookHit = Physics2D.Raycast(transform.position, hookVector, distance, collisionLayermask);
            //If the ray hit a platform or the ceiling, move the hook to the collision point and call the OnTriggerEnter2D function
            if (hookHit.collider != null)
            {
                transform.position = new Vector3(hookHit.point.x, hookHit.point.y, transform.position.z);
                attachHook(hookHit.collider);
            }
            //else move the hook normally
            else
            {
                transform.position = player.transform.position + hookLength * hookVector;
            }

            //transform.position = player.transform.position + hookLength * hookVector;
        }

        line.SetPosition(0, player.transform.position + lineStart);

        //Leptomereies
        if (hit)
        {
            transform.position = connectedRigidbody.transform.position + new Vector3(connectedAnchorTransformOffset.x, connectedAnchorTransformOffset.y, 0);
            line.SetPosition(1, transform.position);
        }
        else
        {
            line.SetPosition(1, transform.position);
        }
    }



    void attachHook(Collider2D other)
    {
        if (other.tag == "Ceiling" || other.tag == "Platform")
        {
            if (hit) return;
            //transform.rotation = Quaternion.EulerAngles(new Vector3(0, 0, 90));

            if (game.NormalizedDiffuclty < 1)
            {
                extraForce = initialExtraForce + game.NormalizedDiffuclty * (finalExtraForce - initialExtraForce);
            }



            if (game.save.isSoundOn())
                AudioSource.PlayClipAtPoint(hookHitSound, transform.position, 0.07f);


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


            float minSpeed = player.GetComponent<Player>().minSpeed;
            if (player.rigidbody2D.velocity.x < minSpeed)
            {
                player.rigidbody2D.velocity = new Vector2(minSpeed, player.rigidbody2D.velocity.y);
            }


            player.rigidbody2D.gravityScale = 0;

        }
    }

}
