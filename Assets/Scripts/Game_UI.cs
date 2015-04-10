using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Game_UI : MonoBehaviour {

	public Game gameScript;
	public Animator uiAnimator;
	public Text scoreText;
	public Text coinScore;
	public Text distanceScore;
	public Text totalScore;
	public Text highScore;
	public Text newHighScore;
	public Button pauseButton;
	public Button playAgainButton;
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


	// Use this for initialization
	void Start () {
		if(Application.isEditor)
			uiAnimator.SetTrigger("start");

		countScore = false;
		scoreCounter = 0;
	}
	
	// Update is called once per frame
	void Update () {
		scoreText.text =((int)gameScript.score.GetTotalScore()).ToString();
		coinCountText.text = gameScript.score.GetCoins().ToString();
		
		if(countScore) {

			if(gameScript.save.isSoundOn() && !audio.isPlaying) {
				audio.clip = scoreAudio;
				audio.Play();

			}
			
			float delta = (scoreSpeed * Time.deltaTime)/scoreTime; 
			
			scoreCounter = Mathf.MoveTowards(scoreCounter,Mathf.Floor(gameScript.score.GetTotalScore()), delta );
			
			int s = (int)(scoreCounter);
			
			totalScore.text = s.ToString();
			
			if(s == Mathf.Floor(gameScript.score.GetTotalScore())) {
				audio.Stop();
				countScore = false;


				
				if(s > gameScript.save.GetHighscore()) 
					ShowNewHighscore();
				else
					playAgainButton.interactable = true;
			}
		}

		if(tapHighscore) {
			if(Input.GetMouseButton(0)) {
				uiAnimator.SetBool("highscore",false);
				audio.Stop();
				tapHighscore = false;
			
				playAgainButton.interactable = true;
				highScore.text = ((int)gameScript.score.GetTotalScore()).ToString();
			}

		}
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

		playAgainButton.interactable = true;
		
		countScore = true;
	}

	void ShowNewHighscore() {

		playAgainButton.interactable = false;

		newHighScore.text = ((int)gameScript.score.GetTotalScore()).ToString();

		uiAnimator.SetBool("highscore",true);

		if(gameScript.save.isSoundOn()) {
			audio.clip = hooray;
			audio.Play();
		}

		gameScript.save.SetHighscore((int)gameScript.score.GetTotalScore());

		tapHighscore = true;
	}

	public void ButtonRestart() {

		if(countScore) {
			scoreCounter = (int)gameScript.score.GetTotalScore();
			return;
		}
		uiAnimator.SetTrigger("restart");
		StartCoroutine(gameScript.WaitAndRestart(1f));
	}

	public void ButtonPause() {

		Time.timeScale = 0;
		pauseButton.interactable = false;
		uiAnimator.SetBool("pause",true);

	}

	public void ButtonResume() {
		uiAnimator.SetBool("pause", false);
		//StartCoroutine(Resume());
		Time.timeScale = 1;
		pauseButton.interactable = true;
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
}
