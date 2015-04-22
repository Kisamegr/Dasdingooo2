using UnityEngine;
using System.Collections;

public class PlatformsGenerator : MonoBehaviour
{

    public GameObject[] platforms;

    public GameObject[] powerUps;

    private float platformFreq;
    private float platformVar;
    private float platformMinDist;

    public float initialPlatformFreq;
    public float initialPlatformVar;
    public float initialPlatformMinDist;

    public float finalPlatformFreq;
    public float finalPlatformVar;
    public float finalPlatformMinDist;



    private float lastPlatformTime;
    private float nextPlatformTime;

    private Queue platformQueue;
    private Transform lastPlatform;


    private Transform player;
    private Transform cleaner;




    public GameObject cratePrefab;

    public int minCrates;

    public int maxCrates;




    [Range(0.0f, 1.0f)]
    public float spawnNothingChance;

    [Range(0.0f, 1.0f)]
    public float spawnCoinsChance;

    [Range(0.0f, 1.0f)]
    public float spawnPowerupChance;

    [Range(0.0f, 1.0f)]
    public float spawnCratesChance;




    public float coinSpawnMargin;

    private CoinsGenerator coinsGenerator;

    private int coinsLayermask;

	public int startPowerupNumber;
	public float powerupDist;





    private bool activeGenerator;

    private Game game;

    // Use this for initialization
    void Start()
    {
        platformQueue = new Queue();

        player = GameObject.FindGameObjectWithTag("Player").transform;

        cleaner = transform.parent.FindChild("Cleaner");

        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        coinsGenerator = transform.parent.FindChild("CoinsSpawner").GetComponent<CoinsGenerator>();

        coinsLayermask = 1 << LayerMask.NameToLayer("Coin");


        platformFreq = initialPlatformFreq;
        platformVar = initialPlatformVar;
        platformMinDist = initialPlatformMinDist;


        nextPlatformTime = Random.Range(-platformVar, platformVar) + platformFreq;
        lastPlatformTime = Time.time;
        lastPlatform = null;


        float spawnChancesSum = spawnNothingChance + spawnCoinsChance + spawnPowerupChance + spawnCratesChance;

        spawnNothingChance = spawnNothingChance / spawnChancesSum;

        spawnCoinsChance = spawnCoinsChance / spawnChancesSum;

        spawnPowerupChance = spawnPowerupChance / spawnChancesSum;

        spawnCratesChance = spawnCratesChance / spawnChancesSum;

		SpawnStartingPowerUps();
    }

    // Update is called once per frame
    void Update()
    {
        if (!activeGenerator) return;


        if (game.NormalizedDiffuclty < 1)
        {
            platformFreq = initialPlatformFreq + game.NormalizedDiffuclty * (finalPlatformFreq - initialPlatformFreq);
            platformVar = initialPlatformVar + game.NormalizedDiffuclty * (finalPlatformVar - initialPlatformVar);
            platformMinDist = initialPlatformMinDist + game.NormalizedDiffuclty * (finalPlatformMinDist - initialPlatformMinDist);
        }



        if (Time.time - lastPlatformTime > nextPlatformTime)
        {
            bool result = CreatePlatform();
            if (!result)
            {
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

        if (lastPlatform == null || (transform.position.x - lastPlatform.position.x > platformMinDist))
        {

            int index = Random.Range(0, platforms.GetLength(0));
            Vector3 pos = new Vector3(transform.position.x, Random.Range(game.stageBottom + 10, game.stageTop - 20), 2);


            GameObject platformPrefab = platforms[index];


            //An uparxoun coins stin perioxh dipla, min kaneis spawn kai epestrepse false

            Vector2 pointA = new Vector2(pos.x - platformPrefab.collider2D.bounds.extents.x - coinSpawnMargin, game.stageBottom);
            Vector2 pointB = new Vector2(pos.x + platformPrefab.collider2D.bounds.extents.x + coinSpawnMargin, game.stageTop);
            if (Physics2D.OverlapArea(pointA, pointB, coinsLayermask) != null)
            {
                Debug.Log("There are coins colliding");
                return false;
            }
            

            GameObject plat = (GameObject)Instantiate(platforms[index], pos, Quaternion.identity);


            float p = Random.value;

            //Check if anything will be spawned on the platform
            if (p > spawnNothingChance)
            {
                //Check for coins
                if (p - spawnNothingChance < spawnCoinsChance)
                {
                    coinsGenerator.generateCoinsOnPlatform(plat.transform);
                }
                //Check for powerup
                else if (p - spawnNothingChance - spawnCoinsChance < spawnPowerupChance)
                {
                    spawnPowerup(plat);
                }
                //check for crates
                else
                {
                    spawnCrates(plat);
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


    private void spawnPowerup(GameObject platform)
    {
        GameObject power;
        int index = Random.Range(0, powerUps.Length);
		
		power = (GameObject)Instantiate(powerUps[index]);


        power.transform.position = new Vector3(platform.transform.position.x, platform.transform.position.y + 4, 0);
        power.transform.parent = platform.transform;
    }


    private void spawnCrates(GameObject platform)
    {


        int noCrates = Random.Range(minCrates, maxCrates);

        float crateWidth = cratePrefab.renderer.bounds.size.x;

        float crateHeight = cratePrefab.renderer.bounds.size.y;


        float gap = Random.Range(0, crateWidth * 2);


        Vector2 platformCenterTop = new Vector2(platform.collider2D.bounds.center.x, platform.collider2D.bounds.max.y);


        float totalWidth = noCrates * crateWidth + (noCrates - 1) * crateWidth;

        float xMin = platformCenterTop.x - totalWidth / 2 + crateWidth / 2;

        float x = xMin;

        float y = platformCenterTop.y + crateHeight / 2;

        for (int i = 0; i < noCrates; i++)
        {
            GameObject crate = (GameObject)Instantiate(cratePrefab, new Vector3(x, y, 0), Quaternion.identity);
            crate.transform.parent = platform.transform;
            x += crateWidth + gap;
        }

    }


	void SpawnStartingPowerUps() {
		Transform origin = GameObject.Find("Starting Powerups").transform;

		float y;
		for(int i=0 ; i<startPowerupNumber ; i++) {
			y = origin.position.y + (i-(startPowerupNumber-1)/2f) * powerupDist/2;

			Instantiate(powerUps[Random.Range(0,powerUps.Length)],new Vector3(origin.position.x, y, origin.position.z),Quaternion.identity);

		}

	}


    public void deactivate()
    {
        activeGenerator = false;
    }

    public void activate()
    {

        activeGenerator = true;
        lastPlatformTime = Time.time;
        nextPlatformTime = Random.Range(-platformVar, platformVar) + platformFreq;
    }


}
