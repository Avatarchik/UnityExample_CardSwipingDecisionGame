using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

/// <summary>
/// ?????
/// Game -> Scripts
/// </summary>
public class GameStateManager : MonoBehaviour {

	[System.Serializable]
    public class mEvent : UnityEvent {}

    /// <summary>
    /// 메뉴를 전환하기 위해 스와이프를 계산하여 게임에서 첫번째 스와이프 이벤트를 만듭니다.
    /// </summary>
    [HideInInspector]
    public int swipeCounter = 0;

    /// <summary>
    /// 나 자신 인스턴스.
    /// </summary>
	public static GameStateManager instance;

    /// <summary>
    /// 게임 상태 열거형. 평소, 게임중, 게임오버.
    /// </summary>
	public enum Gamestate
	{
		idle,
		gameActive,
		gameOver
	}

	[Tooltip("게임의 실제 상태")]
	[ReadOnlyInspector]
    public Gamestate gamestate = Gamestate.idle;

    /// <summary>
    /// 현재 게임상태를 플레이어프랩스에서 꺼내오는 메서드.
    /// </summary>
	void loadGameState()
    {
		gamestate  = (Gamestate)PlayerPrefs.GetInt ("GameState") ;
	}

    /// <summary>
    /// 현재 게임상태를 플레이어프랩스에 저장하는 메서드.
    /// </summary>
	void saveGameState()
    {
		PlayerPrefs.SetInt("GameState",(int)gamestate);
	}

	void Awake()
    {
		instance = this;
        /// 현재 게임 상태를 플레이어프랩스에서 꺼내온다.
		loadGameState ();
	}

	void Start () {

		StartCoroutine (OneFrameDelayStartup ());
	}

	IEnumerator OneFrameDelayStartup()
    {
        /// 시작할 때 Awake-instance 링크 및 등록 때문에 게임을 시작하려면 최소한 한 프레임 지연이 필요합니다.
        yield return null;
		yield return null;
		GameStartup ();
	}


	void GameStartup()
    {
        /// 만약 게임상태가 게임오버 상태라면, 게임상태를 평소상태로 바꿔라.
		if (gamestate == Gamestate.gameOver)
        {
			gamestate = Gamestate.idle;
		}

		/// 만약 게임상태가 if we are idle we trigger the start of a new game
		if (gamestate == Gamestate.idle)
        {
			StartGame ();
		}
	}
		
	public void executeGameover(){
		gamestate = Gamestate.gameOver;

		if (gamesPlayedCounter != null) {
			gamesPlayedCounter.increase (1);	//log the number of played games
		}
			
		ValueManager.나자신.SaveAllMinMaxValues ();			//save min and max values for all values for the statistics tab
		HighScoreNameLinkerGroup.instance.generateLinks ();		
		CardStack.instance.resetCardStack ();					//reset the card stack

		saveGameState ();
		string currentSceneName = SceneManager.GetActiveScene ().name;
		SceneManager.LoadScene (currentSceneName);						//reload the scene for a clean startup of the game
	}

    /// <summary>
    /// 새로운 게임을 시작하기 위한 유니티 이벤트.
    /// 인스펙터에서 Game -> Values에 있는 스크립트 중 valueManager.setRandomValues를 연결한다.
    /// </summary>
	public mEvent OnNewGame;

	public mEvent OnFirstSwipe;

	public void swipe(){
		swipeCounter++;

		if (swipeCounter == 1) {
			OnFirstSwipe.Invoke ();
		}
	}


	void StartGame()
    {
		swipeCounter = 0;

        /// 게임상태가 평소상태이면
		if (gamestate == Gamestate.idle)
        {

			/// 게임시작준비를 한다.
			OnNewGame.Invoke();

			CountryNameGenerator.instance.actualizeTexts (true);
			GenderGenerator.instance.actualizeUI ();
			GameLogger.instance.clearGameLog ();			//delete the last game log for the new game

			gamestate = Gamestate.gameActive;
			saveGameState ();
		}
	}

	void OnDestroy(){
		saveGameState ();
	}

	public scoreCounter gamesPlayedCounter;
}
