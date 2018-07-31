using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingSpot : MonoBehaviour {

	public bool playerVisible = true;
	public Animator anim;
	public bool affectTrail;
	public TrailType trailInHide;
	public TrailType trailAfterHiding;
	public float trailPointTimeAfterHiding;
	public float trailBuffLength;

	Player player;


	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			player = other.GetComponent<Player>();
			player.hide = this;
			player.hidingHandler.AllowHiding = true;
			player.hidingHandler.VisibleInHide = playerVisible;
		}
	}
	void OnTriggerStay(Collider other)
	{
		if (player && !player.hidingHandler.AllowHiding)
		{
			if (other.GetComponent<Player>())
			{
				player = other.GetComponent<Player>();
				player.hide = this;
				player.hidingHandler.AllowHiding = true;
				player.hidingHandler.VisibleInHide = playerVisible;
			}
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			player = other.GetComponent<Player>();
			player.hidingHandler.AllowHiding = false;
		}

	}

	
}
