using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class GameCanvasHandler : MonoBehaviour {

	public HudHandler hudHandler;
	public PauseMenuHandler pauseMenuHandler;



	// Use this for initialization
	void Start () {
		hudHandler = GetComponent<HudHandler>();
		pauseMenuHandler = GetComponent<PauseMenuHandler>();

		pauseMenuHandler.DeactivatePauseMenu();

	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Escape) && !GameMaster.Instance.isGameOver)
		{
			if (GameMaster.Instance.isPaused)
			{
				pauseMenuHandler.ResumeGame();
			}
			else
			{
				pauseMenuHandler.PauseGame();
			}
		}
		if (GameMaster.Instance.isGameOver && !pauseMenuHandler.PauseMenuActive)
		{
			pauseMenuHandler.ActivatePauseMenu(GameMaster.Instance.hasWon ? PauseMenuType.FinishMenu : PauseMenuType.DeadMenu);
		}
	}


}
