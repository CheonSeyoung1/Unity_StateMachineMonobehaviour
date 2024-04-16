using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SimpleStateMachines
{
	[System.Serializable]
	public struct SimpleStateMachineGameObject 
	{
		public bool stateMachineActive;
		public bool active;
	}
	[System.Serializable]
	public struct SimpleStateMachineImage
	{
		public bool stateMachineActive;

		public bool enable;

		public Sprite sprite;
		public Color color;
		public Material material;
		public bool raycastTarget;
		public Vector4 raycastPadding;

		public bool maskable;
	}

	[System.Serializable]
	public struct SimpleStateMachineGroup
	{
		public string name;
		public SimpleStateMachineGameObject simpleStateMachineGameObject;
		public SimpleStateMachineImage simpleStateMachineImage;
	}

	[DisallowMultipleComponent]
	public class SimpleStateMachine : MonoBehaviour
	{
		private static Dictionary<SimpleStateMachine, GameObject> SimpleStateMachineToGameObjectContainer;
		private static Dictionary<string, SimpleStateMachine> StringKeyToSimpleStateMachineContainer;

		[SerializeField] private string key;
		[SerializeField] private SimpleStateMachine parent;
		[SerializeField] private SimpleStateMachine[] childs;

		[SerializeField] private SimpleStateMachineGroup[] stateMachineGroup = new SimpleStateMachineGroup[0];

		[SerializeField] private Image image;

		[SerializeField] private Animator animator;

		[SerializeField] private byte state;
		[SerializeField] private byte maxState;

		public static void SetState(SimpleStateMachine _targetStateMachineMonoBehaviour, byte _state)
		{
			if (_targetStateMachineMonoBehaviour is null) return;
			if (_targetStateMachineMonoBehaviour.state.Equals(_state)) return;
			if (_targetStateMachineMonoBehaviour.maxState <= _state) return;
			_targetStateMachineMonoBehaviour.state = _state;
			Refresh(_targetStateMachineMonoBehaviour);
		}
		public static void SetMaxState(SimpleStateMachine _targetStateMachineMonoBehaviour, byte _maxState)
		{ 
			if (_targetStateMachineMonoBehaviour is null) return;
			_targetStateMachineMonoBehaviour.maxState = _maxState;
			if (_targetStateMachineMonoBehaviour.stateMachineGroup.Length != _targetStateMachineMonoBehaviour.maxState)
			{
				var m_stateMachineGroups = new SimpleStateMachineGroup[_targetStateMachineMonoBehaviour.maxState];
				for (var i = 0; i < _targetStateMachineMonoBehaviour.maxState; i++)
				{
					var m_stateMachineGroup = new SimpleStateMachineGroup();
					if (i < _targetStateMachineMonoBehaviour.stateMachineGroup.Length)
						m_stateMachineGroup = _targetStateMachineMonoBehaviour.stateMachineGroup[i];
					m_stateMachineGroups[i] = m_stateMachineGroup;
				}
				_targetStateMachineMonoBehaviour.stateMachineGroup = m_stateMachineGroups;
			}
			if (_targetStateMachineMonoBehaviour.maxState < _targetStateMachineMonoBehaviour.state)
				SetState(_targetStateMachineMonoBehaviour,_maxState);

		}
		public static void Refresh(SimpleStateMachine _targetStateMachineMonoBehaviour)
		{
			if (SimpleStateMachineToGameObjectContainer is null) SimpleStateMachineToGameObjectContainer = new Dictionary<SimpleStateMachine, GameObject>();
			if (!SimpleStateMachineToGameObjectContainer.ContainsKey(_targetStateMachineMonoBehaviour))
				SimpleStateMachineToGameObjectContainer.Add(_targetStateMachineMonoBehaviour, _targetStateMachineMonoBehaviour.gameObject);

			if (!string.IsNullOrEmpty(_targetStateMachineMonoBehaviour.key) && !(_targetStateMachineMonoBehaviour.parent))
			{
				if (StringKeyToSimpleStateMachineContainer is null) StringKeyToSimpleStateMachineContainer = new Dictionary<string, SimpleStateMachine>();
				if (!StringKeyToSimpleStateMachineContainer.ContainsKey(_targetStateMachineMonoBehaviour.key))
					StringKeyToSimpleStateMachineContainer.Add(_targetStateMachineMonoBehaviour.key, _targetStateMachineMonoBehaviour);
			}
			var m_state = _targetStateMachineMonoBehaviour.state;
			if (m_state >= _targetStateMachineMonoBehaviour.maxState) return;
			var m_applyStateMachineGroup = _targetStateMachineMonoBehaviour.stateMachineGroup[m_state ];

			 var m_targetGameObject = SimpleStateMachineToGameObjectContainer[_targetStateMachineMonoBehaviour];
			if (m_applyStateMachineGroup.simpleStateMachineGameObject.stateMachineActive)
			{ 
				m_targetGameObject.SetActive(m_applyStateMachineGroup.simpleStateMachineGameObject.active);
			}
			var m_targetImage = _targetStateMachineMonoBehaviour.image;
			if (!(m_targetImage is null) && m_applyStateMachineGroup.simpleStateMachineImage.stateMachineActive)
			{
				m_targetImage.enabled = m_applyStateMachineGroup.simpleStateMachineImage.enable;

				m_targetImage.sprite = m_applyStateMachineGroup.simpleStateMachineImage.sprite;
				m_targetImage.color = m_applyStateMachineGroup.simpleStateMachineImage.color;
				m_targetImage.material = m_applyStateMachineGroup.simpleStateMachineImage.material;
				m_targetImage.raycastTarget = m_applyStateMachineGroup.simpleStateMachineImage.raycastTarget;
				m_targetImage.raycastPadding = Vector4.zero+ m_applyStateMachineGroup.simpleStateMachineImage.raycastPadding;

				m_targetImage.maskable = m_applyStateMachineGroup.simpleStateMachineImage.maskable;
			}
			if (_targetStateMachineMonoBehaviour.childs is null) return;
			foreach (var child in _targetStateMachineMonoBehaviour.childs)
			{
				if (child is null) continue;
				child.childs = null;
				child.key = _targetStateMachineMonoBehaviour.key;
				child.parent = _targetStateMachineMonoBehaviour;
				SetState(child, m_state);
			}
		}
		public static void CopyStateMachine(SimpleStateMachine _targetStateMachineMonoBehaviour, byte _targetState, SimpleStateMachineGroup _copyData)
		{
			if (_targetStateMachineMonoBehaviour is null) return;
			var m_paste = new SimpleStateMachineGroup();

			m_paste.name = _copyData.name;
			m_paste.simpleStateMachineGameObject = _copyData.simpleStateMachineGameObject;
		
			m_paste.simpleStateMachineImage = _copyData.simpleStateMachineImage;

			var m_targetState = _targetState;
			var m_pasteStateMachineGroups = new List<SimpleStateMachineGroup>();
			m_pasteStateMachineGroups.AddRange(_targetStateMachineMonoBehaviour.stateMachineGroup);
			if (m_pasteStateMachineGroups.Count() < m_targetState)
			{
				m_targetState = (byte)(_targetStateMachineMonoBehaviour.GetMaxState() + 1);
				SetMaxState(_targetStateMachineMonoBehaviour, m_targetState);
				m_pasteStateMachineGroups.Add(m_paste);
			}
			else m_pasteStateMachineGroups[m_targetState] = m_paste;
			_targetStateMachineMonoBehaviour.stateMachineGroup = m_pasteStateMachineGroups.ToArray();
		}
		public static void RemoveStateMachine(SimpleStateMachine _targetStateMachineMonoBehaviour, byte _targetState)
		{
			if (_targetStateMachineMonoBehaviour.maxState == 0 ||
				_targetStateMachineMonoBehaviour.maxState <= _targetState) return;
			if (_targetStateMachineMonoBehaviour.maxState == 1)
			{
				SetMaxState(_targetStateMachineMonoBehaviour, 0); 
				return;
			}
			var m_applyStateGroups = new List<SimpleStateMachineGroup>();
			m_applyStateGroups.AddRange(_targetStateMachineMonoBehaviour.stateMachineGroup);
			m_applyStateGroups.RemoveAt(_targetState);
			SetMaxState(_targetStateMachineMonoBehaviour, (byte)m_applyStateGroups.Count);
			for (var i = 0; i < m_applyStateGroups.Count; i++)
				_targetStateMachineMonoBehaviour.stateMachineGroup[i] = m_applyStateGroups[i];
		}
		private void OnEnable()
		{
			if (SimpleStateMachineToGameObjectContainer is null) SimpleStateMachineToGameObjectContainer = new Dictionary<SimpleStateMachine, GameObject>();
			if (!SimpleStateMachineToGameObjectContainer.ContainsKey(this))
				SimpleStateMachineToGameObjectContainer.Add(this, gameObject);

			if (string.IsNullOrEmpty(key)) return;
			if (!(parent is null)) return;
			if (StringKeyToSimpleStateMachineContainer is null) StringKeyToSimpleStateMachineContainer = new Dictionary<string, SimpleStateMachine>();
			if (!StringKeyToSimpleStateMachineContainer.ContainsKey(key)) StringKeyToSimpleStateMachineContainer.Add(key, this);
		}
		private void OnDisable()
		{
			if (SimpleStateMachineToGameObjectContainer.ContainsKey(this))
				SimpleStateMachineToGameObjectContainer.Remove(this);

			if (string.IsNullOrEmpty(key)) return;
			if (StringKeyToSimpleStateMachineContainer is null) return;
			if (StringKeyToSimpleStateMachineContainer.ContainsKey(key))
				StringKeyToSimpleStateMachineContainer.Remove(key);
		}
		private void OnDestroy()
		{
			if (childs is null) return;
			foreach (var child in childs)
			{
				if (child is null) continue;
				child.parent = null;
				child.key = string.Empty;
			}
		}
		public byte GetState() => state;
		public byte GetMaxState() => maxState;
		public void Initialize()
		{
			image = GetComponent<Image>();
			animator = GetComponent<Animator>();
		}
		public string GetKey() => this.key;
		public void SetKey(string _key)
		{
			this.key = _key;
			if (childs is null) return;
			foreach (var child in childs)
			{
				if (child is null) continue;
				child.SetKey(this.key);
			}
		}
		public SimpleStateMachine[] GetChilds()
		{
			if (childs is null) return new SimpleStateMachine[0];
			var value = childs.Distinct().Where(child => child is null ? false : !child.Equals(this));

			return value.ToArray();
		}
		public void SetChilds(IEnumerable<SimpleStateMachine> _childs)
		{
			if (_childs is null)
			{
				if (!(childs is null))
				{
					foreach (var child in childs)
					{
						if (child is null) continue;
						child.SetKey(string.Empty);
						child.SetParent(null);
					}
				}

				childs = null;
				return;
			}
			var m_tempChilds = new List<SimpleStateMachine>();
			m_tempChilds.AddRange(_childs);
			foreach (var m_child in _childs)
			{
				if (m_child.childs is null) continue;
				m_tempChilds.AddRange(m_child.childs);
			}

			childs = m_tempChilds.Distinct().Where(child => !(child is null)).ToArray();
			foreach (var child in childs)
			{
				child.SetKey(key);
				child.SetParent(this);
			}
		}
		public SimpleStateMachine GetParent() => this.parent;
		public void SetParent(SimpleStateMachine _parent)
		{
			parent = _parent;
			SetChilds(null);
		}

		public SimpleStateMachineGroup GetStateMachineGroup(byte idx) => stateMachineGroup.Length > idx ? stateMachineGroup[idx] : new SimpleStateMachineGroup();

		public Image GetImage() => image;
		public void SetImage(Image _image) => image = _image;

		public Animator GetAnimator() => animator;
		public void SetAnimator(Animator _animator) => animator = _animator;
	}

}