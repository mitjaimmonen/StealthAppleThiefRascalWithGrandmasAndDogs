using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
	Menu,
	Game
}
public class GameMaster : MonoBehaviour {


	public HudHandler hudHandler;
	public Player player;
	public GameState gameState;
	public int levelNumber;
	private static GameMaster _instance;
	public static GameMaster Instance
	{
		get
		{
			if (_instance == null)
				return null;
			return _instance;
		}
	}
	// Use this for initialization
	void Awake () 
	{

		if (GameObject.FindGameObjectsWithTag("GameMaster").Length > 1)
		{
			Destroy(this.gameObject);
		}

		_instance = this;
		DontDestroyOnLoad(this.gameObject);



		if (gameState == GameState.Game)
		{
			player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

		}
		

	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
