using UnityEngine;
using System.Collections;
using System;

public class Player : MonoBehaviour
{
	public Game gameScript;
    public GameObject hookPrefab;
	public GameObject brokenPrefab;

	public bool alive;
    public bool shotHook;
	public bool hooked;
	public bool jumped;
	public bool startJump;


    public bool inCannon;
    public bool firedFromCannon;

	public float moveForce;
	public float maxSpeed;
    public float minSpeed;
	public float jumpForce;
	public float hookDelay;
	public float hookAngle;
	public float stabilizerForce;

    public GameObject hook;
    private DistanceJoint2D hookJoint;

	private Animator anim;

	private float lastHookTime;

	public bool running;
	public bool onAir;
	public bool facingRight;

	public bool leftGround;
	public float leftGroundTime;


	private bool hitHead;
	private float ceilingPenaltyStart;
	public float ceilingPenaltyDuration = 1f;

	private float zeta;


    public GameObject ghostPrefab;
   
    public float ghostsSpeedThreshold = 0.7f;
    public float ghostsInitFrequency = 0.25f;
    public float ghostsLifetime = 5f;
    private float ghostsLastInitTime;

    private SpriteRenderer spriteRenderer;

	private bool touchStart;
	private bool touchEnd;

	private bool jumpButton;

	public AudioClip[] deathSounds;


    // Use this for initialization
    void Start()
    {
		Input.simulateMouseWithTouches = true;

		alive = true;
        shotHook = false;
		jumped = false;
        hookJoint = (DistanceJoint2D)gameObject.GetComponent<DistanceJoint2D>();
        hookJoint.enabled = false;
		anim = transform.GetChild(0).GetComponent<Animator>();
		jumpButton = false;

		//rigidbody2D.centerOfMass = new Vector2(0,-renderer.bounds.extents.y);

		lastHookTime = -100;
		running = false;
		onAir = true;
		leftGround = false;
		facingRight = true;
		
		spriteRenderer =  transform.GetChild(0).GetComponent<SpriteRenderer>();

    }

	void Death() {

		alive = false;
		cancelHook();

		if(gameScript.save.isSoundOn())
			AudioSource.PlayClipAtPoint(deathSounds[UnityEngine.Random.Range(0,deathSounds.Length)],transform.position,0.85f);

		if(audio.isPlaying)
			audio.Stop();

		transform.GetChild(0).renderer.enabled = false;
		gameObject.collider2D.enabled = false;

		GameObject broken = (GameObject) Instantiate(brokenPrefab,transform.position,Quaternion.identity);
		broken.transform.rotation = transform.rotation;

		for(int i=0 ;i<broken.transform.childCount ; i++) {
			Transform child = (Transform) broken.transform.GetChild(i);

			child.rigidbody2D.velocity = rigidbody2D.velocity;

		}
		rigidbody2D.velocity = Vector2.zero;
		rigidbody2D.isKinematic = true;
		//Time.timeScale = 0.25f;

		GameObject.Find("_GAME").GetComponent<Game>().GameOver();



	}


