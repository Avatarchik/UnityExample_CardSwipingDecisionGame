using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.Profiling;
using UnityEngine.Events;


public class CardStack :  TranslatableContent {



	[Tooltip("Insert all cards here. The reference to this cards are needed to load/save the cards and to decide, what can be drawn next.")]

	public cardCategory[] allCards;

	[System.Serializable]
	public class cardCategory
	{
		[Tooltip("Group name is just for decoration of the card groups.")]
		public string groupName;
		[Tooltip("Pre condition for this group. The conditions of the cards themself are computed with lower priority.")]
		public EventScript.condition[] subStackCondition;
		public GameObject[] groupCards;
	}
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
	[Tooltip("Tracking of number of draws of each card for this game. Categorized like the 'allCards' group")]
	[ReadOnlyInspector] public drawCnts cardDrawCount;
	//fixing the array-serialization bug with empty arrays for jsonUtility by including it to a serializable class
	[System.Serializable]
	public class drawCnts
	{
		public  cardCount[] cnt;
	}

	[Tooltip("All cards, which meet the requirements.")]
	[ReadOnlyInspector] public List<GameObject> availableCards;
	[Tooltip("Card, which is defined by previous card and follows next.")]
	[ReadOnlyInspector] public GameObject followUpCard;
	[Tooltip("Cards, which are defined as high priority. They will be drawn before the usual Stack from 'available Cards', but after the 'follow Up' card.")]
	[ReadOnlyInspector] public List<GameObject> highPriorityCards;

	[Tooltip("The actual spawned card.")]
	[ReadOnlyInspector] public GameObject spawnedCard;
	[Tooltip("This card prefab will spawn, if nothing else is possible.")]
	public GameObject fallBackCard;

	public static CardStack instance;
	[System.Serializable] public class mEvent : UnityEvent {}

	[Tooltip("Link the used swipe script here.")]
	public Swipe swipe;

	//move enable is used to block or enable the card movement while in menu
	private bool cardMoveEnabled = true;
	//the animator for the card (left/right) movement. Each card has its own animator.
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
		Debug.LogError ("Card was not found in stack. Card index couln't be saved.");
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

	/*
	 * Enable/Disable the movement of the card
	 */
	public void setCardMoveEnable(bool enable){
		cardMoveEnabled = enable;
	}

	/*
	 * Get the card move enabled state (e.g. we are inside a menue and its blocked)
	 */
	public bool getCardMoveEnabled(){
		return cardMoveEnabled;
	}

	[Tooltip("If the card moves back to the middle: how fast shall it move?")]
	public float moveBackSpeed = 0.05f;
	[Tooltip("If the card moves out of the screen: how fast shall it move?")]
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

				if (cardMoveEnabled == true) {
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

	[Tooltip("Defines the parent of a new spawned card.")]
	public Transform CardParent;

	[Tooltip("Until which distance should the card be moved out of the screen, until a new card is spawned?")]
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
		if (cardMoveEnabled == true) {
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
			if (valueManager.instance.AreConditinsForResultMet (allCards[i].subStackCondition ) == true) {

				//if the group condition is true, test the cards from the group
				for (int j = 0; j < allCards [i].groupCards.Length; j++) {
					es = allCards [i].groupCards [j].GetComponent<EventScript> ();
					conditionOk = true;

					if (cardDrawCount.cnt [i].drawCnt [j] >= es.maxDraws) {
						conditionOk = false;
					}

					foreach (EventScript.condition c in es.conditions) {
						if (valueManager.instance.getConditionMet (c) == true) {
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
		if (cardMoveEnabled == true) {
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

	/*
	 *	Execution of an swipe to the right. This is called by an event in the inspector of the swipe script. 
	 */
	public void rightSwipe(){
		if (cardMoveEnabled == true) {
			EventScript es = spawnedCard.GetComponent<EventScript> ();
			if (es != null) {
				es.onRightSwipe (); //call the eventscript on the card for stat-changes, linking of follow up cards, etc.
			} else {
				Debug.LogError ("Event script missing on card");
			}
			onCardSwipe.Invoke ();
			nextCard ();
		}
	}

	public void additionalChoices(int choiceNr){
		bool executed = false;
		if (cardMoveEnabled == true) {
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
			Debug.Log ("No duplicate cards in 'allCards' found.");
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
					terms.Add (es.textFields.titleText.textContent);
					terms.Add (es.textFields.questionText.textContent);
					terms.Add (es.textFields.answerLeft.textContent);
					terms.Add (es.textFields.answerRight.textContent);
				}
			}
		}

		if (fallBackCard != null) {
			es = fallBackCard.GetComponent<EventScript> ();

			if (es != null) {
				terms.Add (es.textFields.titleText.textContent);
				terms.Add (es.textFields.questionText.textContent);
				terms.Add (es.textFields.answerLeft.textContent);
				terms.Add (es.textFields.answerRight.textContent);
			}
		}

		if (spawnedCard != null) {
			es = fallBackCard.GetComponent<EventScript> ();

			if (es != null) {
				terms.Add (es.textFields.titleText.textContent);
				terms.Add (es.textFields.questionText.textContent);
				terms.Add (es.textFields.answerLeft.textContent);
				terms.Add (es.textFields.answerRight.textContent);
			}
		}

		return terms;
	}
}
