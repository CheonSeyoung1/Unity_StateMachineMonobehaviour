using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using SimpleStateMachines;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.UIElements;

[CustomPropertyDrawer(typeof(SimpleStateMachineMeshRenderer))]

public class SimpleStateMachineMeshRendererPropertyField : SimpleStateMachineBasePropertyField
{
	private const string materialsPropPath = "materials";
	private const string lighingSettingPropPath = "lightingSetting";
	private const string additionalSettingPropPath = "additionalSetting";

	protected override void DrawProperty(Rect position, SerializedProperty property)
	{
		EditorGUI.PropertyField(position, property.FindPropertyRelative(enablePropPath), true);
		position.y += EditorGUIUtility.singleLineHeight;

		var m_matProp = property.FindPropertyRelative(materialsPropPath);
		EditorGUI.PropertyField(position, m_matProp, true);
		
		if (m_matProp.isExpanded)
			position.y += EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight * (m_matProp.arraySize + 2) + (EditorGUIUtility.standardVerticalSpacing*(m_matProp.arraySize));
		else
			position.y += EditorGUIUtility.singleLineHeight;

		var m_lighingSettingProp = property.FindPropertyRelative(lighingSettingPropPath);
		EditorGUI.PropertyField(position, m_lighingSettingProp, true);
		position.y += m_lighingSettingProp.isExpanded ? EditorGUIUtility.singleLineHeight * 5: EditorGUIUtility.singleLineHeight;

		var m_additionalSettingProp = property.FindPropertyRelative(additionalSettingPropPath);
		EditorGUI.PropertyField(position, m_additionalSettingProp, true);
		position.y += m_additionalSettingProp.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;
	}

	protected override GUIContent GetDefaultLabel()
		=> new GUIContent("MeshRenderer State Machine");

	protected override float GetPropertyActiveHeight(SerializedProperty property)
	{
		var m_matProp = property.FindPropertyRelative(materialsPropPath);
		var m_matHeight = m_matProp.isExpanded ? EditorGUIUtility.singleLineHeight + EditorGUIUtility.singleLineHeight * (m_matProp.arraySize + 2) + (EditorGUIUtility.standardVerticalSpacing * (m_matProp.arraySize)) : EditorGUIUtility.singleLineHeight; 
		
		var m_lighingSettingProp = property.FindPropertyRelative(lighingSettingPropPath);
		var m_lighingSettingHeight = m_lighingSettingProp.isExpanded ? EditorGUIUtility.singleLineHeight * 5 : EditorGUIUtility.singleLineHeight;

		var m_additionalSettingProp = property.FindPropertyRelative(additionalSettingPropPath);
		var m_additionalSettingHeight = m_additionalSettingProp.isExpanded ? EditorGUIUtility.singleLineHeight * 4 : EditorGUIUtility.singleLineHeight;

		return EditorGUIUtility.singleLineHeight * 2 + m_matHeight + m_lighingSettingHeight + m_additionalSettingHeight + EditorGUIUtility.standardVerticalSpacing;
	}

	protected override float GetPropertyDefaultHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight;

	[CustomPropertyDrawer(typeof(SimpleStateMachineMeshRenderer.LightingSetting))]
	private class LightingSettingPropertyField : SimpleStateMachineBasePropertyField
	{
		private const string castShadowsPropPath = "castShadows";
		private const string contributeGlobalIlluminationPropPath = "contributeGlobalIllumination";
		private const string receiveGlobalIlluminationPropPath = "receiveGlobalIllumination";
		protected override GUIContent GetDefaultLabel()=> new GUIContent("Lighting");

		protected override float GetPropertyDefaultHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight;
		protected override float GetPropertyActiveHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight* 6;

		protected override void DrawProperty(Rect position, SerializedProperty property)
		{
			var m_castShadowsProp = property.FindPropertyRelative(castShadowsPropPath);
			EditorGUI.LabelField(position, "Cast Shadows");
			m_castShadowsProp.intValue = EditorGUI.EnumPopup(position, (ShadowCastingMode)m_castShadowsProp.intValue).GetHashCode();
			position.y += EditorGUIUtility.singleLineHeight;
			var m_contributeGlobalIlluminationProp = property.FindPropertyRelative(contributeGlobalIlluminationPropPath);
			EditorGUI.PropertyField(position, m_contributeGlobalIlluminationProp, true);
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.BeginDisabledGroup(!m_contributeGlobalIlluminationProp.boolValue);
			var m_receiveGlobalIlluminationProp = property.FindPropertyRelative(receiveGlobalIlluminationPropPath);
			if ((m_receiveGlobalIlluminationProp.intValue != 1) &&
				(m_receiveGlobalIlluminationProp.intValue != 2))
				m_receiveGlobalIlluminationProp.intValue = 2;
			if (!m_contributeGlobalIlluminationProp.boolValue)
				m_receiveGlobalIlluminationProp.intValue = 2;
			EditorGUI.PropertyField(position, m_receiveGlobalIlluminationProp, true);
			position.y += EditorGUIUtility.singleLineHeight;
			EditorGUI.EndDisabledGroup();
		}
	}
	[CustomPropertyDrawer(typeof(SimpleStateMachineMeshRenderer.AdditionalSetting))]
	private class AdditionalSettingPropertyField : SimpleStateMachineBasePropertyField
	{
		private class RenderingLayerMaskValue
		{
			public RenderingLayerMaskValue(uint _value)=>value = _value;
			public bool change = false;
			public uint value;
		}
		private class RenderingLayerMaskChange
		{
			public RenderingLayerMaskChange(uint _change, RenderingLayerMaskValue _value)
			{
				change = _change;
				value = _value;
			}
			public uint change;
			public RenderingLayerMaskValue value;
		}
		private const string dynamicOcculusionPropPath = "dynamicOcculusion";
		private const string renderingLayerMaskPropPath = "renderingLayerMask";

		protected override GUIContent GetDefaultLabel() => new GUIContent("AdditionalSetting");

		protected override float GetPropertyDefaultHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight;
		protected override float GetPropertyActiveHeight(SerializedProperty property)
		=> EditorGUIUtility.singleLineHeight * 3;
		
		protected override void DrawProperty(Rect position, SerializedProperty property)
		{
			var urpAsset = (UniversalRenderPipelineAsset)GraphicsSettings.renderPipelineAsset;

			EditorGUI.PropertyField(position, property.FindPropertyRelative(dynamicOcculusionPropPath), true);
			position.y += EditorGUIUtility.singleLineHeight;
			var m_renderingLayerMaskProp = property.FindPropertyRelative(renderingLayerMaskPropPath);
			
			EditorGUI.LabelField(position, "RenderingLayerMask");
			position.y += EditorGUIUtility.singleLineHeight;

			m_renderingLayerMaskProp.longValue = (uint)EditorGUI.MaskField(position, m_renderingLayerMaskProp.intValue, urpAsset.renderingLayerMaskNames);
		}
	}
}
