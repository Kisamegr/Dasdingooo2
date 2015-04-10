using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MainMenu : MonoBehaviour {

	public Animator menuAnimator;
	public Animator settingsAnimator;
	public Animator howToAnimator;
	public Button[] menuButtons;
	public Collider2D menuCollider;

	public Toggle musicToggle;
	public Toggle soundsToggle;

	public Save save;

	public GameObject swingGuy;

	public int maxGuys;

	private Queue guyQueue;

	bool howTo;

	// Use this for initialization
	void Start () {
		save = GameObject.Find("_SAVE").GetComponent<Save>();

		Debug.Log("MUSCIO: " + save.isMusicOn());
		Debug.Log("SUONDO: " + save.isSoundOn());

		musicToggle.isOn = save.isMusicOn();
		soundsToggle.isOn = save.isSoundOn();

		guyQueue = new Queue();

		guyQueue.Enqueue(Instantiate(swingGuy));

		howTo = false;
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0)) {

			if(guyQueue.Count > maxGuys)
				Destroy((GameObject) guyQueue.Dequeue());

			guyQueue.Enqueue(Instantiate(swingGuy));

		}

		if(howTo) {
			if(UserClicked()) {
				menuAnimator.SetBool("change",false);
				howToAnimator.SetBool("show",false);
				howTo = false;
			}

		}
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
		menuAnimator.SetBool("change",true);
		menuCollider.enabled = false;
	}

	public void ButtonHowTo() {
		howToAnimator.SetBool("show",true);
		menuAnimator.SetBool("change",true);
		menuCollider.enabled = false;

		howTo = true;
	}

	public void ButtonBackSettings() {
		settingsAnimator.SetTrigger("leave");
		menuAnimator.SetBool("change",false);
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

	private bool UserClicked() {
		
		if(Application.platform == RuntimePlatform.WindowsPlayer || Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.WindowsWebPlayer) {
			
			if(Input.GetKeyDown(KeyCode.Space))
				return true;
			
			
			if(Input.GetMouseButtonDown(0))
				return true;
			
		}
		
		if(Application.platform == RuntimePlatform.Android) {
			Touch t = new Touch ();
			try {
				t = Input.GetTouch (0);
				if (t.phase == TouchPhase.Began) {
					return true;
				}
				
			}
			catch (Exception e) {
				
			}
			
		}
		return false;
	}
}
