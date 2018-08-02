using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum GameState
{
    Menu,
    Game
}

public class GameMaster : MonoBehaviour
{

    public CameraHandler _cameraHandler;
    public GameCanvasHandler _gameCanvas;
    MenuMaster _menuMaster;
    ParticlesMaster _particleMaster;
    public Player _player;
    public GameState gameState;
    public int levelNumber;
    public bool isPaused = false;
    public bool isGameOver = false;
    public bool hasWon = false;

    public bool inAlertMode = false;
    public delegate void AlertDelegate(Transform target);
    public event AlertDelegate alertEvent;

    public delegate void ToggleHiding(Transform position, bool toggle);
    public event ToggleHiding OnHide;

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
        get
        {
            if (_cameraHandler == null)
                _cameraHandler = Camera.main.GetComponent<CameraHandler>();

            return _cameraHandler;
        }
    }
    public GameCanvasHandler gameCanvas
    {
        get
        {
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
        get
        {
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
        get
        {
            if (_menuMaster == null)
            {
                if (GameObject.FindGameObjectWithTag("MenuMaster"))
                    _menuMaster = GameObject.FindGameObjectWithTag("MenuMaster").GetComponent<MenuMaster>();
            }
            return _menuMaster;
        }
    }

    public ParticlesMaster ParticleMaster
    {
        get
        {
            if (_particleMaster == null)
            {
                _particleMaster = GetComponent<ParticlesMaster>();
            }
            return _particleMaster;
        }
    }



    // Use this for initialization
    void Awake()
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
        if (_instance == null)
            _instance = this;

        if (isPaused)
            Time.timeScale = 0;
        else
            Time.timeScale = 1;
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode loadMode)
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

    public bool PlayerIsHiding()
    {
        return player.IsInvisible;
    }

	public bool HasScene(string sceneName)
	{
		return Application.CanStreamedLevelBeLoaded(sceneName);
	}

	public void EndGame(bool win)
	{
		GameMaster.Instance.hasWon = win;
		GameMaster.Instance.isGameOver = true;
		GameMaster.Instance.isPaused = true;


		if (hasWon)
		{
			PlayerPrefsManager.SetLevel(Mathf.Max(PlayerPrefs.GetInt("Level"), GameMaster.Instance.levelNumber+1));
		}
		else
		{
			gameCanvas.hudHandler.DeactivateHud();
			gameCanvas.pauseMenuHandler.ActivatePauseMenu(win ? PauseMenuType.FinishMenu : PauseMenuType.DeadMenu);
		}
	}
	public void LoadScene(string sceneName)
	{
		SceneManager.LoadScene(sceneName);
        Reset();
	}
	public void NextLevel()
	{
		levelNumber++;
        if (HasScene("Level" + levelNumber))
		    SceneManager.LoadScene("Level" + levelNumber);
        else
            SceneManager.LoadScene("MainMenu");
        Reset();
	}

    public void FinishGame()
    {
        GameMaster.Instance.hasWon = true;
        GameMaster.Instance.isGameOver = true;
        GameMaster.Instance.isPaused = true;

        PlayerPrefsManager.SetLevel(Mathf.Max(PlayerPrefs.GetInt("Level"), GameMaster.Instance.levelNumber));
    }
    public void RestartLevel()
    {
        Reset();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    public void Reset()
    {
        hasWon = false;
        isGameOver = false;
        isPaused = false;
    }

  
    //gameplay states things
    public void OnAlertMode(Transform target)
    {
        if (alertEvent != null && !inAlertMode)
        {
            alertEvent(target);
            inAlertMode = true;
        }
    }

    public void setAlertMode(bool toggle)
    {
        inAlertMode = toggle;
    }

    public void playerHide(Transform pos, bool toggle)
    {
        if (OnHide != null)
        {
            OnHide(pos, toggle);
        }
    }

}
