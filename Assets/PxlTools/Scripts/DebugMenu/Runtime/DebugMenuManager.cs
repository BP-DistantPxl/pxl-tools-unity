#if DEVELOPMENT_BUILD || UNITY_EDITOR
// NOTE: These namespaces do not exist in non development builds to prevent hacking
using PxlTools.DebugMenu.Defines;
using PxlTools.DebugMenu.Data;
#endif
using System;
using System.Collections.Generic;
using PxlTools.DebugMenu.Views;
using UnityEngine;
using UnityEngine.UI;

namespace PxlTools.DebugMenu.Runtime
{
	public class DebugMenuManager : MonoBehaviour
	{
		#region Variables

#if DEVELOPMENT_BUILD || UNITY_EDITOR
		[Serializable]
		public struct DebugMenuData
		{
			public List<DebugTabData> Tabs;
			public List<DebugHeaderData> Headers;
			public List<DebugOptionData> Options;
		}
#endif

		// NOTE: Instance must exist and be setup in non development builds to prevent compiler issues on PxlToolsManager...
		// ... Primarily, so that PxlToolsManager can handle deleting instance from scene if it exists in release builds.
		public static DebugMenuManager Instance { get; private set; }

#if DEVELOPMENT_BUILD || UNITY_EDITOR
		public DebugMenuData Data => _debugMenuData;

		[Header("Components:")]
		[SerializeField] private Button _revealButton;
		[SerializeField] private DebugMenuView _debugMenuView;
		[Header("Debug Option Collections:")]
		[SerializeField] private DebugOptionCollection[] _optionCollections;
		[Header("Preset Data:")]
		[SerializeField] private DebugMenuData _debugMenuData;

		private int _revealButtonActivePressCount = 0;
		private float _revealButtonActivePressTimer = 0.0f;
		private int _debugOptionsLastCount = -1;
#endif

		#endregion

		#region MonoBehaviourMethods

		public void Initialise()
		{
			// Set up singleton
			if (Instance != null && Instance != this)
			{
				Destroy(this.gameObject);
				return;
			}
			else
				Instance = this;

#if DEVELOPMENT_BUILD || UNITY_EDITOR
			// Run through attached collection scripts to add to data
			foreach (var optionCollection in _optionCollections)
				optionCollection.Initialise();

			// Initialise view UI and attach close behaviour
			_debugMenuView.Initialise(_debugMenuData);
			_debugMenuView.OnClose.AddListener(SetRevealButtonInteractable);

			// Initialise reveal button behaviours
			_revealButton.interactable = true;
			_revealButton.onClick.AddListener(OnRevealButtonPressed);
#endif
		}

#if DEVELOPMENT_BUILD || UNITY_EDITOR
		private void Update()
		{
			if (_revealButtonActivePressCount > 0)
			{
				// Increment press timer
				_revealButtonActivePressTimer += Time.deltaTime;

				// Timed out before required presses so reset tracking vars
				if (_revealButtonActivePressTimer >= DebugMenuDefines.Input.MAX_PRESS_TIME)
				{
					_revealButtonActivePressCount = 0;
					_revealButtonActivePressTimer = 0.0f;
				}
			}
		}
#endif
		// NOTE: Must end region in non development builds to prevent compiler issues
		#endregion

#if DEVELOPMENT_BUILD || UNITY_EDITOR
		#region InteractionMethods

		/// <summary>
		/// On each reveal button press, we increment press count. If requirement is reached, reveal UI and disable reveal button.
		/// </summary>
		private void OnRevealButtonPressed()
		{
			_revealButtonActivePressCount++;

			if (_revealButtonActivePressCount >= DebugMenuDefines.Input.REQUIRED_PRESSES)
			{
				_debugMenuView.Open();
				_revealButton.interactable = false;
			}
		}

		/// <summary>
		/// Hooked onto debug menu views close button behaviour to re-enable reveal button
		/// </summary>
		private void SetRevealButtonInteractable() => _revealButton.interactable = true;

