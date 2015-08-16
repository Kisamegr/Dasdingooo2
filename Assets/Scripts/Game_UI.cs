using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.Advertisements;
using UnityEngine.Analytics;
using System;

public class Game_UI : MonoBehaviour {

	public Game gameScript;
	public Animator uiAnimator;
	public CanvasGroup touchControlsGroup;
	public Text scoreText;
	public Text coinScore;
	public Text distanceScore;
	public Text totalScore;
	public Text highScore;
	public Text newHighScore;
	public Button pauseButton;
	public CanvasGroup scoreScreenGroup;
	public Text coinCountText;
	public Slider cannonSlider;
	public Text lifeTimerText;
	public Button lifeTimerButton;

	public AudioClip scoreAudio;
	public AudioClip hooray;
	
	private float scoreCounter;
	private float scoreSpeed;
	public float scoreTime;

	private bool countScore;
	private bool tapHighscore;

	private float fDelta;

	private bool tapped;

	private bool adsEnabled;
	private float adp = 0.1f;

	private float lifeTimerHours = 1;

	private  DateTime lifeNextDate;

	private bool updateLifeTimer;


	// Use this for initialization
	void Start () {

		StartCoroutine(StartAds());
		
		//gameScript.save.SetHighscore(0);
		if(Application.isEditor)
			uiAnimator.SetTrigger("start");

		countScore = false;
		scoreCounter = 0;

		tapped = false;
		updateLifeTimer = false;



		if(!PlayerPrefs.HasKey("lifeNextTime"))
			PlayerPrefs.SetString("lifeNextTime", System.DateTime.MinValue.ToBinary().ToString());

		//PlayerPrefs.SetString("lifeNextTime", System.DateTime.Now.ToBinary().ToString());

	
	}
	
	// Update is called once per frame
	void Update () {

		scoreText.text =((int)gameScript.score.GetDistanceScore()).ToString();
		coinCountText.text = gameScript.score.GetCoins().ToString();
		
		if(countScore) {

			if(gameScript.save.isSoundOn() && !GetComponent<AudioSource>().isPlaying) {
				GetComponent<AudioSource>().clip = scoreAudio;
				GetComponent<AudioSource>().Play();

			}
			
			float delta = (scoreSpeed * Time.deltaTime)/scoreTime; 
			
			scoreCounter = Mathf.MoveTowards(scoreCounter,Mathf.Floor(gameScript.score.GetTotalScore()), delta );
			
			int s = (int)(scoreCounter);
			
			totalScore.text = s.ToString();
			
			if(s == Mathf.Floor(gameScript.score.GetTotalScore())) {
				GetComponent<AudioSource>().Stop();
				countScore = false;


				
				if(s > gameScript.save.GetHighscore()) 
					ShowNewHighscore();
				else {
					scoreScreenGroup.interactable = true;

					ShowAd(true);
				}


			}
		}

		if(tapHighscore) {
			if( tapped) {
				uiAnimator.SetBool("highscore",false);
				GetComponent<AudioSource>().Stop();
				tapHighscore = false;
			
				scoreScreenGroup.interactable = true;
				highScore.text = ((int)gameScript.score.GetTotalScore()).ToString();



			}

		}

		/*if(!gameScript.gameRunning) {
			if(Input.GetButtonDown("Space"))
				ButtonRestart();
		}*/

		if(updateLifeTimer) {
			DateTime currentDate = System.DateTime.Now;
			TimeSpan diff = currentDate.Subtract(lifeNextDate);

			int seconds = Mathf.Abs((int)diff.Seconds);
			int minutes = Mathf.Abs((int) diff.Minutes);
			string szero = "";
			string mzero = "";

			if(minutes < 10)
				mzero="0";
			if(seconds < 10)
				szero="0";

			lifeTimerText.text = mzero + minutes + ":" + szero + seconds;
		


			if(diff.TotalSeconds > 0) { 
				Debug.Log("rerereererer");

				lifeTimerButton.interactable = true;
				updateLifeTimer = false;
			}


		}
	}

	public IEnumerator GameOverScreen() {
		//coinScore.text = ((int)gameScript.score.GetCoinScore()).ToString();
		coinScore.text = "#" +  ((int)gameScript.score.GetCoins()).ToString();

		distanceScore.text = ((int)gameScript.score.GetDistanceScore()).ToString() + "m";
		totalScore.text = "0";
		
		highScore.text = gameScript.save.GetHighscore().ToString();
		
		Debug.Log("TOTAL: " + gameScript.score.GetTotalScore()); 

		Destroy(GameObject.Find("Canvas").transform.FindChild("PowerSliderPos").gameObject);

		SetLifeButton();

		gameScript.score.SendScoreAnalytics();

		// Wait the broken player stuff
		yield return new WaitForSeconds(1f);
		
		// Bring the UI
		uiAnimator.SetTrigger("gameover");
		
		// Wait the menu
		yield return new WaitForSeconds(1.5f);
		
		scoreSpeed = gameScript.score.GetTotalScore() / scoreTime;

		scoreScreenGroup.interactable = true;
		
		countScore = true;
	}

