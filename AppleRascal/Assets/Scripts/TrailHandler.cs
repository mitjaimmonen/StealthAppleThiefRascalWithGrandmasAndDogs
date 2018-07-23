using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TrailType
{
	smell,
	footsteps
}
public class TrailHandler : MonoBehaviour {

	public TrailType currentTrailType = TrailType.smell;
	public GameObject trailPointPrefab;
	List<TrailPoint> inactiveTrailPoints = new List<TrailPoint>();
	List<TrailPoint> activeTrailPoints = new List<TrailPoint>();
	TrailPoint lastTrailPoint;
	public int numberOfTrailPoints;
	public float trailFrequencyInSeconds;
	public float trailPointLifetime;


	float trailTimer;


	// Use this for initialization
	void Start () {
		GameObject parent = new GameObject();
		parent.name = "TrailParent";
		for (int i = 0; i < numberOfTrailPoints; i++)
		{
			Debug.Log("Instantiating 'pool'");
			TrailPoint newPoint = Instantiate(trailPointPrefab,transform.position, transform.rotation).GetComponent<TrailPoint>();
			inactiveTrailPoints.Add(newPoint);
			newPoint.gameObject.name = "TrailPoint";
			newPoint.transform.parent = parent.transform;
			newPoint.gameObject.SetActive(false);
			newPoint.trailHandler = this;
		}
	}

	public void DeactivatePoint(TrailPoint trailPoint)
	{
		activeTrailPoints.Remove(trailPoint);
		trailPoint.gameObject.SetActive(false);
		inactiveTrailPoints.Add(trailPoint);
	}
	
	// Update is called once per frame
	void Update () {
		if (trailTimer + trailFrequencyInSeconds < Time.time)
		{
			if (inactiveTrailPoints.Count > 0)
			{
				trailTimer = Time.time;
				TrailPoint newPoint = inactiveTrailPoints[0];
				activeTrailPoints.Add(newPoint);
				inactiveTrailPoints.RemoveAt(0);

				newPoint.deactivationTime = trailPointLifetime;
				newPoint.trailPointType = currentTrailType;
				newPoint.transform.position = transform.position;
				newPoint.gameObject.SetActive(true);

				if (lastTrailPoint)
					newPoint.connectedTrailPoint = lastTrailPoint;
				lastTrailPoint = newPoint;
			}
		}
		
	}
}
