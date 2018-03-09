using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// 인스펙터에서 변수를 읽기전용으로 사용자 커스터마이즈한것을 어트리뷰트로 사용하기 위해 단순히 PropertyAttribute를 상속받은 클래스.
/// 이 클래스이름이 사용자 커스터마이즈 어트리뷰트의 이름이 된다.
/// </summary>
public class ReadOnlyInspector : PropertyAttribute
{
}

#if UNITY_EDITOR

/// <summary>
/// 인스펙터에 노출되는 변수를 읽기전용(보여주기만 하기)만들기 위해, 유니티에서 제공하는 PropertyDrawer를 상속받아 오버라이드해서 커스터마이징하는 클래스.
/// </summary>
[CustomPropertyDrawer(typeof(ReadOnlyInspector))] /// 커스터마이징할 클래스를 지정한다.
public class ReadOnlyDrawer : PropertyDrawer
{
	public override float GetPropertyHeight(SerializedProperty property,
		GUIContent label)
	{
		return EditorGUI.GetPropertyHeight(property, label, true);
	}

	public override void OnGUI(Rect position,
		SerializedProperty property,
		GUIContent label)
	{
		GUI.enabled = false;
		EditorGUI.PropertyField(position, property, label,true);
		GUI.enabled = true;
	}
}

#endif
