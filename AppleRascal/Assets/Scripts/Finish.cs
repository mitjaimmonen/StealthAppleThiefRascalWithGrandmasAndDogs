using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Finish : MonoBehaviour {


	Player player;
	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			player = other.GetComponent<Player>();
			player.AllowFinish = true;
		}

	}
	void OnTriggerExit(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			player.AllowFinish = false;
		}

	}
}
