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

	public GameObject swingGuy;

	public int maxGuys;

	private Queue guyQueue;

	// Use this for initialization
	void Start () {
		save = GameObject.Find("_SAVE").GetComponent<Save>();

		Debug.Log("MUSCIO: " + save.isMusicOn());
		Debug.Log("SUONDO: " + save.isSoundOn());

		musicToggle.isOn = save.isMusicOn();
		soundsToggle.isOn = save.isSoundOn();

		guyQueue = new Queue();

		guyQueue.Enqueue(Instantiate(swingGuy));
	
	}
	
	// Update is called once per frame
	void Update () {

		if(Input.GetMouseButtonDown(0)) {

			if(guyQueue.Count > maxGuys)
				Destroy((GameObject) guyQueue.Dequeue());

			guyQueue.Enqueue(Instantiate(swingGuy));

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
