using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PauseMenuType
{
	PauseMenu,
	FinishMenu,
	DeadMenu
}
public class PauseMenuHandler : MonoBehaviour {

	
	public GameObject pauseMenuPanel;
	public GameObject DeadMenuPanel;
	public GameObject DefaultMenuPanel;
	public GameObject FinishMenuPanel;
	public PauseMenuType currentMenuType;


	GameCanvasHandler gameCanvas;


	void Start()
	{
		gameCanvas = GetComponentInParent<GameCanvasHandler>();
	}


	public bool PauseMenuActive
	{
		get { return pauseMenuPanel.activeSelf; }
	}

	public void DeactivatePauseMenu()
	{
		GameMaster.Instance.cameraHandler.postProcess.SetDepthOfField(0);
		pauseMenuPanel.SetActive(false);
	}
	public void ActivatePauseMenu(PauseMenuType menuType)
	{
		if (currentMenuType == menuType && pauseMenuPanel.activeSelf == true)
			return; //Already active.


		GameMaster.Instance.cameraHandler.postProcess.SetDepthOfField(-0.75f);
		currentMenuType = menuType;

		if (menuType == PauseMenuType.PauseMenu)
		{
			pauseMenuPanel.SetActive(true);
			DeadMenuPanel.SetActive(false);
			FinishMenuPanel.SetActive(false);
			DefaultMenuPanel.SetActive(true);
		}
		else if (menuType == PauseMenuType.FinishMenu)
		{
			pauseMenuPanel.SetActive(true);
			DefaultMenuPanel.SetActive(false);
			DeadMenuPanel.SetActive(false);
			FinishMenuPanel.SetActive(true);

		}
		else if (menuType == PauseMenuType.DeadMenu)
		{
			pauseMenuPanel.SetActive(true);
			DeadMenuPanel.SetActive(true);
			FinishMenuPanel.SetActive(false);
			DefaultMenuPanel.SetActive(false);

		}
	}

	
	public void ResumeGame()
	{
		if (GameMaster.Instance.isPaused)
		{
			gameCanvas.hudHandler.ActivateHud();
			DeactivatePauseMenu();
			GameMaster.Instance.isPaused = false;
		}
	}
	public void PauseGame()
	{
		if (!GameMaster.Instance.isPaused)
		{
			gameCanvas.hudHandler.DeactivateHud();
			ActivatePauseMenu(PauseMenuType.PauseMenu);
			GameMaster.Instance.isPaused = true;
		}
	}

	public void RestartLevel()
	{
		GameMaster.Instance.RestartLevel();
	}
	public void NextLevel()
	{
		GameMaster.Instance.NextLevel();
	}
	public void LoadScene(string sceneName)
	{
		GameMaster.Instance.LoadScene(sceneName);
		
	}
}
