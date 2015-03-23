using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour
{



    public GameObject groundPrefab;
    public GameObject ceilingPrefab;
    public GameObject[] platforms;
    public GameObject batGroupPrefab;
	public GameObject[] powerUps;
	public GameObject[] enemies;

    public float yMax = 18f;
    public float yMin = -21f;
    public float cameraHeight;

	public float enemyFreq;
	public float enemyVar;

    public float platformFreq;
    public float platformVar;
	public float platMinDist;
    private float lastPlatformTime;
    private float nextPlatformTime;
    private Transform platformSpawner;



    private float lastEnemyTime;
    private float nextEnemyTime;
    private Transform enemySpawner;

	 




   /* public float batGroupFreq;
    public float batGroupVar;
    public float batGroupSpawnYmax;
    public float batGroupSpawnYmin;
    public float batGroupSpawnXmax;
    public int batGroupNoBatsMin;
    public int batGroupNoBatsMax;

*/



    public float score;
    public Text scoreText;

    private Vector3 startingPos;


    private float groundWidth;
    private float ceilingWidth;
    private Transform lastGround;
    private Transform lastCeiling;
	private Transform lastPlatform;

    public Transform camTrans;
    public Transform player;
    private Transform cleaner;

    private Queue groundQueue;
    private Queue ceilingQueue;
    private Queue platformQueue;

    void Start()
    {
		Time.timeScale = 1f;
        groundQueue = new Queue();
        ceilingQueue = new Queue();
        platformQueue = new Queue();

        //camTrans =  GameObject.FindGameObjectWithTag("MainCamera").transform;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        cleaner = camTrans.FindChild("Cleaner");
        platformSpawner = camTrans.FindChild("PlatformSpawner");

        Vector3 cameraZero = camTrans.camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 cameraTop = camTrans.camera.ViewportToWorldPoint(new Vector3(0, 1, 0));
        cameraHeight = cameraTop.y - cameraZero.y;

		nextEnemyTime = 6f;
		lastEnemyTime = 0f;

		nextPlatformTime = Random.Range(-platformVar, platformVar) + platformFreq;
		lastPlatformTime = Time.time;

        //Debug.Log(cameraTop);

		groundWidth = groundPrefab.transform.GetChild(0).renderer.bounds.size.x   - 2;
        ceilingWidth = ceilingPrefab.renderer.bounds.size.x;
        //groundWidth = 10;
		

        GameObject ground = null, ceiling = null;

        bool reverse = false;

        for (int i = 0; i < 12; i++)
        {
            Vector3 pos = new Vector3(cameraZero.x + groundWidth * i, yMin, 0);
            ground = (GameObject)GameObject.Instantiate(groundPrefab, pos, Quaternion.identity);

            Transform cog = ground.transform.GetChild(1);
            cog.localScale = new Vector3(groundWidth / cog.renderer.bounds.size.x, cog.localScale.y, cog.localScale.z);

            if (reverse)
                ground.transform.GetChild(0).GetComponent<Cog>().rotationSpeed *= -1;

            reverse = !reverse;
            groundQueue.Enqueue(ground.transform);

            Vector3 pos2 = new Vector3(cameraZero.x + ceilingWidth * i, yMax, 0);
            ceiling = (GameObject)GameObject.Instantiate(ceilingPrefab, pos2, Quaternion.identity);
            ceilingQueue.Enqueue(ceiling.transform);

        }

        lastGround = ground.transform;
        lastCeiling = ceiling.transform;
		lastPlatform = null;

        startingPos = player.position;



        enemySpawner = camTrans.FindChild("EnemySpawner");

    }



    void Update()
    {
        //float yCamera = Mathf.Clamp(player.position.y, yMin + cameraHeight / 2 - 1, yMax - cameraHeight / 2 + 1);
        //camTrans.position = new Vector3(player.position.x + 14, yCamera, camTrans.position.z);


        if (Time.time - lastPlatformTime > nextPlatformTime)
        {
            CreatePlatform();
        }

        if (Time.time - lastEnemyTime > nextEnemyTime)
        {
			int r = Random.Range(0,enemies.Length);

			Instantiate(enemies[r],enemySpawner.position,Quaternion.identity);

			nextEnemyTime = enemyFreq + Random.Range(-enemyVar, enemyVar);
			lastEnemyTime = Time.time;
        }

    }

    void FixedUpdate()
    {
		Physics2D.IgnoreLayerCollision (LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), player.rigidbody2D.velocity.y > 0.05f);



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

        if (platformQueue.Count > 0 && ((Transform)platformQueue.Peek()).position.x < cleaner.position.x)
        {

            Transform platform = (Transform)platformQueue.Dequeue();

            Destroy(platform.gameObject);

        }

    }

    public void CreatePlatform()
	{

		if( lastPlatform == null || (platformSpawner.position.x - lastPlatform.position.x > platMinDist)) {

	        int index = Random.Range(0, platforms.GetLength(0));
	        Vector3 pos = new Vector3(platformSpawner.position.x, Random.Range(yMin + 5, yMax - 20), 0);

	        GameObject plat = (GameObject)Instantiate(platforms[index], pos, Quaternion.identity);


			float p = Random.Range(0f,1f);

			if(p>0.01f) {
				GameObject power;
				float pp = Random.Range(0f,1f);

				if(pp < 0.25f) 
					power = (GameObject) Instantiate(powerUps[0]);	
				else if(pp < 0.5f) 
					power = (GameObject) Instantiate(powerUps[1]);
				else if (pp < 0.75f) 
					power = (GameObject) Instantiate(powerUps[2]);
				else 
					power = (GameObject) Instantiate(powerUps[3]);
				

				power.transform.position = new Vector3(plat.transform.position.x, plat.transform.position.y + 4, 0);
				power.transform.parent = plat.transform;

			}


	        platformQueue.Enqueue(plat.transform);

			lastPlatform = plat.transform;
	        nextPlatformTime = Random.Range(-platformVar, platformVar) + platformFreq;
	        lastPlatformTime = Time.time;
		}
    }

	public void AddNextEnemyTime(float sec) {
		nextEnemyTime += sec;
	}

  /*  public void GenerateBatgroup()
    {
        GameObject batGroup = (GameObject)Instantiate(batGroupPrefab,enemySpawner.position,Quaternion.identity);
        BatGroup batGroupScript = batGroup.GetComponent<BatGroup>();

        int noBats = Random.Range(batGroupNoBatsMin, batGroupNoBatsMax);

        batGroupScript.numberOfBats = noBats;
        batGroupScript.yMax = batGroupSpawnYmax;
        batGroupScript.yMin = batGroupSpawnYmin;
        batGroupScript.xRange = batGroupSpawnXmax;




    }
*/
}
