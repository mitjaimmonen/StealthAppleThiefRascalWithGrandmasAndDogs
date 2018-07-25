using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

	public Transform target;
	public Vector3 offset;
	public float lerpSpeed;
	// Use this for initialization
	void Start () {
		if (!target)
		{
			var playerObj = GameObject.FindGameObjectWithTag("Player");
			if (playerObj)
				target = playerObj.transform;
		}
	}
	
	// Update is called once per frame
	void LateUpdate () {
		if (target)
		{
			// transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * lerpSpeed);
			transform.position = target.transform.position+ offset;
		}
	}
}
