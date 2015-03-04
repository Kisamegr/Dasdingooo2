using UnityEngine;
using System.Collections;

public class ScalePower : Powerup {

	public float scaleFactor;
	public Color scaleColor;

	private Vector3 originalScale;
	private Vector3 targetScale;
	private SpriteRenderer sprite;



	// Use this for initialization
	void Start () {
		base.Start();

		originalScale = player.transform.localScale;
		targetScale = originalScale * scaleFactor;

		sprite = player.transform.GetChild(0).GetComponent<SpriteRenderer>();
			
	}
	

	protected override void AfterPowerEnded() {

		sprite.color = Color.Lerp(sprite.color,Color.white,changeSpeed);
		player.transform.localScale = Vector3.Lerp(player.transform.localScale, originalScale, changeSpeed);

		if(sprite.color == Color.white && Vector3.Distance(player.transform.localScale,originalScale) < 0.01f)
			Destroy(gameObject);
		
	}


	protected override void Power ()
	{
		sprite.color = Color.Lerp(sprite.color,scaleColor,changeSpeed);
		player.transform.localScale = Vector3.Lerp(player.transform.localScale, targetScale, changeSpeed);
	}

	protected override void PowerEnded ()
	{
		state = PowerState.Ended;
	}
}
