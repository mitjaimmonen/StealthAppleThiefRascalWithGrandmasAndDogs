using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shredder : MonoBehaviour {

	void OnTriggerEnter(Collider col)
	{
		if (!col.gameObject.GetComponent<Player>())
			Destroy(col.gameObject);
		else
		{
			Player player = col.gameObject.GetComponent<Player>();
			player.transform.position = Vector3.zero;
		}
	}
}
