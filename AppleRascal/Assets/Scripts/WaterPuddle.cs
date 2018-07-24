using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPuddle : MonoBehaviour {

	public float trailEffectTime;
	public TrailType buffType = TrailType.footsteps;
	public TrailType trailTypeAfterBuff = TrailType.none;
	ParticleSystem splashParticles;

	float splashInterval;


	void OnTriggerStay(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if (player)
		{
			player.trailHandler.ChangeTrailType(TrailType.none);
		}
	}
	void OnTriggerExit(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if (player)
		{
			player.trailHandler.ChangeTrailType(TrailType.footsteps, TrailType.none, trailEffectTime);
		}
	}
}
