using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySoundHandler : MonoBehaviour {

	// Use this for initialization
	public GameObject soundVisualPrefab;
	public bool isActive = true;
	public LayerMask enemyLayerMask;
	[Range(0.0f,10f)] public float minimumSoundRadius = 3.5f;
	[Range(0.0f,10f)] public float shootSoundLength = 1.5f;
	[Range(0.0f,10f)] public float shootSoundVolume = 3.5f;


	float shootSoundTime;
	float currentShootVolume;
	float newShootVolume;
	float newSoundRadius;
	float soundRadius;
	AI thisAI;
	GameObject visual;

	float timeTime;

	void Start () {
		thisAI = GetComponent<AI>();

		visual = Instantiate(soundVisualPrefab, transform.position, transform.rotation);
		
	}
	
	// Update is called once per frame
	void Update () 
	{
		if (isActive)
		{
			timeTime = Time.time;
			CalculateDynamicSounds();
			CalculateSoundRadius();
			DrawVisual();
		}
		else if (visual.activeSelf)
			visual.SetActive(false);
	}
	void FixedUpdate()
	{
		CheckSoundHits();
	}
	public void NewShootSound(float multiplier)
	{
		newShootVolume = currentShootVolume + (shootSoundVolume*multiplier);
		shootSoundTime = timeTime;
	}
	void CalculateDynamicSounds()
	{
		float t = 0;

		if (shootSoundTime + shootSoundLength > timeTime)
		{
			t = (timeTime-shootSoundTime) / shootSoundLength;
			currentShootVolume = newShootVolume * (1-t);
		}

	}
	void CalculateSoundRadius()
	{
		newSoundRadius += currentShootVolume;
		newSoundRadius = Mathf.Max(minimumSoundRadius, newSoundRadius);
		soundRadius = Mathf.Lerp(soundRadius, newSoundRadius, Time.deltaTime*5f);
	}

	void CheckSoundHits()
	{
		RaycastHit[] hit = Physics.SphereCastAll(transform.position, soundRadius, transform.forward, 0.0f, enemyLayerMask);

		for (int i = 0; i < hit.Length; i++)
		{
			if (hit[i].collider.GetComponent<AI>())
			{
				AI enemyAI = hit[i].collider.GetComponent<AI>();
				enemyAI.Hear(transform.position, false);
			}
		}
	}

	void DrawVisual()
	{
		if (!visual.activeSelf)
			visual.SetActive(true);
		visual.transform.position = transform.position;
		visual.transform.rotation = transform.rotation;
		visual.transform.localScale = new Vector3(soundRadius*2, 0.1f, soundRadius*2);
	}

}
