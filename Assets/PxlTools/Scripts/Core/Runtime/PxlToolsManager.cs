using PxlTools.Core.Defines;
using PxlTools.DebugMenu.Runtime;
using PxlTools.Input.Runtime;
using UnityEngine;

namespace PxlTools.Core.Runtime
{
	public class PxlToolsManager : MonoBehaviour
	{
		#region Variables

		public static PxlToolsManager Instance { get; private set; }

		public DebugMenuManager DebugMenuManager => _debugMenuManager;
		public MultiTouchInputManager MultiTouchInputManager => _multiTouchInputManager;
		public DesktopInputManager DesktopInputManager => _desktopInputManager;

		[Header("Settings:")]
		[SerializeField] private PxlToolsDefines.Data.ToolTypes _enabledTools;

		[Header("Editor Utility:")]
		[InspectorButton("OnCreateMissingTools", 300.0f)]
		[SerializeField] private bool _createMissingTools;
		[InspectorButton("OnCleanupUnusedTools", 300.0f)]
		[SerializeField] private bool _cleanupUnusedTools;

		private DebugMenuManager _debugMenuManager;
		private MultiTouchInputManager _multiTouchInputManager;
		private DesktopInputManager _desktopInputManager;

		#endregion

		#region MonoBehaviourMethods

		private void Awake()
		{
			// Set up singleton
			if (Instance != null && Instance != this)
			{
				Destroy(this.gameObject);
				return;
			}
			else
				Instance = this;

			// Set to not destroy between scenes
			DontDestroyOnLoad(this.gameObject);

			// Set bools for development build only tools
			SetDevelopmentOnlyToolFlags();

			// Get, create/delete and initialise all enabled tools
			TryGetToolInstances();
			CreateMissingTools();
			DeleteUnusedTools();
			InitialiseTools();
		}

		#endregion

		#region ReferenceHandlingMethods

		/// <summary>
		/// Used during play and in editor to find instance of each tool in the scene.
		/// </summary>
		private void TryGetToolInstances()
		{
			// Init log string
			var logString = PxlToolsDefines.Strings.GET_TOOLS_LOG_PREFIX;

			// Find debug menu instance or find by component (helpful in editor)
			_debugMenuManager = DebugMenuManager.Instance;
			if (!_debugMenuManager)
				_debugMenuManager = FindFirstObjectByType<DebugMenuManager>(FindObjectsInactive.Include);

			// Increment log
			logString += $"Debug Menu - {_debugMenuManager != null}\n";

			// Find multi touch input manager instance or find by component (helpful in editor)
			_multiTouchInputManager = MultiTouchInputManager.Instance;
			if (!_multiTouchInputManager)
				_multiTouchInputManager = FindFirstObjectByType<MultiTouchInputManager>(FindObjectsInactive.Include);

			// Increment log
			logString += $"MultiTouch Input Manager - {_multiTouchInputManager != null}\n";

			// Find desktop input manager instance or find by component (helpful in editor)
			_desktopInputManager = DesktopInputManager.Instance;
			if (!_desktopInputManager)
				_desktopInputManager = FindFirstObjectByType<DesktopInputManager>(FindObjectsInactive.Include);

			// Increment log
			logString += $"Desktop Input Manager - {_desktopInputManager != null}\n";

			// Other tools to go here...

			// Output result log string
			Debug.LogWarning(logString);
		}

		/// <summary>
		/// Creates prefab instance (during play or in editor) of each tool that cannot be found in scene.
		/// </summary>
		private void CreateMissingTools()
		{
			// Init log string
			var logString = PxlToolsDefines.Strings.CREATE_TOOLS_LOG_PREFIX;

			// Debug menu checks
			if (_enabledTools.HasFlag(PxlToolsDefines.Data.ToolTypes.DebugMenu))
			{
				if (!_debugMenuManager)
				{
					if (Application.isPlaying)
						_debugMenuManager = Instantiate(Resources.Load(PxlToolsDefines.Resources.DEBUG_MENU_PREFAB), this.transform) as DebugMenuManager;
#if UNITY_EDITOR
					else
						_debugMenuManager = UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load(PxlToolsDefines.Resources.DEBUG_MENU_PREFAB), this.transform) as DebugMenuManager;
#endif
					// Increment log
					logString += "Debug Menu\n";
				}
				else
					_debugMenuManager.transform.SetParent(this.transform);
			}

