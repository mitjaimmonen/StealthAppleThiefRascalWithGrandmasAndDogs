using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectingHandler : MonoBehaviour {

	public bool AllowShake;
	public int applesCollected = 0;
	public AppleTree appleTree;

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
