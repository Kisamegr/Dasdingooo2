using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Animator menuAnimator;
	public Animator settingsAnimator;
	public Button[] buttons;
	public Collider2D menuCollider;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonPlay() {
		menuAnimator.SetTrigger("play");
		menuCollider.enabled = false;

		foreach(Button b in buttons) {
			b.interactable = false;
		}

		StartCoroutine(LoadGameLevel(1.5f));
	}


	public void ButtonSettings() {
		settingsAnimator.SetTrigger("enter");
		menuAnimator.SetBool("settings",true);
		menuCollider.enabled = false;
	}

	public void ButtonHowTo() {

	}

	public void ButtonBackSettings() {
		settingsAnimator.SetTrigger("leave");
		menuAnimator.SetBool("settings",false);
		menuCollider.enabled = true;
	}


	IEnumerator LoadGameLevel(float delay) {
		yield return new WaitForSeconds(delay);
		Application.LoadLevel("Main");

	}
}
