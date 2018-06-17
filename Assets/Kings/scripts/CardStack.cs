using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Profiling;
using UnityEngine.Events;


public class CardStack :  TranslatableContent {
    
    /// <summary>
    /// 카드를 인스펙터에서 등록하기 위한 변수들을 모아놓은 클래스. 
    /// </summary>
	[System.Serializable]
	public class cardCategory
	{
		[Tooltip("구분하기 쉽게 배열그룹(요소)이름을 넣어주세요.")]
		public string groupName;

		[Tooltip("이 그룹의 사전 조건. 카드 자체의 조건은 낮은 우선 순위로 계산됩니다.")]
		public EventScript.콘디션[] subStackCondition;

        /// <summary>
        /// 미리 만들어둔 카드를 등록하는 배열 변수.
        /// </summary>
        public GameObject[] groupCards;
	}

    /// <summary>
    /// 게임에서 사용할 (미리 만들어둔) 카드를 등록하기 위한 배열. 
    /// </summary>
    [Tooltip("여기에 모든 카드를 삽입하십시오. 이 카드에 대한 참조는 카드를 로드 / 저장하고 다음에 그릴 수있는 것을 결정할 때 필요합니다.")]
    public cardCategory[] allCards;

    [System.Serializable]
	public class cardIndex
	{
		public int groupIndex;
		public int cardSubIndex;
		public bool validIndex;
	}

	[System.Serializable]
	public class cardCount
	{
		public int[] drawCnt;
	}

	[Tooltip("이 게임에 대한 각 카드의 추첨 갯수를 추적합니다. 'allCards'그룹과 같이 분류됩니다.")]
	[ReadOnlyInspector] // 사용자가 만든 어트리뷰트
    public drawCnts cardDrawCount;

	//fixing the array-serialization bug with empty arrays for jsonUtility by including it to a serializable class
	[System.Serializable]
	public class drawCnts
	{
		public  cardCount[] cnt;
	}

	[Tooltip("요구 사항을 충족시키는 모든 카드.")]
	[ReadOnlyInspector] public List<GameObject> availableCards;
	[Tooltip("이전 카드에서 정의한 카드 다음에 오는 카드..")]
	[ReadOnlyInspector] public GameObject followUpCard;
	[Tooltip("높은 우선 순위로 정의 된 카드. 그들은 '사용 가능한 카드'에서 일반적인 스택 전에 그려 지지만 '후속 카드'이후에 그려집니다.")]
	[ReadOnlyInspector] public List<GameObject> highPriorityCards;

	[Tooltip("실제 스폰된 카드.")]
	[ReadOnlyInspector] public GameObject spawnedCard;
	[Tooltip("이 카드 프리팹은 아무것도 가능하지 않으면 스폰합니다.")]
	public GameObject fallBackCard;

	public static CardStack instance;
	[System.Serializable] public class mEvent : UnityEvent {}

    /// <summary>
    /// Game -> Scripts를 할당
    /// </summary>
	[Tooltip("사용한 스와이프 스크립트를 여기에 연결하십시오.")]
	public Swipe swipe;

    /// <summary>
    /// 카드를 움직여도 되는지 여부 상태를 가지고 있는 변수. 
    /// 카드가 스폰시작되서 끝날때까지는 다른 카드로 넘어가면 안되기때문에 스폰시작때는 true값을 가지고 있고, 
    /// 스폰 끝날때는 false값을 가지고 있게 된다.
    /// true면 게임화면에 등장하는 카드를 움직여도 되는 상태(넘겨도 되는 상태)이다.
    /// </summary>
    private bool 카드이동가능여부 = true;

    //카드의 애니메이터 (왼쪽 / 오른쪽) 움직임. 각 카드에는 자체 애니메이터가 있습니다.
    Animator anim;

	//helper script: to determine a random element, where each of them has a propability
	private RandomElementWithPropability rndCard;

	//internal: index of the actual (last drawn) card
	cardIndex lastCardIndex;

	void Awake(){
		lastCardIndex = new cardIndex ();
		getCardIndex_result = new cardIndex ();

		lastCardIndex.groupIndex = 0;
		lastCardIndex.cardSubIndex = 0;

		rndCard = ScriptableObject.CreateInstance(typeof(RandomElementWithPropability)) as RandomElementWithPropability;
		instance = this;
	}

