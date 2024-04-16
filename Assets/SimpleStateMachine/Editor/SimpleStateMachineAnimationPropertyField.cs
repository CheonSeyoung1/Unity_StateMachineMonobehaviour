using UnityEngine;
using UnityEditor;
using SimpleStateMachines;

[CustomPropertyDrawer(typeof(SimpleStateMachineAnimation))]
public class SimpleStateMachineAnimationPropertyField : SimpleStateMachineBasePropertyField
{
	private const string forcedPropPath = "forced";
	private const string animationKeyPropPath = "animationKey";
	protected override void DrawProperty(Rect position, SerializedProperty property)
	{
		EditorGUI.PropertyField(position, property.FindPropertyRelative(forcedPropPath), true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(animationKeyPropPath), true);
	}

	protected override GUIContent GetDefaultLabel() 
		=> new GUIContent("Animation State Machine");

	protected override float GetPropertyActiveHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight * 3;
	protected override float GetPropertyDefaultHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight;
}
