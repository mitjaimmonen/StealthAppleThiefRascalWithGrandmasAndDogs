using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour {

	public float deactivationTime;
	float time;
	float relativeTime;
	MeshRenderer rend;
	// Use this for initialization
	void OnEnable () {
		if (!rend)
			rend = GetComponentInChildren<MeshRenderer>();
		time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {

		if (time + deactivationTime < Time.time)
		{
			gameObject.SetActive(false);
		}
		else if (rend)
		{
			relativeTime = 1 - ((Time.time - time )/deactivationTime);
			var col = rend.material.color;
			col.a = relativeTime;
			rend.material.color = col;
		}
	}
}
