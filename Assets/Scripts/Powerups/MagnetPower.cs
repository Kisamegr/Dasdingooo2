using UnityEngine;
using System.Collections;

public class MagnetPower : Powerup
{


    void OnTriggerEnter2D(Collider2D other)
    {

        //if(other.gameObject.layer == LayerMask.NameToLayer("Coin")) {
        if (other.tag == "Coin")
        {
            other.GetComponent<Coin>().FollowPlayer();
        }
    }

    void Start()
    {
        base.Start();
    }

    protected override void Power()
    {

    }

    protected override void PowerEnded()
    {
        state = PowerState.Ended;
    }
    public override void Refresh()
    {
        startTime = Time.time;
    }
}
