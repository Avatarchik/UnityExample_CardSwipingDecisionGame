using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

/*
 * The valueManager collects all the values of the game. 
 * 
 * Value Scripts are accessing the manager at their startup and register here, 
 * no manual linking is neccessary.
 */
public class valueManager : MonoBehaviour {

	//create a static accessable instance.
	public static valueManager instance;
	void Awake(){
		if (instance == null) {
			instance = this;
		} else {
			Debug.LogError ("Multiple value managers, this is error prone.");
		}
		awaked = true;
	}

	[Tooltip("장면의 모든 값 리스트")]
	[HideInInspector]
    public List <ValueScript> values;

	//access for value scripts to register in the manager
	public void registerValueScript(ValueScript vs){
		values.Add (vs);
	}

	//search for fitting the first fitting value script through the list
	//ATTENTION: because of the script execution order it's possible this function fails if called from 'Start()' or 'Awake()'
	//Therefore call it with at least one frame delay.
	public ValueScript getFirstFittingValue(valueDefinitions.values v){
		foreach (ValueScript vs in values) {
			if (vs.valueType == v) {

				return vs;
			}
		}
		return null;
	}

	//Test and save the min and max values of the script.
	//This is used after a game to save statistics like maximum age, health etc.
	public void saveAllMinMaxValues(){
		foreach (ValueScript vs in values) {
			vs.saveMinMax ();
		}
	}

	//Test a condition for a card. Example condition: draw this card only, if the age (or any other value) inside the game is between 10-16.
	//the methode returns true, if the condition is met. False if not.
	public bool getConditionMet(EventScript.condition cond, bool showDebug = false){
		
		foreach (ValueScript vs in values) {
			if (vs.valueType == cond.value) {

				if ((vs.value <= cond.valueMax && vs.value >= cond.valueMin)) {
					if(showDebug==true){
					 	Debug.Log ("True: " +cond.valueMin.ToString() + " " + vs.value.ToString() + " " +cond.valueMax.ToString()   );
						Debug.Log (vs.gameObject.name);
					}
					return true;
				} else {
					if (showDebug == true) {
						Debug.Log ("False: " + cond.valueMin.ToString () + " " + vs.value.ToString () + " " + cond.valueMax.ToString ());
						Debug.Log (vs.gameObject.name);
					}
					return false;
				}
			}
		}

		return false;
	}

	//check for a set of conditions if everything is met
	public bool AreConditinsForResultMet(EventScript.condition[] cond){

		bool conditionOk = true;

		foreach (EventScript.condition c in cond) {
			if (getConditionMet (c) == true) {
				//condition is ok.
			} else {
				conditionOk = false;
				break;
			}
		}

		return conditionOk;
	}

	//Change a value. Example: Reduce the health of the player by adding -5
	public void changeValue(valueDefinitions.values type, float valueAdd){
	bool found = false;
		foreach (ValueScript vs in values) {
			if (vs.valueType == type) {

				if (found == true) {
					Debug.LogWarning ("Multiple values of the same type detected: " + type.ToString ());
				}

				found = true;
				vs.addValue (valueAdd);

				//display the value change to the user
				if (vs.UserInterface.showActualization == true) {
					InfoDisplay.instance.addDisplay (vs.UserInterface.miniatureSprite, valueAdd);
				}
			}
		}

		if (found == false) {
			Debug.LogWarning ("Missing value type: " + type.ToString ());
		}
	}

	//Set a value to an exact number. Example: Set the state of marriage to 1
	public void setValue(valueDefinitions.values type, float valueToSet){
		bool found = false;
		float oldValue;
		float valueDifference;
		foreach (ValueScript vs in values) {
			if (vs.valueType == type) {

				if (found == true) {
					Debug.LogWarning ("Multiple values of the same type detected: " + type.ToString ());
				}

				found = true;
				oldValue = vs.value;
				vs.setValue (valueToSet);
				valueDifference = oldValue - vs.value;

				//display the value change to the user
				if (vs.UserInterface.showActualization == true) {
					InfoDisplay.instance.addDisplay (vs.UserInterface.miniatureSprite, valueDifference);
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
		foreach (ValueScript vs in values)
        {
			vs.newGameStart ();
		}
	}

	/*
	 * Desing helper:
	 * Test ingame, if a valueScript is missing or more than one script of an specific value type is within the scene.
	 */
	bool awaked = false;

	public void testForDuplicatesAndMissingValues(){
		int nrOfDetections = 0;
		int i, j;

		if (awaked == true) {
			//test for duplicates
			ValueScript testingValueType;
			int duplicateCnt = 0;

			for (i = 0; i < values.Count; i++) {
				nrOfDetections = 0;
				testingValueType = values [i];

				for (j = 0; j < values.Count; j++) {
					if (testingValueType.valueType == values [j].valueType) {
						nrOfDetections++;
					}
				}

				if (nrOfDetections == 0) {
					//should not happen, by testing each valueScript with the list of valueScripts there should be at least one
				} else if (nrOfDetections == 1) {
					//One is ok, valueScript with an specific value type found itself.
				} else {
					Debug.LogError ("Multible valueScripts with the type '" + testingValueType.valueType.ToString () + "' detected. Detection on gameobject '" + testingValueType.gameObject.name + "'.");
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
			string[] valueTypes = System.Enum.GetNames (typeof(valueDefinitions.values));
			for (i = 0; i < valueTypes.Length; i++) {
				nrOfDetections = 0;

				//test value type against the list
				for (j = 0; j < values.Count; j++) {
					if (valueTypes [i] == values [j].valueType.ToString ()) {
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
