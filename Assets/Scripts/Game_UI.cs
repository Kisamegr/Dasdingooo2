using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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

	public AudioClip scoreAudio;
	public AudioClip hooray;
	
	private float scoreCounter;
	private float scoreSpeed;
	public float scoreTime;

	private bool countScore;
	private bool tapHighscore;

	private float fDelta;

	private bool tapped;


	// Use this for initialization
	void Start () {

		//gameScript.save.SetHighscore(0);
		if(Application.isEditor)
			uiAnimator.SetTrigger("start");

		countScore = false;
		scoreCounter = 0;

		tapped = false;
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
				else
					scoreScreenGroup.interactable = true;
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
	}

	public IEnumerator GameOverScreen() {
		coinScore.text = ((int)gameScript.score.GetCoinScore()).ToString();
		distanceScore.text = ((int)gameScript.score.GetDistanceScore()).ToString();
		totalScore.text = "0";
		
		highScore.text = gameScript.save.GetHighscore().ToString();
		
		Debug.Log("TOTAL: " + gameScript.score.GetTotalScore()); 

		Destroy(GameObject.Find("Canvas").transform.FindChild("PowerSliderPos").gameObject);
		
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
}
