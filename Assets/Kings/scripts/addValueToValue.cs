using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Helper script: modify values by an event.
 */

public class addValueToValue : MonoBehaviour {


	[Tooltip("Define the value changes when calling 'addValues()'")]
	public resultModifierForAddingValueToValue[] valuesToChange;

	[System.Serializable]
	public class resultModifierForAddingValueToValue{
		public ValueDefinitions.값정의 lArgument;
		public float multiplier = 1.0f;
		public ValueDefinitions.값정의 rArgument;
	}

	public void addValues(){
		float rValue = 0f;
		foreach (resultModifierForAddingValueToValue vtv in  valuesToChange) {
			rValue =  ValueManager.나자신.첫번째피팅값가져오기(vtv.rArgument).플레이어프랩스데이터;
			ValueManager.나자신.changeValue (vtv.lArgument, vtv.multiplier * rValue);
		}
	}
}
