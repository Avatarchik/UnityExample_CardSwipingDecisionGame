using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// 로저는 게임이 끝날 때 경기의 요약을 제공합니다.
/// </summary>
public class GameLogger : TranslatableContent {

    public static GameLogger instance;
    
    /// <summary>
    /// 게임 로그를 텍스트로 반환합니다.
    /// </summary>
    /// <returns></returns>
    public string getGameLog(){
		return buildResultText ();	
	}

    /// <summary>
    /// 정의된 텍스트 상자에 gamelog를 표시합니다.
    /// </summary>
    [HideInInspector]
    public Text gameLogText;

    public void showGameLogUI(){
		if (gameLogText != null) {
			gameLogText.text = buildResultText ();
		}
	}

	[System.Serializable]
	public class strList
	{
		public bool locked = false;
		public List <string> gameLogs;
	}

	[Tooltip("실제 게임의 족보.")]
	[ReadOnlyInspector]public strList logs;



	[Tooltip("'textBreakEvery'는 출력 문자열을보다 읽기 쉬운 텍스트로 형식화하기 위해 x 개까지의 줄 바꿈을 생성합니다.")]
	public int textBreakEvery = 1;

	string buildResultText(){
		string result = "";
		int lineCnt = 0;

		foreach (string s in logs.gameLogs) {
			result = result + TranslationManager.translateIfAvail(s) + " ";

			lineCnt++;
			if (lineCnt >= textBreakEvery) {
				result = result + "\n\n";
				lineCnt = 0;
			}
		}
		return result;
	}


	void Awake(){
		instance = this;
	}

	void Start(){
		logs.gameLogs = new List<string> ();
		logs.gameLogs.Clear();
		loadGameLogs ();
		TranslationManager.instance.registerTranslateableContentScript (this);
	}

    /// <summary>
    /// 게임 로그를 강제로로드합니다. 이것은 스크립트 시작시 자동으로 수행됩니다.
    /// </summary>
    /// <returns></returns>
    public bool loadGameLogs(){
		string json = PlayerPrefs.GetString ("gameLog");
		if (string.IsNullOrEmpty (json)) {
			return false;
		} else {
			JsonUtility.FromJsonOverwrite (json, logs);

			return true;
		}
	}

    /// <summary>
    /// 로그를 잠그면 스크립트가 새 로그를 추가하지 못합니다.
    /// 실행중인 게임을로드하면 카드에 이미 기록 된 로그가 추가 될 수 있으므로이 작업이 필요합니다.
    /// </summary>
    /// <param name="doLock"></param>
    public void lockOutput(bool doLock){
		logs.locked = doLock;
	}

	public void saveGameLogs(){
		string json = JsonUtility.ToJson (logs);
		PlayerPrefs.SetString ("gameLog", json);
	}

    /// <summary>
    /// logger가 잠겨 있지 않은 경우 이 게임에 대한 새 로그 항목을 추가하십시오.
    /// </summary>
    /// <param name="log"></param>
    public void addGameLog(string log){
		if (!string.IsNullOrEmpty (log) && logs.locked == false) {

			string txt = log;
			logs.gameLogs.Add (txt);
			saveGameLogs ();

		}
	}
        
    /// <summary>
    /// 'clearGameLog'를 호출하면 게임 로그가 삭제되고 잠금이 제거됩니다.
    /// </summary>
    public void clearGameLog(){
		logs.gameLogs.Clear ();
		logs.locked = false;
		saveGameLogs ();
	}

    /// <summary>
    /// 번역 가능한 모든 용어를 반환하십시오.
    /// </summary>
    /// <returns></returns>
    public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();

		Debug.LogWarning ("'GameLogger'의 문자열은 직접적으로 나열되지 않으므로 번역 용어 목록에 완전히 추가 할 수 없습니다. ");

		return terms;
	}
}
