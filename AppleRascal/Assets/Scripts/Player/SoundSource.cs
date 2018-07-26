using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundSource : MonoBehaviour {

	// Use this for initialization
	public bool debug;
	public bool isPlayer = true;
	public LayerMask enemyLayerMask;
	[Range(0.0f,10f)] public float minimumSoundRadius = 3.5f;
	[Range(0.0f,2.0f)] public float velocityMultiplier = 0.75f;
	[Range(0.0f,1.0f)] public float nonGroundedMultiplier = 0.15f;
	[Range(0.0f,5.0f)] public float trailSoundVolume = 2f;
	[Range(0.0f,5.0f)] public float trailSoundLength = 0.5f;
	[Range(0.0f,5.0f)] public float wetFeetMultiplier = 1.5f;
	[Range(0.0f,5.0f)] public float waterSplashVolume = 1.5f;
	[Range(0.0f,5.0f)] public float waterSplashLength = 0.5f;
	[Range(0.0f,5.0f)] public float waterSplashInterval = 0.5f;

	float newSoundRadius;
	float soundRadius;
	float currentTrailVolume, newTrailSoundVolume;
	float currentWaterSplashVolume;
	float trailSoundTime;
	float waterSplashTime;
	Player player;

	public Mesh debugMesh;
	void Start () {
		if (isPlayer)
			player = GetComponent<Player>();
	}
	
	// Update is called once per frame
	void Update () 
	{
		CalculateDynamicSounds();
		CalculateSoundRadius();
	}
	void FixedUpdate()
	{
		CheckSoundHits();
	}

	public void NewTrailSound(TrailType type, float multiplier)
	{
		newTrailSoundVolume = trailSoundVolume * (type == TrailType.footsteps ? wetFeetMultiplier : 1f);
		newTrailSoundVolume *= multiplier;
		trailSoundTime = Time.time;
	}
	public void NewWaterSplashSound()
	{
		if (waterSplashTime + waterSplashInterval < Time.time)
			waterSplashTime = Time.time;
	}
	void CalculateDynamicSounds()
	{
		if (trailSoundTime + trailSoundLength > Time.time)
		{
			float t = (Time.time-trailSoundTime) / trailSoundLength;
			currentTrailVolume = newTrailSoundVolume * (1-t);
		}

		if (waterSplashTime + waterSplashLength > Time.time)
		{
			float t = (Time.time-waterSplashTime) / waterSplashLength;
			currentWaterSplashVolume = waterSplashVolume * (1-t);

		}
	}
	void CalculateSoundRadius()
	{
		newSoundRadius = player.CurrentVelocity.magnitude*velocityMultiplier;
		newSoundRadius *= player.IsGrounded ? 1f : nonGroundedMultiplier;
		newSoundRadius += currentTrailVolume;
		newSoundRadius += currentWaterSplashVolume;
		newSoundRadius = Mathf.Max(minimumSoundRadius, newSoundRadius);

		soundRadius = Mathf.Lerp(soundRadius, newSoundRadius, Time.deltaTime*5f);
	}

	void CheckSoundHits()
	{
		RaycastHit hit;
		Physics.SphereCast(transform.position, soundRadius, transform.forward, out hit, 0.0f, enemyLayerMask);
	}

	void OnDrawGizmos()
	{
		if (debug && debugMesh)
		{
			Gizmos.color = new Color32(255,255,255, 50);
			Gizmos.DrawMesh(debugMesh, -1,transform.position, transform.rotation, new Vector3(soundRadius*2, 0.25f, soundRadius*2));
		}
	}
}
