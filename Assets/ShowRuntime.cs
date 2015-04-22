using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class ShowRuntime : MonoBehaviour {

	// Use this for initialization
	void Start () {
		GetComponent<Text>().text = Application.platform.ToString();
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
