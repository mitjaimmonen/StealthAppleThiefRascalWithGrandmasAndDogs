using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailPoint : MonoBehaviour {

	public TrailHandler trailHandler;
	public TrailPoint connectedTrailPoint;
	public TrailType trailPointType;
	public float deactivationTime;
	float timer = 0;
	bool _enabled;

	public bool Enabled
	{
		get {return _enabled;}
		set
		{
			if (_enabled != value)
				SetState(value);
			
			_enabled = value;
		}
	}

	void SetState(bool enabled)
	{
		if (enabled)
		{
			timer = Time.time;
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (Enabled)
		{
			if (timer + deactivationTime < Time.time)
			{
				if (!trailHandler)
					Debug.LogError("No Trail Handler assigned in point!");
				else
					trailHandler.DeactivatePoint(this);	

			}
			if (!connectedTrailPoint || connectedTrailPoint.trailPointType != trailPointType)
				Debug.Log("Current trail point type is different than connected trail point type");
			
		}
	}

	void OnDrawGizmos()
	{
		if (Enabled)
		{
			Gizmos.color = Color.green;
			Gizmos.DrawSphere(transform.position, 0.25f);
		}
	}


}
