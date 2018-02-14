//#define ADDITIONAL_CHOICE_0
//#define ADDITIONAL_CHOICE_1


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;


public class EventScript : MonoBehaviour {
	[System.Serializable] public class mEvent : UnityEvent {}

	[System.Serializable]
	public class eventText
	{
		public string textContent;
		public Text textField;
	}

	[System.Serializable]
	public class eventTexts{
		public eventText titleText;
		public eventText questionText;
		public eventText answerLeft;
		public eventText answerRight;
	}
	[Tooltip("Define your card texts and text fields here. The strings can be terms for the 'TranslationManager'.")]
	public eventTexts textFields;

	[Tooltip("If a card is high priority, it will be draw before all other normal cards, but after follow up cards.")]
	public bool isHighPriorityCard = false;
	[Tooltip("Only drawable cards can be randomly drawn because of their condition. Non drawable cards are follow up cards which are defined by previous cards or cards like the gameover statistics.")]
	public bool isDrawable = true;

	[Tooltip("The propability applies to all cards, which met the conditions. Cards with a higher propability are more likely to be drawn.")]
	[Range(0f,1f)] public float cardPropability = 1f;
	[Tooltip("To limit the maximum draws of a card per game, define the 'maxDraws'.")]
	public int maxDraws = 100;

	[System.Serializable]
	public class condition{
		public valueDefinitions.values value;
		public float valueMin = 0f;
		public float valueMax = 100f;
	}

	[Tooltip("Define under wich conditions this card can be drawn. E.g. a marriage card should only be possible if a value type 'age' is in the range of 18 to 100 or the value type 'marriage' is zero (not married yet)")]
	public condition[] conditions;

	[System.Serializable]
	public class resultModifier{
		public valueDefinitions.values modifier;
		public float valueAdd = 0f;
	}

	[System.Serializable]
	public enum resultTypes{
		simple,
		conditional,
		randomConditions,
		random
	}

	[System.Serializable]
	public class modifierGroup{

		public resultModifier[] valueChanges;
		[Tooltip("If this path was taken, will there be a 'follow up' card which takes the story further? Can be left empty.")]
		public GameObject followUpCard;
	}

	[System.Serializable]
	public class result{
		public resultTypes resultType;
		[Tooltip("Which values are modified, if this result is selected?")]
		public modifierGroup modifiers;
		[Tooltip("Depending on further conditions the result can split into two different outcomes. If all conditions are true, the 'Modifiers True' are executed. If one of the conditions fails, the 'Modifiers False'. E.g. the user selected he wants to take a race but his 'agility' value is to low, as outcome he will lose.")]
		public condition[] conditions;
		[Tooltip("Group of value changes, if all conditions are met.")]
		public modifierGroup	modifiersTrue;
		[Tooltip("Group of value changes, if at least one of the conditions fails.")]
		public modifierGroup	modifiersFalse;
		[Tooltip("A result can be split in multible outcomes. The 'Random Mofifiers' can be predefined, the selection of the outcome is randomly one of these.")]
		public modifierGroup[]  randomModifiers;
	}

	[System.Serializable]
	public class resultGroup{
		[Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card left.")]
		public result resultLeft;

		[Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user swipes the card right.")]
		public result resultRight;

		#if ADDITIONAL_CHOICE_0
		[Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user selects addtional choices.")]
		public result additional_choice_0;
		#endif
		#if ADDITIONAL_CHOICE_1
		[Tooltip("Define the result (the changes in values and perhaps a follow up card) if the user selects addtional choices.")]
		public result additional_choice_1;
		#endif
	}

	public resultGroup Results;

	//Try to translate and write the configurated texts to their text-fields. 
	void writeTextFields(){
		if (textFields.titleText.textField != null) {
			textFields.titleText.textField.text  =  TranslationManager.translateIfAvail( textFields.titleText.textContent  );
		}
		if (textFields.questionText.textField != null) {
			textFields.questionText.textField.text = TranslationManager.translateIfAvail (textFields.questionText.textContent);
		}
		if (textFields.answerLeft.textField != null) {
			textFields.answerLeft.textField.text = TranslationManager.translateIfAvail( textFields.answerLeft.textContent);
		}		
		if (textFields.answerRight.textField != null) {
			textFields.answerRight.textField.text = TranslationManager.translateIfAvail( textFields.answerRight.textContent);
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
	/*
	 * Called by an event from the swipe script. 
	 * This triggers the computation of the results for swiping RIGHT and afterward the spawning of a new card.
	 */
	public void onRightSwipe(){
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
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" is not configured in 'EventScript'.");
			#endif
			break;
		case 1:
			#if ADDITIONAL_CHOICE_1
			AdditionalChoice_1_Selection();
			executed = true;
			#else
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" is not configured in 'EventScript'.");
			#endif
			break;
		default:
			Debug.LogError("ADDITIONAL_CHOICE_"+choiceNr.ToString()+" is not configured in 'EventScript'.");
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
			foreach (condition c in res.conditions) {
				rndResult = Random.Range (0f, 1f);
				v = valueManager.instance.getFirstFittingValue (c.value);

				if (v != null) {
					//set the minimum border for the conditon between min and max, 
					//if the real value is over min, the path 'true' is executed
					c.valueMin = v.limits.min + rndResult*(v.limits.max - v.limits.min);	
					c.valueMax = v.limits.max;
				} else {
					Debug.LogWarning ("Missing value type: " + c.value);
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
			valueManager.instance.changeValue (rm.modifier, rm.valueAdd);
		}
			
		OnCardDespawn.Invoke ();
	}

	//execution of a group of value modifications
	void executeValueChanges(modifierGroup modsGroup){

		//reset the user info
		//InfoDisplay.instance.clearDisplay ();

		foreach (resultModifier rm in  modsGroup.valueChanges) {
			valueManager.instance.changeValue (rm.modifier, rm.valueAdd);
		}

		//Tell the cardstack the follow up card.
		//Follow up card can be NULL, the cardstack itself checks the cards before spawning.
		CardStack.instance.followUpCard = modsGroup.followUpCard;

		//show the value changes over the animation (if available)
		//InfoDisplay.instance.startAnimationIfNotEmpty();
	}

	//check for a set of conditions if everything is met
	bool AreConditinsForResultMet(condition[] cond){
		
		bool conditionOk = true;

		foreach (EventScript.condition c in cond) {
			if (valueManager.instance.getConditionMet (c) == true) {
				//condition is ok.
			} else {
				conditionOk = false;
				break;
			}
		}

		return conditionOk;
	}


	void Awake(){
		writeTextFields ();
	}
		
	void Start () {
		OnCardSpawn.Invoke ();
	}

	[Tooltip("Changes of values after the computation of the conditional results. Useful if a value is changed independent of the result, like 'Age +1'.")]
	public resultModifier[] changeValueOnCardDespawn;

	public mEvent OnCardSpawn;
	public mEvent OnCardDespawn;

	public mEvent OnSwipeLeft;
	public mEvent OnSwipeRight;

	#if ADDITIONAL_CHOICE_0
	public mEvent OnAdditionalChoice_0;
	#endif
	#if ADDITIONAL_CHOICE_1
	public mEvent OnAdditionalChoice_1;
	#endif
}
