using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
	Menu,
	Game
}
public class GameMaster : MonoBehaviour {

	public CameraHandler _cameraHandler;
	public GameCanvasHandler _gameCanvas;
	public MenuMaster _menuMaster;
	public Player _player;
	public GameState gameState;
	public int levelNumber;
	public bool isPaused = false;
	public bool isGameOver = false;
	public bool hasWon = false;

	private static GameMaster _instance;
	public static GameMaster Instance
	{
		get
		{
			return _instance;
		}
	}

	public CameraHandler cameraHandler
	{
		get { 
			if (_cameraHandler == null)
				_cameraHandler = Camera.main.GetComponent<CameraHandler>();
			
			return _cameraHandler;
		}
	}
	public GameCanvasHandler gameCanvas
	{
		get {
			if (_gameCanvas == null)
			{
				if (GameObject.FindGameObjectWithTag("GameCanvas"))
					_gameCanvas = GameObject.FindGameObjectWithTag("GameCanvas").GetComponent<GameCanvasHandler>();
			}

			return _gameCanvas;
		}
	}
	public Player player
	{
		get {
			if (_player == null)
			{
				if (GameObject.FindGameObjectWithTag("Player"))
					_player = GameObject.FindGameObjectWithTag("Player").GetComponent<Player>();

			}
			return _player;
		}
	}

	public MenuMaster menuMaster
	{
		get {
			if (_menuMaster == null)
			{
				if ( GameObject.FindGameObjectWithTag("MenuMaster"))
					_menuMaster = GameObject.FindGameObjectWithTag("MenuMaster").GetComponent<MenuMaster>();
			}
			return _menuMaster;
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

		SceneManager.sceneLoaded += OnSceneLoaded;

		Initialize();

	}
	void Update()
	{
		if (_instance ==null)
			_instance = this;

		if (isPaused)	
			Time.timeScale = 0;
		else
			Time.timeScale = 1;		
	}
	void OnSceneLoaded (Scene scene, LoadSceneMode loadMode)
	{
		Initialize();
	}

	void Initialize()
	{
		_instance = this;

		if (SceneManager.GetActiveScene() == SceneManager.GetSceneByBuildIndex(0))
			gameState = GameState.Menu;
		else
			gameState = GameState.Game;
		
		if (gameState == GameState.Game)
		{
			Reset();
		}
		if (gameState == GameState.Menu)
		{
			menuMaster.Initialize();
		}
	}
	
	
	public void Reset()
	{
		hasWon = false;
		isGameOver = false;
		isPaused = false;

	}

	public void FinishGame()
	{
		GameMaster.Instance.hasWon = true;
		GameMaster.Instance.isGameOver = true;
		GameMaster.Instance.isPaused = true;

		if (!PlayerPrefs.HasKey("Level"))
			PlayerPrefs.SetInt("Level", 0);
		PlayerPrefs.SetInt("Level", Mathf.Max(PlayerPrefs.GetInt("Level"), GameMaster.Instance.levelNumber));
	}
	public void RestartLevel()
	{
		Reset();
		SceneManager.LoadScene(SceneManager.GetActiveScene().name);
	}
	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
	}
	public void NextLevel()
	{
		levelNumber++;
		SceneManager.LoadScene("Level" + levelNumber);
	}
}
