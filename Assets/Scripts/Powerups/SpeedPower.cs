using UnityEngine;
using System.Collections;

public class SpeedPower : Powerup {

	public float forcePower;
	public Color speedColor;

	private SpriteRenderer sprite;


	void Start() {
		base.Start();

		simul = true;

		sprite = player.transform.GetChild(0).GetComponent<SpriteRenderer>();


	}



	protected override void BeforePower() {
		sprite.color = Color.Lerp(sprite.color,speedColor,changeSpeed);
		
		if(sprite.color == speedColor)
			state = PowerState.Running;

	}

	protected override void AfterPowerEnded() {
		sprite.color = Color.Lerp(sprite.color,Color.white,changeSpeed);
		
		if(sprite.color == Color.white) 
			Destroy(gameObject);

	}


	protected override void Power ()
	{
		player.rigidbody2D.AddForce(Vector2.right *forcePower,ForceMode2D.Impulse);
	}
	
	protected override void PowerEnded ()
	{
		state = PowerState.Ended;
	}

	public override void Refresh ()
	{
		startTime = Time.time;
	}
}
