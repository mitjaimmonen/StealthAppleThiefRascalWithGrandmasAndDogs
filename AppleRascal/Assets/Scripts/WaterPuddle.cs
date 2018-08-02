using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterPuddle : MonoBehaviour {

	public float splashInterval = 0.1f;
	public float trailEffectTime;
	public TrailType buffType = TrailType.footsteps;
	public TrailType trailTypeAfterBuff = TrailType.none;
	ParticleSystem splashParticles;

	float lastSplashTime;

	void Start()
	{
		splashParticles = GameMaster.Instance.ParticleMaster.waterParticles;
	}


	void OnTriggerStay(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if (player)
		{
			player.trailHandler.ChangeTrailType(TrailType.none);

			if (player.IsWalking && lastSplashTime + splashInterval < Time.time)
			{
				player.playerSoundHandler.NewWaterSplashSound();
				lastSplashTime = Time.time;
				if (splashParticles)
				{
					splashParticles.transform.position = other.transform.position;
					splashParticles.Play();
				}
				
			}
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
