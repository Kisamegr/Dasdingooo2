﻿using UnityEngine;
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
	public GameObject lifePlatformPrefab;


    private Vector3 rotationCenter;


    private Transform smokeParticlesTrans;

    private float shootTime;

    private float shootDelay = 0.1f;

    private bool playerFired = false;

	private Game gameScript;
	private Game_UI ui;

	private bool tapped;

	private Transform playerPosition;

    // Use this for initialization
    void Start()
    {

		gameScript =  GameObject.Find("_GAME").GetComponent<Game>();
		gameScript.deactivateSpawners();

		if(PlayerPrefs.HasKey("hasLife") && PlayerPrefs.GetInt("hasLife") == 1) {
			gameScript.score.LoadScore();
			
			Destroy(transform.parent.FindChild("StartingPlatform").gameObject);
			
			Vector3 pos = transform.position;
			pos.y = Mathf.Clamp(gameScript.score.heightLost,-9,11);
			GameObject lp = (GameObject) Instantiate(lifePlatformPrefab,pos,Quaternion.identity);
			
			transform.parent.parent = lp.transform;
			transform.parent.localPosition = Vector3.zero;

			Camera.main.transform.position = new Vector3(transform.position.x + 5,transform.position.y,Camera.main.transform.position.z);
		}
		playerPosition =  transform.FindChild("PlayerPosition") ;
        //player = GameObject.FindGameObjectWithTag("Player");
        player.transform.parent = transform;
        player.transform.localPosition = playerPosition.localPosition;
        
        
        player.transform.Rotate(0, 0, -90);
        player.GetComponent<Rigidbody2D>().gravityScale = 0;
        player.GetComponent<Rigidbody2D>().fixedAngle = true;
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
			player.transform.localPosition = playerPosition.localPosition;
			player.transform.rotation = playerPosition.rotation;
			player.transform.Rotate(0, 0, -90);

            if(UserClicked () || tapped){
                rotationPhase = false;
                forcePhase = true;
				tapped = false;
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
			if (UserClicked() || tapped)
			{
                fire();
                forcePhase = false;
                rotationPhase = false;
				tapped = false;
            }

			ui.cannonSlider.value = ui.cannonSlider.minValue + (ui.cannonSlider.maxValue - ui.cannonSlider.minValue) * force;
            

        }
    }


    public void fire()
    {
        //Play smoke particles
        smokeParticlesTrans.position = transform.FindChild("SmokePosition").position;

        smokeParticlesTrans.GetComponent<ParticleSystem>().Play();

        //play sound
		if(gameScript.save.isSoundOn())
        	GetComponent<AudioSource>().Play();

        shootTime = Time.time;
        //fire player after delay
        //Invoke("firePlayer", 0.15f);
        //firePlayer();

		ui.uiAnimator.SetTrigger("hideCannon");
		ui.uiAnimator.SetBool("cannon", false);

		if(gameScript.save.isMusicOn())
			player.GetComponent<AudioSource>().PlayDelayed(1);
        
    
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
        float length = transform.FindChild("Cannon1").GetComponent<Renderer>().bounds.size.x;


        Vector3 point1 = transform.position + new Vector3(Mathf.Cos(startAngle * Mathf.Deg2Rad),Mathf.Sin(startAngle * Mathf.Deg2Rad),0)*length;

        Vector3 point2 = transform.position + new Vector3(Mathf.Cos(endAngle * Mathf.Deg2Rad),Mathf.Sin(endAngle * Mathf.Deg2Rad),0)*length;


        Gizmos.color = Color.blue;

        Gizmos.DrawLine(transform.position, point1);

        Gizmos.DrawLine(transform.position, point2);
    
    }

	public void UserTapped() {
		if(!playerFired)
			tapped=true;
	}

	private bool UserClicked() {


		//if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsWebPlayer) {
  
			if(Input.GetKeyDown(KeyCode.Space))
			   return true;


			if(Input.GetMouseButtonDown(0))
				return true;

		//}

		/*if(Application.platform == RuntimePlatform.Android) {
			Touch t = new Touch ();
			try {
				t = Input.GetTouch (0);
				if (t.phase == TouchPhase.Began) {
					return true;
				}
				
			}
			catch (Exception e) {
				
			}
		
		}*/
		return false;
	}




}