    // Update is called once per frame
    void Update()
    {
	
        //An einai mesa sto kanoni min kaneis tpt
        if (inCannon || !alive)
            return;
        

        //An exei petaxtei apo to kanoni tote perimene mexri na arxisei na katevainei. Ka8ws anevainei min kaneis tpt
        if (firedFromCannon)
        {
            if (rigidbody2D.velocity.x > maxSpeed)
            {
                rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
            }


            if (rigidbody2D.velocity.y > -0.1)
            {
			    anim.SetBool("jump",false);
                return;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                firedFromCannon = false;
				rigidbody2D.fixedAngle = false;
            }
        }



		running = false;
		startJump = false;
		zeta = transform.rotation.eulerAngles.z;

		if(zeta > 180)
			zeta = zeta - 360;

		touchStart = false;
		touchEnd = false;


		//Debug.Log(zeta);

		// Stabilizer
		if(!shotHook && !hooked && !onAir) 
			rigidbody2D.AddTorque(-zeta * stabilizerForce,ForceMode2D.Force);

		if(onAir)
			rigidbody2D.AddTorque(-5,ForceMode2D.Force);

		if(shotHook)
			rigidbody2D.AddTorque(-zeta * stabilizerForce  -20,ForceMode2D.Force);

		
		if(hooked) {
			Vector3 hookVec = hook.transform.position - transform.position;
			float angle = Vector3.Angle(Vector3.up,hookVec);

			if(transform.position.x < hook.transform.position.x)
				angle *= -1;

			rigidbody2D.AddTorque(-zeta * stabilizerForce +  angle*2.5f,ForceMode2D.Force);

            if (transform.position.y > hookJoint.connectedBody.transform.position.y + hookJoint.connectedAnchor.y)
            {
                cancelHook();
            }

		}
	


		//Debug.Log(rigidbody2D.velocity.y);
		if (!hooked)
		{
			if (rigidbody2D.velocity.y > 0 && rigidbody2D.gravityScale != 2)
			{
				rigidbody2D.gravityScale = 2;
			}
			if (rigidbody2D.velocity.y <= 0 && rigidbody2D.gravityScale != 1)
			{
				rigidbody2D.gravityScale = 1;
			}

		}
		else {
			jumped = false;
		}

		GetUserInput();


       


        if (rigidbody2D.velocity.x > maxSpeed)
        {
            rigidbody2D.velocity = new Vector2(maxSpeed, rigidbody2D.velocity.y);
        }

		if(leftGround && Time.time-leftGroundTime > 0.2f)
			onAir = true;

		//Initialize a ghost (isws mono otan einai hooked)
        if (rigidbody2D.velocity.x / maxSpeed > ghostsSpeedThreshold && Time.time - ghostsLastInitTime > ghostsInitFrequency)
        {
            instantiateGhost();
        }


		anim.SetBool("running",running);
		anim.SetBool("jump",startJump);
		anim.SetFloat("ySpeed",rigidbody2D.velocity.y);
		anim.SetBool("shotHook",shotHook);
		anim.SetBool("hooked",hooked);
		anim.SetBool("onAir",onAir);
		anim.SetBool("hitCeiling",hitHead);

		hitHead = false;
    }

