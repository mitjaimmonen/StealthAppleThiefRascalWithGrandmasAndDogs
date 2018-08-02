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
	
	bool isHiding;
	bool allowHiding;
	bool visibleInHide;

	bool coroutineRunning = false;

	public bool IsHiding
	{
		get { return isHiding; }
		set { isHiding = value; }
	}
	public bool AllowHiding
	{
		get { return allowHiding; }
		set 
		{
			if (value != allowHiding && GameMaster.Instance.gameCanvas)
				GameMaster.Instance.gameCanvas.hudHandler.SetActionText(value, "Hide");
			allowHiding = value; 
		}
	}
	public bool VisibleInHide
	{
		get { return visibleInHide; }
		set { visibleInHide = value; }
	}

	void Start()
	{
		player = GetComponent<Player>();
	}

	public void StartHiding()
	{
        GameMaster.Instance.playerHide(player.hide.transform,true);
		direction = player.hide.transform.position-player.transform.position;
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
		IsHiding = true;

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

		player.playerVisuals.SetActive(visibleInHide);

		player.transform.position = inPos;
		player.trailHandler.CurrentTrailType = player.hide.trailInHide;
		coroutineRunning =false;
		yield break;
	}

	IEnumerator LerpFromHide()
	{
		coroutineRunning = true;

		player.playerVisuals.SetActive(true);
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
		player.trailHandler.ChangeTrailType(player.hide.trailAfterHiding);
		player.trailHandler.ChangeTrailPointLifetime(player.hide.trailPointTimeAfterHiding, player.hide.trailBuffLength);


		player.OverridingTransform = false;
		IsHiding = false;
		coroutineRunning =false;
      //  GameMaster.Instance.playerHide(player.transform, false);
		yield break;
	}
}