	void Start(){
		
		getCardAnimator ();						//get the animator of the actual card
		StartCoroutine (CardMovement ());		

		TranslationManager.instance.registerTranslateableContentScript (this);

		//Load the draw count list. If it isn't possible, create one.
		bool res = loadDrawCnt ();
		if (res == false ){
			//cardDrawCount.cnt = new cardCount[allCards.Length];
			//for (int i = 0; i < allCards.Length; i++) {
			//	cardDrawCount.cnt[i].drawCnt = new int[ allCards[i].groupCards.Length ];
			//}
			createAllCardDrawCntList(ref cardDrawCount.cnt);
		}

		//if theres an update of the game, the loaded draw count is resized accordingly
		resizeAllCardDrawCnt ();

		//load the last card before quitting the game
		loadGameCard ();
	}


	void createAllCardDrawCntList(ref cardCount[] newList){
		newList = new cardCount[allCards.Length];

		for (int i = 0; i < allCards.Length; i++) {
			newList [i] = new cardCount ();
			newList[i].drawCnt = new int[ allCards[i].groupCards.Length ];
		}
	}
	void copyAllCardDrawCntlist(cardCount[] source, ref cardCount[] target){
		int indexMain= 0;
		//take the shorter index for copy
		if (source.Length > target.Length) {
			indexMain = target.Length;	
		}else{
			indexMain = source.Length;
		}

		for (int i = 0; i < indexMain; i++) {
			int indexSub = 0;
			//take the shorter sub-index for copy
			if (source [i].drawCnt.Length > target [i].drawCnt.Length) {
				indexSub = target [i].drawCnt.Length;
			} else {
				indexSub = source [i].drawCnt.Length;
			}

			//copy the sub-elements
			for(int j = 0; j < indexSub; j++){
				target [i].drawCnt [j] = source [i].drawCnt [j];
			}
		}
			
	}

	void resizeAllCardDrawCnt(){
		//Test, if the size of the group list changed.
		bool listSizeChanged = false;

		if (allCards.Length != cardDrawCount.cnt.Length) {
			listSizeChanged = true;
		} else {
			//even if the upper list is constant, the sub-list have to be tested
			for (int i = 0; i < allCards.Length; i++) {
				if (allCards [i].groupCards.Length != cardDrawCount.cnt [i].drawCnt.Length) {
					listSizeChanged = true;
				}
			}
		}

		//if a change was detected
		if (listSizeChanged == true) {
			//create a new list (temporary)
			cardCount[] tmpList = new cardCount[0];
			createAllCardDrawCntList (ref tmpList);
			//copy actual list to temporary list
			copyAllCardDrawCntlist(cardDrawCount.cnt, ref tmpList);
			//create a new main list
			createAllCardDrawCntList (ref cardDrawCount.cnt);
			//copy the values back to main list
			copyAllCardDrawCntlist(tmpList, ref cardDrawCount.cnt);

			//save the list
			saveDrawCnt();
		}
	}

	/*
	 * Reset the card stack
	 */
	public void resetCardStack(){
		createAllCardDrawCntList (ref cardDrawCount.cnt);
		saveDrawCnt ();
		lastCardIndex.groupIndex = -1;
		lastCardIndex.cardSubIndex = -1;
		saveCardIndex ();
	}

	//Add an card-gameobject to the draw count
	void addDrawCnt(GameObject go){
		for (int i = 0; i < allCards.Length; i++) {
			for (int j = 0; j < allCards [i].groupCards.Length; j++) {
				
				if (allCards [i].groupCards[j] == go) {
					cardDrawCount.cnt [i].drawCnt[j] ++;
					saveDrawCnt ();
					return;
				}
			}
		}
		Debug.LogError ("카드를 스택에서 찾을 수 없습니다. 카드 색인을 저장할 수 없습니다.");
	}

