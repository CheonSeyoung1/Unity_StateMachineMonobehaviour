using UnityEngine;
using UnityEditor;
using SimpleStateMachines;
[CustomPropertyDrawer(typeof(SimpleStateMachineImage))]
public class SimpleStateMachineImagePropertyField : PropertyDrawer
{
	private const string enablePropPath = "enable";

	private const string spritePropPath = "sprite";
	private const string colorPropPath = "color";
	private const string materialPropPath = "material";
	private const string raycastTargetPropPath = "raycastTarget";
	private const string raycastPaddingPropPath = "raycastPadding";

	private const string maskablePropPath = "maskable";
	private static GUIContent defaultLabel = new GUIContent("Image State Machine");
	public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
	{
		var m_raycastPaddingProp = property.FindPropertyRelative(raycastPaddingPropPath);

		return (!property.isExpanded ? EditorGUIUtility.singleLineHeight :
			(EditorGUIUtility.singleLineHeight * 7 +
			(m_raycastPaddingProp.isExpanded? CustomVector4.OpenVector4Height: EditorGUIUtility.singleLineHeight)));
	}
	public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
	{
		var m_pos = new Rect(position);
		m_pos.height = EditorGUIUtility.singleLineHeight;
		property.isExpanded = EditorGUI.Foldout(m_pos, property.isExpanded, defaultLabel);
		if (property.isExpanded)
		{
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative(enablePropPath), true);
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative(spritePropPath), true);
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative(colorPropPath), true);
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative(materialPropPath), true);
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative(raycastTargetPropPath), true);
			position.y += EditorGUIUtility.singleLineHeight;
			
			position.height = EditorGUIUtility.singleLineHeight;
			var m_raycastPaddingProp = property.FindPropertyRelative(raycastPaddingPropPath);
			EditorGUI.PropertyField(position, m_raycastPaddingProp, true);
			position.y += m_raycastPaddingProp.isExpanded ? CustomVector4.OpenVector4Height : EditorGUIUtility.singleLineHeight;
			EditorGUI.PropertyField(position, property.FindPropertyRelative(maskablePropPath), true);
		}
	}

	[CustomPropertyDrawer(typeof(Vector4))]
	public class CustomVector4 : PropertyDrawer
	{
		internal static float OpenVector4Height = EditorGUIUtility.singleLineHeight * 5;
		private const string parentLabel = "RaycastPadding";
		private const string left = "Left";
		private const string bottom = "Bottom";
		private const string right = "Right";
		private const string top = "Top";
		public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
		{
			return !property.isExpanded ? EditorGUIUtility.singleLineHeight : OpenVector4Height;
		}
		public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
		{
			property.isExpanded = EditorGUI.Foldout(position, property.isExpanded, new GUIContent(parentLabel));
			if (property.isExpanded) 
			{
				position.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("x"), new GUIContent(left), true);
				position.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("y"), new GUIContent(bottom), true);
				position.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("z"), new GUIContent(right), true);
				position.y += EditorGUIUtility.singleLineHeight;
				EditorGUI.PropertyField(position, property.FindPropertyRelative("w"), new GUIContent(top), true);
			}
		}
	}
}
