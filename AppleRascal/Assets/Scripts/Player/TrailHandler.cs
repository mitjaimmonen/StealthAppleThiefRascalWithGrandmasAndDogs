using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrailType
{
	none,
	smell,
	footsteps
}
public class TrailHandler : MonoBehaviour {

	[HideInInspector]public Player player;
	public GameObject trailPointPrefab;
	public TrailType defaultTrail = TrailType.smell;
	public float returnToDefaultTrailTime = 10f;
	List<TrailPoint> trailPoints = new List<TrailPoint>();
	TrailPoint lastTrailPoint;
	public int numberOfTrailPoints;
	public bool allowResize;
	public bool debugPoints;
	public float trailFrequencyInSeconds;
	public float trailPointLifetime;

	[Header("Trail visuals")]
	public ParticleSystem smellParticles;
	public FootstepHandler footstepHandler;


	TrailType currentTrailType = TrailType.none;
	float defaultTrailPointLifetime;
	float lastTrailPointTime;
	float lastTrailTypeChangeTime;
	int pointIndex = 0;
	bool buffDelayActive = false;
	bool buffLifetimeActive = false;
	float buffDelayStartTime;
	float buffRelativeTimeLeft;
	GameObject trailParent;


	public TrailType CurrentTrailType
	{
		get {return currentTrailType;}
		set
		{
			if (value != currentTrailType)
			{
				lastTrailTypeChangeTime = Time.time;
				currentTrailType = value;
			}
		}

	}


	// Use this for initialization
	void Start () {
		//Makes a new empty gameObject where points get instantiated
		trailParent = new GameObject();
		trailParent.name = "TrailParent";

		for (int i = 0; i < numberOfTrailPoints; i++)
		{
			CreateNewTrailPoint();
		}
		if (!player)
			player = GetComponent<Player>();
		if (!footstepHandler)
			footstepHandler = GetComponent<FootstepHandler>();

		CurrentTrailType = defaultTrail;
		defaultTrailPointLifetime = trailPointLifetime;
	}

	void CreateNewTrailPoint()
	{
		TrailPoint newPoint = Instantiate(trailPointPrefab,transform.position, transform.rotation).GetComponent<TrailPoint>();
		trailPoints.Add(newPoint);
		newPoint.gameObject.name = "TrailPoint";
		newPoint.transform.parent = trailParent.transform;
		newPoint.trailHandler = this;
	}
	void RemoveTrailPoint()
	{
		TrailPoint pointToRemove = trailPoints[trailPoints.Count-1];
		trailPoints.Remove(pointToRemove);
		Destroy(pointToRemove.gameObject);
	}

	public void DeactivatePoint(TrailPoint trailPoint)
	{
		trailPoint.connectedTrailPoint = null;
		trailPoint.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		SetTrailPoints();
		CreateTrailVisuals();
		
		if (!buffDelayActive && !player.hidingHandler.IsHiding && currentTrailType != defaultTrail && returnToDefaultTrailTime + lastTrailTypeChangeTime < Time.time)
		{
			Debug.Log("Back to default trail");
			CurrentTrailType = defaultTrail;
		}
		if (allowResize)
		{
			int difference = Mathf.Abs(numberOfTrailPoints-trailPoints.Count);
			if (numberOfTrailPoints > trailPoints.Count)
			{
				for(int i = 0; i < difference; i++)
					CreateNewTrailPoint();
			}
			if (numberOfTrailPoints < trailPoints.Count)
			{
				for(int i = 0; i < difference; i++)
					RemoveTrailPoint();
			}
		}
	}

