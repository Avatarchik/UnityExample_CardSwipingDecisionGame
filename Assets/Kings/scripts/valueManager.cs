using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/// <summary>
/// valueManager 클래스는 게임의 모든 값을 수집합니다.
/// valueManager 클래스는 시작 시 관리자에 액세스하고 여기에 등록하며, 수동 연결은 필요하지 않습니다.
/// Game - Values
/// </summary>
public class ValueManager : MonoBehaviour {

    /// <summary>
    /// 싱글톤 사용을 위해 static으로 엑서스 가능한 나 자신의 인스턴스를 만든다.
    /// </summary>
	public static ValueManager 나자신;
    
	void Awake()
    {
        #region 나자신의 싱글톤 객체를 만들어서 할당한다.
        if (나자신 == null)
        {
            나자신 = this;
        }
        else
        {
            Debug.LogError("싱글톤 사용해야합니다. 오류가 발행하기 쉽습니다. ValueManager 클래스의 Awake()를 수정하세요.");
        }
        #endregion

        awaked = true;
	}

    /// <summary>
    /// ValueScript 리스트를 하나 만들어서 게임에서 사용되는 모든 값을 리스트에 추가한다.
    /// Game씬의 Values오브젝트 밑의 자식 오브젝트들이 이 리스트에 모두 등록된다.
    /// </summary>
	[Tooltip("게임에서사용되는 모든 값을 리스트에 추가")]
	[HideInInspector] /// 인스펙터에서 변수를 감추고 싶을때
    public List <ValueScript> 볼륨스크립트리스트;

    /// <summary>
    /// 'ValueScript' 스크립트를 리스트에 등록되도록 하는 메서드.
    /// 결과적으로 Game씬의 Values오브젝트 밑의 자식 오브젝트들이 이 리스트에 모두 등록된다.
    /// </summary>
    /// <param name="볼륨스크립트">ValueScript 클래스 타입의 객체</param>
    public void ValueScript리스트에추가(ValueScript 볼륨스크립트)
    {
		볼륨스크립트리스트.Add (볼륨스크립트);
	}

    /// <summary>
    /// 목록을 통해 첫번째 피팅 값 스크립트 피팅 검색
    /// 주의 : 스크립트 실행 순서때문에 'Start()' 또는 'Awake()'에서 호출하면 이 함수가 실패 할 수 있습니다.
    /// 따라서 적어도 하나의 프레임 지연으로 호출하십시오.
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
	public ValueScript 첫번째피팅값가져오기(ValueDefinitions.값정의 정의)
    {
		foreach (ValueScript 게임에서 in 볼륨스크립트리스트)
        {
			if (게임에서.내역활 == 정의)
            {
				return 게임에서;
			}
		}
		return null;
	}

	/// <summary>
    /// 스크립트의 최소값과 최대값을 테스트하고 저장하십시오.
    /// 이것은 게임 후에 최대 연령, 건강 등의 통계를 저장하는데 사용됩니다.
    /// </summary>
	public void SaveAllMinMaxValues()
    {
		foreach (ValueScript 게임에서 in 볼륨스크립트리스트)
        {
			게임에서.최소최대값저장 ();
		}
	}

    /// 카드의 조건을 테스트하십시오. 예제 조건 : 게임 내 연령 (또는 다른 값)이 10-16 사이 인 경우에만이 카드를 뽑습니다.
    /// methode는 조건이 충족 될 경우 true를 반환합니다.그렇지 않으면 거짓.
    /// </summary>
    /// <param name="cond"></param>
    /// <param name="showDebug"></param>
    /// <returns></returns>
    public bool GetConditionMet(EventScript.콘디션 cond, bool showDebug = false){
		
		foreach (ValueScript vs in 볼륨스크립트리스트) {
			if (vs.내역활 == cond.정의) {

				if ((vs.플레이어프랩스데이터 <= cond.valueMax && vs.플레이어프랩스데이터 >= cond.valueMin)) {
					if(showDebug==true){
					 	Debug.Log ("True: " +cond.valueMin.ToString() + " " + vs.플레이어프랩스데이터.ToString() + " " +cond.valueMax.ToString()   );
						Debug.Log (vs.gameObject.name);
					}
					return true;
				} else {
					if (showDebug == true) {
						Debug.Log ("False: " + cond.valueMin.ToString () + " " + vs.플레이어프랩스데이터.ToString () + " " + cond.valueMax.ToString ());
						Debug.Log (vs.gameObject.name);
					}
					return false;
				}
			}
		}

		return false;
	}

	//check for a set of conditions if everything is met
	public bool AreConditinsForResultMet(EventScript.콘디션[] cond){

		bool conditionOk = true;

		foreach (EventScript.콘디션 c in cond) {
			if (GetConditionMet (c) == true) {
				//condition is ok.
			} else {
				conditionOk = false;
				break;
			}
		}

		return conditionOk;
	}

