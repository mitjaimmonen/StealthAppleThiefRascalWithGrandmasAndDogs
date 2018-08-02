using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSoundHandler : MonoBehaviour {

	// Use this for initialization
	public GameObject soundVisualPrefab;
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
	[Range(0.0f,10.0f)] public float treeShakeVolume = 1.5f;
	[Range(0.0f,5.0f)] public float treeShakeLength = 0.75f;

	float newSoundRadius;
	float soundRadius;
	float currentTrailVolume, newTrailSoundVolume;
	float currentWaterSplashVolume;
	float currentTreeShakeVolume, newTreeSoundVolume;

	float trailSoundTime;
	float waterSplashTime;
	float treeShakeTime;
	Player player;
	GameObject visual;

	float timeTime;

	void Start () {
		player = GetComponent<Player>();
		visual = Instantiate(soundVisualPrefab, transform.position, transform.rotation);
		visual.transform.parent = transform;
		visual.transform.localPosition = new Vector3(0,-0.4f, 0);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		timeTime = Time.time;
		CalculateDynamicSounds();
		CalculateSoundRadius();
		DrawVisual();
	}
	void FixedUpdate()
	{
		CheckSoundHits();
	}

	public void NewTrailSound(TrailType type, float multiplier)
	{
		newTrailSoundVolume = trailSoundVolume * (type == TrailType.footsteps ? wetFeetMultiplier : 1f);
		newTrailSoundVolume *= multiplier;
		trailSoundTime = timeTime;
	}

	public void NewTreeShakeSound()
	{
		newTreeSoundVolume = currentTreeShakeVolume + treeShakeVolume;
		treeShakeTime = timeTime;

	}
	public void NewWaterSplashSound()
	{
		if (waterSplashTime + waterSplashInterval < timeTime)
			waterSplashTime = timeTime;
	}
	void CalculateDynamicSounds()
	{
		float t = 0;

		if (trailSoundTime + trailSoundLength > timeTime)
		{
			t = (timeTime-trailSoundTime) / trailSoundLength;
			currentTrailVolume = newTrailSoundVolume * (1-t);
		}

		if (waterSplashTime + waterSplashLength > timeTime)
		{
			t = (timeTime-waterSplashTime) / waterSplashLength;
			currentWaterSplashVolume = waterSplashVolume * (1-t);
		}

		if (treeShakeTime + treeShakeLength > timeTime)
		{
			t = (timeTime-treeShakeTime) / treeShakeLength;
			currentTreeShakeVolume = Mathf.Lerp(newTreeSoundVolume, 0, t);
		}
	}
	void CalculateSoundRadius()
	{
		newSoundRadius = player.IsWalking ? player.CurrentVelocity.magnitude*velocityMultiplier : minimumSoundRadius;
		newSoundRadius *= player.IsGrounded ? 1f : nonGroundedMultiplier;
		newSoundRadius += currentTrailVolume;
		newSoundRadius += currentWaterSplashVolume;
		newSoundRadius += currentTreeShakeVolume;
		newSoundRadius = Mathf.Max(minimumSoundRadius, newSoundRadius);

		soundRadius = Mathf.Lerp(soundRadius, newSoundRadius, Time.deltaTime*5f);
	}

	void CheckSoundHits()
	{
		RaycastHit[] hit = Physics.SphereCastAll(transform.position, soundRadius, transform.forward, 0.0f, enemyLayerMask);

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i].collider.GetComponentInParent<Granny>())
			{
                Granny enemyAI = hit[i].collider.GetComponentInParent<Granny>();
				enemyAI.Hear(transform, true);
			}
		}
	}

	void DrawVisual()
	{
		visual.transform.localScale = new Vector3(soundRadius*2, 0.1f, soundRadius*2);
	}

}