	//Get the index of an card - gameobject. Returns the group and the sub-index within the group.
	cardIndex getCardIndex_result;
	cardIndex getCardIndex(GameObject go){
		for (int i = 0; i < allCards.Length; i++) {
			for (int j = 0; j < allCards[i].groupCards.Length; j++) {
				if (allCards [i].groupCards[j] == go) {
					getCardIndex_result.groupIndex = i;
					getCardIndex_result.cardSubIndex = j;
					getCardIndex_result.validIndex = true;
					return getCardIndex_result;
				}
			}
		}
		getCardIndex_result.groupIndex = 0;
		getCardIndex_result.cardSubIndex = 0;
		getCardIndex_result.validIndex = false;
		return getCardIndex_result;
	}

	//load/save the draw count list, the actual card 
	bool loadDrawCnt(){
		string json = PlayerPrefs.GetString ("drawCnt");
		if (string.IsNullOrEmpty (json)) {
			return false;
		} else {
			//allCard = JsonHelper.getJsonArray<int>(json);
			JsonUtility.FromJsonOverwrite(json,cardDrawCount);
			return true;
		}	
	}
	void saveDrawCnt(){
		//string json = JsonHelper.arrayToJson<int>(allCard);
		string json = JsonUtility.ToJson(cardDrawCount);
		PlayerPrefs.SetString ("drawCnt", json);
	}
	void saveCardIndex(){
		PlayerPrefs.SetString ("Cind", JsonUtility.ToJson( lastCardIndex ));
	}
	cardIndex getCardIndex(){
		JsonUtility.FromJsonOverwrite( PlayerPrefs.GetString ("Cind"), lastCardIndex );

		return lastCardIndex;
	}
		
	void loadGameCard(){
		getCardIndex();

		//if a new game starts with index -1 , no card is loaded. Also the game has to be in the state 'gameActive'

		if (lastCardIndex.groupIndex >= 0 && lastCardIndex.groupIndex < allCards.Length 
			&& GameStateManager.instance.gamestate == GameStateManager.Gamestate.gameActive) 
		{
			Destroy (spawnedCard);
			//don't load with spawn card, it would count as new draw count increment.

			//If the game structure changes due an update, we have to test if the index of the last spawned card is still valid.
			//If it is, span the card. If not, spawn the fallback card.
			if (cardIndexValid (lastCardIndex) == true) {
				spawnedCard = (GameObject)Instantiate (allCards [lastCardIndex.groupIndex].groupCards [lastCardIndex.cardSubIndex]);
			} else {
				spawnedCard = (GameObject)Instantiate (fallBackCard);
			}

			getCardAnimator ();
			spawnedCard.transform.SetParent (CardParent);
			spawnedCard.transform.localScale = new Vector3 (1f, 1f, 1f);
			actMoveDistance = 0f;
		}
	}
	bool cardIndexValid(cardIndex ci){
		//test, if index is within allowed range of upper group
		if (ci.groupIndex >= allCards.Length) {
			return false;
		}
		//test, if index is within allowed range of sub group
		if (ci.cardSubIndex >= allCards [ci.groupIndex].groupCards.Length) {
			return false;
		}

		return true;
	}

    /// <summary>
    /// 게임에서 사용하는 카드를 좌우로 넘겨서 움직일 수 있는 상태인지 여부를 파라미터로 받아서 카드이동가능여부 변수에 전달하는 메서드.
    /// 본 메서드의 파라미터값은 GlobalMessages 오브젝트의 인스펙터에서 넘겨 받을 수 있게 세팅한다.
    /// 카드가 스폰시작할때는 움직일수 없게 본 메서드에 파라미터로 넘겨주는 bool값을 인스펙터에서 미리 체크해제(false)하고
    /// 카드가 스폰이 끝나서 움직일 수 있을때는 본 메서드에 파라미터로 넘겨주는 bool값을 인스펙터에서 미리 체크(true)해놓는다.
    /// true면 카드를 움직일 수 있는 상태이고, false면 카드를 아직 움직일 수 없는 상태이다.
    /// </summary>
    /// <param name="상태">true면 카드를 움직일 수 있는 상태이고, false면 카드를 아직 움직일 수 없는 상태인것</param>
    public void 카드움직임가능여부세팅(bool 상태)
    {
		카드이동가능여부 = 상태;
	}

	/*
	 * Get the card move enabled state (e.g. we are inside a menue and its blocked)
	 */
	public bool getCardMoveEnabled(){
		return 카드이동가능여부;
	}

