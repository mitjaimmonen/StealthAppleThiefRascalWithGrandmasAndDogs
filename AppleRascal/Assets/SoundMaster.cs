using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FMOD;

public class SoundMaster : MonoBehaviour {

	[SerializeField, FMODUnity.EventRef] string Leap;
	[SerializeField, FMODUnity.EventRef] string treeShake;
	[SerializeField, FMODUnity.EventRef] string appleCollect;
	[SerializeField, FMODUnity.EventRef] string getShot;
	[SerializeField, FMODUnity.EventRef] string shootBlunderbuss;
	[SerializeField, FMODUnity.EventRef] string loodSpillShort;
	[SerializeField, FMODUnity.EventRef] string footstep;
	[SerializeField, FMODUnity.EventRef] string footstepWet;
	[SerializeField, FMODUnity.EventRef] string hideTrashcan;
	[SerializeField, FMODUnity.EventRef] string hideTrashPile;
	[SerializeField, FMODUnity.EventRef] string PlayerGetTornApart;
	[SerializeField, FMODUnity.EventRef] string waterSplash;
	[SerializeField, FMODUnity.EventRef] string playerGetBit;


	float appleSoundTimer = 0;

	void Start()
	{
		if (!Camera.main.GetComponent<FMODUnity.StudioListener>())
			Camera.main.gameObject.AddComponent<FMODUnity.StudioListener>();
	}

	public void SoundAppleCollect(Vector3 position)
	{
		if (appleSoundTimer+0.075f < Time.time)
		{
			appleSoundTimer = Time.time;
			FMODUnity.RuntimeManager.PlayOneShot(appleCollect, position);
		}
	}
	public void SoundTreeShake(Vector3 treePosition)
	{
		FMODUnity.RuntimeManager.PlayOneShot(treeShake, treePosition);
	}
	public void SoundGetShot(Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot(getShot, position);
	}
	public void SoundGetBit(Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot(playerGetBit, position);
	}
	public void SoundGetTornApart(Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot(PlayerGetTornApart, position);
	}
	
	public void SoundShootBlunderbuss(Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot(shootBlunderbuss, position);
	}

	public void SoundFootstep(Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot(footstep, position);
	}
	
	public void SoundFootstepWet(Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot(footstepWet, position);
	}
	public void SoundWaterSplash(Vector3 position)
	{
		FMODUnity.RuntimeManager.PlayOneShot(waterSplash, position);
	}
}
