using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Linq;
using UnityEngine.UI;
using SimpleStateMachines;
[CustomEditor(typeof(SimpleStateMachine))]
[CanEditMultipleObjects]
public class SimpleStateMachineEditor : Editor
{
	private SimpleStateMachine stateMachineMonoBehaviour;
	private string key;
	private SimpleStateMachine parent;
	private SerializedProperty childsProp;

	private byte state;
	private byte maxState;
	private bool stateOpenSwitch;
	private SerializedProperty stateMachineGroupProp;

	private static bool hasCopyData = false;
	private static SimpleStateMachineGroup copyData;

	private Image image;
	private Animator animator;

	private void OnEnable()
	{
		stateMachineMonoBehaviour = target as SimpleStateMachine;
		if (stateMachineMonoBehaviour is null) return;
		stateMachineMonoBehaviour.Initialize();
		childsProp = serializedObject.FindProperty("childs");
		stateMachineGroupProp = serializedObject.FindProperty("stateMachineGroup");
	}
	public override void OnInspectorGUI()
	{
		if (stateMachineMonoBehaviour is null) return;

		var m_keynParentToggle = parent is null;

		///Key & Parent
		EditorGUI.BeginDisabledGroup(!m_keynParentToggle);
		DefaultDraw();
		EditorGUI.EndDisabledGroup();
		StateMachineDraw();
	
		serializedObject.ApplyModifiedProperties();
		if (GUI.changed)
		{ 
			EditorUtility.SetDirty(stateMachineMonoBehaviour);
			SimpleStateMachine.Refresh(stateMachineMonoBehaviour);
		}
	}
	private void DefaultDraw()
	{
		var m_childs = new List<SimpleStateMachine>();
		m_childs.AddRange(stateMachineMonoBehaviour.GetChilds());
		m_childs.Add(null);

		key = stateMachineMonoBehaviour.GetKey();
		stateMachineMonoBehaviour.SetKey(EditorGUILayout.TextField("Key", key));

		parent = stateMachineMonoBehaviour.GetParent();
			
		if (parent is null)
		{
			EditorGUILayout.PropertyField(childsProp);
		}
		else stateMachineMonoBehaviour.SetParent(EditorGUILayout.ObjectField("Parent", parent, typeof(SimpleStateMachine), true) as SimpleStateMachine);
	}
	private void StateMachineDraw()
	{
		var m_labelCenterAlignmentStyle = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };
		var m_toolbarStyle = new GUIStyle(GUI.skin.label) { fontStyle = FontStyle.Bold, alignment = TextAnchor.MiddleCenter };
		
		EditorGUILayout.Space();
		EditorGUILayout.LabelField("Target Components");
		image = stateMachineMonoBehaviour.GetImage();
		if (!(image is null))
		{
			EditorGUI.BeginDisabledGroup(true);
			stateMachineMonoBehaviour.SetImage(EditorGUILayout.ObjectField("Image", image, typeof(Image), true) as Image);
			EditorGUI.EndDisabledGroup();
		}

		animator = stateMachineMonoBehaviour.GetAnimator();
		if (animator)
		{
			EditorGUI.BeginDisabledGroup(true);
			stateMachineMonoBehaviour.SetAnimator(EditorGUILayout.ObjectField("Animator", animator, typeof(Animator), true) as Animator);
			EditorGUI.EndDisabledGroup();
		}
		EditorGUILayout.Space();
		state = stateMachineMonoBehaviour.GetState();
		maxState = stateMachineMonoBehaviour.GetMaxState();

		EditorGUILayout.BeginHorizontal();
		
