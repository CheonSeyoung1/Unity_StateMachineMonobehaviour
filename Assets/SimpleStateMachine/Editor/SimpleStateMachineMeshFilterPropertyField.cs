using UnityEngine;
using UnityEditor;
using SimpleStateMachines;
[CustomPropertyDrawer(typeof(SimpleStateMachineMeshFilter))]
public class SimpleStateMachineMeshFilterPropertyField : SimpleStateMachineBasePropertyField
{
	private const string meshPropPath = "mesh";
	protected override void DrawProperty(Rect position, SerializedProperty property)
	{
		EditorGUI.PropertyField(position, property.FindPropertyRelative(meshPropPath), true);
	}

	protected override GUIContent GetDefaultLabel()
		=> new GUIContent("MeshFilter State Machine");

	protected override float GetPropertyActiveHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight  * 2;

	protected override float GetPropertyDefaultHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight;
}
