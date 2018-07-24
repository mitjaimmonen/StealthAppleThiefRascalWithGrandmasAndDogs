using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrailType
{
	smell,
	footsteps
}
public class TrailHandler : MonoBehaviour {

	[HideInInspector]public Player player;
	public TrailType currentTrailType = TrailType.smell;
	public GameObject trailPointPrefab;
	List<TrailPoint> trailPoints = new List<TrailPoint>();
	TrailPoint lastTrailPoint;
	public int numberOfTrailPoints;
	public bool allowResize;
	public float trailFrequencyInSeconds;
	public float trailPointLifetime;

	[Header("Trail visuals")]
	public ParticleSystem smellParticles;
	public FootstepHandler footstepHandler;


	float trailTimer;
	int pointIndex = 0;
	GameObject trailParent;


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
	}

	void CreateNewTrailPoint()
	{
		Debug.Log("Instantiating 'pool'");
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
		trailPoint.Enabled = false;
	}
	
	// Update is called once per frame
	void Update () 
	{
		SetTrailPoints();
		CreateTrailVisuals();

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
		if (trailTimer + trailFrequencyInSeconds < Time.time)
		{
			if (pointIndex >= trailPoints.Count)
				pointIndex = 0;

			if (!trailPoints[pointIndex].Enabled)
			{
				trailTimer = Time.time;
				TrailPoint newPoint = trailPoints[pointIndex];

				newPoint.deactivationTime = trailPointLifetime;
				newPoint.trailPointType = currentTrailType;
				newPoint.transform.position = transform.position;
				newPoint.Enabled = true;

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
			if (smellParticles && !smellParticles.isPlaying)
			{
				var main = smellParticles.main;
				main.duration = trailPointLifetime;
				main.startLifetime = trailPointLifetime;
				main.loop = true;
				smellParticles.Play();
			}
		}
		else if (currentTrailType == TrailType.footsteps)
		{
			if (smellParticles && smellParticles.isPlaying)
				smellParticles.Stop();
			
			footstepHandler.CreateFootsteps();

		}
	}
}
