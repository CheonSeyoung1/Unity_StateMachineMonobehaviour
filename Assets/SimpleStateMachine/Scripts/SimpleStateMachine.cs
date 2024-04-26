using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
namespace SimpleStateMachines
{
	public interface SimpleStateMachineBase<T>
	{
		public void Apply(T _target);
	}

	[System.Serializable]
	public struct SimpleStateMachineGameObject : SimpleStateMachineBase<GameObject>
	{
		public bool stateMachineActive;
		public bool active;

		public void Apply(GameObject _target)
		{
			if (_target is null) return;
			if (!stateMachineActive) return;
			_target.SetActive(active);
		}
	}
	[System.Serializable]
	public struct SimpleStateMachineTransform 
	{
		public bool stateMachineActive;
		/// <summary>
		/// true = global
		/// false = local
		/// </summary>
		public bool global;
		public bool positionApply;
		public Vector3 position;
		public bool rotationApply;
		public Vector3 rotation;
		public bool scaleApply;
		public Vector3 scale;

		public void Apply(Transform _target)
		{
			if (_target is null) return;
			if (!stateMachineActive) return;

			if (global)
			{
				if (positionApply){ _target.position = position; }
				if (rotationApply) { _target.rotation = Quaternion.Euler(rotation.x,rotation.y,rotation.z); }
			}
			else
			{
				if (positionApply) { _target.localPosition = position; }
				if (rotationApply) { _target.localRotation = Quaternion.Euler(rotation.x, rotation.y, rotation.z); }
				if (scaleApply) { _target.localScale = scale; }
			}
		}
	}

	[System.Serializable]
	public struct SimpleStateMachineMeshFilter 
	{
		public bool stateMachineActive;
		public Mesh mesh;
		public void Apply(MeshFilter _target)
		{
			if (_target is null) return;
			if (!stateMachineActive) return;
			_target.mesh = mesh;
		}
	}

	[System.Serializable]
	public struct SimpleStateMachineMeshRenderer
	{
		[System.Serializable]
		public struct LightingSetting
		{
			public UnityEngine.Rendering.ShadowCastingMode castShadows;
			public bool contributeGlobalIllumination;
			public ReceiveGI receiveGlobalIllumination;
		}
		[System.Serializable]
		public struct AdditionalSetting
		{
			public bool dynamicOcculusion;
			public uint renderingLayerMask;
		}

		public bool stateMachineActive; 
		
		public bool enable;
		public Material[] materials;

		public LightingSetting lightingSetting;
		public AdditionalSetting additionalSetting;
		public void Apply(MeshRenderer _target)
		{
			if (_target is null) return;
			if (!stateMachineActive) return;
			_target.enabled = enable;
			Material m_mat = null;
			if (!(materials is null))
			{ 
				var m_mats = materials.Where(m_mat => !(m_mat is null));
				if (m_mats.Any())
					m_mat = m_mats.First();
			}
			_target.material = m_mat;
			_target.materials = materials;

			_target.shadowCastingMode = lightingSetting.castShadows;
			if (_target.gameObject.isStatic != lightingSetting.contributeGlobalIllumination)
				_target.gameObject.isStatic = lightingSetting.contributeGlobalIllumination;
			_target.receiveGI = lightingSetting.receiveGlobalIllumination;

			_target.allowOcclusionWhenDynamic = additionalSetting.dynamicOcculusion;
			_target.renderingLayerMask = additionalSetting.renderingLayerMask;
		}
	}

	[System.Serializable]
	public struct SimpleStateMachineRectTransform { }


	[System.Serializable]
	public struct SimpleStateMachineImage : SimpleStateMachineBase<Image>
	{
		public bool stateMachineActive;

		public bool enable;

		public Sprite sprite;
		public Color color;
		public Material material;
		public bool raycastTarget;
		public Vector4 raycastPadding;

		public bool maskable;
		public void Apply(Image _target)
		{
			if (_target is null) return;
			if (!stateMachineActive) return;
			_target.enabled = enable;

			_target.sprite = sprite;
			_target.color = color;
			_target.material = material;
			_target.raycastTarget = raycastTarget;
			_target.raycastPadding = Vector4.zero + raycastPadding;

			_target.maskable = maskable;
		}
	}
	[System.Serializable]
	public struct SimpleStateMachineAnimator : SimpleStateMachineBase<Animator>
	{
		public bool stateMachineActive;

		public bool enable;

		public RuntimeAnimatorController controller;
		public Avatar avatar;
		public bool applyRootMotion;
		public AnimatorUpdateMode updateMode;
		public AnimatorCullingMode cullingMode;

