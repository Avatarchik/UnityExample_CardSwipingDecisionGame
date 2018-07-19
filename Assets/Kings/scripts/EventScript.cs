//#define ADDITIONAL_CHOICE_0
//#define ADDITIONAL_CHOICE_1

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


/// <summary>
/// ???????????
/// 본 클래스는 'EventScriptEditor'스크립트에서 에디팅되서 인스펙터에서 보여진다.
/// Game -> MainMenuCard
/// </summary>
public class EventScript : MonoBehaviour {

    /// <summary>
    /// 인스펙터에서 유니티 이벤트 사용하기 위해 직렬화해준다. 
    /// </summary>
    [System.Serializable]
    public class 유니티이벤트 : UnityEvent {}

	[System.Serializable]
	public class 이벤트텍스트
	{
		public string 텍스트컨텍트;

        public Text 텍스트필드;
	}

    /// <summary>
    /// 카드에 들어가는 텍스트의 종류를 규정한 클래스.
    /// </summary>
	[System.Serializable] // 클래스 타입의 변수를 인스펙터에 노출시키기 위한 어트리뷰트
	public class 이벤트텍스트모음
    {
        /// <summary>
        /// 카드의 타이틀 텍스트
        /// </summary>
		public 이벤트텍스트 타이틀텍스트;

        /// <summary>
        /// 카드 하단의 질문 텍스트.
        /// </summary>
		public 이벤트텍스트 질문텍스트;

        /// <summary>
        /// 플레이어가 선택할 수 있는 2가지 선택지중 카드를 움직일때 왼쪽 모서리의 선택지 텍스트.
        /// </summary>
		public 이벤트텍스트 왼쪽선택텍스트;

        /// <summary>
        /// 플레이어가 선택할 수 있는 2가지 선택지중 카드를 움직일때 오른쪽 모서리의 선택지 텍스트.
        /// </summary>
		public 이벤트텍스트 오른쪽선택텍스트;
	}

	[Tooltip("카드 텍스트와 텍스트 필드를 여기에 정의하십시오. 문자열은 'TranslationManager'용어 일 수 있습니다.")]
	public 이벤트텍스트모음 textFields;

	[Tooltip("카드가 우선 순위가 높으면 다른 모든 일반 카드보다 먼저 나오지만 후속 카드를 사용하면 그려집니다.")]
	public bool isHighPriorityCard = false;

    [Tooltip("drawable cards 카드는 그 상태때문에 무작위로 그려질 수 있습닏. Non drawable cards는 이전 카드 또는 게임 오버 통계와 같은 카드에 의해 정의된 후속 카드입니다.")]
	public bool isDrawable = true;

	[Tooltip("확률은 조건을 충족시킨 모든 카드에 적용됩니다. 높은 확률의 카드가 그려지기 쉽습니다.")]
	[Range(0f,1f)] public float cardPropability = 1f;
	[Tooltip("게임 당 최대 카드 뽑기를 제한하려면 'maxDraws'를 정의하십시오.")]
	public int maxDraws = 100;

	[System.Serializable]
	public class 콘디션
    {
        /// <summary>
        /// 게임에서 사용되는 이름, 돈, 카리스마등 모든 정의를 열거한 열거형 변수.
        /// </summary>
		public ValueDefinitions.값정의 정의;

        /// <summary>
        /// ???
        /// </summary>
		public float valueMin = 0f;


        /// <summary>
        /// ????
        /// </summary>
        public float valueMax = 100f;
	}

	[Tooltip("이 카드가 그려 질 수있는 조건 하에서 정의하십시오. 예 : 결혼 카드는 'age'값 유형이 18에서 100 사이이거나 '결혼'값 유형이 0 (결혼하지 않은 경우) 인 경우에만 가능해야합니다.")]
	public 콘디션[] conditions;

	[System.Serializable]
	public class resultModifier
    {
		public ValueDefinitions.값정의 modifier;
		public float valueAdd = 0f;
	}

    /// <summary>
    /// 결과 유형.
    /// </summary>
	[System.Serializable] // 인스펙터에 노출시키기 위한 어트리뷰트.
	public enum resultTypes
    {
		simple,
		conditional,
		randomConditions,
		random
	}

	[System.Serializable]
	public class modifierGroup{

		public resultModifier[] valueChanges;

		[Tooltip("이 길을 택한 경우 스토리를 추가로 취하는 '후속 조치'카드가 있습니까? 비워 둘 수 있습니다.")]
		public GameObject followUpCard;
	}

	[System.Serializable]
	public class result
    {
        /// <summary>
        /// 결과 유형에 대한 타입을 선택하는 열거형.
        /// </summary>
		public resultTypes resultType;

        [Tooltip("이 결과가 선택되면 수정되는 값은 무엇입니까?")]
		public modifierGroup modifiers;

