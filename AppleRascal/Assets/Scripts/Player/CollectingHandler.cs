using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingHandler : MonoBehaviour {

	public bool AllowShake
	{
		get { return allowShake;}
		set 
		{
			if (value != allowShake && GameMaster.Instance.hudHandler)
				GameMaster.Instance.hudHandler.TreeTrigger(value); 
			allowShake = value;
		}
	}
	public int applesCollected = 0;
	public AppleTree appleTree;

	bool allowShake;

	public void ShakeTree()
	{
		if (AllowShake && appleTree)
			appleTree.ShakeTree();
	}

	public void CollectApple()
	{
		applesCollected ++;
		GameMaster.Instance.hudHandler.SetApplesAmount(applesCollected);
	}
}
