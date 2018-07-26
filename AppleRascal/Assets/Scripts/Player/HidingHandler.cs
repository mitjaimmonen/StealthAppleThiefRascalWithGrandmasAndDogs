using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingHandler : MonoBehaviour {
	public float hideMoveTime;
	float magnitude;
	Vector3 direction;
	Player player;
	Vector3 inPos;
	Vector3 midPos;
	Vector3 outPos;
	
	bool coroutineRunning = false;

	void Start()
	{
		player = GetComponent<Player>();
	}

	public void StartHiding()
	{
		direction = player.transform.position - player.hide.transform.position;
		magnitude = direction.magnitude;

		midPos = player.transform.position + (direction.normalized * magnitude/2f) + (Vector3.up * magnitude/2f);
		inPos = player.hide.transform.position;
		outPos = player.transform.position;
		if (!coroutineRunning)
			StartCoroutine(LerpToHide());
		
	}

	public void EndHiding()
	{
		if (!coroutineRunning)
			StartCoroutine(LerpFromHide());
	}



	IEnumerator LerpToHide()
	{
		coroutineRunning = true;

		player.OverridingTransform = true;
		player.IsHiding = true;

		float time = Time.time;
		float t = 0, smoothLerp = 0;
		Vector3 newPos;
		while (time + hideMoveTime > Time.time)
		{
            t = (Time.time-time)/hideMoveTime;
            smoothLerp = t*t * (3f - 2f*t);
			Vector3 lerpstart = Vector3.Lerp(outPos,midPos, t);
			Vector3 lerpEnd = Vector3.Lerp(midPos,inPos, t);
			newPos = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			player.transform.position = newPos;

			yield return null;
		}

		player.transform.position = inPos;
		coroutineRunning =false;
		yield break;
	}

	IEnumerator LerpFromHide()
	{
		coroutineRunning = true;

		player.OverridingTransform = true;

		float time = Time.time;
		float t = 0, smoothLerp = 0;
		Vector3 newPos;
		while (time + hideMoveTime > Time.time)
		{
            t = (Time.time-time)/hideMoveTime;
            smoothLerp = t*t * (3f - 2f*t);
			Vector3 lerpstart = Vector3.Lerp(inPos,midPos, t);
			Vector3 lerpEnd = Vector3.Lerp(midPos,outPos, t);
			newPos = Vector3.Lerp(lerpstart, lerpEnd, smoothLerp);
			player.transform.position = newPos;

			yield return null;
		}

		player.transform.position = outPos;


		player.OverridingTransform = false;
		player.IsHiding = false;
		coroutineRunning =false;
		yield break;
	}
}