	[Tooltip("만약 카드가 중간으로 돌아가면 : 얼마나 빨리 움직일까?")]
	public float moveBackSpeed = 0.05f;
	[Tooltip("만약 카드가 화면밖으로 나가면 : 얼마나 빨리 움직일까?")]
	public float moveOutSpeed = 0.05f;


	//helper variables
	float moveCardDistance = 0f;
	float oldMoveCardDistance = 0f;
	float actMoveDistance = 0f;
	bool moveBackEnabled = true;

	//process the movement of the card
	IEnumerator CardMovement(){

		yield return null;

		while (true) {
			if (anim != null && moveBackEnabled == true) {

				Vector2 moveCardVector = swipe.getScaledSwipeVector ();

				if (카드이동가능여부 == true) {
					if (moveCardVector.x > 0f) {
						moveCardDistance = moveCardVector.magnitude;
					} else {
						moveCardDistance = -moveCardVector.magnitude;
					}
				} else {
					moveCardDistance = 0f;
				}

				if (swipe.pressed == false) {
					if (Mathf.Abs (actMoveDistance) > (Mathf.Abs (moveCardDistance) + 0.1f)) {
						//move it back to 0-position

						if (actMoveDistance > 0) {
							actMoveDistance -= moveBackSpeed * Time.deltaTime * Screen.width;	
							if (actMoveDistance < 0f) {
								actMoveDistance = 0f;
							}
						} else {
							actMoveDistance += moveBackSpeed * Time.deltaTime * Screen.width;
							if (actMoveDistance > 0f) {
								actMoveDistance = 0f;
							}
						}
					} else {
						actMoveDistance = moveCardDistance;
					}
				} else {
					actMoveDistance = moveCardDistance;
				}

				anim.SetFloat ("CardPos", actMoveDistance);
			}
			yield return null;
		}
	}
		

	void getCardAnimator(){
		if (spawnedCard != null) {
			anim = spawnedCard.GetComponent<Animator> ();
		}
	}

	[Tooltip("새로운 스폰된 카드의 부모를 정의합니다.")]
	public Transform CardParent;

	[Tooltip("새로운 카드가 스폰될 때까지 카드를 화면에서 어느 거리까지 이동해야합니까?")]
	public float moveOutMax = 20f;
	IEnumerator moveCardOut(){

		//Debug.Log ("move out");

		moveBackEnabled = false;

		while (Mathf.Abs (actMoveDistance) <= moveOutMax) {
			if (anim != null) {
				if (actMoveDistance > 0) {
					actMoveDistance += moveOutSpeed * Time.deltaTime * Screen.width;	
				} else {
					actMoveDistance -= moveOutSpeed * Time.deltaTime * Screen.width;
				}
			}
			anim.SetFloat ("CardPos", actMoveDistance);

			moveBackEnabled = false; //fixes bug, where move back is enabled, but this routine is also active

			yield return null;
		}

		newCard ();

		moveBackEnabled = true;
	}
	//start the coroutine for moving the card (and spawning a new one after computation)
	public void nextCard(){
		if (카드이동가능여부 == true) {
			StartCoroutine (moveCardOut ());
		}
	}


	//get an random standardcard according to its propability
	GameObject randomStandardCard(){
		rndCard.resetElements ();
		EventScript es = null;
		foreach (GameObject go in availableCards) {
			es = go.GetComponent<EventScript> ();
			if (es != null) {
				rndCard.addElement (go, es.cardPropability);
			} else {
				Debug.LogError("Missing 'EventScript' on card '" + go.name + "'");
			}
		}
		return rndCard.getRandomElement ();
	}

	//spawn a card, set its parameters and count it
	GameObject spawnCard(GameObject go){
		//Debug.Log ("new card");
		spawnedCard = (GameObject)Instantiate (go);

		addDrawCnt (go);
		lastCardIndex = getCardIndex (go);
		saveCardIndex ();

		getCardAnimator ();
		spawnedCard.transform.SetParent (CardParent);
		spawnedCard.transform.localScale = new Vector3 (1f, 1f, 1f);
		actMoveDistance = 0f;

		return spawnedCard;
	}