        [Tooltip("추가 조건에 따라 결과가 두 가지 결과로 나뉠 수 있습니다. 모든 조건이 참이면 'Modifiers True'가 실행됩니다. 조건 중 하나가 실패하면 'Modifiers False'입니다. 예 : 사용자는 그가 경주를하고 싶다고 선택했지만 그의 민첩성은 낮아 결과는 떨어질 것입니다.")]
		public 콘디션[] conditions;

        [Tooltip("모든 조건이 충족되는 경우 값 그룹이 변경됩니다.")]
		public modifierGroup	modifiersTrue;

        [Tooltip("조건 중 하나 이상이 실패하면 값 그룹이 변경됩니다.")]
		public modifierGroup	modifiersFalse;

        [Tooltip("결과는 여러 결과로 나눌 수 있습니다. 'Random Modifiers'는 미리 정의 할 수 있으며, 결과의 선택은 무작위로 이들 중 하나입니다.")]
		public modifierGroup[]  randomModifiers;
	}

	[System.Serializable]
	public class resultGroup
    {
		[Tooltip("사용자가 카드를 왼쪽으로 스와이프하면 결과 (값의 변경 및 후속 카드)를 정의하십시오.")]
		public result resultLeft;

		[Tooltip("사용자가 카드를 오른쪽으로 스와이프 한 경우 결과 (값의 변경 및 후속 카드)를 정의하십시오.")]
		public result resultRight;

		#if ADDITIONAL_CHOICE_0
		[Tooltip("사용자가 추가 선택 사항을 선택하면 결과 (값의 변경 및 후속 조치 카드)를 정의하십시오.")]
		public result additional_choice_0;
		#endif

		#if ADDITIONAL_CHOICE_1
		[Tooltip("사용자가 추가 선택 사항을 선택하면 결과 (값의 변경 및 후속 조치 카드)를 정의하십시오.")]
		public result additional_choice_1;
		#endif
	}

    /// <summary>
    /// 인스펙터에 resultGroup 클래스에서 정의된 변수들을 노출하기 위한 변수.
    /// </summary>
	public resultGroup Results;

    /// <summary>
    /// 구성된 텍스트를 텍스트 필드로 번역하고 작성하십시오.
    /// </summary>
    void 텍스트필드쓰기()
    {
		if (textFields.타이틀텍스트.텍스트필드 != null) {
			textFields.타이틀텍스트.텍스트필드.text  =  TranslationManager.translateIfAvail( textFields.타이틀텍스트.텍스트컨텍트  );
		}
		if (textFields.질문텍스트.텍스트필드 != null) {
			textFields.질문텍스트.텍스트필드.text = TranslationManager.translateIfAvail (textFields.질문텍스트.텍스트컨텍트);
		}
		if (textFields.왼쪽선택텍스트.텍스트필드 != null) {
			textFields.왼쪽선택텍스트.텍스트필드.text = TranslationManager.translateIfAvail( textFields.왼쪽선택텍스트.텍스트컨텍트);
		}		
		if (textFields.오른쪽선택텍스트.텍스트필드 != null) {
			textFields.오른쪽선택텍스트.텍스트필드.text = TranslationManager.translateIfAvail( textFields.오른쪽선택텍스트.텍스트컨텍트);
		}
	}

	/*
	 * Called by an event from the swipe script. 
	 * This triggers the computation of the results for swiping LEFT and afterward the spawning of a new card.
	 */
	public void onLeftSwipe(){
		result res = Results.resultLeft;
		computeResult (res);
		OnSwipeLeft.Invoke ();
	}

    /// <summary>
    /// 스 와이프 스크립트의 이벤트에 의해 호출됩니다.
    /// 이는 오른쪽으로 스와이프 한 결과와 새로운 카드를 스폰 한 후의 결과 계산을 트리거합니다.
    /// </summary>
	public void onRightSwipe()
    {
		result res = Results.resultRight;
		computeResult (res);
		OnSwipeRight.Invoke ();
	}


