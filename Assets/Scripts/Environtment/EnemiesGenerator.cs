using UnityEngine;
using System.Collections;

public class EnemiesGenerator : MonoBehaviour {


    public GameObject[] enemies;

    private float enemyFreq;
    private float enemyVar;

    public float initialEnemyFreq;
    public float initialEnemyVar;

    public float finalEnemyFreq;
    public float finalEnemyVar;

    private float lastEnemyTime;
    private float nextEnemyTime;

    private bool activeGenerator = false;

    private Game game;

	// Use this for initialization
	void Start () {
        game = GameObject.Find("_GAME").GetComponent<Game>();
        enemyFreq = initialEnemyFreq;
        enemyVar = initialEnemyVar;
        nextEnemyTime = 6f;
        lastEnemyTime = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

        if (!activeGenerator) return;

        if (game.NormalizedDiffuclty < 1)
        {
             enemyFreq = initialEnemyFreq + game.NormalizedDiffuclty*(finalEnemyFreq - initialEnemyFreq);
             enemyVar = initialEnemyVar + game.NormalizedDiffuclty*(finalEnemyVar - initialEnemyVar);
        }



        if (Time.time - lastEnemyTime > nextEnemyTime)
        {
            int r = Random.Range(0, enemies.Length);

            Instantiate(enemies[r], transform.position, Quaternion.identity);

            nextEnemyTime = enemyFreq + Random.Range(-enemyVar, enemyVar);
            lastEnemyTime = Time.time;
        }
	}


    public void AddNextEnemyTime(float sec)
    {
        nextEnemyTime += sec;
    }


    public void deactivate()
    {
        activeGenerator = false;
    }

    public void activate()
    {

        activeGenerator = true;
        lastEnemyTime = Time.time;
        nextEnemyTime = enemyFreq + Random.Range(-enemyVar, enemyVar);
    }


}
