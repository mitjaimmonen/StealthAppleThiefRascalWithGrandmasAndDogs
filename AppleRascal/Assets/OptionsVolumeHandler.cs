using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SoundType
{
	music,
	sfx
}
public class OptionsVolumeHandler : MonoBehaviour {

	public SoundType volumeHandlerType = SoundType.music;
	public Text volumeText;
	public GameObject menuApplePrefab;
	public Transform appleParent;
	public Transform box;
	List<GameObject> apples = new List<GameObject>();

	int applesActive = 0;
	int index = 0;
	int volume = 0;

	bool disableScaling = false;

	float appleAddTime;
	Vector3 pos;
	Quaternion rot;

	Vector3 originalScale;

	public int Index
	{
		get {return index;}
		set 
		{
			if (value > 100)
				index = 0;
			if (value < 0)
				index = 99;
			else
				index = value;
		}
	}


	void Start () {

		originalScale = box.localScale;
		disableScaling = true;
		for (int i = 0; i < 100; i++)
		{
			apples.Add(Instantiate(menuApplePrefab, pos, rot));
			apples[i].SetActive(false);
			apples[i].transform.parent = appleParent;
		}
		if (volumeHandlerType == SoundType.music)
			volume = PlayerPrefsManager.GetMusicVolume();
		if (volumeHandlerType == SoundType.sfx)
			volume = PlayerPrefsManager.GetSfxVolume();
		volumeText.text = volume.ToString();
	}

	public int SetVolume(int value, bool applyAsOffset)
	{
		disableScaling = false;

		if (applyAsOffset)
			volume += value;
		else
			volume = value;
		
		volume = Mathf.Clamp(volume, 0,100);
		volumeText.text = volume.ToString();
		Debug.Log("Volume: " + volume);
		return volume;
	}

	void AddApple()
	{
		Debug.Log("add apple called");
		if (applesActive < 100)
		{
			bool added = false;
			while (!added)
			{
				if (!apples[index].activeSelf)
				{
					appleAddTime = Time.time;
					applesActive++;
					added = true;
					Debug.Log("While");

					pos = appleParent.position + new Vector3(Random.Range(-0.3f,0.3f), Random.Range(-0.3f,0.3f), Random.Range(-0.3f,0.3f));
					rot = Quaternion.Euler(Random.Range(0f,360f), Random.Range(0f,360f), Random.Range(0f,360f));
					apples[index].transform.position = pos;
					apples[index].transform.rotation = rot;
					apples[index].SetActive(true);
					if (!disableScaling)
						box.localScale = box.localScale + new Vector3(0.05f,0.05f,0.05f);
				}
				else
					Index++;

			}
		}
	}

	void RemoveApple()
	{
		if (applesActive > 0)
		{
			bool removed = false;
			while (!removed)
			{
				if (apples[index].activeSelf)
				{
					applesActive--;
					removed = true;
					Debug.Log("While");
					if (!disableScaling)
						box.localScale = box.localScale + new Vector3(-0.05f,-0.05f,-0.05f);
					apples[index].SetActive(false);
				}
				else
					Index--;

			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (volume > applesActive && appleAddTime + 0.01f < Time.time)
		{
			AddApple();
		}
		else if (volume < applesActive)
		{
			RemoveApple();
		}


		if (transform.localScale != originalScale)
		{
			box.localScale = Vector3.Lerp(box.localScale, originalScale, Time.deltaTime*5f);
		}
	}
}
