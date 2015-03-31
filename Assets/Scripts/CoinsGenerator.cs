using UnityEngine;
using System.Collections;

public class CoinsGenerator : MonoBehaviour
{


    public GameObject coinPrefab;

    public float coinsMinTime;

    public float coinsMaxTime;

    public int minCoins;

    public int maxCoins;

    public float playerPositionVar;

   

    private float lastCoinTime;

    private float nextCoinTime;

    private Transform playerTrans;

    private Game game;

    private int platformsLayermask;

    private float stageTop;

    private float stageBottom;

    public float platformCheckWidth;



    // Use this for initialization
    void Start()
    {
        playerTrans = GameObject.FindGameObjectWithTag("Player").transform;

        game = GameObject.FindGameObjectWithTag("GameController").GetComponent<Game>();

        stageBottom = game.stageBottom;

        stageTop = game.stageTop;

        lastCoinTime = Time.time;

        platformsLayermask = 1 << LayerMask.NameToLayer("Platform");
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time - lastCoinTime > nextCoinTime)
        {
            //An uparxei platform trigurw, ksanaelegkse se 0.5 secs
            Vector2 pointA = new Vector2(transform.position.x - platformCheckWidth/2, stageBottom);
            Vector2 pointB = new Vector2(transform.position.x + platformCheckWidth,stageTop);
            if(Physics2D.OverlapArea(pointA, pointB, platformsLayermask) != null){
                   Debug.Log("There is a platform");
                   nextCoinTime += 1f;
            }else
            {
                int noCoins = Random.Range(minCoins, maxCoins);

                generateCoins(noCoins);

                nextCoinTime = Random.Range(coinsMinTime, coinsMaxTime);

                lastCoinTime = Time.time;
            }
        }

    }



    public void generateCoins(int noCoins)
    {
        Vector2 coinSize = new Vector2(coinPrefab.renderer.bounds.size.x, coinPrefab.renderer.bounds.size.y);

        float gapWidth = Random.Range(coinSize.x / 2, coinSize.x);
        float gapHeight = Random.Range(coinSize.y / 4, coinSize.y / 2);

        float yIncrease;

        if (playerTrans.rigidbody2D.velocity.y > 0)
        {
            yIncrease = Random.Range(-coinSize.y / 4, 0);
        }
        else
        {
            yIncrease = Random.Range(0, coinSize.y / 4);
        }


        float yMin = playerTrans.position.y + Random.Range(-playerPositionVar, playerPositionVar);

        yMin = Mathf.Clamp(yMin, stageBottom + 10, stageTop - 10);

        


        //An einai gia zigzag (meta mporw na valw pi8anothtes)
        if (Mathf.Abs(yIncrease) < coinSize.y / 12.0f)
        {
            float zigzagMult = Random.Range(2.5f, 3.5f);
            float zigzagHeight = zigzagMult * coinSize.y;
            float zigzagBottom = yMin - zigzagHeight / 2;
            float zigzagTop = yMin + zigzagHeight / 2;
            generateCoinsZigZag(noCoins, transform.position.x, zigzagBottom, zigzagTop);
            Debug.Log("ZigZag");
        }
        else
        {
            if (yIncrease < 0)
            {
                yMin += yIncrease * noCoins/2;
            }
            generateCoinsRow(noCoins, transform.position.x, yMin, yIncrease);
            Debug.Log("Row");
        }
    }





    private void generateCoinsRow(int noCoins, float xMin, float yMin, float yIncrease)
    {
        Player player = playerTrans.GetComponent<Player>();

        Vector2 coinSize = new Vector2(coinPrefab.renderer.bounds.size.x, coinPrefab.renderer.bounds.size.y);

        float gapWidth = Random.Range(coinSize.x / 2, coinSize.x);
        float gapHeight = Random.Range(coinSize.y / 4, coinSize.y / 2);


        //Generate 4 to 2 rows of coins
        for (int noRows = 4; noRows >= 2; noRows--)
        {
            //An exei ftasei stis 2 grammes kai to plh8os twn coins einai perittos, kanton artio kai an to zigzag = true, kane generate 
            if (noRows == 2 && noCoins % 2 != 0)
            {
                noCoins++;
            }

            if (noCoins % noRows == 0)
            {
                int noColumns = noCoins / noRows;

                for (int row = 0; row < noRows; row++)
                {
                    float y = yMin + row * (coinSize.y + gapHeight);
                    float x = xMin;
                    for (int column = 0; column < noColumns; column++)
                    {
                        GameObject coinGO = (GameObject)Instantiate(coinPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        Coin coin = coinGO.GetComponent<Coin>();
                        coin.game = game;
                        coin.player = player;


                        x += coinSize.x + gapWidth;
                        y += yIncrease;

                    }
                }
                return;
            }
        }
    }






    private void generateCoinsZigZag(int noCoins, float xMin, float zigzagBottom, float zigzagTop)
    {
        Player player = playerTrans.GetComponent<Player>();



        Vector2 coinSize = new Vector2(coinPrefab.renderer.bounds.size.x, coinPrefab.renderer.bounds.size.y);

        float gapWidth = Random.Range(coinSize.x / 2, coinSize.x);
        float gapHeight = Random.Range(coinSize.y / 4, coinSize.y / 2);


        int noRows = 2;

        //An exei ftasei stis 2 grammes kai to plh8os twn coins einai perittos, kanton artio kai an to zigzag = true, kane generate 
        if (noCoins % 2 != 0)
        {
            noCoins++;
        }

        int noColumns = noCoins / noRows;
        float x = xMin;

        int quartileSize = noColumns / 4;

        //Oi korufes tou zigzag
        int[] peaks = new int[5];

        peaks[0] = 0;
        for (int i = 1; i < peaks.Length; i++)
        {
            peaks[i] = peaks[i - 1] + quartileSize;
        }
        int currentQuartile = 0;

        float yMin;

        float zigzagHeight = zigzagTop - zigzagBottom;

        //sthlh sthlh  
        for (int column = 0; column < noColumns; column++)
        {
            //An se artio tetarthmorio, tote auksousa 
            if (currentQuartile % 2 == 0)
            {
                yMin = zigzagBottom + zigzagHeight * ((column - peaks[currentQuartile]) / (quartileSize * 1.0f));
            }
            else
            {
                yMin = zigzagTop - zigzagHeight * ((column - peaks[currentQuartile]) / (quartileSize * 1.0f));
            }


           

            //Generate auth th sthlh apo coins
            for (int row = 0; row < noRows; row++)
            {
                float y = yMin + row * (gapHeight + coinSize.y);
                GameObject coinGO = (GameObject)Instantiate(coinPrefab, new Vector3(x, y, 0), Quaternion.identity);
                Coin coin = coinGO.GetComponent<Coin>();
                coin.game = game;
                coin.player = player;
            }
            x += coinSize.x + gapWidth;


            //An ftasame stin epomenh korufh, dld trexousa sthlh pollaplasio tou mege8ous tou tetarthmoriou, auksise to trexwn tetarthmorio
            if ((column + 1) % quartileSize == 0)
            {
                currentQuartile++;
            }

        }


    }


    public void generateCoinsOnPlatform(Transform platform)
    {
        Player player = playerTrans.GetComponent<Player>();

        int noCoins = Random.Range(minCoins, maxCoins);
        noCoins = (int)(0.75f * noCoins);

        Vector2 coinSize = new Vector2(coinPrefab.renderer.bounds.size.x, coinPrefab.renderer.bounds.size.y);

        float gapWidth = Random.Range(coinSize.x / 2, coinSize.x);
        float gapHeight = Random.Range(coinSize.y / 4, coinSize.y / 2);

        Vector2 platformCenterTop = new Vector2(platform.collider2D.bounds.center.x, platform.collider2D.bounds.max.y);

        float xMin = platformCenterTop.x;
        float yMin = platformCenterTop.y + coinSize.y/2;
        float yIncrease = 0;

        //Generate 4 to 2 rows of coins
        for (int noRows = 3; noRows >= 2; noRows--)
        {
            //An exei ftasei stis 2 grammes kai to plh8os twn coins einai perittos, kanton artio kai an to zigzag = true, kane generate 
            if (noRows == 2 && noCoins % 2 != 0)
            {
                noCoins++;
            }

            if (noCoins % noRows == 0)
            {

                int noColumns = noCoins / noRows;

                if(noColumns % 2 == 0){
                    xMin = platformCenterTop.x - noColumns * coinSize.x / 2 - (noColumns / 2 - 0.5f) * gapWidth;
                }else{
                    xMin = platformCenterTop.x - noColumns * coinSize.x / 2 - (noColumns - 1) * gapWidth / 2;
                }


                for (int row = 0; row < noRows; row++)
                {
                    float y = yMin + row * (coinSize.y + gapHeight);
                    float x = xMin;
                    for (int column = 0; column < noColumns; column++)
                    {
                        GameObject coinGO = (GameObject)Instantiate(coinPrefab, new Vector3(x, y, 0), Quaternion.identity);
                        Coin coin = coinGO.GetComponent<Coin>();
                        coin.game = game;
                        coin.player = player;

                        coinGO.transform.parent = platform;
                        x += coinSize.x + gapWidth;
                        y += yIncrease;

                    }
                }
                return;
            }
        }
    }


    void OnDrawGizmosSelected()
    {


        Vector3 topLeft = new Vector3(transform.position.x - platformCheckWidth/2, 40, 0);
     
        Vector3 topRight = new Vector3(transform.position.x + platformCheckWidth,40,0);

        Vector3 bottomLeft = new Vector3(transform.position.x - platformCheckWidth/2, -40, 0);
     
        Vector3 bottomRight = new Vector3(transform.position.x + platformCheckWidth,-40,0);
     
        Gizmos.color = Color.green;

        Gizmos.DrawLine(topRight, bottomRight);
        Gizmos.DrawLine(bottomLeft, topLeft);

    }

}
