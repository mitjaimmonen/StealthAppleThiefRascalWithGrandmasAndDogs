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
	public Canvas optionsCanvas;
	public GameObject defaultStartSelection, defaultLevelSelection, defaultOptionsSelection;

	public int unlockedLevel = 0;

	Transform selecetedObject;

	// Use this for initialization
	void Start () {
		Initialize();
	}
	
	// Update is called once per frame
	void Update () {
		if (GameMaster.Instance.gameState != GameState.Game)
		{
			SetSelections();

		}
	}

	public void Initialize()
	{
		Reset();
	}

	public void Reset()
	{
		StartCanvas.gameObject.SetActive(true);
		levelCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(false);
		if (PlayerPrefs.HasKey("Level"))
			unlockedLevel = PlayerPrefs.GetInt("Level");
		else
			unlockedLevel = 0;
	}
	void SetSelections()
	{
		if (EventSystem.current.currentSelectedGameObject == null)
		{
			if (menuState == MainMenuState.startMenu)
				EventSystem.current.SetSelectedGameObject(defaultStartSelection);
			if (menuState == MainMenuState.levelMenu)
				EventSystem.current.SetSelectedGameObject(defaultLevelSelection);
		}
		else if (selecetedObject != null && selecetedObject != EventSystem.current.currentSelectedGameObject.transform)
		{
			Renderer rend = selecetedObject.GetComponentInChildren<Renderer>();
			var pos =rend.transform.localPosition;
			pos.z = -0f;
			rend.transform.localPosition = pos;
			selecetedObject = EventSystem.current.currentSelectedGameObject.transform;

		}
		else if (selecetedObject != null)
		{
			Renderer rend = selecetedObject.GetComponentInChildren<Renderer>();
			var pos = rend.transform.localPosition;
			pos.z = -50f;
			rend.transform.localPosition = pos;
		}
		else
			selecetedObject = EventSystem.current.currentSelectedGameObject.transform;
		


		Debug.Log(selecetedObject);
	}


	public void PointerEnter(GameObject obj)
	{
		EventSystem.current.SetSelectedGameObject(obj);
	}

	public void StartMenuStart()
	{
		StartCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(false);
		levelCanvas.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultLevelSelection);
		menuState = MainMenuState.levelMenu;

	}
	public void StartMenuOptions()
	{
		StartCanvas.gameObject.SetActive(false);
		levelCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultOptionsSelection);
		menuState = MainMenuState.optionsMenu;
	}

	public void OptionsMenuBack()
	{
		levelCanvas.gameObject.SetActive(false);
		optionsCanvas.gameObject.SetActive(false);
		StartCanvas.gameObject.SetActive(true);
		EventSystem.current.SetSelectedGameObject(defaultStartSelection);
		menuState = MainMenuState.startMenu;
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
