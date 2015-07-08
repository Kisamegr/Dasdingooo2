using UnityEngine;
using System.Collections;

public class MyCamera : MonoBehaviour
{


    private GameObject playerGO;

    private Player playerScript;

    private Camera mainCamera;

    public float minSize;

    public float maxSize;

    public float sizeChangeSpeed;

    public float playerRelPosX;

	private Game game;

    // Use this for initialization
    void Start()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerGO.GetComponent<Player>();
        mainCamera = Camera.main;

		game = GameObject.Find("_GAME").GetComponent<Game>();



    }

    // Update is called once per frame
    void Update()
    {
		if(!game.gameRunning) 
			return;

        //Handle camera size
        float temp;
        if (playerGO.GetComponent<Rigidbody2D>().velocity.x < playerScript.minSpeed)
        {
            temp = 0;
        }
        else
        {
            temp = (playerGO.GetComponent<Rigidbody2D>().velocity.x ) / (playerScript.finalMaxSpeed );

        }
        float targetSize = minSize + temp * (maxSize - minSize);

        if (mainCamera.orthographicSize != targetSize)
        {
            if (mainCamera.orthographicSize < targetSize)
            {
                mainCamera.orthographicSize += sizeChangeSpeed * Time.deltaTime;
            }
            else
            {
                mainCamera.orthographicSize -= sizeChangeSpeed * Time.deltaTime;
            }
        }


        //Handle camera position
       // Vector3 cameraBottomLeft = camera.ViewportToWorldPoint(new Vector3(0, 0, 0));
       // Vector3 cameraTopRight = camera.ViewportToWorldPoint(new Vector3(1, 1, 0));
        //float cameraHeight =  cameraTopRight.y - cameraBottomLeft.y;
        //float cameraWidth =  cameraTopRight.x - cameraBottomLeft.x;


        float cameraHeight = 2f * GetComponent<Camera>().orthographicSize;
        float cameraWidth = cameraHeight * GetComponent<Camera>().aspect;

      
        float yCamera = Mathf.Clamp(playerGO.transform.position.y, game.stageBottom + cameraHeight / 2 + 2f, game.stageTop - cameraHeight / 2 + 1);
        float xCamera = playerGO.transform.position.x + (0.5f - playerRelPosX) * cameraWidth;





        GetComponent<Camera>().transform.position = new Vector3(xCamera, yCamera, GetComponent<Camera>().transform.position.z);
    }
}
