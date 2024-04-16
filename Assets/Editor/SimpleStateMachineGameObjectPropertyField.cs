using UnityEngine;
using UnityEditor;
using SimpleStateMachines;
[CustomPropertyDrawer(typeof(SimpleStateMachineGameObject))]

public class SimpleStateMachineGameObjectPropertyField : PropertyDrawer
{
	private const string activePropPath = "active";
	private static GUIContent defaultLabel = new GUIContent("GameObject State Machine");
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		return !property.isExpanded ? EditorGUIUtility.singleLineHeight : EditorGUIUtility.singleLineHeight * 2;
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var m_pos = new Rect(position);
		m_pos.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(m_pos, property.isExpanded, defaultLabel);
		if (property.isExpanded)
		{
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative(activePropPath), true);
		}
	}
}
