using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * Helper script: modify values by an event.
 */
public class setValue : MonoBehaviour {

	[Tooltip("Define the value changes when calling 'addValues()'")]
	public EventScript.resultModifier[] valuesToChange;

	public void setValues(){
		foreach (EventScript.resultModifier rm in  valuesToChange) {
			ValueManager.나자신.setValue (rm.modifier, rm.valueAdd);
		}
	}
}