	void newCard(){

//remove old Card
		Destroy (spawnedCard);
		anim = null;
//test for possible new cards

		//Performance test for sorting the cards:
//		System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch();
//		sw.Start();

		sortForPossibleCards();

//		sw.Stop();
//		double elapsedMilliseconds = sw.Elapsed.TotalMilliseconds;
//		Debug.Log ("Sort duration "+elapsedMilliseconds.ToString () + " ms");

//spawn new card, dependent on priorities:
		//Highest priority 	: no cards left in any stack: spawn fallback card
		//next priority		: follow up card
		//next priority		: high priority cards
		//lowest priority	: a random card from available statck

		int cardCnt	= availableCards.Count + highPriorityCards.Count;
		if (followUpCard != null) {
			cardCnt += 1;
		}

		if (cardCnt == 0) {

			spawnCard (fallBackCard);

		} else {

			if (followUpCard != null) {
				//Debug.Log ("Follow up card: " + followUpCard.name);
				spawnCard (followUpCard);

			} else {
				int highPrioCnt = highPriorityCards.Count;

				if (highPrioCnt == 0) {
					//standard card
					//Random with propabilities
					GameObject go = randomStandardCard();
					spawnCard (go);
					//Debug.Log ("Standard card: " + go.name);						
				} else {
					//high priority card
					int rnd = Random.Range (0, highPrioCnt);
					spawnCard (highPriorityCards [rnd]);
					//Debug.Log ("High prio card: " + highPriorityCards [rnd].name);
					highPriorityCards.RemoveAt(rnd);
				}
			}
		}
	}

	public mEvent onCardSwipe;

	//sorting the all-cards pool for available cards, which meet the conditions
	void sortForPossibleCards(){

		EventScript es = null;
		availableCards.Clear ();
		highPriorityCards.Clear ();
		bool conditionOk = true;

		for (int i = 0; i<allCards.Length; i++) {
			//test, if condition for group is met
			if (ValueManager.나자신.AreConditinsForResultMet (allCards[i].subStackCondition ) == true) {

				//if the group condition is true, test the cards from the group
				for (int j = 0; j < allCards [i].groupCards.Length; j++) {
					es = allCards [i].groupCards [j].GetComponent<EventScript> ();
					conditionOk = true;

					if (cardDrawCount.cnt [i].drawCnt [j] >= es.maxDraws) {
						conditionOk = false;
					}

					foreach (EventScript.콘디션 c in es.conditions) {
						if (ValueManager.나자신.GetConditionMet (c) == true) {
							//condition is ok.
						} else {
							conditionOk = false;
							break;
						}
					}

					if (conditionOk == true && es.isDrawable == true) {
						if (es.isHighPriorityCard == true) {
							highPriorityCards.Add (allCards [i].groupCards [j]);
						} else {
							availableCards.Add (allCards [i].groupCards [j]);
						}
					}
				}
			}
		}
	}


	/*
	 *	Execution of an swipe to the left. This is called by an event in the inspector of the swipe script. 
	 */
	public void leftSwipe(){
		if (카드이동가능여부 == true) {
			EventScript es = spawnedCard.GetComponent<EventScript> ();
			if (es != null) {
				es.onLeftSwipe ();	//call the eventscript on the card for stat-changes, linking of follow up cards, etc.
			} else {
				Debug.LogError ("Event script missing on card");
			}
			onCardSwipe.Invoke ();
			nextCard ();
		}
	}

    /// <summary>
    /// 카드를 오른쪽으로 스와이프하는 기능을 실행한다.
    /// 본 메서드는 'Swipe'클래스에서 usualSwipes.swipeRight에 유니티 이벤트로 인스펙터에서 등록해서 사용하는 메서드이다. 
    /// </summary>
    public void rightSwipe()
    {
        /// 카드를 움직일 수 있는 상태이면
		if (카드이동가능여부 == true)
        {
			EventScript es = spawnedCard.GetComponent<EventScript> ();

            if (es != null)
            {
				es.onRightSwipe (); //통계 변경, 후속 카드 연결 등을 위해 카드의 이벤트 스크립트를 호출하십시오.
            }
            else
            {
				Debug.LogError ("카드의 usualSwipes.swipeRight에 이벤트 스크립트가 누락되었습니다. CardStack.rightSwipe()를 유니티 이벤트로 등록해주세요.");
			}

            onCardSwipe.Invoke ();
			nextCard ();
		}
	}