		#endregion

		#region DataAdjustmentMethods

		/// <summary>
		/// Allows new debug options to be added via direct call rather than just in editor.
		/// NOTE: DebugOptionData.OptionSubType should match the derived SubData class.
		/// </summary>
		/// <param name="debugOption">The debug option to add to the debug menu.</param>
		/// <param name="tabTitle">Title of the tab to add the option to, if new, we create it, if null, option will be visible at all times.</param>
		/// <param name="headerTitle">Title of the header to add the option to, if new, we create it, if null, option will be visible at all times.</param>
		/// <param name="delayUpdate">Whether to update the debug menu view once this option is added or delay update (usually used if adjusting several to only update once all are complete).</param>
		public void AddOption(DebugOptionData debugOption, string tabTitle = null, string headerTitle = null, bool delayUpdate = false)
		{
			// Initialise lists if needed
			if (_debugMenuData.Options == null)
				_debugMenuData.Options = new List<DebugOptionData>();
			if (_debugMenuData.Headers == null)
				_debugMenuData.Headers = new List<DebugHeaderData>();
			if (_debugMenuData.Tabs == null)
				_debugMenuData.Tabs = new List<DebugTabData>();

			// Check if option already exists and ignore if required
			if (_debugMenuData.Options.Contains(debugOption))
			{
				Debug.LogError(string.Format(DebugMenuDefines.Strings.MenuManager.OPTION_EXISTS_ERROR, debugOption.Id));
				return;
			}

			// Add option to data
			_debugMenuData.Options.Add(debugOption);

			// Only assign tab or header if details are provided
			if (tabTitle != null && headerTitle != null)
			{
				// Look for existing tab data
				bool doesTabExist = false;
				foreach (DebugTabData tab in _debugMenuData.Tabs)
				{
					if (tab.Id == tabTitle)
					{
						// Tab already exists, so add header id if it isn't already tracked.
						doesTabExist = true;
						if (!tab.HeaderIDs.Contains(tabTitle + headerTitle))
							tab.HeaderIDs.Add(tabTitle + headerTitle);
					}
				}

				// Tab data doesnt exist, so create new one, assign header ID and add to tabs list.
				if (!doesTabExist)
				{
					var newTab = new DebugTabData()
					{
						Title = tabTitle,
						HeaderIDs = new List<string>()
					};
					newTab.HeaderIDs.Add(tabTitle + headerTitle);
					_debugMenuData.Tabs.Add(newTab);
				}

				// Look for existing header data
				bool doesHeaderExist = false;
				foreach (DebugHeaderData header in _debugMenuData.Headers)
				{
					if (header.Id == tabTitle + headerTitle)
					{
						// Header already exists, so add debug option id if it isn't already tracked.
						doesHeaderExist = true;
						if (!header.OptionIds.Contains(debugOption.Id))
							header.OptionIds.Add(debugOption.Id);
					}
				}

				// Header data doesnt exist, so create new one, assign option ID and add to headers list.
				if (!doesHeaderExist)
				{
					var newHeader = new DebugHeaderData()
					{
						Id = tabTitle + headerTitle,
						Title = headerTitle,
						OptionIds = new List<string>()
					};
					newHeader.OptionIds.Add(debugOption.Id);
					_debugMenuData.Headers.Add(newHeader);
				}
			}

			// Do not update menu view (most likely, more option changes are due so delay was requested)
			if (!delayUpdate)
				_debugMenuView.UpdateDataUI(Data);
		}

