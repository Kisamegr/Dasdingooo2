using UnityEngine;
using System.Collections;

public class EnemiesPower : Powerup {

	public float enemyPenalty;

	void Start() {
		base.Start();
		

		GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
		
		if (enemies.Length > 0)
		{
			for (int i = 0; i < enemies.Length; i++)
			{
				Debug.Log(i + "  " + enemies[i].name);
				Enemy en = enemies[i].GetComponent<Enemy>();
				
				if (en == null)
				{
					en = enemies[i].GetComponentInChildren<Enemy>();
				}
				
				if (en != null)
				{
					en.Death();
				}
				
			}
		}
		
		
		
		GameObject.Find("_GAME").GetComponent<Game>().AddNextEnemyTime(enemyPenalty);
		

		
	}

	protected override void Power ()
	{
		// Do nothing
	}
	
	protected override void PowerEnded ()
	{
		state = PowerState.Ended;
	}
	
	public override void Refresh ()
	{
		// Do nothing
	}
}
