using UnityEngine;
using System.Collections;

public class MagnetPower : Powerup {

	void OnTriggerStay(Collider other) {
		Debug.Log("HALILOYA");
		
		if(other.tag == "Coin") {
			other.rigidbody2D.isKinematic = false;
			other.rigidbody2D.AddForce(transform.position - other.transform.position);
		}
	}
	
	protected override void Power ()
	{
		//Do nothing...
	}
	
	protected override void PowerEnded ()
	{
		//Do nothing...
	}
	public override void Refresh ()
	{
		//Do nothing...
	}
}