		public void Apply(Animator _target)
		{
			if (!(_target)) return;
			if (!stateMachineActive) return;

			_target.enabled = enable;
			_target.runtimeAnimatorController = controller;
			_target.avatar = avatar;
			_target.applyRootMotion = applyRootMotion;
			_target.updateMode = updateMode;
			_target.cullingMode = cullingMode;
		}
	}
	[System.Serializable]
	public struct SimpleStateMachineAnimation : SimpleStateMachineBase<Animator>
	{
		public bool stateMachineActive;
		public bool forced;
		public string animationKey;

		public void Apply(Animator _target)
		{
			if (!(_target)) return;
			if (!stateMachineActive) return;
		}
	}
	[System.Serializable]
	public struct SimpleStateMachineGroup
	{
		public string name;
		public SimpleStateMachineGameObject simpleStateMachineGameObject;
		public SimpleStateMachineTransform simpleStateMachineTransform;
		public SimpleStateMachineMeshFilter simpleStateMachineMeshFilter;
		public SimpleStateMachineMeshRenderer simpleStateMachineMeshRenderer;
		public SimpleStateMachineImage simpleStateMachineImage;
		public SimpleStateMachineAnimator simpleStateMachineAnimator;
		public SimpleStateMachineAnimation simpleStateMachineAnimation;
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
		[SerializeField] private MeshFilter meshFilter;
		[SerializeField] private MeshRenderer meshRenderer;
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
			m_applyStateMachineGroup.simpleStateMachineGameObject.Apply(m_targetGameObject);

			var m_targetTransform = _targetStateMachineMonoBehaviour.transform;
			var m_targetRectTransform = m_targetTransform as RectTransform;
			if (m_targetTransform && m_targetRectTransform is null)
				m_applyStateMachineGroup.simpleStateMachineTransform.Apply(m_targetTransform);

			var m_targetMeshFilter = _targetStateMachineMonoBehaviour.meshFilter;
			m_applyStateMachineGroup.simpleStateMachineMeshFilter.Apply(m_targetMeshFilter);

			var m_targetMeshRenderer = _targetStateMachineMonoBehaviour.meshRenderer;
			m_applyStateMachineGroup.simpleStateMachineMeshRenderer.Apply(m_targetMeshRenderer);

			var m_targetImage = _targetStateMachineMonoBehaviour.image;
			m_applyStateMachineGroup.simpleStateMachineImage.Apply(m_targetImage);

			var m_targetAnimator = _targetStateMachineMonoBehaviour.animator;
			m_applyStateMachineGroup.simpleStateMachineAnimator.Apply(m_targetAnimator);
			m_applyStateMachineGroup.simpleStateMachineAnimation.Apply(m_targetAnimator);


			if (_targetStateMachineMonoBehaviour.childs is null) return;
			var m_childs = new List<SimpleStateMachine>();
			m_childs.AddRange(_targetStateMachineMonoBehaviour.childs);
			foreach (var child in _targetStateMachineMonoBehaviour.childs)
			{
				if (child is null) continue;
				if (child.childs is null) continue;
				m_childs.AddRange(child.childs);
				child.childs = null;
			}
			foreach (var child in m_childs)
			{
				if (child is null) continue;
				child.key = _targetStateMachineMonoBehaviour.key;
				child.parent = _targetStateMachineMonoBehaviour;
				SetState(child, m_state);
			}
			_targetStateMachineMonoBehaviour.childs = m_childs.ToArray();
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
			meshFilter = GetComponent<MeshFilter>();
			meshRenderer = GetComponent<MeshRenderer>();
			image = GetComponent<Image>();
			animator = GetComponent<Animator>();
		}
		public string GetKey() => this.key;
		public void SetKey(string _key)
		{
			if ((!string.IsNullOrEmpty(this.key)) && this.parent is null)
			{
				if (!(StringKeyToSimpleStateMachineContainer is null))
				{
					if (StringKeyToSimpleStateMachineContainer.ContainsKey(this.key))
						StringKeyToSimpleStateMachineContainer.Remove(this.key);
				}
			}

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

		public MeshFilter GetMeshFilter() => meshFilter;
		public void SetMeshFilter(MeshFilter _meshFilter) => meshFilter = _meshFilter;
		public MeshRenderer GetMeshRenderer() => meshRenderer;
		public void SetMeshRenderer(MeshRenderer _meshRenderer) => meshRenderer = _meshRenderer;

		public Image GetImage() => image;
		public void SetImage(Image _image) => image = _image;

		public Animator GetAnimator() => animator;
		public void SetAnimator(Animator _animator) => animator = _animator;
	}

}