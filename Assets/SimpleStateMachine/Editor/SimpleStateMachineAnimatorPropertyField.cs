using UnityEngine;
using UnityEditor;
using SimpleStateMachines;

[CustomPropertyDrawer(typeof(SimpleStateMachineAnimator))]
public class SimpleStateMachineAnimatorPropertyField : SimpleStateMachineBasePropertyField
{
	private const string controllerPropPath = "controller";
	private const string avatarPropPath = "avatar";
	private const string applyRootMotionPropPath = "applyRootMotion";
	private const string updateModePropPath = "updateMode";
	private const string cullingModePropPath = "cullingMode";
	protected override void DrawProperty(Rect position, SerializedProperty property)
	{
		EditorGUI.PropertyField(position, property.FindPropertyRelative(enablePropPath), true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(controllerPropPath), true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(avatarPropPath), true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(applyRootMotionPropPath), true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(updateModePropPath), true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.PropertyField(position, property.FindPropertyRelative(cullingModePropPath), true);
	}

	protected override GUIContent GetDefaultLabel() 
		=> new GUIContent("Animator State Machine");

	protected override float GetPropertyActiveHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight * 7;

	protected override float GetPropertyDefaultHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight;
}
