using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EventScript))]
public class EventScriptEditor : Editor {
	public override void OnInspectorGUI ()
	{
		showScriptField ();

		showSerializedElement ("textFields");
		showSerializedElement ("isDrawable");

		EventScript es = (EventScript)serializedObject.targetObject;

		if (es.isDrawable == true) {
			showSerializedElement ("isHighPriorityCard");

			if (es.isHighPriorityCard == false) {
				showSerializedElement ("cardPropability");
			}
				showSerializedElement ("maxDraws");

		}

		showSerializedElement ("conditions");
		showSerializedElement ("Results");
		showSerializedElement ("changeValueOnCardDespawn");
		showSerializedElement ("OnCardSpawn");
		showSerializedElement ("OnCardDespawn");

		showSerializedElement ("OnSwipeLeft");
		showSerializedElement ("OnSwipeRight");

		GUILayout.Space (15);

		//base.OnInspectorGUI ();
	}

	void showSerializedSubElement(string class1, string class2){
		SerializedProperty c1 = serializedObject.FindProperty (class1);
		SerializedProperty c2 = c1.FindPropertyRelative (class2);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(c2, true);
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}

	void showSerializedElement(string class1){
		SerializedProperty c1 = serializedObject.FindProperty (class1);
		EditorGUI.BeginChangeCheck();
		EditorGUILayout.PropertyField(c1, true);
		if(EditorGUI.EndChangeCheck())
			serializedObject.ApplyModifiedProperties();
	}

	void showScriptField(){
		//show the script field
		serializedObject.Update();
		SerializedProperty prop = serializedObject.FindProperty("m_Script");
		GUI.enabled = false;
		EditorGUILayout.PropertyField(prop, true, new GUILayoutOption[0]);
		GUI.enabled = true;
		serializedObject.ApplyModifiedProperties();

	}
}

// TextFieldDrawer
[CustomPropertyDrawer(typeof(EventScript.eventText))]
public class TextFieldDrawer : PropertyDrawer
{
	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		// Using BeginProperty / EndProperty on the parent property means that
		// prefab override logic works on the entire property.
		EditorGUI.BeginProperty(position, label, property);

		// Draw label
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

		// Don't make child fields be indented
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		float scndWidth = 50f;

		// Calculate rects
		var textRect = new Rect(position.x, position.y, (position.width-scndWidth) * 0.98f, position.height);
		var textFieldRect = new Rect(position.x + (position.width-scndWidth)  , position.y, scndWidth , position.height);

		//var textRect = new Rect(position.x, position.y, position.width * 0.88f, position.height);
		//var textFieldRect = new Rect(position.x + position.width*0.9f  , position.y, position.width * 0.1f , position.height);


		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(textRect, property.FindPropertyRelative("textContent"), GUIContent.none);
		EditorGUI.PropertyField(textFieldRect, property.FindPropertyRelative("textField"), GUIContent.none);


		// Set indent back to what it was
		EditorGUI.indentLevel = indent;

		EditorGUI.EndProperty();
	}
}

// Modifier Drawer
[CustomPropertyDrawer(typeof(EventScript.resultModifier))]
public class ModifierDrawer : PropertyDrawer
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
		var modRect = new Rect(position.x, position.y, position.width * 0.70f, position.height);
		var valRect = new Rect(position.x + position.width * 0.71f  , position.y, position.width * 0.29f , position.height);

		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(modRect, property.FindPropertyRelative("modifier"), GUIContent.none);
		EditorGUI.PropertyField(valRect, property.FindPropertyRelative("valueAdd"), GUIContent.none);


		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}


// ConditionDrawer
[CustomPropertyDrawer(typeof(EventScript.condition))]
public class ConditionDrawer : PropertyDrawer
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
		var modRect = new Rect(position.x, position.y, position.width * 0.58f, position.height);
		var valminRect = new Rect(position.x + position.width * 0.59f  , position.y, position.width * 0.20f , position.height);
		var valmaxRect = new Rect(position.x + position.width * 0.8f  , position.y, position.width * 0.20f , position.height);


		// Draw fields - passs GUIContent.none to each so they are drawn without labels
		EditorGUI.PropertyField(modRect, property.FindPropertyRelative("value"), GUIContent.none);
		EditorGUI.PropertyField(valminRect, property.FindPropertyRelative("valueMin"), GUIContent.none);
		EditorGUI.PropertyField(valmaxRect, property.FindPropertyRelative("valueMax"), GUIContent.none);

		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}
}


// Drawer for the Results-selection
[CustomPropertyDrawer(typeof(EventScript.result))]
public class ResultDrawer : PropertyDrawer
{

	float mySize = 0f;

	// Draw the property inside the given rect
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		//don't alter
		EditorGUI.BeginProperty(position, label, property);
		position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);
		var indent = EditorGUI.indentLevel;
		EditorGUI.indentLevel = 0;

		float startY = position.y;

		//show the result type selection
		EditorGUI.PropertyField( new Rect(position.x, position.y , position.width, position.height) , property.FindPropertyRelative ("resultType"),GUIContent.none,true ); 

		position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("resultType"),GUIContent.none ,true);

		//dependent on selection
		EventScript.resultTypes res =  (EventScript.resultTypes) property.FindPropertyRelative ("resultType").enumValueIndex;
		if (res == EventScript.resultTypes.simple) {

			EditorGUI.PropertyField (new Rect (50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative ("modifiers"), true); 
			position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("modifiers"),GUIContent.none ,true);

		} else if (res == EventScript.resultTypes.conditional || res == EventScript.resultTypes.randomConditions) {

			EditorGUI.PropertyField (new Rect (50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative ("conditions"), true); 
			position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("conditions"),GUIContent.none ,true);

			EditorGUI.PropertyField (new Rect (50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative ("modifiersTrue"), true); 
			position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("modifiersTrue"),GUIContent.none ,true);

			EditorGUI.PropertyField (new Rect (50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative ("modifiersFalse"), true); 
			position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("modifiersFalse"),GUIContent.none ,true);

		} else if (res == EventScript.resultTypes.random) {
			EditorGUI.PropertyField (new Rect (50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative ("randomModifiers"), true); 
			position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("randomModifiers"),GUIContent.none ,true);
		}

		//draw the events
		//EditorGUI.PropertyField (new Rect (50, position.y, position.x + position.width - 50, position.height), property.FindPropertyRelative ("OnSwipe"), true); 
		//position.y += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("OnSwipe"),GUIContent.none ,true);

		mySize = position.y - startY;

		//don't alter
		EditorGUI.indentLevel = indent;
		EditorGUI.EndProperty();
	}

	float getSize(SerializedProperty property){
		mySize = 0f;

		mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("resultType"),GUIContent.none ,true);
		//dependent on selection
		EventScript.resultTypes res =  (EventScript.resultTypes) property.FindPropertyRelative ("resultType").enumValueIndex;
		if (res == EventScript.resultTypes.simple) {

			mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("modifiers"),GUIContent.none ,true);

		} else if (res == EventScript.resultTypes.conditional || res == EventScript.resultTypes.randomConditions) {

			mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("conditions"),GUIContent.none ,true);
			mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("modifiersTrue"),GUIContent.none ,true);
			mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("modifiersFalse"),GUIContent.none ,true);

		} else if (res == EventScript.resultTypes.random) {
			mySize += EditorGUI.GetPropertyHeight(property.FindPropertyRelative ("randomModifiers"),GUIContent.none ,true);
		}
		return mySize;
	}

	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{

		return getSize (property);
		//return mySize;

	}


}
	