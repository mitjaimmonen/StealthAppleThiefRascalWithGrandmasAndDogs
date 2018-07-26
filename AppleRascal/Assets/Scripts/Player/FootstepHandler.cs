using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepHandler : MonoBehaviour {

	public GameObject leftFoot, rightFoot;
	public float stepDistance;
	public float stepInterval;
	public float offsetX;
	public int amountOfFeet;
	TrailHandler trailHandler;

	bool leftSide;
	float lastStepTime;
	int leftIndex = 0, rightIndex = 0;
	List<Footstep> leftFeet = new List<Footstep>();
	List<Footstep> rightFeet = new List<Footstep>();
	Transform lastStepTransform;
	Transform parent;

	// Use this for initialization
	void Start () {

		if (!trailHandler)
			trailHandler = GetComponent<TrailHandler>();

		parent = new GameObject().transform;
		parent.gameObject.name = "Footstep Visuals Parent";

		for (int i = 0; i < amountOfFeet; i++)
		{
			Footstep foot;

			foot = Instantiate(leftFoot, transform.position, transform.rotation).GetComponent<Footstep>();
			leftFeet.Add(foot);
			foot.transform.parent = parent;
			foot.gameObject.SetActive(false);

			foot = Instantiate(rightFoot, transform.position, transform.rotation).GetComponent<Footstep>();
			rightFeet.Add(foot);
			foot.transform.parent = parent;
			foot.gameObject.SetActive(false);
		}
	}

	public void CreateFootsteps()
	{
		if (leftFoot && rightFoot)
		{
			if ((lastStepTransform && (lastStepTransform.position - transform.position).magnitude < stepDistance) && lastStepTime + stepInterval > Time.time)
			return;

			if (leftSide)
			{
				if (leftIndex >= leftFeet.Count)
					leftIndex = 0;
				
				if (!leftFeet[leftIndex].gameObject.activeSelf)
				{
					leftFeet[leftIndex].transform.position = transform.position - transform.right*offsetX;
					leftFeet[leftIndex].transform.rotation = transform.rotation;
					leftFeet[leftIndex].deactivationTime = trailHandler.trailPointLifetime;
					lastStepTransform = leftFeet[leftIndex].transform;
					leftFeet[leftIndex].gameObject.SetActive(true);
					leftIndex++;
				}
			}
			else
			{
				if (rightIndex >= rightFeet.Count)
					rightIndex = 0;
					
				if (!rightFeet[rightIndex].gameObject.activeSelf)
				{
					rightFeet[rightIndex].transform.position = transform.position + transform.right*offsetX;
					rightFeet[rightIndex].transform.rotation = transform.rotation;
					rightFeet[rightIndex].deactivationTime = trailHandler.trailPointLifetime;
					lastStepTransform = rightFeet[rightIndex].transform;
					rightFeet[rightIndex].gameObject.SetActive(true);
					rightIndex++;
				}
			}

			lastStepTime = Time.time;
			lastStepTransform.parent = parent;
			leftSide = !leftSide;
		}
	}
}
