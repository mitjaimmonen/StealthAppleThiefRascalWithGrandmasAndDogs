using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;


public enum MainMenuState
{
	startMenu,
	levelMenu,
	optionsMenu
}

public class MenuMaster : MonoBehaviour {

	public MainMenuState menuState;
	public Canvas StartCanvas;
	public Canvas levelCanvas;
	public TextMesh applyText;
	public TextMesh resetPrefsText;
	public List<TextMesh> levelTexts = new List<TextMesh>();
	public Canvas optionsCanvas;
	public GameObject defaultStartSelection, defaultLevelSelection, defaultOptionsSelection;
	public OptionsVolumeHandler musicVolumeHandler;
	public OptionsVolumeHandler sfxVolumeHandler;

	public int unlockedLevel = 0;

	Transform selecetedObject;


	bool resetPrefsOnApply = false;
	bool setSfxOnApply;
	bool setMusicOnApply;

	int musicVolume;
	int sfxVolume;

	bool settingMusicVolume;
	bool settingAudioVolume;
	int volumeOffset = 0;
	float timer = 0;

	List<Transform> lerpUp = new List<Transform>();
	List<Transform> lerpDown = new List<Transform>();

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameMaster.Instance.gameState != GameState.Game)
		{
			SetSelections();

			if (settingAudioVolume && timer>0.05f)
			{
				timer = 0;
				sfxVolume = sfxVolumeHandler.SetVolume(volumeOffset, true);
			}
			if (settingMusicVolume && timer>0.05f)
			{
				timer = 0;
				musicVolume = musicVolumeHandler.SetVolume(volumeOffset, true);

			}
			timer += Time.deltaTime;

			for (int i = 0; i < lerpUp.Count; i++)
			{
				Vector3 newpos = new Vector3(lerpUp[i].localPosition.x,lerpUp[i].localPosition.y, -50f);
				lerpUp[i].localPosition = Vector3.Lerp(lerpUp[i].localPosition, newpos, Time.deltaTime * 10f);
				if (lerpUp[i].localPosition == newpos || lerpDown.Contains(lerpUp[i]))
					lerpUp.Remove(lerpUp[i]);
			}
			for (int i = 0; i < lerpDown.Count; i++)
			{
				Vector3 newpos = new Vector3(lerpDown[i].localPosition.x,lerpDown[i].localPosition.y, 0);
				lerpDown[i].localPosition = Vector3.Lerp(lerpDown[i].localPosition, newpos, Time.deltaTime * 10f);
				if (lerpDown[i].localPosition == newpos)
					lerpDown.Remove(lerpDown[i]);
			}

		}
	}

	public void Initialize()
	{
		optionsCanvas.gameObject.SetActive(false);
		levelCanvas.gameObject.SetActive(false);
		StartCanvas.gameObject.SetActive(true);
		Reset();
	}

	public void Reset()
	{
		resetPrefsText.text = "Reset \n saved data";
		applyText.text = "Apply";
		StartCanvas.gameObject.SetActive(true);
		levelCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(false);
		unlockedLevel = PlayerPrefsManager.GetLevel();
		resetPrefsOnApply = false;
		setMusicOnApply = false;
		setSfxOnApply = false;
	}
	void SetSelections()
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			if (menuState == MainMenuState.startMenu)
				EventSystem.current.SetSelectedGameObject(defaultStartSelection);
			if (menuState == MainMenuState.levelMenu)
				EventSystem.current.SetSelectedGameObject(defaultLevelSelection);
			if (menuState == MainMenuState.optionsMenu)
				EventSystem.current.SetSelectedGameObject(defaultOptionsSelection);
		}
		else if (selecetedObject != null && selecetedObject != EventSystem.current.currentSelectedGameObject.transform)
		{
			selecetedObject = EventSystem.current.currentSelectedGameObject.transform;
		}
		else if (selecetedObject == null)
			selecetedObject = EventSystem.current.currentSelectedGameObject.transform;
		
	}


	public void PointerEnter(GameObject obj)
	{
		EventSystem.current.SetSelectedGameObject(obj);
		Renderer rend = obj.GetComponentInChildren<Renderer>();
		if (lerpDown.Contains(rend.transform))
			lerpDown.Remove(rend.transform);
		lerpUp.Add(rend.transform);
		Debug.Log("Pointer enter: " + obj);


	}
	public void PointerExit(GameObject obj)
	{
		Debug.Log("Pointer exit: " + obj);


		Renderer rend = obj.GetComponentInChildren<Renderer>();
		if (lerpUp.Contains(rend.transform))
			lerpUp.Remove(rend.transform);
		lerpDown.Add(rend.transform);
	}

	public void StartMenuStart()
	{
		unlockedLevel = PlayerPrefsManager.GetLevel();
		
		for(int i = 0; i < levelTexts.Count; i++)
		{
			if (!GameMaster.Instance.HasScene("Level" + i))
				levelTexts[i].text = "Coming \n soon!";
			else if (i > unlockedLevel)
				levelTexts[i].text = "Locked";
			else
				levelTexts[i].text ="Level \n" + i; 
		}

		StartCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(false);
		levelCanvas.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultLevelSelection);
		menuState = MainMenuState.levelMenu;

	}
	public void StartMenuOptions()
	{
		applyText.text = "Apply";
		musicVolume = musicVolumeHandler.SetVolume(PlayerPrefsManager.GetMusicVolume(), false);
		sfxVolume = sfxVolumeHandler.SetVolume(PlayerPrefsManager.GetSfxVolume(), false);
		resetPrefsOnApply = false;
		setSfxOnApply = false;
		setMusicOnApply = false;

		StartCanvas.gameObject.SetActive(false);
		levelCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultOptionsSelection);
		menuState = MainMenuState.optionsMenu;
	}

	public void OptionsMenuBack()
	{

		applyText.text = "Apply";
		resetPrefsOnApply = false;
		setSfxOnApply = false;
		setMusicOnApply = false;
		musicVolume = musicVolumeHandler.SetVolume(PlayerPrefsManager.GetMusicVolume(), false);
		sfxVolume = sfxVolumeHandler.SetVolume(PlayerPrefsManager.GetSfxVolume(), false);

		levelCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(false);
		StartCanvas.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultStartSelection);
		menuState = MainMenuState.startMenu;

		
	}

	public void OptionsMenuMusicBtnDown(int volOffset)
	{
		applyText.text = "Apply";
		volumeOffset = volOffset;
		settingMusicVolume = true;
		setMusicOnApply = true;
	}
	public void OptionsMenuAudioBtnDown(int volOffset)
	{
		applyText.text = "Apply";
		volumeOffset = volOffset;
		settingAudioVolume = true;
		setSfxOnApply = true;
	}
	public void OptionsMenuMusicBtnUp()
	{
		applyText.text = "Apply";
		settingMusicVolume = false;
	}
	public void OptionsMenuAudioBtnUp()
	{
		applyText.text = "Apply";
		settingAudioVolume = false;
	}
	public void OptionsMenuApply()
	{
		if (resetPrefsOnApply)
			ResetPrefs();
		if (setMusicOnApply)
			PlayerPrefsManager.SetMusicVolume(musicVolume);
		if (setSfxOnApply)
			PlayerPrefsManager.SetSfxVolume(sfxVolume);

		musicVolumeHandler.SetVolume(musicVolume, false);
		sfxVolumeHandler.SetVolume(sfxVolume, false);
		
		applyText.text = "Applied!";
		resetPrefsText.text = "Reset \n saved data";
	}

	public void OptionsMenuResetPrefs()
	{
		applyText.text = "Apply";
		resetPrefsOnApply = !resetPrefsOnApply;
		if (resetPrefsOnApply)
			resetPrefsText.text = "Saved data will\n be erased on Apply!";
		else
			resetPrefsText.text = "Reset \n saved data";


	}
	void ResetPrefs()
	{
		Debug.Log("RESET");
		PlayerPrefsManager.ResetAll();
		musicVolume = musicVolumeHandler.SetVolume(PlayerPrefsManager.GetMusicVolume(), false);
		sfxVolume = sfxVolumeHandler.SetVolume(PlayerPrefsManager.GetSfxVolume(), false);
	}


	public void LevelMenuBack()
	{
		levelCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(false);
		StartCanvas.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultStartSelection);
		menuState = MainMenuState.startMenu;
	}

	public void LevelMenuLoadLevel(int level)
	{
		if (level <= unlockedLevel)
		{
			SceneManager.LoadScene("Level" + level);
			GameMaster.Instance.levelNumber = level;
		}
	}

	public void MenuQuit()
	{
		Application.Quit();
	}
}