		/// <summary>
		/// Allows an array of new debug options to be added via direct call rather than just in editor.
		/// NOTE: DebugOptionData.OptionSubType should match the derived SubData class.
		/// </summary>
		/// <param name="debugOptions">The debug options to add to the debug menu.</param>
		/// <param name="tabTitle">Title of the tab to add the option to, if new, we create it, if null, option will be visible at all times.</param>
		/// <param name="headerTitle">Title of the header to add the option to, if new we create it, if null, option will be visible at all times.</param> 
		/// <param name="delayUpdate">Whether to update the debug menu view once these options are added or delay update (usually used if adjusting several to only update once all are complete).</param>
		public void AddOptions(DebugOptionData[] debugOptions, string tabTitle = null, string headerTitle = null, bool delayUpdate = false)
		{
			// Iterate through each option and add as required
			foreach (DebugOptionData option in debugOptions)
			{
				AddOption(option, tabTitle, headerTitle, true);
			}

			// Do not update menu view (most likely, more option changes are due so delay was requested)
			if (!delayUpdate)
				_debugMenuView.UpdateDataUI(Data);
		}

		/// <summary>
		/// Allows a debug option to be removed from the debug menu.
		/// </summary>
		/// <param name="debugOption">The debug option to remove from the debug menu.</param>
		/// <param name="delayUpdate">Whether to update the debug menu view once this option is removed or delay update (usually used if adjusting several to only update once all are complete).</param>
		public void RemoveOption(DebugOptionData debugOption, bool delayUpdate = false)
		{
			// If lists are null, something went wrong, nothing to remove.
			if (_debugMenuData.Options == null) return;
			if (_debugMenuData.Headers == null) return;
			if (_debugMenuData.Tabs == null) return;

			if (!_debugMenuData.Options.Contains(debugOption))
			{
				Debug.LogError(string.Format(DebugMenuDefines.Strings.MenuManager.OPTION_DOESNT_EXIST_ERROR, debugOption.Id));
				return;
			}

			// Remove from options list
			_debugMenuData.Options.Remove(debugOption);

			// Prep list to track headers and tabs that are now empty
			var headersToRemove = new List<DebugHeaderData>();
			var tabsToRemove = new List<DebugTabData>();

			// Iterate through each header and find any with this option attached
			foreach (var headerData in _debugMenuData.Headers)
			{
				if (headerData.OptionIds.Contains(debugOption.Id))
				{
					// Remove option from header and if header is now empty, add to empty header tracker.
					headerData.OptionIds.Remove(debugOption.Id);
					if (headerData.OptionIds.Count <= 0)
						headersToRemove.Add(headerData);
				}
			}

			// Iterate through each header that is now empty of options.
			foreach (var header in headersToRemove)
			{
				// Iterate through each tab and find any with this header attached
				foreach (var tabData in _debugMenuData.Tabs)
				{
					if (tabData.HeaderIDs.Contains(header.Id))
					{
						// Remove header from tab and if tab is now empty, add to empty tab tracker.
						tabData.HeaderIDs.Remove(header.Id);
						if (tabData.HeaderIDs.Count <= 0)
							tabsToRemove.Add(tabData);
					}
				}

				// Remove header from data tracking.
				if (_debugMenuData.Headers.Contains(header))
					_debugMenuData.Headers.Remove(header);
			}

			// Finally, iterate through each tab that is now empty of headers.
			foreach (var tab in tabsToRemove)
			{
				// Remove tab from data tracking
				if (_debugMenuData.Tabs.Contains(tab))
					_debugMenuData.Tabs.Remove(tab);
			}

			// Do not update menu view (most likely, more option changes are due so delay was requested)
			if (!delayUpdate)
				_debugMenuView.UpdateDataUI(Data);
		}

		/// <summary>
		/// Allows an array of debug options to be removed from the debug menu.
		/// </summary>
		/// <param name="debugOptions">The debug options to remove from the debug menu.</param>
		/// <param name="delayUpdate">Whether to update the debug menu view once these options are removed or delay update (usually used if adjusting several to only update once all are complete).</param>
		public void RemoveOptions(DebugOptionData[] debugOptions, bool delayUpdate = false)
		{
			// Iterate through each option and remove as required (delay update until all are removed)
			foreach (DebugOptionData option in debugOptions)
			{
				RemoveOption(option, true);
			}

			// Do not update menu view (most likely, more option changes are due so delay was requested)
			if (!delayUpdate)
				_debugMenuView.UpdateDataUI(Data);
		}

