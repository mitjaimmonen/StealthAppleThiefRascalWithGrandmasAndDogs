using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class PlayerPrefsManager {

	public static void SetLevel(int levelIndex)
	{
		PlayerPrefs.SetInt("Level", levelIndex);
	}
	public static int GetLevel()
	{
		if (!PlayerPrefs.HasKey("Level"))
			PlayerPrefs.SetInt("Level", 0);
		
		return PlayerPrefs.GetInt("Level");
	}

	public static void SetTutorialDone(bool tutorialDone)
	{
		//1 = enable tutorial
		PlayerPrefs.SetInt("Tutorial",tutorialDone ? 1 : 0);
	}
	public static bool GetTutorialDone()
	{
		if (!PlayerPrefs.HasKey("Tutorial"))
			PlayerPrefs.SetInt("Tutorial", 0);
		
		return PlayerPrefs.GetInt("Tutorial") == 1;
	}

	public static void SetMusicVolume(int volume)
	{
		PlayerPrefs.SetInt("MusicVolume", volume);
	}

	public static int GetMusicVolume()
	{
		if (!PlayerPrefs.HasKey("MusicVolume"))
			PlayerPrefs.SetInt("MusicVolume", 80);

		return PlayerPrefs.GetInt("MusicVolume");
	}

	public static void SetSfxVolume(int volume)
	{
		PlayerPrefs.SetInt("SfxVolume", volume);
	}

	public static int GetSfxVolume()
	{
		if (!PlayerPrefs.HasKey("SfxVolume"))
			PlayerPrefs.SetInt("SfxVolume", 80);

		return PlayerPrefs.GetInt("SfxVolume");
	}
}
