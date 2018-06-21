using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 스코어 계산하는 클래스.
/// </summary>
public class ScoreCalculator : MonoBehaviour {

    /// <summary>
    /// 스코어 점수.
    /// </summary>
	public float score;

	public static ScoreCalculator instance;

    /// <summary>
    /// 게임이 끝나고 결과화면 나올때 표시할 정보를 담고 있는 클래스. 
    /// 이 클래스는 게임이 끝나고 결과화면에서 표시할 통계를 사용자가 인스펙터에서 설정하기 위한 변수들을 가지고 있다.
    /// 게임 스코어(통치기간, 병사, 사람, 종교, 돈)와 승수를
    /// </summary>
	[System.Serializable] /// 클래스 타입의 변수를 인스펙터에 노출하는 애트리뷰트
	public class scoreRelevantPair
    {
        /// <summary>
        /// 게임에서 사용되는 모든 값의 유형들 타입의 변수(돈, 성별, 나라 등등)
        /// 인스펙터에서 목록 선택 형태로 고르기 위한 방식으로 사용하기 위해 변수를 선언한다.
        /// </summary>
		public ValueDefinitions.값정의 valueType;

        /// <summary>
        /// 이긴 수.
        /// </summary>
		public float multiplier;
	}

    /// <summary>
    /// 
    /// </summary>
	public scoreRelevantPair[] scoreValues;



    public ScoreCounter[] extraScores;

	public ScoreCounter highScore;

	public ScoreCounter maxHighScore;

    /// <summary>
    /// 점수 계산하는 메서드
    /// </summary>
	public void calculateScore()
    {
        /// 스코어 값을 일단 0으로 초기화.
		score = 0f;

		ValueScript vs;

        foreach (scoreRelevantPair srp in scoreValues) {
			vs = ValueManager.나자신.첫번째피팅값가져오기 (srp.valueType);
			score += vs.플레이어프랩스데이터 * srp.multiplier;
		}

		foreach (ScoreCounter sc in extraScores) {
			score += sc.getScore ();
		}

		if (highScore != null) {
			highScore.setScore(Mathf.RoundToInt(score));
		}

		if (maxHighScore != null) {
			maxHighScore.setMaxScore(Mathf.RoundToInt(score));
		}
	}

	void Awake() {
		instance = this;
	}

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}

#if UNITY_EDITOR

[CustomPropertyDrawer(typeof(ScoreCalculator.scoreRelevantPair))]
public class scoreRelevantPairDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//don't alter
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		// Calculate rects
		var modRect = new Rect(position.x, position.y, position.width * 0.50f, position.height);
		var valRect = new Rect(position.x + position.width * 0.52f  , position.y, position.width * 0.48f , position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(modRect, property.FindPropertyRelative("valueType"), GUIContent.none);
		EditorGUI.PropertyField(valRect, property.FindPropertyRelative("multiplier"), GUIContent.none);


		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}
#endif
