using System.Collections;

public class Score {

	private float distanceScore;
	private float coinScore;
	private float coinCounter;

	public Score() {
		distanceScore = 0;
		coinScore = 0;
		coinCounter = 0;
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
		return distanceScore;
	}

	public float GetCoinScore() {
		return coinScore;
	}

	public float GetCoins() {
		return coinCounter;
	}

	public float GetTotalScore() {
		return distanceScore + coinScore;
	}
}
