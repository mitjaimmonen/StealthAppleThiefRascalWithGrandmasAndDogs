using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour {

	public bool setPlayerInvisible;
	public Animator anim;
	public bool affectTrail;
	public TrailType trailTypeAfterHiding;
	public float trailBuffLength;

	Player player;


	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			player = other.GetComponent<Player>();
			player.hide = this;
			player.AllowHiding = true;
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (player && !player.AllowHiding)
		{
			if (other.GetComponent<Player>())
			{
				player = other.GetComponent<Player>();
				player.hide = this;
				player.AllowHiding = true;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			player = other.GetComponent<Player>();
			player.AllowHiding = false;
		}

	}

	
}
