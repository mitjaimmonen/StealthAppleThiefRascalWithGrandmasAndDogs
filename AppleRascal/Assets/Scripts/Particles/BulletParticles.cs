using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletParticles : MonoBehaviour {

	// Use this for initialization
	WeaponParticleLauncher launcher;
	bool hasCollided = false;
	void Start () {
		launcher = GetComponentInParent<WeaponParticleLauncher>();
	}

	void ResetCollision()
	{
		hasCollided = false;
	}
	
	void OnParticleCollision(GameObject other)
	{
		if (!hasCollided)
		{
			if (other.GetComponentInChildren<Player>())
			{
				Debug.Log("Collided with player");
				hasCollided = true;
				other.GetComponentInChildren<Player>().GetHit();
				Invoke("ResetCollision", 0.5f);
			}

		}
	}
}
