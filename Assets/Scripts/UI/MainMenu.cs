using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;

public class MainMenu : MonoBehaviour {

	public Animator menuAnimator;
	public Animator settingsAnimator;
	public Animator howToAnimator;
	public CanvasGroup mainMenuGroup;
	public Text[] statsTexts;
	public Button htNextButton;
	public Button htPrevButton;
	public Image howToImage;
	public Sprite[] howToSprites;
	public Collider2D menuCollider;

	public Toggle musicToggle;
	public Toggle soundsToggle;

	public Save save;

	public GameObject swingGuy;

	public int maxGuys;

	private Queue guyQueue;

	private int howToIndex;

	private bool howTo;
	private bool settings;

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
		settings = false;

		if(!PlayerPrefs.HasKey("launch_date"))
			PlayerPrefs.SetString("launch_date",DateTime.Now.ToString());

	
	}
	
	// Update is called once per frame
	void Update () {

		if(UserClicked()) {

			if(guyQueue.Count > maxGuys)
				Destroy((GameObject) guyQueue.Dequeue());

			guyQueue.Enqueue(Instantiate(swingGuy));

		}

		if(settings) {
			setStatsTime();
		}

	}

	public void ButtonPlay() {
		menuAnimator.SetTrigger("exit");
		menuCollider.enabled = false;
		
		StartCoroutine(LoadGameLevel(1.5f));
	}


	public void ButtonSettings() {
		settingsAnimator.SetTrigger("enter");
		menuAnimator.SetBool("change",true);
		menuCollider.enabled = false;
		mainMenuGroup.interactable = false;

		settings = true;
		StartCoroutine(LoadStats());
	}

	public void ButtonHowTo() {
		howToAnimator.SetBool("show",true);
		menuAnimator.SetBool("change",true);
		menuCollider.enabled = false;
		mainMenuGroup.interactable = false;

		howToImage.sprite = howToSprites[0];
		htPrevButton.interactable = false;
		howToIndex = 0;

		howTo = true;
	}

	public void ButtonQuit() {
		menuAnimator.SetTrigger("exit");
		menuCollider.enabled = false;
		mainMenuGroup.interactable = false;

		StartCoroutine(Quit(1f));
	}

	public void ButtonBackSettings() {
		settingsAnimator.SetTrigger("leave");
		menuAnimator.SetBool("change",false);
		menuCollider.enabled = true;
		mainMenuGroup.interactable = true;
		
		settings = false;
		StartCoroutine(save.SaveStats());
	}

	public void ToggleMusic() {
		save.SetMusic(musicToggle.isOn);
	}

	public void ToggleSounds() {
		save.SetSounds(soundsToggle.isOn);
	}

	public void ButtonHowToBack() {
		menuAnimator.SetBool("change",false);
		howToAnimator.SetBool("show",false);
		mainMenuGroup.interactable = true;

		howTo = false;
	}

	public void ButtonHowToNext() {
		howToIndex++;

		howToImage.sprite = howToSprites[howToIndex];

		if(howToIndex == howToSprites.Length-1)
			htNextButton.interactable = false;

		htPrevButton.interactable = true;
	}

	public void ButtonHowToPrevious() {
		howToIndex--;
		
		howToImage.sprite = howToSprites[howToIndex];
		
		if(howToIndex == 0)
			htPrevButton.interactable = false;

		htNextButton.interactable = true;

	}




	IEnumerator LoadStats() {

		statsTexts[0].text = save.totalDistance + "m";
		statsTexts[1].text = save.totalCoins.ToString();
		statsTexts[2].text = save.totalPowerups.ToString();
		statsTexts[3].text = save.totalJumps.ToString();
		statsTexts[4].text = save.totalDeaths.ToString();

		yield break;
	}


	private void setStatsTime() {
		int totalTime = (int)save.totalTime + (int)Time.realtimeSinceStartup - (int)save.lastSaveTime;

		statsTexts[5].text = (totalTime / 3600).ToString();
		statsTexts[6].text = ((totalTime % 3600) / 60).ToString();
		statsTexts[7].text = ((totalTime % 3600) % 60).ToString();


	}


	IEnumerator LoadGameLevel(float delay) {
		yield return new WaitForSeconds(delay);
		Application.LoadLevel("Main");

	}

	IEnumerator Quit(float delay) {
		yield return new WaitForSeconds(delay);
		Application.Quit();

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