	//Change a value. Example: Reduce the health of the player by adding -5
	public void changeValue(ValueDefinitions.값정의 type, float valueAdd){
	bool found = false;
		foreach (ValueScript vs in 볼륨스크립트리스트) {
			if (vs.내역활 == type) {

				if (found == true) {
					Debug.LogWarning ("Multiple values of the same type detected: " + type.ToString ());
				}

				found = true;
				vs.addValue (valueAdd);

				//display the value change to the user
				if (vs.UI조절.showActualization == true) {
					InfoDisplay.instance.addDisplay (vs.UI조절.아이콘이미지, valueAdd);
				}
			}
		}

		if (found == false) {
			Debug.LogWarning ("Missing value type: " + type.ToString ());
		}
	}

	//Set a value to an exact number. Example: Set the state of marriage to 1
	public void setValue(ValueDefinitions.값정의 type, float valueToSet){
		bool found = false;
		float oldValue;
		float valueDifference;
		foreach (ValueScript vs in 볼륨스크립트리스트) {
			if (vs.내역활 == type) {

				if (found == true) {
					Debug.LogWarning ("Multiple values of the same type detected: " + type.ToString ());
				}

				found = true;
				oldValue = vs.플레이어프랩스데이터;
				vs.새로운값저장 (valueToSet);
				valueDifference = oldValue - vs.플레이어프랩스데이터;

				//display the value change to the user
				if (vs.UI조절.showActualization == true) {
					InfoDisplay.instance.addDisplay (vs.UI조절.아이콘이미지, valueDifference);
				}
			}
		}

		if (found == false) {
			Debug.LogWarning ("Missing value type: " + type.ToString ());
		}
	}

    /// <summary>
    /// setRandomValues는 스크립트에서 정의 된 Range 내의 모든 'ValueScript'값을 무작위로 지정합니다.
    /// 이것은 일반적으로 게임을 시작할 때 다른 경험을 가능하게하고 값을 정의 된 상태로 재설정하기 위해 호출됩니다.
    /// </summary>
	public void setRandomValues()
    {
		foreach (ValueScript vs in 볼륨스크립트리스트)
        {
			vs.새로운게임실행 ();
		}
	}
    
    /// <summary>
    /// 만약 valueScript가 없거나 게임씬내에 특정 값 유형의 스크립트가 둘 이상있는 경우를 체크하기 위한 테스트하기 위한 변수
    /// </summary>
    bool awaked = false;

    /// <summary>
    /// 중복 및 누락값이 있는지 테스트하는 메서드
    /// </summary>
	public void testForDuplicatesAndMissingValues()
    {
		int nrOfDetections = 0;
		int i, j;

        /// awaked값이 true면 중복 테스트를 수행한다.
		if (awaked == true)
        {
			//test for duplicates
			ValueScript testingValueType;
			int duplicateCnt = 0;

			for (i = 0; i < 볼륨스크립트리스트.Count; i++) {
				nrOfDetections = 0;
				testingValueType = 볼륨스크립트리스트 [i];

				for (j = 0; j < 볼륨스크립트리스트.Count; j++) {
					if (testingValueType.내역활 == 볼륨스크립트리스트 [j].내역활) {
						nrOfDetections++;
					}
				}

				if (nrOfDetections == 0) {
					//should not happen, by testing each valueScript with the list of valueScripts there should be at least one
				} else if (nrOfDetections == 1) {
					//One is ok, valueScript with an specific value type found itself.
				} else {
					Debug.LogError ("Multible valueScripts with the type '" + testingValueType.내역활.ToString () + "' detected. Detection on gameobject '" + testingValueType.gameObject.name + "'.");
					duplicateCnt++;
				}
			}
			if (duplicateCnt == 0) {
				Debug.Log ("No duplicate value types within the value scripts detected.");
			} else {
				Debug.Log (duplicateCnt.ToString () + " collisions for value types within the value scripts detected.");
			}

			//test for missing values
			int notLinkedCnt = 0;
			string[] valueTypes = System.Enum.GetNames (typeof(ValueDefinitions.값정의));
			for (i = 0; i < valueTypes.Length; i++) {
				nrOfDetections = 0;

				//test value type against the list
				for (j = 0; j < 볼륨스크립트리스트.Count; j++) {
					if (valueTypes [i] == 볼륨스크립트리스트 [j].내역활.ToString ()) {
						nrOfDetections++;
					}
				}

				if (nrOfDetections == 0) {
					Debug.LogWarning ("Value type '" + valueTypes [i] + "' is not assigned to any 'valueScript'.");
					notLinkedCnt++;
				} else if (nrOfDetections == 1) {
					//One is ok, one value type per value script
				} else {
					Debug.LogError ("Value type '" + valueTypes [i] + "' is not assigned to " + nrOfDetections.ToString () + " 'valueScript'.");
				}
			}

			if (notLinkedCnt == 0) {
				Debug.Log ("All value types are linked to value scripts.");
			} else {
				Debug.LogWarning (notLinkedCnt.ToString () + " value types are not linked to value scripts.");
			}
		}else{
			Debug.LogWarning ("Test for duplicates and missing value types is only possible if unity is in play mode.");
		}
	}

}
