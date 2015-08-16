using UnityEngine;
using System.Collections;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

	public Image theImage;

	public Game gameScript;
    public GameObject hookPrefab;
	public GameObject brokenPrefab;

	public bool alive;
    public bool shotHook;
	public bool hooked;


    public bool inCannon;
    public bool firedFromCannon;

	public float moveForce;
	private float maxSpeed;
    public float initialMaxSpeed;
    public float finalMaxSpeed;

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

    public AudioClip headHitSound;

    private float lastDownwardJumpTime;
    private bool jumpedDownwards;
	private bool isJumpingDownwards;
    private bool jumpedUpwards;

	private bool finishedFireFromCannon;

    // Use this for initialization
    void Start()
    {
		Input.simulateMouseWithTouches = true;

		alive = true;
        shotHook = false;
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

        maxSpeed = initialMaxSpeed;
		spriteRenderer =  transform.GetChild(0).GetComponent<SpriteRenderer>();

		finishedFireFromCannon = false;
    }


    // Update is called once per frame
    void Update()
    {
	
        //An einai mesa sto kanoni min kaneis tpt
        if (inCannon || !alive)
            return;



        //Update max speed 
        if (gameScript.NormalizedDiffuclty < 1)
        {
            maxSpeed = initialMaxSpeed + gameScript.NormalizedDiffuclty * (finalMaxSpeed - initialMaxSpeed);
        }
        

        //An exei petaxtei apo to kanoni tote perimene mexri na arxisei na katevainei. Ka8ws anevainei min kaneis tpt
        if (firedFromCannon)
        {
            if (GetComponent<Rigidbody2D>().velocity.x > maxSpeed)
            {
                GetComponent<Rigidbody2D>().velocity = new Vector2(maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
            }


            if (GetComponent<Rigidbody2D>().velocity.y > -0.1)
            {
			    anim.SetBool("jump",false);
                return;
            }
            else
            {
                transform.rotation = Quaternion.Euler(0, 0, 0);
                firedFromCannon = false;
				GetComponent<Rigidbody2D>().fixedAngle = false;

				finishedFireFromCannon = true;
            }
        }



		running = false;
		zeta = transform.rotation.eulerAngles.z;

		if(zeta > 180)
			zeta = zeta - 360;

		touchStart = false;
		touchEnd = false;


		//Stabilizer
		if(!shotHook && !hooked && !onAir) 
			GetComponent<Rigidbody2D>().AddTorque(-zeta * stabilizerForce,ForceMode2D.Force);

		if(onAir)
			GetComponent<Rigidbody2D>().AddTorque(-5,ForceMode2D.Force);

		if(shotHook)
			GetComponent<Rigidbody2D>().AddTorque(-zeta * stabilizerForce  -20,ForceMode2D.Force);

		
		if(hooked) {
			Vector3 hookVec = hook.transform.position - transform.position;
			float angle = Vector3.Angle(Vector3.up,hookVec);

			if(transform.position.x < hook.transform.position.x)
				angle *= -1;

			GetComponent<Rigidbody2D>().AddTorque(-zeta * stabilizerForce +  angle*2.5f,ForceMode2D.Force);

            //If the player is above the hook then cancel the hook
            if (transform.position.y > hookJoint.connectedBody.transform.position.y + hookJoint.connectedAnchor.y)
            {
                cancelHook();
            }
		}
	




        //Change the gravity when the player is moving upwards
		if (!hooked)
		{
			if (GetComponent<Rigidbody2D>().velocity.y > 0 && GetComponent<Rigidbody2D>().gravityScale != 2)
			{
				GetComponent<Rigidbody2D>().gravityScale = 2;
			}
			if (GetComponent<Rigidbody2D>().velocity.y <= 0 && GetComponent<Rigidbody2D>().gravityScale != 1)
			{
				GetComponent<Rigidbody2D>().gravityScale = 1;
			}

		}
        //reset jumps when hooked
		else {
			jumpedUpwards = false;
			jumpedDownwards = false;
		}


        //Stop downward jump
        if (isJumpingDownwards && Time.time > lastDownwardJumpTime + 0.3f )
        {
            GetComponent<Rigidbody2D>().velocity =  new Vector2(GetComponent<Rigidbody2D>().velocity.x,-6);
            isJumpingDownwards = false;
        }

		if(!onAir)
		{
			running = true;
			transform.GetComponent<Rigidbody2D>().AddForce(Vector2.right * moveForce, ForceMode2D.Force);
		}

        //
		GetUserInput();


        //Apply speed threshold
        if (GetComponent<Rigidbody2D>().velocity.x > maxSpeed)
        {
            GetComponent<Rigidbody2D>().velocity = new Vector2(maxSpeed, GetComponent<Rigidbody2D>().velocity.y);
        }

		if(leftGround && Time.time-leftGroundTime > 0.2f)
			onAir = true;

		//Initialize a ghost (isws mono otan einai hooked)
        if (GetComponent<Rigidbody2D>().velocity.x / maxSpeed > ghostsSpeedThreshold && Time.time - ghostsLastInitTime > ghostsInitFrequency)
        {
            instantiateGhost();
        }




        //Set Animation
		anim.SetBool("running",running);
		anim.SetFloat("ySpeed",GetComponent<Rigidbody2D>().velocity.y);
		anim.SetBool("shotHook",shotHook);
		anim.SetBool("hooked",hooked);
		anim.SetBool("onAir",onAir);
		anim.SetBool("hitCeiling",hitHead);

		hitHead = false;
    }



	public void JumpUP() {

		Debug.Log("MPIIEPIEPIEPIEPEIE");

		if(!jumpedUpwards && !hooked && finishedFireFromCannon && gameScript.gameRunning) {

			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x,0);
			GetComponent<Rigidbody2D>().AddForce(Vector2.up*jumpForce , ForceMode2D.Impulse);
			
			jumpedUpwards = true;

			gameScript.save.totalJumps++;

			anim.SetTrigger("jump");


		}
	}

	public void JumpDown() {
		if(!jumpedDownwards && !hooked && finishedFireFromCannon && gameScript.gameRunning) {
			GetComponent<Rigidbody2D>().velocity = new Vector2(GetComponent<Rigidbody2D>().velocity.x * 0.8f, 0);
			GetComponent<Rigidbody2D>().AddForce(-Vector2.up * jumpForce * 0.8f, ForceMode2D.Impulse);

			lastDownwardJumpTime = Time.time;
			jumpedDownwards = true;
			isJumpingDownwards = true;
			
			gameScript.save.totalJumps++;

			anim.SetTrigger("jump");

		}
	}



	void GetUserInput() {

		//if(Application.platform == RuntimePlatform.WindowsPlayer 
         //   || Application.platform == RuntimePlatform.WindowsEditor 
        //    || Application.platform == RuntimePlatform.WindowsWebPlayer) {

			bool mouseDown = Input.GetMouseButtonDown(0);
			bool mouseUp = Input.GetMouseButtonUp(0);

			bool swing=false;

			if(mouseDown || mouseUp) {


				Vector3 mousePos = Input.mousePosition;

				if(mousePos.x > Screen.width/2)
					swing = true;
			}

			if (Input.GetKeyDown(KeyCode.Space) || (mouseDown && swing))
			{
				shootHook ();
			}
			else if (Input.GetKeyUp(KeyCode.Space) || (mouseUp && swing))
			{
				cancelHook();
			}

		bool upwardJump = false;
		bool downwardJump = false;
		
		
		if (mouseDown)
		{
			if (Input.mousePosition.y > Screen.height / 2 && Input.mousePosition.x < Screen.width / 2)
			{
				upwardJump = true;
			}
			
			if (Input.mousePosition.y < Screen.height / 2 && Input.mousePosition.x < Screen.width / 2)
			{
				downwardJump = true;
			}
		}
		
		
		//Upward Jump
		//if((Input.GetKeyDown(KeyCode.UpArrow) || jumpButton || (mouseDown && !swing && upwardJump)) && !jumped && !hooked) {
		if((Input.GetKeyDown(KeyCode.UpArrow))){
			upwardJump = true;
		}
		
		//Downward Jump
		//if ((Input.GetKeyDown(KeyCode.DownArrow)  || jumpButton || (mouseDown && !swing && !upwardJump)) && !jumpedDownwards && !hooked && onAir)
		if ((Input.GetKeyDown(KeyCode.DownArrow) ))
		{
			
			downwardJump = true;
			
		}
		
		
		if (upwardJump)
		{
			JumpUP();
		}
		
		if (downwardJump)
		{
			JumpDown();
		}
//
	//	}
		

		/*

        //Android Controls

		if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.MetroPlayerX86 ||  Application.platform == RuntimePlatform.MetroPlayerX64) {
			Touch t = new Touch ();
			try {
				t = Input.GetTouch (0);
				
				if (t.phase == TouchPhase.Began) {
                    if (t.position.x >= Screen.width / 2)
                    {
                        shootHook();
                    }
                    else
                    {
                        if ( jumpButton  && !jumped && !hooked)
                        {
                            rigidbody2D.velocity = new Vector2(rigidbody2D.velocity.x, 0);


                            rigidbody2D.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);
                            jumped = true;
                            startJump = true;
                            jumpButton = false;
                        }
                    }
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



		/*if (Input.GetAxis("Horizontal") < 0 )
		{
			running = true;
			if(facingRight)
				Flip();
			transform.rigidbody2D.AddForce(-Vector2.right * moveForce, ForceMode2D.Force);
			//transform.position = new Vector3(transform.position.x - 0.2f, transform.position.y,0);
		}*/
	}




    void Death()
    {

        alive = false;
        cancelHook();

		gameScript.score.heightLost = transform.position.y;

        if (gameScript.save.isSoundOn())
            AudioSource.PlayClipAtPoint(deathSounds[UnityEngine.Random.Range(0, deathSounds.Length)], transform.position, 0.85f);

        if (GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();

        transform.GetChild(0).GetComponent<Renderer>().enabled = false;
        gameObject.GetComponent<Collider2D>().enabled = false;

        GameObject broken = (GameObject)Instantiate(brokenPrefab, transform.position, Quaternion.identity);
        broken.transform.rotation = transform.rotation;

        for (int i = 0; i < broken.transform.childCount; i++)
        {
            Transform child = (Transform)broken.transform.GetChild(i);

            child.GetComponent<Rigidbody2D>().velocity = GetComponent<Rigidbody2D>().velocity;

        }
        GetComponent<Rigidbody2D>().velocity = Vector2.zero;
        GetComponent<Rigidbody2D>().isKinematic = true;
        //Time.timeScale = 0.25f;

        GameObject.Find("_GAME").GetComponent<Game>().GameOver();



    }





	void FixedUpdate() {
		//Physics2D.IgnoreLayerCollision(LayerMask.GetMask("Enemy"),LayerMask.GetMask("Player"),true);
	}

    void OnCollisionEnter2D(Collision2D other)
    {
        if (other.collider.tag == "Ceiling")
			HitHead ();

		Debug.Log("COLLISION HELL YEAH");
		if(other.collider.tag == "Ground" || other.gameObject.layer == LayerMask.NameToLayer("Enemy")) {
			Debug.Log("ME TO ENEMUFADFAF");

			Death();
		}
        

		if(other.collider.tag == "Platform")
		{
            if (shotHook || hooked)
            {
                cancelHook();
            }
			if(other.contacts[0].point.y > other.transform.position.y) {
				onAir = false;
				leftGround = false;
				jumpedUpwards = false;
				jumpedDownwards = false;
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

            Vector2 velocity = GetComponent<Rigidbody2D>().velocity;
            velocity.x = (1 - crate.slowdownPercent) * velocity.x;

            GetComponent<Rigidbody2D>().velocity = velocity;

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





	public void shootHook()
	{
		Debug.Log("SHOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOOT");
		if (!shotHook && (Time.time - lastHookTime > hookDelay) && (Time.time - ceilingPenaltyStart > ceilingPenaltyDuration) && finishedFireFromCannon && gameScript.gameRunning && alive)
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
			GetComponent<Rigidbody2D>().gravityScale = 1;
			Destroy(hook);
			hookJoint.enabled = false;
		}
	}

	void HitHead() {
        if (gameScript.save.isSoundOn())
            AudioSource.PlayClipAtPoint(headHitSound, transform.position, 2f);
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
        GetComponent<Rigidbody2D>().AddForce(cannonForce, ForceMode2D.Impulse);
        if (GetComponent<Rigidbody2D>().velocity.y > 0)
        {
            GetComponent<Rigidbody2D>().gravityScale = 2;
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

	public void JustDoIt() {
		//Color c = theImage.color;
		//c.a = 0.4f;
		//theImage.color = c;
	}

	public void DontDoIt() {
		//Color c = theImage.color;
		//c.a = 0;
		//theImage.color = c;
	}

}