		/// <summary>
		/// Allows a header (found directly by headerID) and all attached debug options to be removed from the debug menu.
		/// </summary>
		/// <param name="headerID">The ID of the header to remove from the debug menu.</param>
		/// <param name="delayUpdate">Whether to update the debug menu view once the header is removed or delay update (usually used if adjusting several to only update once all are complete).</param>
		public void RemoveHeaderByID(string headerID, bool delayUpdate = false)
		{
			// If list is null, something went wrong, nothing to remove.
			if (_debugMenuData.Headers == null) return;

			// Collect list of all option datas attached to the header.
			var optionsToRemove = new List<DebugOptionData>();
			optionsToRemove.AddRange(GetHeaderOptionDatas(headerID));

			// Use remove option logic to handle safe removal (this will also remove empty tabs and headers automatically).
			RemoveOptions(optionsToRemove.ToArray(), true);

			// Do not update menu view (most likely, more option changes are due so delay was requested)
			if (!delayUpdate)
				_debugMenuView.UpdateDataUI(Data);
		}

		/// <summary>
		/// Allows a header (found via header title and tab title parsing into headerID) and all attached debug options to be removed from the debug menu.
		/// </summary>
		/// <param name="tabTitle">The title of the tab the header is attached to (this matches tabID)./param>
		/// <param name="headerTitle">The title of the header to remove from the debug menu.</param>
		/// <param name="delayUpdate">Whether to update the debug menu view once the header is removed or delay update (usually used if adjusting several to only update once all are complete).</param>
		public void RemoveHeaderByTitle(string tabTitle, string headerTitle, bool delayUpdate = false) => RemoveHeaderByID(tabTitle + headerTitle, delayUpdate);

		/// <summary>
		/// Allows a tab and all attached headers (with in turn, their attached debug options) to be removed from the debug menu.
		/// </summary>
		/// <param name="tabID">The ID of the tab to remove from the debug menu.</param>
		/// <param name="delayUpdate">Whether to update the debug menu view once the tab is removed or delay update (usually used if adjusting several to only update once all are complete).</param>
		public void RemoveTab(string tabID, bool delayUpdate = false)
		{
			// If list is null, something went wrong, nothing to remove.
			if (_debugMenuData.Tabs == null) return;

			// Iterate through tab datas to find the requested tab
			foreach (var tab in _debugMenuData.Tabs)
			{
				if (tab.Id == tabID)
				{
					// Collect list of all option datas attached to all of the headers, that are attached to the tab.
					var optionsToRemove = new List<DebugOptionData>();
					foreach (var headerID in tab.HeaderIDs)
					{
						optionsToRemove.AddRange(GetHeaderOptionDatas(headerID));
					}

					// Use remove option logic to handle safe removal (this will also remove empty tabs and headers automatically).
					RemoveOptions(optionsToRemove.ToArray(), true);

					// Do not update menu view (most likely, more option changes are due so delay was requested)
					if (!delayUpdate)
						_debugMenuView.UpdateDataUI(Data);
				}
			}
		}

		#endregion

		#region HelperMethods

		/// <summary>
		/// Finds and returns a list of all the option data objects attached to a header via headerID.
		/// </summary>
		/// <param name="headerID">The ID of the header we want to find the attached debug options of.</param>
		/// <returns>List of DebugOptionDatas attached to the specified header.</returns>
		public List<DebugOptionData> GetHeaderOptionDatas(string headerID)
		{
			// If list is null, something went wrong, nothing to remove.
			if (_debugMenuData.Headers == null) return null;
			if (_debugMenuData.Options == null) return null;

			var optionDatas = new List<DebugOptionData>();
			foreach (var headerData in _debugMenuData.Headers)
			{
				if (headerData.Id == headerID)
				{
					foreach (var optionData in _debugMenuData.Options)
					{
						if (headerData.OptionIds.Contains(optionData.Id))
						{
							optionDatas.Add(optionData);
						}
					}
				}
			}

			return optionDatas;
		}

