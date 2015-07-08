using UnityEngine;
using System.Collections;

public class BatGroup : Enemy {



    public GameObject batPrefab;

	public int batsMin;
	
	public int batsMax;

    public float heightVariance;

    public float xRange;

    public float batsScaleMin;

    public float batsScaleMax;

    public float batsSpeed;


	private int numberOfBats;

    private Player playerScript;

    private int layerMask;

	private Bat[] bats;

	private bool running;

    public AudioClip hookCutSound;

	// Use this for initialization
	void Start () {
		base.Start();
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        layerMask = 1 << batPrefab.layer;

		numberOfBats = Random.Range(batsMin,batsMax);

        Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();
        float stageBottom = game.stageBottom;
        float stageTop = game.stageTop;
        
        
        float spawnHeightBase = playerScript.transform.position.y + Random.Range(heightVariance, 2*heightVariance);
       
        spawnHeightBase = Mathf.Clamp(spawnHeightBase, stageBottom + heightVariance, stageTop - heightVariance);

        transform.position.Set(transform.position.x, spawnHeightBase, transform.position.z);

		bats = new Bat[numberOfBats];

        for (int i = 0; i < numberOfBats; i++)
        {
            float yOffset = transform.position.y + (2 * Random.value - 1) * heightVariance;
            float xOffset = (Random.value-0.5f) * xRange;
            GameObject bat = (GameObject)Instantiate(batPrefab,new Vector3(transform.position.x + xOffset,yOffset,transform.position.z), Quaternion.identity);
            bat.transform.parent = transform;
			bats[i] =  bat.GetComponent<Bat>();
            bats[i].speed = new Vector2(-batsSpeed, 0);
            
            float randomScale = Random.Range(batsScaleMin,batsScaleMax);
			bats[i].transform.localScale = new Vector3(-randomScale, randomScale, 1);

        }

		running = true;

	}
	
	// Update is called once per frame
	void Update () {
		base.Update();

		if(!running)
			return;
		
        if (playerScript.hooked || playerScript.shotHook)
        {
            //Xalia tropos ginete kai kalutera pernontas mono to hook angle, prepei na valw kai to position pou ksekinaei to hook apo ton paikth
            Vector2 direction = new Vector2(playerScript.hook.transform.position.x - playerScript.transform.position.x, playerScript.hook.transform.position.y - playerScript.transform.position.y);
            float distance = Vector2.Distance(playerScript.transform.position,playerScript.hook.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(playerScript.transform.position, direction,distance,layerMask);
            if (hit)
            {
                if (GameObject.Find("_GAME").GetComponent<Game>().save.isSoundOn())
                {
                    AudioSource.PlayClipAtPoint(hookCutSound, transform.position, 0.5f);
                }
                playerScript.cancelHook();
            }
        }
	}

	public override void Death ()
	{
		running = false;

		foreach(Bat bat in bats) {
			bat.GetComponent<Rigidbody2D>().gravityScale = 1;
			bat.GetComponent<Rigidbody2D>().fixedAngle = false;
			bat.GetComponent<Rigidbody2D>().AddTorque(Random.Range(-30,30));
		}

		StartCoroutine(Kill(8));
	}
}
