using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Apple : MonoBehaviour {

	public Player player;

	public float magnetDistance;
	public float magnetStrength = 5f;

	Rigidbody rb;
	Vector3 dirToPlayer;

	void Awake()
	{
		rb = GetComponent<Rigidbody>();
	}

	void OnEnable()
	{
		rb = GetComponent<Rigidbody>();
		rb.AddForce(Random.insideUnitSphere.normalized);
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.GetComponent<Player>())
		{
			other.GetComponent<Player>().collectingHandler.CollectApple();
			Destroy(this.gameObject);
		}
	}

	void FixedUpdate()
	{
		dirToPlayer = player.transform.position-transform.position;
		if (dirToPlayer.magnitude < magnetDistance)
		{
			rb.AddForce(dirToPlayer.normalized* (magnetDistance - dirToPlayer.magnitude)*magnetStrength);
		}
	}

}
