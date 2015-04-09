using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour {

	public Animator menuAnimator;
	public Animator settingsAnimator;
	public Button[] menuButtons;
	public Collider2D menuCollider;

	public Toggle musicToggle;
	public Toggle soundsToggle;

	public Save save;

	// Use this for initialization
	void Start () {
		save = GameObject.Find("_SAVE").GetComponent<Save>();

		musicToggle.isOn = save.isMusicOn();
		soundsToggle.isOn = save.isSoundOn();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void ButtonPlay() {
		menuAnimator.SetTrigger("play");
		menuCollider.enabled = false;

		foreach(Button b in menuButtons) {
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

	public void ToggleMusic() {
		save.SetMusic(musicToggle.isOn);
	}

	public void ToggleSounds() {
		save.SetSounds(soundsToggle.isOn);
	}

	IEnumerator LoadGameLevel(float delay) {
		yield return new WaitForSeconds(delay);
		Application.LoadLevel("Main");

	}
}