			// Multi touch input manager checks
			if (_enabledTools.HasFlag(PxlToolsDefines.Data.ToolTypes.MultiTouchInputManager))
			{
				if (!_multiTouchInputManager)
				{
					if (Application.isPlaying)
						_multiTouchInputManager = Instantiate(Resources.Load(PxlToolsDefines.Resources.MULTITOUCH_INPUT_PREFAB), this.transform) as MultiTouchInputManager;
#if UNITY_EDITOR
					else
						_multiTouchInputManager = UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load(PxlToolsDefines.Resources.MULTITOUCH_INPUT_PREFAB), this.transform) as MultiTouchInputManager;
#endif
					// Increment log
					logString += "MultiTouch Input Manager\n";
				}
				else
					_multiTouchInputManager.transform.SetParent(this.transform);
			}

			// Desktop input manager checks
			if (_enabledTools.HasFlag(PxlToolsDefines.Data.ToolTypes.DesktopInputManager))
			{
				if (!_desktopInputManager)
				{
					if (Application.isPlaying)
						_desktopInputManager = Instantiate(Resources.Load(PxlToolsDefines.Resources.DESKTOP_INPUT_PREFAB), this.transform) as DesktopInputManager;
#if UNITY_EDITOR
					else
						_desktopInputManager = UnityEditor.PrefabUtility.InstantiatePrefab(Resources.Load(PxlToolsDefines.Resources.DESKTOP_INPUT_PREFAB), this.transform) as DesktopInputManager;
#endif
					// Increment log
					logString += "Desktop Input Manager\n";
				}
				else
					_desktopInputManager.transform.SetParent(this.transform);
			}

			// Other tools to go here...

			// Output result log string
			if (logString == PxlToolsDefines.Strings.CREATE_TOOLS_LOG_PREFIX)
				logString += "None";
			Debug.LogWarning(logString);
		}

		/// <summary>
		/// Deletes any tool instances that exist in scene (during play and in editor)
		/// </summary>
		private void DeleteUnusedTools()
		{
			// Init log string
			var logString = PxlToolsDefines.Strings.DELETE_TOOLS_LOG_PREFIX;

			if (!_enabledTools.HasFlag(PxlToolsDefines.Data.ToolTypes.DebugMenu) && _debugMenuManager)
			{
				if (Application.isPlaying)
					Destroy(_debugMenuManager.gameObject);
				else
					DestroyImmediate(_debugMenuManager.gameObject);

				_debugMenuManager = null;

				// Increment log
				logString += "Debug Menu\n";
			}

			if (!_enabledTools.HasFlag(PxlToolsDefines.Data.ToolTypes.MultiTouchInputManager) && _multiTouchInputManager)
			{
				if (Application.isPlaying)
					Destroy(_multiTouchInputManager.gameObject);
				else
					DestroyImmediate(_multiTouchInputManager.gameObject);

				_multiTouchInputManager = null;

				// Increment log
				logString += "MultiTouch Input Manager\n";
			}

			if (!_enabledTools.HasFlag(PxlToolsDefines.Data.ToolTypes.DesktopInputManager) && _desktopInputManager)
			{
				if (Application.isPlaying)
					Destroy(_desktopInputManager.gameObject);
				else
					DestroyImmediate(_desktopInputManager.gameObject);

				_desktopInputManager = null;

				// Increment log
				logString += "Desktop Input Manager\n";
			}

			// Other tools to go here...

			// Output result log string
			if (logString == PxlToolsDefines.Strings.DELETE_TOOLS_LOG_PREFIX)
				logString += "None";
			Debug.LogWarning(logString);
		}

		/// <summary>
		/// Run through and initialise all tools that are enabled
		/// </summary>
		private void InitialiseTools()
		{
			_debugMenuManager?.Initialise();
			_multiTouchInputManager?.Initialise(true);
			_desktopInputManager?.Initialise(true);
		}

		#endregion

		#region DevelopmentBuildOnlyHandlingMethods

		/// <summary>
		/// Sets development build only tools flag to false so that they are auto removed / ignored in release builds.
		/// </summary>
		private void SetDevelopmentOnlyToolFlags()
		{
#if !DEVELOPMENT_BUILD && !UNITY_EDITOR
			_enabledTools = ~PxlToolsDefines.Data.ToolTypes.DebugMenu;
#endif
		}

		#endregion

		#region EditorInspectorHandlingMethods
#if UNITY_EDITOR
		/// <summary>
		/// Editor utility button method, reuses functions to create any tools that are missing
		/// </summary>
		private void OnCreateMissingTools()
		{
			TryGetToolInstances();
			CreateMissingTools();
		}

		/// <summary>
		/// Editor utility button method, reuses functions to delete any tools that are not needed
		/// </summary>
		private void OnCleanupUnusedTools()
		{
			TryGetToolInstances();
			DeleteUnusedTools();
		}
#endif
#endregion
	}
}