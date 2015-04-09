using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Enemy : MonoBehaviour {

	public GameObject warningPrefab;
	protected Game gameScript;

	private RectTransform warningRect;
	private Image warningImage;

	private Camera cam;

	private float originalScale;
	private float minDist;
	private float xOffset;

	// Use this for initialization
	protected void Start () {

		gameScript = GameObject.Find("_GAME").GetComponent<Game>();
		cam = GameObject.Find("Main Camera").GetComponent<Camera>();

		if( warningPrefab != null){
			GameObject warning = (GameObject) Instantiate(warningPrefab);
			warning.transform.SetParent( GameObject.Find("Canvas").transform,false);

			warningRect = warning.GetComponent<RectTransform>();
			warningImage = warning.GetComponent<Image>();


			originalScale = warningRect.localScale.x;

			warningRect.position = new Vector3( Screen.width - 50, Mathf.Clamp(cam.WorldToScreenPoint(transform.position).y,5, Screen.height - 5),0); 
			warningImage.color = Color.white;
			minDist=500;
			xOffset = Screen.width /15;
		}
	}
	
	// Update is called once per frame
	protected void Update () {
	
		if(warningRect != null){

			Vector3 enemyPos = cam.WorldToScreenPoint(transform.position);
			float xDist = enemyPos.x - warningRect.position.x;
			warningRect.position = new Vector3( Screen.width - xOffset, Mathf.Clamp(enemyPos.y,5, Screen.height - 5),0); 


			if(xDist < minDist) {

				if(xDist < 10)
				{
					Destroy(warningRect.gameObject);
					warningRect = null;
					warningImage = null;

				}
				else
					warningRect.localScale = new Vector3(originalScale - (minDist - xDist)*originalScale/minDist, originalScale -(minDist - xDist)*originalScale/minDist, 1);
			}

		}
	}

	public abstract void Death();

	protected IEnumerator Kill(float delay) {

		yield return new WaitForSeconds(delay);

		if(warningRect!=null)
			Destroy(warningRect.gameObject);
		warningRect = null;
		warningImage = null;

		if(transform.parent != null)
			Destroy(transform.parent.gameObject);
		else
			Destroy(gameObject);

	}

}
