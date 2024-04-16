using UnityEngine;
using UnityEditor;

public abstract class SimpleStateMachineBasePropertyField : PropertyDrawer
{
	protected const string enablePropPath = "enable";

	protected abstract GUIContent GetDefaultLabel();

	protected abstract float GetPropertyDefaultHeight(SerializedProperty property);
	protected abstract float GetPropertyActiveHeight(SerializedProperty property);
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label) => 
		property.isExpanded ? GetPropertyActiveHeight(property) : GetPropertyDefaultHeight(property);

	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var m_pos = new Rect(position);
		m_pos.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(m_pos, property.isExpanded, GetDefaultLabel());
		if (property.isExpanded)
		{
			m_pos.y+= EditorGUIUtility.singleLineHeight;
			DrawProperty(m_pos , property);
		}
	}
	protected abstract void DrawProperty(Rect position, SerializedProperty property);
}
