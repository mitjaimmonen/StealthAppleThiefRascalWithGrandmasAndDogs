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
			player.IsInWater = true;

			if (player.IsWalking && lastSplashTime + splashInterval < Time.time)
			{
				player.playerSoundHandler.NewWaterSplashSound();
				GameMaster.Instance.SoundMaster.SoundWaterSplash(player.transform.position);
				lastSplashTime = Time.time;
				if (splashParticles)
				{
					splashParticles.transform.position = other.transform.position;
					splashParticles.Play();
				}
				else
					splashParticles = GameMaster.Instance.ParticleMaster.waterParticles;
			}
		}

	}
	void OnTriggerExit(Collider other)
	{
		Player player = other.GetComponent<Player>();
		if (player)
		{
			player.IsInWater = false;
			player.trailHandler.ChangeTrailType(TrailType.footsteps, TrailType.none, trailEffectTime);
		}
	}
}
