using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Footstep : MonoBehaviour {

	public float deactivationTime;
	float time;
	// Use this for initialization
	void OnEnable () {
		Debug.Log("Deactivation time: " + deactivationTime);
		time = Time.time;
	}
	
	// Update is called once per frame
	void Update () {
		if (time + deactivationTime < Time.time)
		{
			gameObject.SetActive(false);
		}
	}
}
