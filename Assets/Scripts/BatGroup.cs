using UnityEngine;
using System.Collections;

public class BatGroup : MonoBehaviour {



    public GameObject batPrefab;

    public int numberOfBats;

    public float yMin;

    public float yMax;

    public float xRange;



    private Player playerScript;

    private int layerMask;

	// Use this for initialization
	void Start () {
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();
        layerMask = 1 << batPrefab.layer;


        for (int i = 0; i < numberOfBats; i++)
        {
            float yOffset = yMin + (yMax - yMin) * Random.value;
            float xOffset = Random.value * xRange;
            GameObject bat = (GameObject)Instantiate(batPrefab,new Vector3(transform.position.x + xOffset,yOffset,transform.position.z), Quaternion.identity);
            bat.transform.parent = transform;
            bat.GetComponent<Bat>().speed = new Vector2(-10, 0);
            
            float randomScale = 0.7f + 0.3f * Random.value;
            bat.transform.localScale = new Vector3(-randomScale, randomScale, 1);
        }

	}
	
	// Update is called once per frame
	void Update () {
        if (playerScript.hooked || playerScript.shotHook)
        {
            //Xalia tropos ginete kai kalutera pernontas mono to hook angle, prepei na valw kai to position pou ksekinaei to hook apo ton paikth
            Vector2 direction = new Vector2(playerScript.hook.transform.position.x - playerScript.transform.position.x, playerScript.hook.transform.position.y - playerScript.transform.position.y);
            float distance = Vector2.Distance(playerScript.transform.position,playerScript.hook.transform.position);
            RaycastHit2D hit = Physics2D.Raycast(playerScript.transform.position, direction,distance,layerMask);
            if (hit)
            {
                playerScript.cancelHook();
            }
        }
	}
}
