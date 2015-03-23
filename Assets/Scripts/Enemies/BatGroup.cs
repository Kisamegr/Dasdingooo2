using UnityEngine;
using System.Collections;

public class BatGroup : Enemy {



    public GameObject batPrefab;

	public int batsMin;
	
	public int batsMax;

    public float yMin;

    public float yMax;

    public float xRange;


	private int numberOfBats;

    private Player playerScript;

    private int layerMask;

	// Use this for initialization
	void Start () {
		base.Start();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        layerMask = 1 << batPrefab.layer;

		numberOfBats = Random.Range(batsMin,batsMax);

        for (int i = 0; i < numberOfBats; i++)
        {
            float yOffset = transform.position.y + (yMax - yMin) * (Random.value-0.5f);
            float xOffset = (Random.value-0.5f) * xRange;
            GameObject bat = (GameObject)Instantiate(batPrefab,new Vector3(transform.position.x + xOffset,yOffset,transform.position.z), Quaternion.identity);
            bat.transform.parent = transform;
            bat.GetComponent<Bat>().speed = new Vector2(-10, 0);
            
            float randomScale = 0.7f + 0.3f * Random.value;
            bat.transform.localScale = new Vector3(-randomScale, randomScale, 1);
        }

	}
	
	// Update is called once per frame
	void Update () {
		base.Update();
		/*
        if (playerScript.hooked || playerScript.shotHook)
        {
            //Xalia tropos ginete kai kalutera pernontas mono to hook angle, prepei na valw kai to position pou ksekinaei to hook apo ton paikth
            Vector2 direction = new Vector2(playerScript.hook.transform.position.x - playerScript.transform.position.x, playerScript.hook.transform.position.y - playerScript.transform.position.y);
            float distance = Vector2.Distance(playerScript.transform.position,playerScript.hook.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(playerScript.transform.position, direction,distance,layerMask);
            if (hit)
            {
                playerScript.cancelHook();
            }
        }*/
	}
}
