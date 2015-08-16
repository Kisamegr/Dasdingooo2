using UnityEngine;
using System.Collections;
using System;

public class Save : MonoBehaviour {

	private bool musicOn;
	private bool soundsOn;
	private int highScore;

	public float totalDistance;
	public int totalCoins;
	public int totalPowerups;
	public int totalJumps;
	public int totalDeaths;
	public float totalTime;

	public float lastSaveTime;


	/*void Awake() {
		//DontDestroyOnLoad(transform.gameObject);
	}*/

	void Start() {

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

		StartCoroutine(InitStats());

	}

	public IEnumerator InitStats() {
		
		if(!PlayerPrefs.HasKey("total_distance"))
			PlayerPrefs.SetFloat("total_distance",0);
		
		if(!PlayerPrefs.HasKey("total_coins"))
			PlayerPrefs.SetInt("total_coins",0);
		
		if(!PlayerPrefs.HasKey("total_powerups"))
			PlayerPrefs.SetInt("total_powerups",0);
		
		if(!PlayerPrefs.HasKey("total_jumps"))
			PlayerPrefs.SetInt("total_jumps",0);
		
		if(!PlayerPrefs.HasKey("total_deaths"))
			PlayerPrefs.SetInt("total_deaths",0);

		if(!PlayerPrefs.HasKey("total_time"))
			PlayerPrefs.SetInt("total_time",0);

		totalDistance = PlayerPrefs.GetFloat("total_distance");
		totalCoins = PlayerPrefs.GetInt("total_coins");
		totalPowerups = PlayerPrefs.GetInt("total_powerups");
		totalJumps = PlayerPrefs.GetInt("total_jumps");
		totalDeaths = PlayerPrefs.GetInt("total_deaths");
		totalTime = PlayerPrefs.GetFloat("total_time");

		lastSaveTime = Time.realtimeSinceStartup;

		yield break;
	}

	public IEnumerator SaveStats() {
		PlayerPrefs.SetFloat("total_distance",totalDistance);
		PlayerPrefs.SetInt("total_coins",totalCoins);
		PlayerPrefs.SetInt("total_powerups",totalPowerups);
		PlayerPrefs.SetInt("total_jumps",totalJumps);
		PlayerPrefs.SetInt("total_deaths",totalDeaths);

		totalTime+= Time.realtimeSinceStartup - lastSaveTime;
		PlayerPrefs.SetFloat("total_time",totalTime);

		lastSaveTime = Time.realtimeSinceStartup;

		yield break;
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

	/*private float totalDistance;
	private int totalCoins;
	private int totalPowerups;
	private int totalJumps;
	private int totalDeaths;
	private float totalTime;

	public float getTotalDistance() {
		return totalDistance;
	}
	public int getTotalCoins() {
		return totalCoins;
	}
	public int getTotalPowerups() {
		return totalPowerups;
	}

	public int getTotalJumps() {
		return totalJumps;
	}
	public int getTotalDeaths() {
		return totalDeaths;
	}*/
	 
}
