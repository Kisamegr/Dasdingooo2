using UnityEngine;
using System.Collections;

public class ScalePower : Powerup {

	public float scaleFactor;
	public Color scaleColor;

	private Vector3 originalScale;
	private Vector3 targetScale;
	private SpriteRenderer sprite;

	private Transform playerTrans;



	// Use this for initialization
	void Start () {
		base.Start();

		originalScale = player.transform.localScale;
		targetScale = originalScale * scaleFactor;

		sprite = player.transform.GetChild(0).GetComponent<SpriteRenderer>();

		playerTrans = player.transform;
			
	}

	protected override void BeforePower() {
		player.GetComponent<Rigidbody2D>().mass *= 1.5f;
		player.GetComponent<Rigidbody2D>().angularDrag *= 5;
		state = PowerState.Running;
		
	}

	protected override void AfterPowerEnded() {

		sprite.color = Color.Lerp(sprite.color,Color.white,changeSpeed);
		playerTrans.localScale = Vector3.Lerp(player.transform.localScale, originalScale, changeSpeed);

		if(sprite.color == Color.white && Vector3.Distance(player.transform.localScale,originalScale) < 0.01f)
			Destroy(gameObject);
		
	}


	protected override void Power ()
	{
		if(sprite.color != scaleColor && Vector3.Distance(player.transform.localScale,targetScale) > 0.01f) {
			sprite.color = Color.Lerp(sprite.color,scaleColor,changeSpeed);
			playerTrans.localScale = Vector3.Lerp(player.transform.localScale, targetScale, changeSpeed);
		}
	}

	protected override void PowerEnded ()
	{
		player.GetComponent<Rigidbody2D>().mass /= 1.5f;
		player.GetComponent<Rigidbody2D>().angularDrag /= 5;
		state = PowerState.Ended;
	}

	public override void Refresh ()
	{
		startTime = Time.time;
	}
}