	public bool ExecuteAddtionalChoices(int choiceNr){
		bool executed = false;

		switch (choiceNr) {
		case 0:
			#if ADDITIONAL_CHOICE_0
			AdditionalChoice_0_Selection();
			executed = true;
			#else
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" 'EventScript'에 구성되지 않았습니다.");
			#endif
			break;
		case 1:
			#if ADDITIONAL_CHOICE_1
			AdditionalChoice_1_Selection();
			executed = true;
			#else
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" 'EventScript'에 구성되지 않았습니다.");
			#endif
			break;
		default:
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" 'EventScript'에 구성되지 않았습니다.");
			break;	
		}

		return executed;
	}

	#if ADDITIONAL_CHOICE_0
	/* 
	 * This triggers the computation of the results for additonal options and afterward the spawning of a new card.
	 */
	public void AdditionalChoice_0_Selection(){
		result res = Results.additional_choice_0;
		computeResult (res);
		OnAdditionalChoice_0.Invoke ();
	}
	#endif
	#if ADDITIONAL_CHOICE_1
	/* 
	 * This triggers the computation of the results for additonal options and afterward the spawning of a new card.
	 */
	public void AdditionalChoice_1_Selection(){
		result res = Results.additional_choice_1;
		computeResult (res);
		OnAdditionalChoice_1.Invoke ();
	}
	#endif

	//Computation logic for executing a result.
	//Depending on the configuration of the card the corresponding results are selected.
	void computeResult(result res){

		if (res.resultType == resultTypes.simple) {
			
			//If the result is configured as 'simple' just execute the value modifiers.
			executeValueChanges (res.modifiers);

		} else if (res.resultType == resultTypes.conditional) {
		
			//If the result is configured as 'conditional' validate the conditions and
			//execute the depending modifiers.
			if (AreConditinsForResultMet (res.conditions)) {
				executeValueChanges (res.modifiersTrue);
			} else {
				executeValueChanges (res.modifiersFalse);
			}

		} else if (res.resultType == resultTypes.randomConditions) {

			//If the result is configured as 'randomConditions':
			//1. Randomize the borders of predefined value-typ dependencies.
			//2. Validate the new conditions.
			//3. Execute outcome dependent value changes.

			float rndResult = 1f;
			ValueScript v = null;
			foreach (콘디션 c in res.conditions) {
				rndResult = Random.Range (0f, 1f);
				v = ValueManager.나자신.첫번째피팅값가져오기 (c.정의);

				if (v != null) {
					//set the minimum border for the conditon between min and max, 
					//if the real value is over min, the path 'true' is executed
					c.valueMin = v.범위.최소값 + rndResult*(v.범위.최대값 - v.범위.최소값);	
					c.valueMax = v.범위.최대값;
				} else {
					Debug.LogWarning ("Missing value type: " + c.정의);
				}

			}

			if (AreConditinsForResultMet (res.conditions)) {
				executeValueChanges (res.modifiersTrue);
			} else {
				executeValueChanges (res.modifiersFalse);
			}

		}else if (res.resultType == resultTypes.random) {

			//If the result is configured as 'random':
			//Select randomly a modifier-group out of the defined pool and execute the value changes.
			if (res.randomModifiers.Length != 0) {
				int rndResult = Random.Range (0, res.randomModifiers.Length);
				executeValueChanges (res.randomModifiers[rndResult]);
			} else {
				Debug.LogWarning ("Missing random results-list");
			}

		} else {
			Debug.LogError ("Path not reachable?");
		}

		foreach (resultModifier rm in  changeValueOnCardDespawn) {
			ValueManager.나자신.changeValue (rm.modifier, rm.valueAdd);
		}
			
		OnCardDespawn.Invoke ();
	}

	//execution of a group of value modifications
	void executeValueChanges(modifierGroup modsGroup){

		//reset the user info
		//InfoDisplay.instance.clearDisplay ();

		foreach (resultModifier rm in  modsGroup.valueChanges) {
			ValueManager.나자신.changeValue (rm.modifier, rm.valueAdd);
		}

		//Tell the cardstack the follow up card.
		//Follow up card can be NULL, the cardstack itself checks the cards before spawning.
		CardStack.instance.followUpCard = modsGroup.followUpCard;

		//show the value changes over the animation (if available)
		//InfoDisplay.instance.startAnimationIfNotEmpty();
	}

	//check for a set of conditions if everything is met
	bool AreConditinsForResultMet(콘디션[] cond){
		
		bool conditionOk = true;

		foreach (EventScript.콘디션 c in cond) {
			if (ValueManager.나자신.GetConditionMet (c) == true) {
				//condition is ok.
			} else {
				conditionOk = false;
				break;
			}
		}

		return conditionOk;
	}


	void Awake(){
		텍스트필드쓰기 ();
	}
		
	void Start () {
		OnCardSpawn.Invoke ();
	}

	[Tooltip("조건부 결과 계산 후 값 변경. 'Age +1'과 같이 값이 결과와 독립적으로 변경되는 경우 유용합니다.")]
	public resultModifier[] changeValueOnCardDespawn;

	public 유니티이벤트 OnCardSpawn;
	public 유니티이벤트 OnCardDespawn;

	public 유니티이벤트 OnSwipeLeft;
	public 유니티이벤트 OnSwipeRight;

	#if ADDITIONAL_CHOICE_0
	public mEvent OnAdditionalChoice_0;
	#endif
	#if ADDITIONAL_CHOICE_1
	public mEvent OnAdditionalChoice_1;
	#endif
}