		/// <summary>
		/// Closes debug menu view and returns interactivity to reveal button.
		/// Quality of life public function allowing external functions to force close when necessary.
		/// </summary>
		public void ForceCloseDebugMenu()
		{
			_debugMenuView.Close();
			_revealButton.interactable = true;
		}

		#endregion

		#region EditorInspectorHandlingMethods
#if UNITY_EDITOR
		/// <summary>
		/// Runs validation when component Serialized data is updated. 
		/// Adds new inherited class to inspector based on type to allow differing inspectors based on required option data.
		/// </summary>
		private void OnValidate()
		{
			if (Application.isPlaying) return;
			if (_debugMenuData.Options == null) return;

			// If tracker is less than 0, we are reloading the scene or component and should treat existing options as valid, so set flag
			if (_debugOptionsLastCount < 0)
			{
				_debugOptionsLastCount = _debugMenuData.Options.Count;
				return;
			}

			// Iterate through debug options...
			for (int i = 0; i < _debugMenuData.Options.Count; i++)
			{
				// Store option
				DebugOptionData option = _debugMenuData.Options[i];

				// Only create new class instance if there is no valid data OR is new debug option
				if (IsDataValid(option) && !IsNewDebugOption(i)) continue;

				switch (option.Type)
				{
					case OptionSubType.Slider:
						option.SubData = new DebugOptionSliderData();
						break;
					case OptionSubType.Toggle:
						option.SubData = new DebugOptionToggleData();
						break;
					case OptionSubType.Label:
						option.SubData = new DebugOptionLabelData();
						break;
					case OptionSubType.Button:
						option.SubData = new DebugOptionButtonData();
						break;
					case OptionSubType.DropDown:
						option.SubData = new DebugOptionDropDownData();
						break;
					case OptionSubType.Log:
						option.SubData = new DebugOptionLogData();
						break;
				}

				// Update option
				_debugMenuData.Options[i] = option;
			}

			// Update tracking variable
			_debugOptionsLastCount = _debugMenuData.Options.Count;
		}

		/// <summary>
		/// Validates if the DebugOption.Data is the correct derived class based on DebugOption.Type.
		/// NOTE: Using [SerializedReference] for inherited variables requires manual creation of correct data type to allow inspector assignment.
		/// </summary>
		/// <param name="option">DebugOption to validate.</param>
		/// <returns>True or false on whether the option.Data is the correct inherited class based on OptionType.</returns>
		private bool IsDataValid(DebugOptionData option)
		{
			if (option.SubData == null) return false;
			else
			{
				switch (option.Type)
				{
					case OptionSubType.Slider:
						return (option.SubData is DebugOptionSliderData);

					case OptionSubType.Toggle:
						return (option.SubData is DebugOptionToggleData);

					case OptionSubType.Label:
						return (option.SubData is DebugOptionLabelData);

					case OptionSubType.Button:
						return (option.SubData is DebugOptionButtonData);

					case OptionSubType.DropDown:
						return (option.SubData is DebugOptionDropDownData);

					case OptionSubType.Log:
						return (option.SubData is DebugOptionLogData);

					default:
						return false;
				}
			}
		}

		/// <summary>
		/// Checks whether the index is greater or equal to our last validations debug options count (and is therefore new). 
		/// </summary>
		/// <param name="index">The array index our option to check is</param>
		/// <returns>True or false on whether we are handling a newly added debug option.</returns>
		private bool IsNewDebugOption(int index) => index >= _debugOptionsLastCount;
#endif
		#endregion
#endif
	}
}