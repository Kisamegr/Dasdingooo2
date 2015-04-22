using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
	public Game_UI uiScript;
    public GameObject groundPrefab;
    public GameObject ceilingPrefab;


    public float stageTop = 18f;
    public float stageBottom = -21f;

    public float initialStageBottom;
    public float finalStageBottom;

    public float metersLimitDifficulty;
    private float normalizedDiffuculty;


    public float cameraHeight;

    private Vector3 startingPos;
	private Vector3 lastPosition;


    private float groundWidth;
    private float ceilingWidth;
    private Transform lastGround;
    private Transform lastCeiling;

    public Transform camTrans;
    public Transform player;
    private Transform cleaner;

    private Queue groundQueue;
    private Queue ceilingQueue;


    public int coinsCollected;


    private CoinsGenerator coinsGenerator;
    private EnemiesGenerator enemiesGenerator;
    private PlatformsGenerator platformsGenerator;


	public bool gameRunning;

	public Score score;
	public Save save;



    public AudioSource[] backgroundMusic;


    void Start()
    {
        stageBottom = initialStageBottom;

        Time.timeScale = 1f;
        groundQueue = new Queue();
        ceilingQueue = new Queue();

		score = new Score();


		GameObject s =  GameObject.Find("_SAVE");

		if(s == null) {
			s = new GameObject("_SAVE");
			s.AddComponent<Save>();
		}
	
		save = s.GetComponent<Save>();

        //camTrans =  GameObject.FindGameObjectWithTag("MainCamera").transform;
        //player = GameObject.FindGameObjectWithTag("Player").transform;
        cleaner = camTrans.FindChild("Cleaner");

        Vector3 cameraZero = camTrans.camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
        Vector3 cameraTop = camTrans.camera.ViewportToWorldPoint(new Vector3(0, 1, 0));
        cameraHeight = cameraTop.y - cameraZero.y;


        //Debug.Log(cameraTop);

        groundWidth = groundPrefab.transform.GetChild(0).renderer.bounds.size.x - 2;
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
		lastPosition = player.position;
        coinsGenerator = camTrans.FindChild("CoinsSpawner").GetComponent<CoinsGenerator>();
        enemiesGenerator = camTrans.FindChild("EnemiesSpawner").GetComponent<EnemiesGenerator>();
        platformsGenerator = camTrans.FindChild("PlatformsSpawner").GetComponent<PlatformsGenerator>();





    }




    void Update()
    {
        //float yCamera = Mathf.Clamp(player.position.y, yMin + cameraHeight / 2 - 1, yMax - cameraHeight / 2 + 1);
        //camTrans.position = new Vector3(player.position.x + 14, yCamera, camTrans.position.z);

        if (normalizedDiffuculty < 1)
        {
            normalizedDiffuculty = player.position.x / metersLimitDifficulty;
            if (normalizedDiffuculty > 1)
            {
                normalizedDiffuculty = 1;
            }

            stageBottom = initialStageBottom + normalizedDiffuculty * (finalStageBottom - initialStageBottom);
        }



    }



    void FixedUpdate()
    {

		
		UpdateDistanceScore();


        if (((Transform)groundQueue.Peek()).position.x < cleaner.position.x)
        {

            Transform ground = (Transform)groundQueue.Dequeue();
            groundQueue.Enqueue(ground);
            ground.position = new Vector3(lastGround.position.x + groundWidth, stageBottom, 0);

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




    public float NormalizedDiffuclty
    {
        get
        {
            return normalizedDiffuculty;
        }
    }

	public void GameOver() {
		platformsGenerator.deactivate();
		enemiesGenerator.deactivate();
		coinsGenerator.deactivate();

		StartCoroutine(uiScript.GameOverScreen());
	}





	public IEnumerator WaitAndRestart(float sec) {
		yield return new WaitForSeconds(sec);
		Application.LoadLevel(Application.loadedLevel);
	}




    public void AddNextEnemyTime(float enemyPenalty)
    {
        enemiesGenerator.AddNextEnemyTime(enemyPenalty);
    }


    public void activateSpawners()
    {
        coinsGenerator.activate();
        enemiesGenerator.activate();
        platformsGenerator.activate();
    }

    public void deactivateSpawners()
    {
        coinsGenerator.deactivate();
        enemiesGenerator.deactivate();
        platformsGenerator.deactivate();
    }



	private void UpdateDistanceScore() {
		float sc = (player.position.x - startingPos.x) / 2;
		score.SetDistanceScore(sc);
	}




    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(new Vector3(-100, stageTop, 0), new Vector3(100, stageTop, 0));

        Gizmos.color = Color.red;
        Gizmos.DrawLine(new Vector3(-100, initialStageBottom, 0), new Vector3(100, initialStageBottom, 0));

        Gizmos.DrawLine(new Vector3(-100, finalStageBottom, 0), new Vector3(100, finalStageBottom, 0));
    }

}