	public void additionalChoices(int choiceNr){
		bool executed = false;
		if (카드이동가능여부 == true) {
			EventScript es = spawnedCard.GetComponent<EventScript> ();
			if (es != null) {
				executed = es.ExecuteAddtionalChoices (choiceNr); //call the eventscript on the card for stat-changes, linking of follow up cards, etc.
			} else {
				Debug.LogError ("Event script missing on card");
			}
			onCardSwipe.Invoke ();
			if (executed == true) {
				nextCard ();
			}
		}
	}

	/*
	 * Test the 'allCards' stack for missing duplicate cards.
	 * This also tests for missing Prefabs/Gameobjects in the list.
	 */

	public void testForDuplicateCards(){
		bool conditionOk = true;
		GameObject testGameObject;
		int allDuplicatesCounter = 0;

		cardIndex index;

		//go through all cards in 'allCards'
		for (int i = 0; i<allCards.Length; i++) {
			for (int j = 0; j < allCards [i].groupCards.Length; j++) {

				testGameObject = allCards [i].groupCards [j];

				if (testGameObject == null) {
					Debug.LogWarning ("Element of 'allCards' (group index: " + i.ToString () + ", sub index: " + j.ToString () + ") in group '" + allCards [i].groupName + "': Prefab/Gameobject is missing"); 
				}

				//test against all cards
				for (int k = 0; k<allCards.Length; k++) {
					for (int l = 0; l < allCards [k].groupCards.Length; l++) {
						if (testGameObject != null && allCards [k].groupCards [l] != null) {
							if (testGameObject == allCards [k].groupCards [l]) {
								//if the same card is with different index is detected (the object compared with itself): inform the user
								if ( !(i==k && j==l) ) {
									Debug.LogWarning ("Duplicate of card '" + testGameObject.name + "' in group '" + allCards [i].groupName + "' found (group index: " + i.ToString () + ", sub index: " + j.ToString () + "): in group '" + allCards [k].groupName + "' (group index: " + k.ToString () + ", sub index: " + l.ToString () + ")");
									allDuplicatesCounter++;
								}
							}
						} 
					}
				}
			}
		}

		if (allDuplicatesCounter == 0) {
			Debug.Log ("'allCards'에 중복 카드가 없습니다.");
		} else {
			Debug.LogError (allDuplicatesCounter.ToString () + " duplicate cards in 'allCards' found. Counting of cards and load/save of actual card could fail.");
		}
	}

	/*
	 * Return all possible translatable terms
	 */
	public override List<string> getTranslatableTerms ()
	{
		List<string> terms = new List<string> ();
		terms.Clear ();
		EventScript es;

		for (int i = 0; i < allCards.Length; i++) {
			for (int j = 0; j < allCards [i].groupCards.Length; j++) {

				es = allCards [i].groupCards [j].GetComponent<EventScript> ();

				if (es != null) {
					terms.Add (es.textFields.타이틀텍스트.텍스트컨텍트);
					terms.Add (es.textFields.질문텍스트.텍스트컨텍트);
					terms.Add (es.textFields.왼쪽선택텍스트.텍스트컨텍트);
					terms.Add (es.textFields.오른쪽선택텍스트.텍스트컨텍트);
				}
			}
		}

		if (fallBackCard != null) {
			es = fallBackCard.GetComponent<EventScript> ();

			if (es != null) {
				terms.Add (es.textFields.타이틀텍스트.텍스트컨텍트);
				terms.Add (es.textFields.질문텍스트.텍스트컨텍트);
				terms.Add (es.textFields.왼쪽선택텍스트.텍스트컨텍트);
				terms.Add (es.textFields.오른쪽선택텍스트.텍스트컨텍트);
			}
		}

		if (spawnedCard != null) {
			es = fallBackCard.GetComponent<EventScript> ();

			if (es != null) {
				terms.Add (es.textFields.타이틀텍스트.텍스트컨텍트);
				terms.Add (es.textFields.질문텍스트.텍스트컨텍트);
				terms.Add (es.textFields.왼쪽선택텍스트.텍스트컨텍트);
				terms.Add (es.textFields.오른쪽선택텍스트.텍스트컨텍트);
			}
		}

		return terms;
	}
}
