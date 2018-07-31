using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BloodSpillLauncher : MonoBehaviour {

	public LayerMask allowedDecalLayers;
	[Tooltip ("Max amount of decals in 100 milliseconds.")]
	public int maxLoopCount = 15;
	SplatterDecal decal;
	ParticleSystem thisPS;
	List<ParticleCollisionEvent> collisionEvents = new List<ParticleCollisionEvent>();

	int loopCount;
	int oldLoopCount = 0;
	float collisionCountTimer = 0, splashTimer = 0;
	// Use this for initialization
	void Start () {
		decal = GetComponentInChildren<SplatterDecal>();
		thisPS = GetComponent<ParticleSystem>();

	}

	private void OnParticleCollision(GameObject other)
	{
		Debug.Log("Hit");
		if (allowedDecalLayers == (allowedDecalLayers | (1 << other.layer)))
		{
			if (collisionCountTimer < Time.time - 0.1f)
			{
				oldLoopCount = 0;
				collisionCountTimer= Time.time;
			}

			ParticlePhysicsExtensions.GetCollisionEvents (thisPS, other, collisionEvents);
			loopCount = Mathf.Clamp(collisionEvents.Count, 0, maxLoopCount-oldLoopCount);

			if (loopCount > 0)
			{
				oldLoopCount += loopCount;
				Debug.Log("ALlowed");

				if (splashTimer < Time.time - 0.01f)
				{
					for (int i = 0; i < collisionEvents.Count;i++)
					{
						if (!collisionEvents[i].colliderComponent)
							continue;

						decal.ParticleHit (collisionEvents [i]);
					}
				}
			}
		}
	}
}