	void SetLifeButton() {
	
		if(!Advertisement.IsReady()) {
			lifeTimerButton.interactable = false;
			lifeTimerText.text = "N/A";
			adsEnabled = false;
			return;
		}

		DateTime currentDate = System.DateTime.Now;
		
		//Grab the old time from the player prefs as a long
		long lastTime = Convert.ToInt64(PlayerPrefs.GetString("lifeNextTime"));
		
		//Convert the old time from binary to a DataTime variable
		lifeNextDate = DateTime.FromBinary(lastTime);
		
		//Use the Subtract method and store the result as a timespan variable
		TimeSpan difference = currentDate.Subtract(lifeNextDate);

		Debug.Log("DIFF: " + difference.TotalHours);

		if(difference.TotalHours > 0 ) {
			lifeTimerButton.interactable = true;
		}
		else {

			lifeTimerButton.interactable = false;
			updateLifeTimer = true;
		}

	}

	void ShowNewHighscore() {

		scoreScreenGroup.interactable = false;

		newHighScore.text = ((int)gameScript.score.GetTotalScore()).ToString();

		uiAnimator.SetBool("highscore",true);

		if(gameScript.save.isSoundOn()) {
			GetComponent<AudioSource>().clip = hooray;
			GetComponent<AudioSource>().Play();
		}

		gameScript.save.SetHighscore((int)gameScript.score.GetTotalScore());

		tapHighscore = true;
		tapped = false;
	}

	public void ButtonRestart() {

		if(countScore) {
			scoreCounter = (int)gameScript.score.GetTotalScore();

			return;
		}

		if(Advertisement.isShowing)
			return;

		uiAnimator.SetTrigger("exit");
		StartCoroutine(gameScript.WaitAndRestart(1.2f));
	}

	public void ButtonMenuEnd() {
		if(countScore) {
			scoreCounter = (int)gameScript.score.GetTotalScore();
			return;
		}

		uiAnimator.SetTrigger("exit");
		StartCoroutine(LoadMainMenu(1.2f));
	}

	public void ButtonPause() {

		Time.timeScale = 0;
		pauseButton.interactable = false;
		uiAnimator.SetBool("pause",true);

		touchControlsGroup.interactable = false;


	}

	public void ButtonResume() {
		uiAnimator.SetBool("pause", false);
		//StartCoroutine(Resume());
		Time.timeScale = 1;
		pauseButton.interactable = true;

		touchControlsGroup.interactable = true;

	}

	public void ButtonMenu() {
		Time.timeScale = 1;
		Application.LoadLevel(0);
	}

	public void ButtonQuit() {
		Application.Quit();
	}

	public void ButtonLife() {
		uiAnimator.SetBool("adpanel",true);
	}

	public void ButtonNoAD() {
		uiAnimator.SetBool("adpanel",false);
	}

	public void ButtonYesAD() {

		uiAnimator.SetBool("adpanel",false);
		lifeTimerButton.interactable = false;


		ShowRewardAd();

	}


	void OnLevelWasLoaded(int level) {
		uiAnimator.SetTrigger("start");
	}




	IEnumerator Resume() {
		yield return new WaitForSeconds(1f);
		Time.timeScale = 1;
		pauseButton.interactable = true;

	}

	IEnumerator LoadMainMenu(float delay) {
		yield return new WaitForSeconds(delay);
		Application.LoadLevel(0);
	}

	public void UserTapped() {
		tapped = true;
	}

	public bool ShowAd(bool chance = false) {

		if(!adsEnabled)
			return false;

		if(chance) {
			float r = UnityEngine.Random.value;

			if(r > adp)
				return false;
		}
		
		if (Advertisement.IsReady("defaultZone"))
		{
			ShowOptions options = new ShowOptions();
			options.resultCallback = results => {
				switch (results)
				{
				case(ShowResult.Failed):
					//AdDetails.text = "Ad failed!";
					break;
					
				case(ShowResult.Finished):
					//if (AdType == 1) AdDetails.text = "You just earned a gem!";
					//else AdDetails.text = "Thanks for watching the Ad";
					break;
					
				case(ShowResult.Skipped):
					//AdDetails.text = "Ad was skipped :(";
					break;
				}
			};

			
			Advertisement.Show("defaultZone",options);
			
			return true;
		}
		
		return false;
	}

	public bool ShowRewardAd ()
	{
		if(!adsEnabled)
			return false;

		if (Advertisement.IsReady("rewardedVideoZone"))
		{
			ShowOptions options = new ShowOptions();
			options.resultCallback = results => {
				switch (results)
				{
				case(ShowResult.Failed):
					//AdDetails.text = "Ad failed!";
					break;
					
				case(ShowResult.Finished):
					//if (AdType == 1) AdDetails.text = "You just earned a gem!";
					//else AdDetails.text = "Thanks for watching the Ad";



					break;
					
				case(ShowResult.Skipped):
					//AdDetails.text = "Ad was skipped :(";
					break;

				}
				PlayerPrefs.SetString("lifeNextTime", System.DateTime.Now.AddHours(lifeTimerHours).ToBinary().ToString());

				gameScript.score.SaveScore();
				uiAnimator.SetTrigger("exit");
				StartCoroutine(gameScript.WaitAndRestart(1.2f));
			};


			Advertisement.Show("rewardedVideoZone",options);

			return true;
		}

		return false;
	}

	IEnumerator StartAds() {
		if(Advertisement.isSupported ) {
			adsEnabled = true;
			Advertisement.Initialize ("53654", false);
		}
		else {
			lifeTimerButton.interactable = false;
			lifeTimerText.text = "N/A";
			adsEnabled = false;
			
		}
		yield break;
	}

	void OnApplicationQuit() {

		if(Time.realtimeSinceStartup > 30) {
			System.Collections.Generic.Dictionary<string,object> time = new System.Collections.Generic.Dictionary<string,object>();

			time.Add("timePlayed", Time.realtimeSinceStartup);

			Analytics.CustomEvent("userSession",time);

		}


	}


}
