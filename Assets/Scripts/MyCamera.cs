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

    private float yMin;

    private float yMax;

    public float playerRelPosX;


    // Use this for initialization
    void Start()
    {
        playerGO = GameObject.FindGameObjectWithTag("Player");
        playerScript = playerGO.GetComponent<Player>();
        mainCamera = Camera.main;

        Game game = GameObject.Find("_GAME").GetComponent<Game>();

        yMin = game.yMin;
        yMax = game.yMax;

    }

    // Update is called once per frame
    void Update()
    {


        //Handle camera size
        float temp;
        if (playerGO.rigidbody2D.velocity.x < playerScript.minSpeed)
        {
            temp = 0;
        }
        else
        {
            temp = (playerGO.rigidbody2D.velocity.x - playerScript.minSpeed) / (playerScript.maxSpeed - playerScript.minSpeed);

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


        float cameraHeight = 2f * camera.orthographicSize;
        float cameraWidth = cameraHeight * camera.aspect;

      
        float yCamera = Mathf.Clamp(playerGO.transform.position.y, yMin + cameraHeight / 2 + 2f, yMax - cameraHeight / 2 + 1);
        float xCamera = playerGO.transform.position.x + (0.5f - playerRelPosX) * cameraWidth;


        camera.transform.position = new Vector3(xCamera, yCamera, camera.transform.position.z);
    }
}
