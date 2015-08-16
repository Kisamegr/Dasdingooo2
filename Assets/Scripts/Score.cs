using System.Collections;
using UnityEngine;
using UnityEngine.Analytics;

public class Score {

	private float distanceScore;
	private float coinScore;
	private float coinCounter;

	public float heightLost;
	public float lifeDistanceScore;

	public Score() {
		distanceScore = 0;
		coinScore = 0;
		coinCounter = 0;
		heightLost = 0;
		lifeDistanceScore = 0;
	}

	public void SaveScore() {
		PlayerPrefs.SetFloat("lifeDistanceScore", distanceScore);
		PlayerPrefs.SetFloat("lifeCoinsScore", coinScore);
		PlayerPrefs.SetFloat("lifeCoinCounter", coinCounter);
		PlayerPrefs.SetFloat("lifeHeightLost", heightLost);

		PlayerPrefs.SetInt("hasLife",1);
	}

	public void LoadScore() {
		lifeDistanceScore = PlayerPrefs.GetFloat("lifeDistanceScore");
		coinScore = PlayerPrefs.GetFloat("lifeCoinsScore");
		coinCounter = PlayerPrefs.GetFloat("lifeCoinCounter");
		heightLost = PlayerPrefs.GetFloat("lifeHeightLost");

		PlayerPrefs.SetInt("hasLife",0);

		Debug.Log(lifeDistanceScore);

	}

	public void SendScoreAnalytics() {

		System.Collections.Generic.Dictionary<string,object> stats = new System.Collections.Generic.Dictionary<string,object>();
		stats.Add("distanceScore",GetDistanceScore());
		stats.Add("coinNumber", GetCoins());
		stats.Add("coinScore", GetCoinScore());
		stats.Add("totalScore", GetTotalScore());

		AnalyticsResult result = Analytics.CustomEvent("userScore",stats);

		Debug.Log(result.ToString());
	}

	public void SetDistanceScore(float d) {
		distanceScore = d;
	}

	public void AddCoinScore(float c) {
		coinScore += c;
	}

	public void AddCoin() {
		coinCounter++;
	}

	public float GetDistanceScore() {
		return distanceScore + lifeDistanceScore;
	}

	public float GetCoinScore() {
		return coinScore;
	}

	public float GetCoins() {
		return coinCounter;
	}

	public float GetTotalScore() {
		return distanceScore + coinScore + lifeDistanceScore;
	}
}
