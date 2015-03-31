using UnityEngine;
using System.Collections;

public class EnemiesPower : Powerup {

	public float enemyPenalty;


	protected override void Power ()
	{
		Debug.Log("ADSDASDASDSA");
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
                    en.Kill();
                }

            }
        }



		GameObject.Find("_GAME").GetComponent<Game>().AddNextEnemyTime(enemyPenalty);

		Destroy(gameObject);
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
