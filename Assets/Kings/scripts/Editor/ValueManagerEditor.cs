using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ValueManager))]
public class valueMangerEditor : Editor {

	public override void OnInspectorGUI ()
	{
		base.OnInspectorGUI ();

		ValueManager myScript = (ValueManager)target;

		EditorGUILayout.HelpBox ("Test of duplicates and missing values are only possible ingame.",MessageType.Info);

		if(GUILayout.Button("Test duplicate/missing"))
		{
			myScript.testForDuplicatesAndMissingValues();
		}
	}
}
