using UnityEngine;
using System.Collections;
using System;

public class Cannon : MonoBehaviour
{


    private bool rotationPhase;

    private bool forcePhase;

    //
    public float rotation;

    public float rotationSpeed;

    public float startAngle;

    public float endAngle;
    //
    private float force;

    public float maxForce;

    public float minForce;

    public float forceSpeed;

    //

    public GameObject player;


    private Vector3 rotationCenter;


    private Transform smokeParticlesTrans;

    private float shootTime;

    private float shootDelay = 0.1f;

    private bool playerFired = false;

	private Game gameScript;
	private Game_UI ui;

    // Use this for initialization
    void Start()
    {
        
        //player = GameObject.FindGameObjectWithTag("Player");
        player.transform.parent = transform;
        player.transform.localPosition = transform.FindChild("PlayerPosition").localPosition;
        
        
        player.transform.Rotate(0, 0, -90);
        player.rigidbody2D.gravityScale = 0;
        player.rigidbody2D.fixedAngle = true;
        player.GetComponent<Player>().inCannon = true;


        if (startAngle > endAngle)
        {
            float temp = startAngle;
            startAngle = endAngle;
            endAngle = temp;
        }
        rotation = startAngle;
        transform.rotation = Quaternion.Euler(0, 0, startAngle);
        rotationPhase = true;
        forcePhase = false;
        force = 0;

        rotationCenter = transform.FindChild("RotationCenter").transform.position;

        smokeParticlesTrans = transform.parent.FindChild("SmokeParticles");
        smokeParticlesTrans.GetComponent<ParticleSystem>().Pause();

        shootTime = Time.time;
        playerFired = false;

		ui = GameObject.Find("_GAME").GetComponent<Game_UI>();
        gameScript =  GameObject.Find("_GAME").GetComponent<Game>();
		gameScript.deactivateSpawners();

		ui.uiAnimator.SetBool("cannon",true);
    }

    // Update is called once per frame
    void Update()
    {


		
		//An exei ginei fire, perimene mexri to delay
        if (!forcePhase && !rotationPhase)
        {
            if(Time.time - shootTime >= shootDelay && !playerFired){
                firePlayer();

                gameScript.activateSpawners();
				gameScript.gameRunning = true;
            }
        }

        

        if (rotationPhase)
        {
            rotation += rotationSpeed * Time.deltaTime;

            if (rotation > endAngle)
            {
                transform.rotation = Quaternion.Euler(0, 0, endAngle);
                rotation = endAngle;
                rotationSpeed = -rotationSpeed;
            }

            if (rotation < startAngle)
            {
                transform.rotation = Quaternion.Euler(0, 0, startAngle);
                rotation = startAngle;
                rotationSpeed = -rotationSpeed;
            }

            transform.RotateAround(rotationCenter, Vector3.forward, rotationSpeed * Time.deltaTime);
            //transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);

            if(UserClicked ()){
                rotationPhase = false;
                forcePhase = true;
                return;
            }
        }


        if (forcePhase)
        {
            //Debug.Log(force);
            force += forceSpeed * Time.deltaTime;
            if (force >= 1)
            {
                force = 1;
                forceSpeed = -forceSpeed;
            }

            if (force <= 0)
            {
                force = 0;
                forceSpeed = -forceSpeed;
            }
			if (UserClicked())
			{
                fire();
                forcePhase = false;
                rotationPhase = false;
            }

			ui.cannonSlider.value = ui.cannonSlider.minValue + (ui.cannonSlider.maxValue - ui.cannonSlider.minValue) * force;
            

        }
    }


    private void fire()
    {
        //Play smoke particles
        smokeParticlesTrans.position = transform.FindChild("SmokePosition").position;

        smokeParticlesTrans.GetComponent<ParticleSystem>().Play();

        //play sound
		if(gameScript.save.isSoundOn())
        	audio.Play();

        shootTime = Time.time;
        //fire player after delay
        //Invoke("firePlayer", 0.15f);
        //firePlayer();

		ui.uiAnimator.SetTrigger("hideCannon");
		ui.uiAnimator.SetBool("cannon", false);

		if(gameScript.save.isMusicOn())
			player.audio.PlayDelayed(1);
        
    
    }


    private void firePlayer()
    {
        Vector2 cannonForceDir = new Vector2(Mathf.Cos(transform.rotation.eulerAngles.z * Mathf.Deg2Rad), Mathf.Sin(transform.rotation.eulerAngles.z * Mathf.Deg2Rad));

        float finalForce = minForce + force * (maxForce - minForce);

        player.GetComponent<Player>().fireFromCannon(cannonForceDir * finalForce);

        playerFired = true ;
    }


    public void OnDrawGizmosSelected()
    {
        float length = transform.FindChild("Cannon1").renderer.bounds.size.x;


        Vector3 point1 = transform.position + new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad),Mathf.Sin(startAngle * Mathf.Deg2Rad),0)*length;

        Vector3 point2 = transform.position + new Vector3(Mathf.Cos(endAngle * Mathf.Deg2Rad),Mathf.Sin(endAngle * Mathf.Deg2Rad),0)*length;


        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, point1);

        Gizmos.DrawLine(transform.position, point2);
    
    }

	private bool UserClicked() {

		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsWebPlayer) {
  
			if(Input.GetKeyDown(KeyCode.Space))
			   return true;


			if(Input.GetMouseButtonDown(0))
				return true;

		}

		if(Application.platform == RuntimePlatform.Android) {
			Touch t = new Touch ();
			try {
				t = Input.GetTouch (0);
				if (t.phase == TouchPhase.Began) {
					return true;
				}
				
			}
			catch (Exception e) {
				
			}
		
		}
		return false;
	}




}
