using UnityEngine;
using System.Collections;

public class CoinsGenerator : MonoBehaviour {


    public int coinsFrequency;

    public int coinsLeft;

    public GameObject coinPrefab;

    private int lastCoinTime;


	// Use this for initialization
	void Start () {
        coinsLeft = coinsFrequency;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


    public void removeCoins(int noCoins)
    {
        coinsLeft -= noCoins;
    }


    public void generateCoinsRow(int noCoins,Transform parent)
    {
        GameObject coins = new GameObject("Coins");
        if (parent != null)
        {
            coins.transform.parent = parent;
        }
        if (noCoins % 2 == 0)
        {
            //
            for (int i = 0; i < noCoins / 2; i++)
            {
                GameObject coin = (GameObject)Instantiate(coinPrefab);
                coin.transform.parent = coins.transform;
            }
            for (int i = 0; i < noCoins / 2; i++)
            {

            }
        }
        else
        {

        }

    }

}
