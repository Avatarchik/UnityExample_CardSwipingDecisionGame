using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Helper script: modify values by an event.
 */

public class AddValue : MonoBehaviour {


	[Tooltip("Define the value changes when calling 'addValues()'")]
	public EventScript.resultModifier[] valuesToChange;


	public void addValues(){
		foreach (EventScript.resultModifier rm in  valuesToChange) {
			ValueManager.나자신.changeValue (rm.modifier, rm.valueAdd);
		}
	}
}
