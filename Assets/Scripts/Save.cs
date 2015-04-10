using UnityEngine;
using System.Collections;

public class Save : MonoBehaviour {

	private bool musicOn;
	private bool soundsOn;
	private int highScore;

	/*void Awake() {
		//DontDestroyOnLoad(transform.gameObject);
	}*/

	void Start() {
		Debug.Log("asdadasdsadsa");

		if(!PlayerPrefs.HasKey("music")) {
			PlayerPrefs.SetInt("music",1);
			musicOn = true;
		}
		else {
			int m = PlayerPrefs.GetInt("music");

			if(m==1)
				musicOn = true;
			else
				musicOn = false;
		}

		if(!PlayerPrefs.HasKey("sounds")) {
			PlayerPrefs.SetInt("sounds",1);
			soundsOn = true;
		}
		else {
			int s = PlayerPrefs.GetInt("sounds");
			
			if(s==1)
				soundsOn = true;
			else
				soundsOn = false;
		}

		if(!PlayerPrefs.HasKey("highscore")) {
			PlayerPrefs.SetInt("highscore",0);
			highScore = 0;
		}
		else {
			highScore = PlayerPrefs.GetInt("highscore");
		}

		Debug.Log(musicOn);
	}


	public void SetMusic(bool on) {
		if(on) 
			PlayerPrefs.SetInt("music",1);
		else
			PlayerPrefs.SetInt("music",0);

		musicOn = on;
	}

	public void SetSounds(bool on) {
		if(on) 
			PlayerPrefs.SetInt("sounds",1);
		else
			PlayerPrefs.SetInt("sounds",0);
		
		soundsOn = on;
	}

	public void SetHighscore(int high) {
		PlayerPrefs.SetInt("highscore",high);
		highScore = high;
	}

	public bool isMusicOn() {
		return musicOn;
	}

	public bool isSoundOn() {
		return soundsOn;
	}

	public int GetHighscore() {
		return highScore;
	}
}
