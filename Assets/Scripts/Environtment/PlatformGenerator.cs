using UnityEngine;
using System.Collections;

public class PlatformGenerator : MonoBehaviour {

    public GameObject[] platforms;

    public GameObject[] powerUps;    

    public float platformFreq;
    public float platformVar;
    public float platMinDist;
    private float lastPlatformTime;
    private float nextPlatformTime;

    private Queue platformQueue;
    private Transform lastPlatform;


    private Transform player;
    private Transform cleaner;


    private float stageBottom;
    private float stageTop;


    
    [Range(0.0f, 1.0f)]
    public float spawnAnythingChance;

    [Range(0.0f, 1.0f)]
    public float spawnCoinsChance;


    private CoinsGenerator coinsGenerator;

    private int coinsLayermask;

    public float coinSpawnMargin;


	// Use this for initialization
	void Start () {
        platformQueue = new Queue();


        lastPlatformTime = -1;
        lastPlatform = null;


        player = GameObject.FindGameObjectWithTag("Player").transform;


        cleaner = transform.parent.FindChild("Cleaner");


        Game game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        stageBottom = game.stageBottom;
        stageTop = game.stageTop;

        coinsGenerator = transform.parent.FindChild("CoinsSpawner").GetComponent<CoinsGenerator>();

        coinsLayermask = 1 << LayerMask.NameToLayer("Coin");
	}
	
	// Update is called once per frame
	void Update () {
        if( lastPlatformTime > 0 && Time.time - lastPlatformTime > nextPlatformTime)
        {
            bool result = CreatePlatform();
            if(!result){
                nextPlatformTime += 0.5f;
            }
        }
	}

    void FixedUpdate()
    {
        Physics2D.IgnoreLayerCollision(LayerMask.NameToLayer("Player"), LayerMask.NameToLayer("Platform"), player.rigidbody2D.velocity.y > 0.05f);


        if (platformQueue.Count > 0 && ((Transform)platformQueue.Peek()).position.x < cleaner.position.x)
        {

            Transform platform = (Transform)platformQueue.Dequeue();

            Destroy(platform.gameObject);

        }
    
    }

    public bool CreatePlatform()
    {

        if (lastPlatform == null || (transform.position.x - lastPlatform.position.x > platMinDist))
        {

            int index = Random.Range(0, platforms.GetLength(0));
            Vector3 pos = new Vector3(transform.position.x, Random.Range(stageBottom + 10, stageTop - 20), 2);


            GameObject platformPrefab = platforms[index];


            //An uparxoun coins stin perioxh dipla, min kaneis spawn kai epestrepse false

            Vector2 pointA = new Vector2(pos.x - platformPrefab.collider2D.bounds.extents.x - coinSpawnMargin, stageBottom);
            Vector2 pointB = new Vector2(pos.x + platformPrefab.collider2D.bounds.extents.x + coinSpawnMargin, stageTop);
            if (Physics2D.OverlapArea(pointA, pointB, coinsLayermask) != null)
            {
                Debug.Log("There are coins colliding");
                return false;
            }


            GameObject plat = (GameObject)Instantiate(platforms[index], pos, Quaternion.identity);


            float p = Random.value;

            if (p < spawnAnythingChance)
            {
                p = Random.value;
                if (p < spawnCoinsChance)
                {
                   coinsGenerator.generateCoinsOnPlatform(plat.transform);
                }
                else
                {
                    GameObject power;
                    float pp = Random.Range(0f, 1f);

                    if (pp < 0.25f)
                        power = (GameObject)Instantiate(powerUps[0]);
                    else if (pp < 0.5f)
                        power = (GameObject)Instantiate(powerUps[1]);
                    else if (pp < 0.75f)
                        power = (GameObject)Instantiate(powerUps[2]);
                    else
                        power = (GameObject)Instantiate(powerUps[3]);


                    power.transform.position = new Vector3(plat.transform.position.x, plat.transform.position.y + 4, 0);
                    power.transform.parent = plat.transform;
                }
            }


            platformQueue.Enqueue(plat.transform);

            lastPlatform = plat.transform;
            nextPlatformTime = Random.Range(-platformVar, platformVar) + platformFreq;
            lastPlatformTime = Time.time;
            return true;
        }
        return false;
    }

	public void StartGenerating() {
		lastPlatformTime = Time.time;
		nextPlatformTime = Random.Range(-platformVar, platformVar) + platformFreq;

	}
}