		stateOpenSwitch = EditorGUILayout.Foldout(stateOpenSwitch,"State");
		if (maxState > 0)
		{
			EditorGUI.BeginDisabledGroup(state == 0);
			if (GUILayout.Button(EditorGUIUtility.IconContent("d_tab_prev"), GUILayout.MaxWidth(25)))
				SimpleStateMachine.SetState(stateMachineMonoBehaviour, --state);
			EditorGUI.EndDisabledGroup();

			EditorGUILayout.LabelField(state.ToString(), m_labelCenterAlignmentStyle, GUILayout.MaxWidth(50));

			EditorGUI.BeginDisabledGroup(state+1 == maxState);
			if (GUILayout.Button(EditorGUIUtility.IconContent("d_tab_next"), GUILayout.MaxWidth(25)))
				SimpleStateMachine.SetState(stateMachineMonoBehaviour, ++state);
			EditorGUI.EndDisabledGroup();
		}
		else
		{
			stateOpenSwitch = true;
			EditorGUILayout.LabelField("None", m_labelCenterAlignmentStyle, GUILayout.MaxWidth(50));
		}
		EditorGUILayout.EndHorizontal();
		EditorGUILayout.Space();
		if (stateOpenSwitch)
		{
			EditorGUILayout.BeginHorizontal(EditorStyles.toolbar);

			EditorGUILayout.LabelField("", m_toolbarStyle, GUILayout.MaxWidth(15));
			EditorGUILayout.LabelField("Idx", m_toolbarStyle, GUILayout.MaxWidth(55));
			EditorGUILayout.LabelField("Name", m_toolbarStyle, GUILayout.MaxWidth(55));
			EditorGUILayout.LabelField("Info", m_toolbarStyle);
			EditorGUILayout.EndHorizontal();
			for (byte i = 0; i < maxState; i++)
			{
				var m_stateMachineGroupProp = stateMachineGroupProp.GetArrayElementAtIndex(i);
				var m_stateMachineNameProp = m_stateMachineGroupProp.FindPropertyRelative("name");
				var m_stateMachineGameObjectProp = m_stateMachineGroupProp.FindPropertyRelative("simpleStateMachineGameObject");
				var m_stateMachineImageProp = m_stateMachineGroupProp.FindPropertyRelative("simpleStateMachineImage");
				var m_stateMachineAnimatorProp = m_stateMachineGroupProp.FindPropertyRelative("simpleStateMachineAnimator");
				var m_stateMachineAnimationProp = m_stateMachineGroupProp.FindPropertyRelative("simpleStateMachineAnimation");

				var m_rect =  EditorGUILayout.BeginHorizontal(GUI.skin.box);
				if (i == state)
				{
					var m_tempCOlor = Color.cyan;
					m_tempCOlor.a = 0.05f;
					EditorGUI.DrawRect(m_rect, m_tempCOlor);
				}
				var m_totalOpen =
					m_stateMachineGameObjectProp.isExpanded ||
					m_stateMachineImageProp.isExpanded ||
					m_stateMachineAnimatorProp.isExpanded ||
					m_stateMachineAnimationProp.isExpanded;
				if (m_totalOpen)
				{
					if (GUILayout.Button(EditorGUIUtility.IconContent("icon dropdown@2x"), GUI.skin.name, GUILayout.MaxWidth(15)))
					{
						m_stateMachineGameObjectProp.isExpanded = false;
						m_stateMachineImageProp.isExpanded = false;
						m_stateMachineAnimatorProp.isExpanded = false;
						m_stateMachineAnimationProp.isExpanded = false;
					}
				}
				else
				{
					if (GUILayout.Button(EditorGUIUtility.IconContent("forward"), GUI.skin.name,GUILayout.MaxWidth(15)))
					{
						m_stateMachineGameObjectProp.isExpanded = true;
						m_stateMachineImageProp.isExpanded = true;
						m_stateMachineAnimatorProp.isExpanded = true;
						m_stateMachineAnimationProp.isExpanded = true;
					}
				}

				EditorGUILayout.LabelField(i.ToString(), m_labelCenterAlignmentStyle, GUILayout.MaxWidth(55));
				m_stateMachineNameProp.stringValue = EditorGUILayout.TextArea(m_stateMachineNameProp.stringValue, new GUIStyle(GUI.skin.textArea) { alignment = TextAnchor.MiddleCenter }, GUILayout.MaxWidth(55));
				EditorGUILayout.BeginVertical();
				DrawStateMachineProp(m_stateMachineGameObjectProp, _view : true);
				DrawStateMachineProp(m_stateMachineImageProp, _view : !(image is null));
				DrawStateMachineProp(m_stateMachineAnimatorProp, _view : animator);
				DrawStateMachineProp(m_stateMachineAnimationProp, _view : animator);
				EditorGUILayout.BeginHorizontal();
				if (GUILayout.Button("Copy"))
				{ 
					hasCopyData = true;
					copyData = stateMachineMonoBehaviour.GetStateMachineGroup(i);
				}
				EditorGUI.BeginDisabledGroup(!hasCopyData);
				if (GUILayout.Button("Paste"))
					SimpleStateMachine.CopyStateMachine(stateMachineMonoBehaviour, i, copyData);

				EditorGUI.EndDisabledGroup();
				if (GUILayout.Button("Remove"))
					SimpleStateMachine.RemoveStateMachine(stateMachineMonoBehaviour,i);
				EditorGUILayout.EndHorizontal();
				EditorGUILayout.EndVertical();
				EditorGUILayout.EndHorizontal();
			}
			EditorGUILayout.BeginHorizontal();
			EditorGUI.BeginDisabledGroup(maxState == byte.MaxValue);
			EditorGUI.BeginDisabledGroup(!hasCopyData);
			if (GUILayout.Button("Paste Add"))
				SimpleStateMachine.CopyStateMachine(stateMachineMonoBehaviour, ++maxState, copyData);
			EditorGUI.EndDisabledGroup();
			if (GUILayout.Button("Add"))
				SimpleStateMachine.SetMaxState(stateMachineMonoBehaviour, ++maxState);
			EditorGUI.EndDisabledGroup();
			EditorGUILayout.EndHorizontal();
		}
	}
	private void DrawStateMachineProp(SerializedProperty _serializedProperty, bool _view)
	{
		const string stateMachineActivePath = "stateMachineActive";

		if (!_view) return;

		EditorGUILayout.BeginHorizontal();
		var m_stateMachineActiveProp = _serializedProperty.FindPropertyRelative(stateMachineActivePath);
		EditorGUILayout.PropertyField(m_stateMachineActiveProp, new GUIContent(""), GUILayout.MaxWidth(25));
		EditorGUI.BeginDisabledGroup(!m_stateMachineActiveProp.boolValue);
		EditorGUILayout.PropertyField(_serializedProperty, new GUIContent(""));
		EditorGUI.EndDisabledGroup();
		EditorGUILayout.EndHorizontal();
	}
}
