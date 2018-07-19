using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// 옵션창에서 사용자가 배경음악(Music)을 켜고 끄는 행위에 따라 'MusicPlayer' 스크립트를 활성화비활성화하고, 그에 따른 토글버튼에 텍스트를 표시해주는 명령들을 처리하는 클래스
/// Game -> Music
/// </summary>
public class UniToggle : MonoBehaviour {

    /// <summary>
    /// 상황에 따라 활성화되거나 비활성화되어야 하는 스크립트를 연결하기 위한 변수.
    /// 사용자가 옵션창에서 '뮤직' 토글을 눌렀을때 'On''Off'된다. 이때 여기에 연결된 스크립트를 활성화/비활성화시키는 방식으로 제어한다.
    /// 여기서는 Game -> Music -> MusicPlayer 스크립트를 할당한다.
    /// 인스펙터에서 집어넣을때 주의할점은 게임씬에서 끌어다 할당하면 안되고, 인스펙터에서 MusicPlayer스크립트를 드래그 해서 할당해야 한다.
    /// </summary>
	public MonoBehaviour 타겟스크립트;

    /// <summary>
    /// 'MusicPlayer' 스크립트가 게임 첫 시작시 상태에 대해 정의하는 메서드.
    /// 게임시작시에는 활성화되어 있어야 하니깐 true값을 주자.
    /// </summary>
    public bool 최초시작시활성화여부 = true;

    /// <summary>
    /// 현재 뮤직(배경음악)이 켜진 상태인지 꺼진 상태인지 현재 상태를 플레이어프랩스에 저장하기 위한 키값
    /// </summary>
    public string 키값 = "온버튼스크립트";

	/// <summary>
    /// 플레이어가 옵션창에서 온오프시키는 '뮤직' 토글을 여기에 연결한다. 
    /// </summary>
	public Toggle 토글옵션;

    /// <summary>
    /// 옵션창에서 뮤직이 온오프될때마다 토글상태를 표시하기 위한 텍스트. 
    /// </summary>
    public Text 토글창표시텍스트;                

    /// <summary>
    /// 뮤직이 켜진 상태일때 토글창표시텍스트에 넣어주기 위해 미리 정의해둔 텍스트.
    /// </summary>
    public string 켜졌을때텍스트 = "켜짐";          

    /// <summary>
    /// 뮤직이 꺼진 상태일때 토글창표시텍스트에 넣어주기 위해 미리 정의해둔 텍스트.
    /// </summary>
    public string 꺼졌을때텍스트 = "꺼짐";				
	
    //privates
	private bool 스크립트활성화여부 = false;

	void Start () {
		
		if (PlayerPrefs.HasKey (키값) == false) /// 플레이어프랩스에 키값이 없다면.
        {
			스크립트활성화여부 = 최초시작시활성화여부;

            스크립트현재상태저장 ();
		}
        else
        {
			스크립트현재상태가져오기();
		}

		testScriptStateOnStart ();

		/// 스크립트활성화 또는 비활성화에 따른 각각 처리해야 할 일을 처리하자.
		스크립트활성화비활성화때처리하는명령 ();
	}

	/*
	 * Function setScriptState
	 * Call this function by a button, external script or even from a toggle to set the 
	 * state of the script. The state will be automatically saved and 
	 * loaded on next start of this script.
	 * 
	 * */
	public void setScriptState(bool enabled)
    {
		//Debug.Log ("set state to " + enabled.ToString ());
		스크립트활성화여부 = enabled;
		스크립트현재상태저장 ();
		스크립트활성화비활성화때처리하는명령 ();
	}

	/*
	 * Function invertScriptState
	 * Call this function mainly by a button to invert the 
	 * state of the script (ON-> OFF or OFF-> ON). 
	 * Therefore you can switch the script on and off with a single button.
	 * 
	 * The state will be automatically saved and 
	 * loaded on next start of this script.
	 * 
	 * */
	public void btnOnly_invertScriptState()
    {
		스크립트활성화여부 = !스크립트활성화여부;
		스크립트현재상태저장 ();
		스크립트활성화비활성화때처리하는명령 ();
	}


	/// <summary>
    /// 스크립트가 활성화상태이면 플레이어프랩스에 1을 저장하고, 비활성화상태라면 0을 저장한다.
    /// 즉, 플레이어프랩스에 1이 저장되어 있다면, 배경음악이 플레이되는 상태이고, 0이면 배경음악이 중단된 상태라는 것이다.
    /// </summary>
	void 스크립트현재상태저장()
    {
		if (스크립트활성화여부 == true)
        {
			PlayerPrefs.SetInt (키값, 1);
		}
        else
        {
			PlayerPrefs.SetInt(키값,0);
		}
	}

    /// <summary>
    /// 플레이어프랩스에 1이 저장되어 있다면, 배경음악이 플레이되는 상태이고, 0이면 배경음악이 중단된 상태라는 것이다.
    /// 따라서 플레이어프랩스에 저장된 데이터를 가져와서 해당 값에 맞게 스크립트활성화여부 변수에 true 또는 false값을 넣어준다.
    /// </summary>
    void 스크립트현재상태가져오기()
    {
		int 현재상태 = PlayerPrefs.GetInt (키값); 

        if (현재상태 == 0)
        {
			스크립트활성화여부 = false;
		}
        else
        {
			스크립트활성화여부 = true;
		}
	}

	/// <summary>
    /// 스크립트가 활성화 또는 비활성때 처리해야 하는 명령들.
    /// </summary>
	void 스크립트활성화비활성화때처리하는명령(){

        /// 타겟스크립트 활성화상태도 변경하고
        if (타겟스크립트 != null) /// 타겟 스크립트가 빈값이 아니면
        {
			타겟스크립트.enabled = 스크립트활성화여부; /// 타겟스크립트의 활성화여부를 스크립트활성화여부의 변수가 가지고 있는 값을 할당한다.
		}
        else
        {
            Debug.LogError(name + "오브젝트의 스크립트 활성 / 비활성에 대한 스크립트 설정이 불가능합니다."); /// 콘솔에 에러메시지를 출력한다.
		}

        /// 토글옵션 상태로 변경하고
		if (토글옵션 != null) /// 토글버튼이 연결된 상태라면
        {
			토글옵션.isOn = 스크립트활성화여부; /// 연결된 토글버튼 ON/OFF 상태를 스크립트활성화여부의 변수가 가지고 있는 값을 할당한다.
		}

		/// 토글
		if(토글창표시텍스트 !=null) /// 토글창표시텍스트가 연결된 상태라면
        {
			if(스크립트활성화여부 == true) /// 스크립트가 활성화된 상태라면
            {
				토글창표시텍스트.text = 켜졌을때텍스트; /// 토글텍스트에 "켜짐"이라고 표시한다.
			}
            else /// 스크립트가 비활성화된 상태라면
            {
				토글창표시텍스트.text = 꺼졌을때텍스트; /// 토글텍스트에 "꺼짐"이라고 표시한다.
			}
		}
	}

	/// <summary>
    /// 테스트용 스크립트.
    /// </summary>
	void testScriptStateOnStart(){
		if (타겟스크립트 != null) {
			if(타겟스크립트.enabled == true){
				Debug.LogError("Linked Monobehaviour is enabled at startup. This can cause Runtime and start problems.");
			}else{
				//Script is available and not activated at start-up. This is ok.
			}
		}else {
			Debug.LogError("Script for Enable/Disable not settable on " + name + " object.");
		}
	}
}
