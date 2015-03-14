using UnityEngine;
using System.Collections;

public class RepeatTest : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        transform.localScale = transform.localScale + Vector3.up * Time.deltaTime;
        renderer.material.mainTextureScale = new Vector2(1.0f, transform.localScale.y);
	}
}
