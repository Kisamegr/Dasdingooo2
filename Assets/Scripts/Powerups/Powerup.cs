using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public abstract class Powerup : MonoBehaviour {

	protected enum PowerState {
		Started, Running, Ended
	};

	public int id;
	public float duration;
	public float changeSpeed;
	public GameObject player;
	public GameObject sliderPrefab;


    public AudioClip pickupSound;
    public AudioClip sound;

	protected float startTime;
	protected PowerState state;
	protected bool simul=false; // True for overlapping the BeforePower and Power methods

	protected Slider slider;
	// Use this for initialization
	protected virtual void Start () {
		startTime = Time.time;
		state = PowerState.Started;

		if(sliderPrefab != null)  {
			GameObject sl = (GameObject) Instantiate(sliderPrefab);
			sl.transform.SetParent(GameObject.Find("Canvas").transform.FindChild("PowerSliderPos").transform,false);

			slider = sl.GetComponent<Slider>();

		}
	}

	protected virtual void Update() {	
		if(slider != null ) {
			slider.value = 1 - (Time.time - startTime)/( duration);

			if(Time.time - startTime > duration) {
				Destroy(slider.gameObject);
			}

		}
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

	public abstract void Refresh();


}
