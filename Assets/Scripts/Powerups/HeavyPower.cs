using UnityEngine;
using System.Collections;

public class HeavyPower : Powerup {

	public float massMultiplier=4;
	public Color powerColor;

	private float originalMass;
	private float originalGravity;

	private SpriteRenderer sprite;


	void Start() {
		base.Start();

		originalMass = player.rigidbody2D.mass;
		sprite = player.transform.GetChild(0).GetComponent<SpriteRenderer>();

		player.rigidbody2D.mass = originalMass*massMultiplier;
	}
	
	
	
	protected override void BeforePower() {
		sprite.color = Color.Lerp(sprite.color,powerColor,changeSpeed);
		
		
		if(sprite.color == powerColor)
			state = PowerState.Running;
		
	}
	
	protected override void AfterPowerEnded() {
		sprite.color = Color.Lerp(sprite.color,Color.white,changeSpeed);
		
		if(sprite.color == Color.white) 
			base.AfterPowerEnded();
		
		
		
	}

	
	
	protected override void Power ()
	{
		// Do nothing
	}
	
	protected override void PowerEnded ()
	{
		player.rigidbody2D.mass = originalMass;
		state = PowerState.Ended;
	}
	
	public override void Refresh ()
	{
		startTime = Time.time;
	}
}
