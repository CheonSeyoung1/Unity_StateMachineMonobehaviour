using UnityEngine;
using UnityEditor;
using SimpleStateMachines;
[CustomPropertyDrawer(typeof(SimpleStateMachineGameObject))]

public class SimpleStateMachineGameObjectPropertyField : SimpleStateMachineBasePropertyField
{
	private const string activePropPath = "active";
	protected override void DrawProperty(Rect position, SerializedProperty property)
	=>EditorGUI.PropertyField(position, property.FindPropertyRelative(activePropPath), true);

	protected override GUIContent GetDefaultLabel()
		=>new GUIContent("GameObject State Machine");

	protected override float GetPropertyActiveHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight * 2;

	protected override float GetPropertyDefaultHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight;
}
