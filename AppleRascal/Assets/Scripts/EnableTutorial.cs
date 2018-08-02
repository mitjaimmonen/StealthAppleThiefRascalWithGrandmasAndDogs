using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TutorialType
{
	jump,
	crawl,
	move
}
public class EnableTutorial : MonoBehaviour {

	public TutorialType tutorialType;

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponentInChildren<Player>())
		{
			Debug.Log("Hasjumped = false");
			if (tutorialType == TutorialType.jump)
				other.GetComponentInChildren<Player>().hasJumped = false;
			if (tutorialType == TutorialType.move)
				other.GetComponentInChildren<Player>().hasMoved = false;
			if (tutorialType == TutorialType.crawl)
				other.GetComponentInChildren<Player>().hasCrawled = false;
		}
	}
}
