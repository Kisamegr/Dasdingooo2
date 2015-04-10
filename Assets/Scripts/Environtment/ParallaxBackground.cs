using UnityEngine;
using System.Collections;

public class ParallaxBackground : MonoBehaviour
{
    public GameObject[] layers;

    private GameObject[] secondLayers;

    public float[] constantSpeed;

    public float[] maxSpeed;

    private float[] speed;

    private Player player;

	private Save save;

    public float width;
    public float height;

    // Use this for initialization
    void Start()
    {
		GameObject p = GameObject.FindGameObjectWithTag("Player");

		if(p!=null)
        	player = p.GetComponent<Player>();
		else 
			player = null;

      
        secondLayers = new GameObject[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            secondLayers[i] = (GameObject)Instantiate(layers[i], layers[i].transform.position + new Vector3(width, 0, 0.1f), Quaternion.identity);
            secondLayers[i].transform.parent = transform;
        }
        speed = new float[layers.Length];

		save = GameObject.Find("_SAVE").GetComponent<Save>();



    }

    // Update is called once per frame
    void Update()
    {
		if(save.isMusicOn()) {
			if(!audio.isPlaying)
				audio.Play();
		} else
			if(audio.isPlaying)
				audio.Stop();

        float playerNormalizedSpeed = 0.5f;

		if(player != null) {
	        if (player.inCannon)
	        {
	            playerNormalizedSpeed = 0;
	        }
	        else
	        {
	            playerNormalizedSpeed =  player.rigidbody2D.velocity.x / player.maxSpeed;
				transform.position = new Vector3(player.transform.position.x + 10, transform.position.y, transform.position.z);
	        }


	        for (int i = 0; i < speed.Length; i++)
	        {
	            speed[i] = constantSpeed[i] + maxSpeed[i] * playerNormalizedSpeed;
	        }

		}




        for (int i = 0; i < layers.Length; i++)
        {
            layers[i].transform.Translate(-speed[i] * Time.deltaTime, 0, 0);
            secondLayers[i].transform.Translate(-speed[i] * Time.deltaTime, 0, 0);


            //layers[i].renderer.material.SetTextureOffset("_MainTex",layers[i].renderer.material.GetTextureOffset("_MainTex") + new Vector2(-speed[i] , 0));
             
            if (layers[i].transform.localPosition.x < -width)
            {
                layers[i].transform.position = secondLayers[i].transform.position + new Vector3(width, 0, 0);
            }
            if (secondLayers[i].transform.localPosition.x < -width)
            {
                secondLayers[i].transform.position = layers[i].transform.position + new Vector3(width, 0, 0);
            }
        }

    }


    public void OnDrawGizmosSelected()
    {

        if (width < 0)
        {
            width = 1;
        }

        if (height < 0)
        {
            height = 1;
        }

        for (int i = 0; i < layers.Length; i++)
        {
            Vector3 layerRendererSize = layers[i].transform.renderer.bounds.size;

            float xp = layers[i].transform.localScale.x / layerRendererSize.x;
            if (float.IsNaN(xp)) xp = 0.1f;

            float yp = layers[i].transform.localScale.y / layerRendererSize.y;
            if (float.IsNaN(yp)) yp = 0.1f;

            layers[i].transform.localScale = new Vector3(width * xp, height * yp, 1);

        }





        Vector3 leftDown = new Vector3(transform.position.x - width / 2, transform.position.y - height / 2);

        Vector3 leftUp = new Vector3(transform.position.x - width / 2, transform.position.y + height / 2);

        Vector3 RightDown = new Vector3(transform.position.x + width / 2, transform.position.y - height / 2);

        Vector3 RightUp = new Vector3(transform.position.x + width / 2, transform.position.y + height / 2);

        Gizmos.color = Color.white;
        Gizmos.DrawLine(leftDown, leftUp);
        Gizmos.DrawLine(leftUp, RightUp);
        Gizmos.DrawLine(RightUp, RightDown);
        Gizmos.DrawLine(RightDown, leftDown);


    }
}
