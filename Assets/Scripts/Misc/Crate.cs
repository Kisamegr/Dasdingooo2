using UnityEngine;
using System.Collections;

public class Crate : MonoBehaviour
{


    public float destructionDuration;

    public float slowdownPercent;

    private float destroyTime;

    private bool destroyed;

    private SpriteRenderer spriteRenderer;

    public AudioClip stumbleSound;


    // Use this for initialization
    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        destroyed = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (destroyed)
        {
            if (Time.time - destroyTime > destructionDuration)
            {
                Destroy(gameObject);
            }
            else
            {
                float alpha = 1 - (Time.time - destroyTime) / destructionDuration;
                spriteRenderer.color = new Color(1f, 1f, 1f, alpha);
            }
        }

    }




    public bool isDestroyed()
    {
        return destroyed;
    }

    public void Destroy()
    {
        AudioSource.PlayClipAtPoint(stumbleSound,transform.position);
        destroyTime = Time.time;
        rigidbody2D.isKinematic = false;
        rigidbody2D.AddTorque(-100f);
        rigidbody2D.gravityScale = 1f;
        destroyed = true;
    }

}
