using UnityEngine;
using System.Collections;

public class EnemyGenerator : MonoBehaviour {
	public GameObject[] enemies;
	public float enemyFreq;
	public float enemyVar;
	private float lastEnemyTime;
	private float nextEnemyTime;

	// Use this for initialization
	void Start () {
		lastEnemyTime = -1;
	}
	
	// Update is called once per frame
	void Update () {


		if (lastEnemyTime >= 0 && Time.time - lastEnemyTime > nextEnemyTime)
		{
			int r = Random.Range(0,enemies.Length);
			
			Instantiate(enemies[r],transform.position,Quaternion.identity);
			nextEnemyTime = enemyFreq + Random.Range(-enemyVar, enemyVar);
			lastEnemyTime = Time.time;
		}
	}


	public void StartGenerating() {
		lastEnemyTime = Time.time;
		nextEnemyTime = enemyFreq + Random.Range(-enemyVar, enemyVar);
	}

	public void AddNextEnemyTime(float sec) {
		nextEnemyTime += sec;
	}
}
