using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraHandler : MonoBehaviour {

	public Transform target;
	public Vector3 offset;
	public float lerpSpeed;
	PostProcessingHandler _postProcess;

	public PostProcessingHandler postProcess
	{
		get 
		{
			if (_postProcess == null)
				_postProcess = GetComponent<PostProcessingHandler>();
			return _postProcess;

		}
	}
	// Use this for initialization
	void Awake () {
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
			transform.position = Vector3.Lerp(transform.position, target.position + offset, Time.deltaTime * lerpSpeed);
		}
	}
}
