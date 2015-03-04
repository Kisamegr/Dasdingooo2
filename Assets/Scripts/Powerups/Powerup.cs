using UnityEngine;
using System.Collections;

public abstract class Powerup : MonoBehaviour {

	protected enum PowerState {
		Started, Running, Ended
	};

	public float duration;
	public float changeSpeed;
	public GameObject player;

	protected float startTime;
	protected PowerState state;
	protected bool simul=false;


	// Use this for initialization
	protected virtual void Start () {
		startTime = Time.time;
		state = PowerState.Started;
	}

	protected virtual void FixedUpdate() {



		if(state == PowerState.Started)
			BeforePower();	
	
		if(state != PowerState.Started || simul) {
			if(Time.time - startTime < duration)
				Power ();
			else if(state == PowerState.Running)
				PowerEnded();
			else
				AfterPowerEnded();
		}

	}

	protected abstract void Power();
	protected abstract void PowerEnded();

	protected virtual void BeforePower() {
		state = PowerState.Running;
	}

	protected virtual void AfterPowerEnded() {
		Destroy(gameObject);
	}


}
