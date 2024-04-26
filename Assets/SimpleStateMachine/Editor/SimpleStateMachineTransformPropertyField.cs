using UnityEngine;
using UnityEditor;
using SimpleStateMachines;

[CustomPropertyDrawer(typeof(SimpleStateMachineTransform))]
public class SimpleStateMachineTransformPropertyField : SimpleStateMachineBasePropertyField
{
	private const string globalPropPath = "global";
	private const string positionApplyPropPath = "positionApply";
	private const string positionPropPath = "position";
	private const string rotationApplyPropPath = "rotationApply";
	private const string rotationPropPath = "rotation";
	private const string scaleApplyPropPath = "scaleApply";
	private const string scalePropPath = "scale";
	protected override void DrawProperty(Rect position, SerializedProperty property)
	{
		var m_globalProp = property.FindPropertyRelative(globalPropPath);
		EditorGUI.LabelField(position, "Apply Area");
		position.y += EditorGUIUtility.singleLineHeight; 
		var m_globalValue = EditorGUI.Popup(position,m_globalProp.boolValue?0:1,new[] {"Global","Local" } );
	
		m_globalProp.boolValue = m_globalValue == 0 ? true : false;
		position.y += EditorGUIUtility.singleLineHeight;

		var m_positionApplyProp = property.FindPropertyRelative(positionApplyPropPath);
		EditorGUI.PropertyField(position, m_positionApplyProp, new GUIContent("Position"),true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.BeginDisabledGroup(!m_positionApplyProp.boolValue);
		EditorGUI.PropertyField(position, property.FindPropertyRelative(positionPropPath), new GUIContent() ,true);
		EditorGUI.EndDisabledGroup();
		position.y += EditorGUIUtility.singleLineHeight;

		var m_rotationApplyProp = property.FindPropertyRelative(rotationApplyPropPath);
		EditorGUI.PropertyField(position, m_rotationApplyProp, new GUIContent("Rotation"),true);
		position.y += EditorGUIUtility.singleLineHeight;
		EditorGUI.BeginDisabledGroup(!m_rotationApplyProp.boolValue);
		EditorGUI.PropertyField(position, property.FindPropertyRelative(rotationPropPath), new GUIContent(), true);
		EditorGUI.EndDisabledGroup();
		position.y += EditorGUIUtility.singleLineHeight;

		if (!m_globalProp.boolValue)
		{
			var m_scaleApplyProp = property.FindPropertyRelative(scaleApplyPropPath);
			EditorGUI.PropertyField(position, m_scaleApplyProp, new GUIContent("Scale"), true);
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginDisabledGroup(!m_scaleApplyProp.boolValue);
			EditorGUI.PropertyField(position, property.FindPropertyRelative(scalePropPath), new GUIContent(), true);
			EditorGUI.EndDisabledGroup();
			EditorGUI.EndDisabledGroup();
		}
	}

	protected override GUIContent GetDefaultLabel()
		=> new GUIContent("Transform State Machine");

	protected override float GetPropertyActiveHeight(SerializedProperty property)
	{ 
		var m_globalProp = property.FindPropertyRelative(globalPropPath);
		return EditorGUIUtility.singleLineHeight * (5 + (m_globalProp.boolValue?2:4));
	}

	protected override float GetPropertyDefaultHeight(SerializedProperty property)
			=> EditorGUIUtility.singleLineHeight;
}
