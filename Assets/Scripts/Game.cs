using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour
{

    public GameObject groundPrefab;
    public GameObject ceilingPrefab;


    public float stageTop = 18f;
    public float stageBottom = -21f;
    public float cameraHeight;
	 


    public float score;
    public Text scoreText;
	public int coinsCollected;

    private Vector3 startingPos;


    private float groundWidth;
    private float ceilingWidth;
    private Transform lastGround;
    private Transform lastCeiling;

    public Transform camTrans;
    public Transform player;
    private Transform cleaner;

    private Queue groundQueue;
    private Queue ceilingQueue;

	public bool gameRunning;

	private PlatformGenerator platformGenerator;
	private EnemyGenerator enemyGenerator;
	private CoinsGenerator coinsGenerator;



    void Start()
    {
		Time.timeScale = 1f;
		gameRunning = false;

        groundQueue = new Queue();
        ceilingQueue = new Queue();

        //camTrans =  GameObject.FindGameObjectWithTag("MainCamera").transform;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        cleaner = camTrans.FindChild("Cleaner");

        Vector3 cameraZero = camTrans.camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 cameraTop = camTrans.camera.ViewportToWorldPoint(new Vector3(0, 1, 0));
        cameraHeight = cameraTop.y - cameraZero.y;




        //Debug.Log(cameraTop);

		groundWidth = groundPrefab.transform.GetChild(0).renderer.bounds.size.x   - 2;
        ceilingWidth = ceilingPrefab.renderer.bounds.size.x;
        //groundWidth = 10;
		

        GameObject ground = null, ceiling = null;

        bool reverse = false;

        for (int i = 0; i < 12; i++)
        {
            Vector3 pos = new Vector3(cameraZero.x + groundWidth * i, stageBottom, 0);
            ground = (GameObject)GameObject.Instantiate(groundPrefab, pos, Quaternion.identity);

            Transform cog = ground.transform.GetChild(1);
            cog.localScale = new Vector3(groundWidth / cog.renderer.bounds.size.x, cog.localScale.y, cog.localScale.z);

            if (reverse)
                ground.transform.GetChild(0).GetComponent<Cog>().rotationSpeed *= -1;

            reverse = !reverse;
            groundQueue.Enqueue(ground.transform);

            Vector3 pos2 = new Vector3(cameraZero.x + ceilingWidth * i, stageTop, 0);
            ceiling = (GameObject)GameObject.Instantiate(ceilingPrefab, pos2, Quaternion.identity);
            ceilingQueue.Enqueue(ceiling.transform);

        }

        lastGround = ground.transform;
        lastCeiling = ceiling.transform;

        startingPos = player.position;

		platformGenerator = camTrans.FindChild("PlatformSpawner").GetComponent<PlatformGenerator>();
		enemyGenerator = camTrans.FindChild("EnemySpawner").GetComponent<EnemyGenerator>();
		coinsGenerator = camTrans.FindChild("CoinsSpawner").GetComponent<CoinsGenerator>();



    }

	public void StartGame() {

		gameRunning = true;

		platformGenerator.StartGenerating();
		enemyGenerator.StartGenerating();
		coinsGenerator.StartGenerating();
	}



    void Update()
    {
        //float yCamera = Mathf.Clamp(player.position.y, yMin + cameraHeight / 2 - 1, yMax - cameraHeight / 2 + 1);
        //camTrans.position = new Vector3(player.position.x + 14, yCamera, camTrans.position.z);
		if(!gameRunning)
			return;

        

    }

    void FixedUpdate()
    {
		if(!gameRunning) 
			return;

        score = player.position.x - startingPos.x;
        score /= 10;
        scoreText.text = "" + ((int)score);



        if (((Transform)groundQueue.Peek()).position.x < cleaner.position.x)
        {

            Transform ground = (Transform)groundQueue.Dequeue();
            groundQueue.Enqueue(ground);
            ground.position = new Vector3(lastGround.position.x + groundWidth, ground.position.y, 0);

            lastGround = ground;

        }

        if (((Transform)ceilingQueue.Peek()).position.x < cleaner.position.x)
        {

            Transform ceiling = (Transform)ceilingQueue.Dequeue();
            ceilingQueue.Enqueue(ceiling);
            ceiling.position = new Vector3(lastCeiling.position.x + ceilingWidth, ceiling.position.y, 0);

            lastCeiling = ceiling;

        }


    }


	public void AddNextEnemyTime(float sec) {
		enemyGenerator.AddNextEnemyTime(sec);
	}

	void OnLevelWasLoaded(int level) {
		Debug.Log("LEVEEVEVEL  " + level);
	}

 
    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(-100,stageTop,0),new Vector3(100,stageTop,0));
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-100,stageBottom,0),new Vector3(100,stageBottom,0));
    }

}