	void GetUserInput() {

		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsWebPlayer) {

			bool mouseDown = Input.GetMouseButtonDown(0);
			bool mouseUp = Input.GetMouseButtonUp(0);

			bool swing=false;

			if(mouseDown || mouseUp) {


				Vector3 mousePos = Input.mousePosition;

				if(mousePos.x > Screen.width/2)
					swing = true;
			}

			if (Input.GetKeyDown(KeyCode.X) || (mouseDown && swing))
			{
				shootHook ();
			}
			
			else if (Input.GetKeyUp(KeyCode.X) || (mouseUp && swing))
			{
				cancelHook();
			}
			
		
			if((Input.GetKeyDown(KeyCode.Z) || jumpButton || (mouseDown && !swing)) && !jumped && !hooked) {
				rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x,0);
				
				
				rigidbody2D.AddForce(Vector2.up*jumpForce , ForceMode2D.Impulse);
				jumped = true;
				startJump = true;
				jumpButton = false;
			}

		}
		
		if (Application.platform == RuntimePlatform.Android) {
			//Debug.Log("INSEROTOOO");
			Touch t = new Touch ();
			try {
				t = Input.GetTouch (0);
				
				if (t.phase == TouchPhase.Began) {
					
					shootHook();
				}
				else if (t.phase == TouchPhase.Ended) {
					cancelHook();
				}
				
			}
			catch (Exception e) {
				//Debug.Log (e);
			}
		}

		//if (Input.GetAxis("Horizontal") > 0 || !onAir )
		if(!onAir)
		{
			running = true;
			//if(!facingRight)
			//	Flip();
			transform.rigidbody2D.AddForce(Vector2.right * moveForce, ForceMode2D.Force);
			//transform.position = new Vector3(transform.position.x + 0.2f, transform.position.y,0);
		}
		/*if (Input.GetAxis("Horizontal") < 0 )
		{
			running = true;
			if(facingRight)
				Flip();
			transform.rigidbody2D.AddForce(-Vector2.right * moveForce, ForceMode2D.Force);
			//transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y,0);
		}*/
	}

	void FixedUpdate() {
		//Physics2D.IgnoreLayerCollision(LayerMask.GetMask("Enemy"),LayerMask.GetMask("Player"),true);
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ceiling")
			HitHead ();

		if(other.collider.tag == "Ground" || other.gameObject.layer == LayerMask.NameToLayer("Enemy"))
			Death();
        

		if(other.collider.tag == "Platform")
		{
            if (shotHook || hooked)
            {
                cancelHook();
            }
			if(other.contacts[0].point.y > other.transform.position.y) {
				onAir = false;
				leftGround = false;
				jumped = false;
			}
			//else
			//	HitHead();
		}

    }



    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.tag == "Crate")
        {
            Crate crate = other.GetComponent<Crate>();
            if (crate.isDestroyed())
            {
                return;
            }

            Vector2 velocity = rigidbody2D.velocity;
            velocity.x = (1 - crate.slowdownPercent) * velocity.x;

            rigidbody2D.velocity = velocity;

            //Maybe change animation;

            crate.Destroy();
        }        
    }




	void OnCollisionExit2D(Collision2D coll) {
		if(coll.collider.tag == "Platform")
		{
			leftGround = true;
			leftGroundTime = Time.time;
		}
	}

	public void ButtonJump() {
		if(!jumped)
			jumpButton = true;
	}



	public void shootHook()
	{
		if (!shotHook && Time.time - lastHookTime > hookDelay && Time.time - ceilingPenaltyStart > ceilingPenaltyDuration)
		{
			shotHook = true;
			hook = (GameObject)Instantiate(hookPrefab, transform.position, Quaternion.identity);
			hook.transform.parent = transform;
			hook.GetComponent<Hook>().player = gameObject;
			hook.GetComponent<Hook>().hookAngle = hookAngle;


			lastHookTime = Time.time;
		}
	}
	
	public void cancelHook()
	{
		if (shotHook || hooked)
		{
			shotHook = false;
			hooked = false;
			rigidbody2D.gravityScale = 1;
			Destroy(hook);
			hookJoint.enabled = false;
		}
	}

	void HitHead() {
		hitHead = true;
		ceilingPenaltyStart = Time.time;
		cancelHook();
	}

	void Flip()
	{
		// Switch the way the player is labelled as facing
		facingRight = !facingRight;
		
		// Multiply the player's x local scale by -1
		Vector3 theScale = transform.localScale;
		theScale.x *= -1;
		transform.localScale = theScale;
	}



    public void fireFromCannon(Vector2 cannonForce)
    {
        transform.parent = null;
        anim.SetBool("jump", true);
        rigidbody2D.AddForce(cannonForce, ForceMode2D.Impulse);
        if (rigidbody2D.velocity.y > 0)
        {
            rigidbody2D.gravityScale = 2;
        }
        inCannon = false;
        firedFromCannon = true;
    }
	
	
    public void instantiateGhost(){
        GameObject ghost = (GameObject)Instantiate(ghostPrefab, transform.position, transform.rotation);

        Ghost ghostScript = ghost.GetComponent<Ghost>();
        ghostScript.sprite = spriteRenderer.sprite;
        ghostScript.lifetime = ghostsLifetime;
		ghostScript.color = spriteRenderer.color;
		ghost.transform.localScale = transform.localScale;
        
        ghostsLastInitTime = Time.time;

    }

}
