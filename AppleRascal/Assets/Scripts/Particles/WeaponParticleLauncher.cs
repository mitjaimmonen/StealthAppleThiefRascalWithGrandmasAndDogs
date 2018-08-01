using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponParticleLauncher : MonoBehaviour {

	[Tooltip("Make it bigger for burst weapons and smaller for automatic weapons."), Range(0,10f)]
	public float shootSoundMultiplier;
	public ParticleSystem muzzleFlashPS;
	public ParticleSystem bulletPS;
	public ParticleSystem muzzleSmokePS;
	public EnemySoundHandler enemySoundHandler;
	[HideInInspector]public BulletParticles bulletParticles;
	public bool debugShoot;

	void Start()
	{
		bulletParticles = GetComponentInChildren<BulletParticles>();
		enemySoundHandler = GetComponentInParent<EnemySoundHandler>();
	}

	void Update()
	{
		if (debugShoot)
		{
			Shoot();
			debugShoot = false;
		}
	}

	public void Shoot()
	{
		if (muzzleFlashPS)
			muzzleFlashPS.Play();
		if (bulletPS)
			bulletPS.Play();
		if (muzzleSmokePS)
			muzzleSmokePS.Play();

		if (enemySoundHandler)
			enemySoundHandler.NewShootSound(shootSoundMultiplier);
	}
}
