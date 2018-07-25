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
	public float trailFrequencyInSeconds;
	public float trailPointLifetime;

	[Header("Trail visuals")]
	public ParticleSystem smellParticles;
	public FootstepHandler footstepHandler;


	TrailType currentTrailType = TrailType.none;
	float lastTrailPointTime;
	float lastTrailTypeChangeTime;
	int pointIndex = 0;
	bool buffDelayActive = false;
	float buffDelayStartTime;
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
		trailPoint.gameObject.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () 
	{
		SetTrailPoints();
		CreateTrailVisuals();
		
		if (!buffDelayActive && currentTrailType != defaultTrail && returnToDefaultTrailTime + lastTrailTypeChangeTime < Time.time)
		{
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
		if (lastTrailPointTime + trailFrequencyInSeconds < Time.time)
		{
			if (pointIndex >= trailPoints.Count)
				pointIndex = 0;

			if (!trailPoints[pointIndex].gameObject.activeSelf)
			{
				lastTrailPointTime = Time.time;
				TrailPoint newPoint = trailPoints[pointIndex];

				newPoint.deactivationTime = trailPointLifetime;
				newPoint.trailPointType = CurrentTrailType;
				newPoint.transform.position = transform.position;
				newPoint.gameObject.SetActive(true);

				if (lastTrailPoint)
					newPoint.connectedTrailPoint = lastTrailPoint;
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
				if (!smellParticles.isPlaying)
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
			if (!player.IsDashing)
				footstepHandler.CreateFootsteps();

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
		buffDelayStartTime = Time.time;
		if (!buffDelayActive)
			StartCoroutine(ChangeTrailTypeWithDelay(trailTypeAfterBuff, buffTime));
	}

	IEnumerator ChangeTrailTypeWithDelay(TrailType newType, float buffTime)
	{
		buffDelayActive = true;
		while(buffTime + buffDelayStartTime > Time.time)
		{
			//NOTE: buffDelayStartTime can update as long as player is triggering ChangeTrailType()
			yield return new WaitForSeconds(0.1f);
		}
		CurrentTrailType = newType;
		buffDelayActive = false;
		yield break;

	}
}