	void SetTrailPoints()
	{
		if (!player.IsMoving || player.hidingHandler.IsHiding)
			return;
		

		if (lastTrailPointTime + (trailFrequencyInSeconds * (player.IsCrawling ? 2f : 1f)) < Time.time)
		{
			if (pointIndex >= trailPoints.Count)
				pointIndex = 0;

			if (!trailPoints[pointIndex].gameObject.activeSelf)
			{
				lastTrailPointTime = Time.time;
				TrailPoint newPoint = trailPoints[pointIndex];

				if (!player.IsGrounded && currentTrailType == TrailType.footsteps)
					newPoint.trailPointType = TrailType.none;
				else
					newPoint.trailPointType = CurrentTrailType;

				newPoint.deactivationTime = trailPointLifetime;
				newPoint.transform.position = transform.position;
				newPoint.gameObject.SetActive(true);
				player.soundSource.NewTrailSound(newPoint.trailPointType, buffDelayActive ? 2f-buffRelativeTimeLeft : 1f);

				if (lastTrailPoint)
					lastTrailPoint.connectedTrailPoint = newPoint;
				lastTrailPoint = newPoint;
				pointIndex++;
			}
		}
	}

	void CreateTrailVisuals()
	{
		if (currentTrailType == TrailType.smell)
		{
			if (smellParticles)
			{
				var main = smellParticles.main;
				main.startLifetime = trailPointLifetime;
				if (!smellParticles.isPlaying || !smellParticles.isEmitting)
				{
					main.loop = true;
					smellParticles.Play();
				}
			}
		}
		else if (currentTrailType == TrailType.footsteps)
		{
			if (smellParticles && smellParticles.isPlaying)
				smellParticles.Stop();
			if (!player.IsDashing && player.IsGrounded)
			{
				if (buffDelayActive)
					footstepHandler.CreateFootsteps(buffRelativeTimeLeft);
				else
					footstepHandler.CreateFootsteps(1);
			}

		}
		else if (currentTrailType == TrailType.none)
		{
			if (smellParticles && smellParticles.isPlaying)
				smellParticles.Stop();
		}
	}

	public void ChangeTrailType(TrailType newTrailType)
	{
		CurrentTrailType = newTrailType;
	}
	public void ChangeTrailType(TrailType newTrailType,TrailType trailTypeAfterBuff, float buffTime)
	{
		CurrentTrailType = newTrailType;
		if (!buffDelayActive)
		{
			Debug.Log("Change trail type , starting coroutine to delay.");

			buffDelayStartTime = Time.time;
			StartCoroutine(ChangeTrailTypeWithDelay(trailTypeAfterBuff, buffTime));
		}
	}

	IEnumerator ChangeTrailTypeWithDelay(TrailType newType, float buffTime)
	{
		buffDelayActive = true;
		TrailType type = CurrentTrailType;
		while(buffTime + buffDelayStartTime > Time.time)
		{
			//NOTE: buffDelayStartTime can update as long as player is triggering ChangeTrailType()
			yield return new WaitForSeconds(0.1f);

			buffRelativeTimeLeft = (Time.time-buffDelayStartTime) / buffTime;

			//In case trail has already changed during buff, exit routine
			if (type != currentTrailType)
			{
				buffDelayActive = false;
				yield break;
			}
		}
		CurrentTrailType = newType;
		Debug.Log("Change trail type with delay.");
		buffDelayActive = false;
		yield break;

	}

	public void ChangeTrailPointLifetime(float lifetime, float buffTime)
	{
		if (!buffLifetimeActive)
			StartCoroutine(ChangeTrailPointLifetimeBuff(lifetime, buffTime));
	}

	IEnumerator ChangeTrailPointLifetimeBuff(float lifetime, float buffTime)
	{
		buffLifetimeActive = true;
		float time = Time.time;
		TrailType type = CurrentTrailType;
		trailPointLifetime = lifetime;
		while (time+buffTime > Time.time)
		{
			yield return new WaitForSeconds(0.1f);
			if (type != currentTrailType)
			{
				trailPointLifetime = defaultTrailPointLifetime;
				buffLifetimeActive = false;
				yield break;

			}
		}

		trailPointLifetime = defaultTrailPointLifetime;
		buffLifetimeActive = false;
		yield break;
	}
}
