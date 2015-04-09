using UnityEngine;
using System.Collections;

public class EnemiesGenerator : MonoBehaviour {


    public GameObject[] enemies;

    public float enemyFreq;
    public float enemyVar;

    private float lastEnemyTime;
    private float nextEnemyTime;

    private bool activeGenerator = false;

	// Use this for initialization
	void Start () {
        nextEnemyTime = 6f;
        lastEnemyTime = Time.time;

	}
	
	// Update is called once per frame
	void Update () {

        if (!activeGenerator) return;


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
    }


}
