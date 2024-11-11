using System;
using UnityEditor;
using UnityEngine;

namespace PxlTools.BuildAutomation.Data
{
	[CreateAssetMenu(fileName = "BuildAutomationConfig", menuName = "PxlTools/ScriptableObjects/BuildAutomationConfig", order = 1)]
	public class BuildAutomationConfig : ScriptableObject
	{
		public SceneReference[] MainScenes => _mainScenes;
		public SceneReference[] DemoScenes => _demoScenes;
		public bool IsDevelopmentBuild => _isDevelopmentBuild;
		public bool IncludeDemoScenes => _includeDemoScenes;
		public string[] AdditionalScriptingDefines => _additionalScriptingDefines;
		public string BuildName => _buildName;
		public string ReportName => _reportName;
		public string IOSPathAppend => _iOSPathAppend;
		public string AndroidPathAppend => _androidPathAppend;
		public string WebGLPathAppend => _webGLPathAppend;
		public string StandaloneBuildOutputPath => _standaloneBuildOutputPath;
		public string ExportBuildOutputPath => _exportBuildOutputPath;

		[Serializable]
		public struct SceneReference
		{
			public string Name;
			public string Path;
		}

		[SerializeField] private SceneReference[] _mainScenes;
		[SerializeField] private SceneReference[] _demoScenes;
		[Header("Advanced Settings: (overridden by command prompt)")]
		[SerializeField] private bool _isDevelopmentBuild = true;
		[SerializeField] private bool _includeDemoScenes = false;
		[SerializeField] private string[] _additionalScriptingDefines;
		[Header("Paths/Naming: (overridden by command prompt)")]
		[SerializeField] private string _buildName = "Default";
		[SerializeField] private string _reportName = "buildreport";
		[SerializeField] private string _iOSPathAppend = "iOS";
		[SerializeField] private string _androidPathAppend = "Android";
		[SerializeField] private string _webGLPathAppend = "WebGL";
		[SerializeField] private string _standaloneBuildOutputPath = "Builds/Standalone";
		[SerializeField] private string _exportBuildOutputPath = "Builds/Exported";

		/// <summary>
		/// When automated build overwrites our config, we need to save the value for the builds runtime reference.
		/// NOTE: If in editor, we need to set the file dirty and save assets to store change.
		/// </summary>
		/// <param name="value">Bool on whether to include demo scenes or not.</param>
		public void SetIncludeDemoScenes(bool value)
		{
			_includeDemoScenes = value;
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
#endif
		}

		public void SetIsDevelopmentBuild(bool value)
		{
			_isDevelopmentBuild = value;
#if UNITY_EDITOR
			EditorUtility.SetDirty(this);
			AssetDatabase.SaveAssets();
#endif
		}
	}
}
