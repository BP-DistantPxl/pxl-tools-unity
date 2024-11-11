#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System.Collections.Generic;
using PxlTools.BuildAutomation.Data;
using PxlTools.BuildAutomation.Utils;
using PxlTools.Core.Runtime;
using PxlTools.DebugMenu.Data;
using PxlTools.DebugMenu.Defines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

namespace PxlTools.DebugMenu.Runtime
{
	public class DebugSceneAccess : DebugOptionCollection
	{
		#region Variables

		public struct SceneReferenceData
		{
			public string IDPrefix;
			public BuildAutomationConfig.SceneReference[] SceneReferences;
			public UnityAction<int> OnDropDownChanged;
			public UnityAction OnButtonClicked;
		}

		private int _currentMainSceneDropDownIndex;
		private int _currentDemoSceneDropDownIndex;
		private SceneReferenceData _mainScenesReferenceData;
		private SceneReferenceData _demoScenesReferenceData;

		#endregion

		#region InheritedMethods

		public override void Initialise()
		{
			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Set up initial default drop down values
			_currentMainSceneDropDownIndex = 0;
			_currentDemoSceneDropDownIndex = 0;

			// Add main scenes and if config specifies, demo scenes to debug menu.
			AddMainSceneOptions(config.MainScenes);
			if (config.IncludeDemoScenes)
				AddDemoSceneOptions(config.DemoScenes);

			Debug.Log(string.Format(DebugMenuDefines.Strings.Common.TAB_ADDED, DebugMenuDefines.Strings.Scenes.TAB_NAME)); ;
		}

		#endregion

		#region DebugMenuPopulationMethods

		/// <summary>
		/// Generates SceneReferenceData for main scene collection and adds option datas to debug menu.
		/// </summary>
		/// <param name="mainSceneReferences">Array of scene references to populate reference data with.</param>
		private void AddMainSceneOptions(BuildAutomationConfig.SceneReference[] mainSceneReferences)
		{
			_mainScenesReferenceData = new SceneReferenceData()
			{
				IDPrefix = DebugMenuDefines.Strings.Scenes.MAIN_PREFIX,
				SceneReferences = mainSceneReferences,
				OnDropDownChanged = OnMainScenesDropDownChanged,
				OnButtonClicked = OnMainScenesButtonClicked
			};

			PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetSceneDatas(_mainScenesReferenceData), DebugMenuDefines.Strings.Scenes.TAB_NAME, DebugMenuDefines.Strings.Scenes.MAIN_HEADER_NAME, true);
		}

		/// <summary>
		/// Generates SceneReferenceData for demo scene collection and adds option datas to debug menu.
		/// </summary>
		/// <param name="demoSceneReferences">Array of scene references to populate reference data with.</param>
		private void AddDemoSceneOptions(BuildAutomationConfig.SceneReference[] demoSceneReferences)
		{
			_demoScenesReferenceData = new SceneReferenceData()
			{
				IDPrefix = DebugMenuDefines.Strings.Scenes.DEMO_PREFIX,
				SceneReferences = demoSceneReferences,
				OnDropDownChanged = OnDemoScenesDropDownChanged,
				OnButtonClicked = OnDemoScenesButtonClicked
			};

			PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetSceneDatas(_demoScenesReferenceData), DebugMenuDefines.Strings.Scenes.TAB_NAME, DebugMenuDefines.Strings.Scenes.DEMO_HEADER_NAME, true);
		}

		#endregion

		#region PublicEventMethods

		// Handle main scene drop down change and launch main scene button clicked.
		public void OnMainScenesDropDownChanged(int newIndex) => _currentMainSceneDropDownIndex = newIndex;
		public void OnMainScenesButtonClicked()
		{
			var sceneToLoad = _mainScenesReferenceData.SceneReferences[_currentMainSceneDropDownIndex];
			SceneManager.LoadScene(sceneToLoad.Path, LoadSceneMode.Single);
			Debug.Log(string.Format(DebugMenuDefines.Strings.Scenes.MAIN_SCENE_LOADED, sceneToLoad.Name, sceneToLoad.Path));
			DebugMenuManager.Instance.ForceCloseDebugMenu();
		}

		// Handle demo drop down change and launch demo scene button clicked.
		public void OnDemoScenesDropDownChanged(int newIndex) => _currentDemoSceneDropDownIndex = newIndex;
		public void OnDemoScenesButtonClicked()
		{
			var sceneToLoad = _demoScenesReferenceData.SceneReferences[_currentDemoSceneDropDownIndex];
			SceneManager.LoadScene(sceneToLoad.Path, LoadSceneMode.Single);
			Debug.Log(string.Format(DebugMenuDefines.Strings.Scenes.DEMO_SCENE_LOADED, sceneToLoad.Name, sceneToLoad.Path));
			DebugMenuManager.Instance.ForceCloseDebugMenu();
		}

		#endregion

		#region DebugDataGeneratingMethods

		/// <summary>
		/// Creates a dynamic drop down and button populated by the scene reference data passed in.
		/// </summary>
		/// <param name="sceneReferenceData">Scene Reference data struct holding all required options to dynamically link scenes with debug options.</param>
		/// <returns>Array of DebugOptionData (of varying sub types), ready for use in debug menu.</returns>
		private DebugOptionData[] GetSceneDatas(SceneReferenceData sceneReferenceData)
		{
			// Generate list of scene names from scene references
			var sceneNames = new List<string>();
			foreach (var sceneReference in sceneReferenceData.SceneReferences)
			{
				sceneNames.Add(sceneReference.Name);
			}

			// Generate drop down debug option, populated with scene reference names
			var scenesDropDown = new DebugOptionData()
			{
				Id = $"drpdwn_{sceneReferenceData.IDPrefix}Scenes",
				Type = OptionSubType.DropDown,
				SubData = new DebugOptionDropDownData(sceneReferenceData.OnDropDownChanged)
				{
					DisplayText = $"Select {sceneReferenceData.IDPrefix} Scene",
					Options = sceneNames,
					Value = 0
				}
			};

			// Generate button debug option that when clicked launches the current selected scene from drop down.
			var scenesButton = new DebugOptionData()
			{
				Id = $"btn_{sceneReferenceData.IDPrefix}Scenes",
				Type = OptionSubType.Button,
				SubData = new DebugOptionButtonData(sceneReferenceData.OnButtonClicked)
				{
					DisplayText = $"Launch {sceneReferenceData.IDPrefix} Scene",
					ButtonText = "Launch"
				}
			};

			return new DebugOptionData[]
			{
				scenesDropDown,
				scenesButton
			};
		}

		#endregion
	}
}
#endif