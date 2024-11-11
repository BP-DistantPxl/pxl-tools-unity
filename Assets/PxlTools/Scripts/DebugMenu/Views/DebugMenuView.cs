#if DEVELOPMENT_BUILD || UNITY_EDITOR
// NOTE: These namespaces do not exist in non development builds to prevent hacking
using PxlTools.DebugMenu.Views.UI;
using PxlTools.DebugMenu.Defines;
using PxlTools.DebugMenu.Data;
#endif
using DG.Tweening;
using System.Collections.Generic;
using PxlTools.Common.BaseClasses;
using PxlTools.Extensions.UI;
using PxlTools.DebugMenu.Runtime;
using UnityEngine;
using System;

namespace PxlTools.DebugMenu.Views
{
	// NOTE: Class name must exist in non development builds to prevent compiler issues on DebugMenuManager...
	// ... Primarily, so that PxlToolsManager can handle deleting instance from scene if it exists in release builds.
	public class DebugMenuView : PxlView
	{
#if DEVELOPMENT_BUILD || UNITY_EDITOR
		#region Variables

		[Serializable]
		public struct UnsortedOptionsHeader
		{
			public GameObject GameObject;
			public RectTransform HolderTransform;
		}

		[Header("Components:")]
		[SerializeField] private Canvas _parentCanvas;
		[SerializeField] private RectTransform _rectTransform;
		[SerializeField] private CanvasGroup _canvasGroup;
		[SerializeField] private Button _closeButton;
		[SerializeField] private Button _moveButton;
		[SerializeField] private Button _resizeButton;
		[SerializeField] private RectTransform _tabScrollContent;
		[SerializeField] private RectTransform _optionsScrollContent;
		[SerializeField] private UnsortedOptionsHeader _unsortedOptionsHeader;
		[Header("Prefabs:")]
		[SerializeField] private GameObject _tabPrefab;
		[SerializeField] private GameObject _headerPrefab;
		[SerializeField] private GameObject _optionTogglePrefab;
		[SerializeField] private GameObject _optionSliderPrefab;
		[SerializeField] private GameObject _optionLabelPrefab;
		[SerializeField] private GameObject _optionButtonPrefab;
		[SerializeField] private GameObject _optionDropDownPrefab;
		[SerializeField] private GameObject _optionLogPrefab;

		private Dictionary<string, DebugTabUI> _tabsDictionary;
		private Dictionary<string, DebugHeaderUI> _headersDictionary;
		private Dictionary<string, DebugOptionUI> _optionsDictionary;
		private bool _isResizing = false;
		private bool _isMoving = false;
		private Vector3 _lastResizePosition;
		private Vector3 _lastMovePosition;

		#endregion

		#region PxlViewInheritedMethods

		/// <summary>
		/// Initialises view UI
		/// </summary>
		/// <param name="data">Data object defining the debug options to add to the menu.</param>
		public override void Initialise(object data = null)
		{
			// Ensure menu visuals and settings start as closed
			_isViewOpen = true;
			Close(true);

			// Initialize dictionaries
			_tabsDictionary = new Dictionary<string, DebugTabUI>();
			_headersDictionary = new Dictionary<string, DebugHeaderUI>();
			_optionsDictionary = new Dictionary<string, DebugOptionUI>();

			// Hook up close, move and resize button event logic
			_closeButton.OnClick.AddListener(OnCloseClicked);
			_moveButton.OnPressed.AddListener(() => SetMoveEnabled(true));
			_moveButton.OnReleased.AddListener(() => SetMoveEnabled(false));
			_resizeButton.OnPressed.AddListener(() => SetResizeEnabled(true));
			_resizeButton.OnReleased.AddListener(() => SetResizeEnabled(false));

			// Generate data UI elements
			if (data != null)
				SetDataUI((DebugMenuManager.DebugMenuData)data);
		}

		/// <summary>
		/// Opens debug menu view
		/// </summary>
		/// <param name="forceInstant">Forces the menu to open instantly instead of with an animation.</param>
		public override void Open(bool forceInstant = false)
		{
			if (!_isViewOpen)
				ToggleMenu(forceInstant);
		}

		/// <summary>
		/// Closes debug menu view.
		/// </summary>
		/// <param name="forceInstant">Forces the menu to close instantly instead of with an animation.</param>
		public override void Close(bool forceInstant = false)
		{
			if (_isViewOpen)
				ToggleMenu(forceInstant);
		}

		#endregion

		#region MonoBehaviourMethods

		private void Update()
		{
			if (!_isViewOpen) return;

			// Run resize or move window logic if needed
			if (_isResizing) OnResizeUpdate();
			if (_isMoving) OnMoveUpdate();
		}

		#endregion

		#region ViewDisplayMethods

		/// <summary>
		/// Handles opening or closing the menu via fade animation or instant switch.
		/// </summary>
		/// <param name="forceInstant">If true, change visual state instantly.</param>
		public void ToggleMenu(bool forceInstant = false)
		{
			if (_isAnimating)
				return;

			_isViewOpen = !_isViewOpen;
			_canvasGroup.interactable = _isViewOpen;
			_canvasGroup.blocksRaycasts = _isViewOpen;

			if (forceInstant)
				_canvasGroup.alpha = _isViewOpen ? DebugMenuDefines.Animation.OPEN_ALPHA : DebugMenuDefines.Animation.CLOSED_ALPHA;
			else
			{
				_isAnimating = true;
				_canvasGroup.DOFade(_isViewOpen ? DebugMenuDefines.Animation.OPEN_ALPHA : DebugMenuDefines.Animation.CLOSED_ALPHA, DebugMenuDefines.Animation.FADE_DURATION).OnComplete(() =>
				{
					_isAnimating = false;
				});
			}
		}

		#endregion

		#region UIGenerationMethods

		/// <summary>
		/// Resets all UI and builds from dataset from scratch.
		/// </summary>
		/// <param name="data">Data used to populate UI.</param>
		public void SetDataUI(DebugMenuManager.DebugMenuData data)
		{
			// Delete all existing UI 
			ClearExistingDataUI();
			AddMissingUI(data);

			// Turn on or off the unsorted group
			_unsortedOptionsHeader.GameObject.SetActive(_unsortedOptionsHeader.HolderTransform.childCount > 0);

			// Trigger initial tab click
			if (data.Tabs.Count > 0)
				OnClickTab(data.Tabs[data.Tabs.Count - 1]);
		}

		/// <summary>
		/// Clears and destroys all existing debug tabs, headers and options.
		/// </summary>
		private void ClearExistingDataUI()
		{
			// Delete unsorted debug options
			foreach (Transform transform in _unsortedOptionsHeader.HolderTransform)
			{
				Destroy(transform.gameObject);
			}

			// Delete headers to delete sorted debug options
			if (_headersDictionary != null)
			{
				foreach (DebugHeaderUI header in _headersDictionary.Values)
				{
					Destroy(header.gameObject);
				}
			}

			// Delete debug tab game objects
			if (_tabsDictionary != null)
			{
				foreach (DebugTabUI tab in _tabsDictionary.Values)
				{
					Destroy(tab.gameObject);
				}
			}

			// Clear dictionaries
			_tabsDictionary.Clear();
			_headersDictionary.Clear();
			_optionsDictionary.Clear();
		}

		/// <summary>
		/// Supplement to SetDataUI(), allows UI elements to be removed if no longer used and added if new to the data. Avoids deleting all and rebuilding again.
		/// </summary>
		/// <param name="data">Data used to populate UI.</param>
		public void UpdateDataUI(DebugMenuManager.DebugMenuData data)
		{
			// Deletes UI that we no longer have in data and generates any new additions.
			RemoveUnneededUI(data);
			AddMissingUI(data);

			// Turn on or off the unsorted group
			_unsortedOptionsHeader.GameObject.SetActive(_unsortedOptionsHeader.HolderTransform.childCount > 0);

			// Trigger initial tab click
			if (data.Tabs.Count > 0)
				OnClickTab(data.Tabs[data.Tabs.Count - 1]);
		}

		/// <summary>
		/// Removes all existing UI elements that are no longer present in the data object. Also, cleans any headers and tabs that are then made empty.
		/// </summary>
		/// <param name="data">Data used to populate UI.</param>
		private void RemoveUnneededUI(DebugMenuManager.DebugMenuData data)
		{
			if (_optionsDictionary != null)
			{
				// Clone dictionary as we want to delete from our original copy
				var cloneOptionsDictionary = new Dictionary<string, DebugOptionUI>(_optionsDictionary);
				// Iterate through existing options...
				foreach (var optionKVP in cloneOptionsDictionary)
				{
					//...if they do not exist in the data...
					if (!data.Options.Contains(optionKVP.Value.Data))
					{
						//...remove from our UI tracking dictionary and destroy the object.
						_optionsDictionary.Remove(optionKVP.Key);
						Destroy(optionKVP.Value.gameObject);
					}
				}
			}

			if (_headersDictionary != null)
			{
				// Clone dictionary as we want to delete from our original copy
				var cloneHeadersDictionary = new Dictionary<string, DebugHeaderUI>(_headersDictionary);
				// Iterate through existing headers...
				foreach (var headerKVP in cloneHeadersDictionary)
				{
					// ...if they do not exist in the data...
					if (!data.Headers.Contains(headerKVP.Value.Data))
					{
						// ...remove from our UI tracking dictionary and destroy the object
						_headersDictionary.Remove(headerKVP.Key);
						Destroy(headerKVP.Value.gameObject);
					}
				}
			}

			if (_tabsDictionary != null)
			{
				// Clone dictionary as we want to delete from our original copy
				var cloneTabsDictionary = new Dictionary<string, DebugTabUI>(_tabsDictionary);
				// Iterate through existing tabs...
				foreach (var tabKVP in cloneTabsDictionary)
				{
					// ...if they do not exist in the data...
					if (!data.Tabs.Contains(tabKVP.Value.Data))
					{
						// ...remove from our UI tracking dictionary and destroy the object
						_tabsDictionary.Remove(tabKVP.Key);
						Destroy(tabKVP.Value.gameObject);
					}
				}
			}
		}

		/// <summary>
		/// Adds UI objects that exist within the data but are not yet present in the DebugMenuView.
		/// </summary>
		/// <param name="data">Data used to populate UI.</param>
		private void AddMissingUI(DebugMenuManager.DebugMenuData data)
		{
			// Iterate through debug option datas and generate them as UI
			if (data.Options != null)
			{
				foreach (DebugOptionData option in data.Options)
				{
					if (!_optionsDictionary.ContainsKey(option.Id))
						CreateOption(option);
				}
			}

			// Iterate through header datas and generate them as UI.
			if (data.Headers != null)
			{
				foreach (DebugHeaderData headerData in data.Headers)
				{
					DebugHeaderUI headerComponent = null;
					// Find and use existing header if possible, otherwise create new one
					if (_headersDictionary.TryGetValue(headerData.Id, out var existingHeader))
						headerComponent = existingHeader;
					else
						headerComponent = CreateHeader(headerData);

					// Move option UI object into header transform.
					foreach (var optionID in headerData.OptionIds)
						AssignOptionToHeader(optionID, headerComponent);
				}
			}

			// Iterate through tab datas and generate them as UI.
			if (data.Tabs != null)
			{
				foreach (DebugTabData tabData in data.Tabs)
				{
					if (!_tabsDictionary.ContainsKey(tabData.Id))
						CreateTab(tabData);
				}
			}
		}

		/// <summary>
		/// Returns a prefab that matches the desired option sub type.
		/// </summary>
		/// <param name="type">The option sub type to find a matching prefab for.</param>
		/// <returns>The prefab of the passed in type.</returns>
		private GameObject GetTemplateForType(OptionSubType type)
		{
			GameObject result = null;
			switch (type)
			{
				case OptionSubType.Slider:
					result = _optionSliderPrefab;
					break;
				case OptionSubType.Toggle:
					result = _optionTogglePrefab;
					break;
				case OptionSubType.Label:
					result = _optionLabelPrefab;
					break;
				case OptionSubType.Button:
					result = _optionButtonPrefab;
					break;
				case OptionSubType.DropDown:
					result = _optionDropDownPrefab;
					break;
				case OptionSubType.Log:
					result = _optionLogPrefab;
					break;
			}

			return result;
		}

		/// <summary>
		/// Creates and populates debug option within content area of debug menu.
		/// </summary>
		/// <param name="option">The debug option data used to initialise the option.</param>
		private void CreateOption(DebugOptionData option)
		{
			// Check for existing option (must have unique IDs)
			if (_optionsDictionary.ContainsKey(option.Id))
			{
				Debug.LogError(string.Format(DebugMenuDefines.Strings.MenuView.OPTION_EXISTS_ERROR, option.Id));
				return;
			}

			// Prep object and component vars
			GameObject optionInstance = null;
			DebugOptionUI optionComponent = null;

			// Generate based on type
			GameObject template = GetTemplateForType(option.Type);
			if (template != null)
			{
				optionInstance = Instantiate(template, _unsortedOptionsHeader.HolderTransform);
				optionComponent = optionInstance.GetComponent<DebugOptionUI>();
			}

			// Check for errors
			if (optionInstance == null) return;
			if (optionComponent == null) return;

			// Run setdata override
			optionInstance.transform.localPosition = Vector3.zero;
			optionComponent.SetData(option);

			// Add to dictionary
			_optionsDictionary.Add(option.Id, optionComponent);
		}

		/// <summary>
		/// Creates and populates debug tab within the tab area of debug menu.
		/// </summary>
		/// <param name="tab">The tab data used to initialise the tab.</param>
		private void CreateTab(DebugTabData tab)
		{
			// Check for existing tab
			if (_tabsDictionary.ContainsKey(tab.Id))
			{
				Debug.LogError(string.Format(DebugMenuDefines.Strings.MenuView.TAB_EXISTS_ERROR, tab.Id));
				return;
			}

			// Instantiate and setup tab UI objects
			GameObject tabInstance = Instantiate(_tabPrefab, _tabScrollContent.transform);
			tabInstance.transform.localPosition = Vector3.zero;
			tabInstance.transform.SetAsFirstSibling();
			DebugTabUI tabComponent = tabInstance.GetComponent<DebugTabUI>();
			tabComponent.SetData(tab);
			tabComponent.OnClicked.AddListener(OnClickTab);

			// Add to tracking dictionary
			_tabsDictionary.Add(tab.Id, tabComponent);
			
		}

		/// <summary>
		/// Creates and populates debug header within the content area of debug menu.
		/// </summary>
		/// <param name="headerID">The header ID used to identify between headers.</param>
		/// <param name="header">The header data used to initialise the header.</param>
		/// <returns></returns>
		private DebugHeaderUI CreateHeader(DebugHeaderData header)
		{
			// Check for existing header (within same tab)
			if (_headersDictionary.ContainsKey(header.Id))
			{
				Debug.LogError(string.Format(DebugMenuDefines.Strings.MenuView.HEADER_EXISTS_ERROR, header.Id));
				return null;
			}

			// Instantiate and setup header UI objects
			GameObject headerInstance = Instantiate(_headerPrefab, _optionsScrollContent.transform);
			headerInstance.transform.localPosition = Vector3.zero;
			DebugHeaderUI headerComponent = headerInstance.GetComponent<DebugHeaderUI>();
			headerComponent.SetData(header);

			// Add to header tracking dictionary with tab as ID prefix
			_headersDictionary.Add(header.Id, headerComponent);

			return headerComponent;
		}

		/// <summary>
		/// Groups debug options into header object transform to improve debug menu organisation and layout.
		/// </summary>
		/// <param name="optionID">The debug option ID to assign to the header.</param>
		/// <param name="header">The header for the debug option to be assigned to.</param>
		private void AssignOptionToHeader(string optionID, DebugHeaderUI header)
		{
			// Find in spawned options and attach to header for UI layout
			if (_optionsDictionary.TryGetValue(optionID, out DebugOptionUI option))
			{
				option.transform.SetParent(header.OptionsHolder);
			}
		}

		#endregion

		#region InteractionMethods

		/// <summary>
		/// Highlights or unhighlights the clicked tab and displays the associated options/headers within the content area.
		/// </summary>
		/// <param name="tabData"></param>
		private void OnClickTab(DebugTabData tabData)
		{
			foreach (DebugTabUI tab in _tabsDictionary.Values)
			{
				if (tab.Data == tabData)
					tab.Highlight();
				else
					tab.Unhighlight();
			}
			ShowDebugOptionsTab(tabData);
		}

		/// <summary>
		/// Disables all headers and options not included within this tab, then enables all that are included.
		/// </summary>
		/// <param name="tabData"></param>
		private void ShowDebugOptionsTab(DebugTabData tabData)
		{
			// Turn off all header groups
			foreach (DebugHeaderUI headerObject in _headersDictionary.Values)
			{
				headerObject.gameObject.SetActive(false);
			}

			// Turn on each header listed within tab data so only correct options are visible
			string tabPrefix = tabData.Title + "/";
			foreach (string headerID in tabData.HeaderIDs)
			{
				if (_headersDictionary.TryGetValue(headerID, out DebugHeaderUI value))
				{
					value.gameObject.SetActive(true);
				}
			}
		}

		/// <summary>
		/// Closes view and triggers additional behaviour if any has been hooked up.
		/// </summary>
		private void OnCloseClicked()
		{
			// Trigger any additional on close behaviour
			if (_isViewOpen)
				_onClose?.Invoke();

			// Close view
			Close();
		}

		/// <summary>
		/// Sets boolean flag to enable or disable window resizing logic.
		/// </summary>
		/// <param name="enabled">Whether to enable or disable the resizing logic.</param>
		private void SetResizeEnabled(bool enabled = false)
		{
			_lastResizePosition = GetCanvasSpaceInputPosition();
			_isResizing = enabled;
		}

		/// <summary>
		/// Calculates input movement and resizes UI window within min / max limits.
		/// </summary>
		private void OnResizeUpdate()
		{
			// Grab input and canvas position vars
			var newResizePosition = GetCanvasSpaceInputPosition();
			var minOffset = _rectTransform.offsetMin;
			var maxOffset = _rectTransform.offsetMax;

			// Calculate resize change
			var resizePositionDiff = _lastResizePosition - newResizePosition;

			// Calculate new window width and limit to min.
			var newWidth = _rectTransform.rect.width - resizePositionDiff.x;
			if (newWidth < DebugMenuDefines.Input.MINIMUM_WIDTH)
			{
				resizePositionDiff.x -= DebugMenuDefines.Input.MINIMUM_WIDTH - newWidth;
				newResizePosition.x += DebugMenuDefines.Input.MINIMUM_WIDTH - newWidth;
			}

			// Calculate new window height and limit to min.
			var newHeight = _rectTransform.rect.height + resizePositionDiff.y;
			if (newHeight < DebugMenuDefines.Input.MINIMUM_HEIGHT)
			{
				resizePositionDiff.y += DebugMenuDefines.Input.MINIMUM_HEIGHT - newHeight;
				newResizePosition.y -= DebugMenuDefines.Input.MINIMUM_HEIGHT - newHeight;
			}

			// Adjust window sizes
			maxOffset.x -= resizePositionDiff.x;
			minOffset.y -= resizePositionDiff.y;

			// Clamp offsets to limit from resizing outside of window
			if (maxOffset.x > 0)
			{
				newResizePosition.x -= maxOffset.x;
				maxOffset.x = 0;
			}
			if (minOffset.y < 0)
			{
				newResizePosition.y -= minOffset.y;
				minOffset.y = 0;
			}

			// Apply result to canvas
			_rectTransform.offsetMin = minOffset;
			_rectTransform.offsetMax = maxOffset;

			// Store new resize posiiton for next frame...
			_lastResizePosition = newResizePosition;
		}


		/// <summary>
		/// Sets boolean flag to enable or disable window moving logic.
		/// </summary>
		/// <param name="enabled">Whether to enable or disable the moving logic.</param>
		private void SetMoveEnabled(bool enabled = false)
		{
			_lastMovePosition = GetCanvasSpaceInputPosition();
			_isMoving = enabled;
		}

		/// <summary>
		/// Calculates input movement and translates UI window within min / max limits.
		/// </summary>
		private void OnMoveUpdate()
		{
			// Grab input and canvas position vars
			var newMovePosition = GetCanvasSpaceInputPosition();
			var maxXPosition = ((Screen.width / _parentCanvas.scaleFactor) - _rectTransform.rect.width) / 2.0f;
			var maxYPosition = ((Screen.height / _parentCanvas.scaleFactor) - _rectTransform.rect.height) / 2.0f;

			// Calculate resize change and new position result
			var movePositionDiff = newMovePosition - _lastMovePosition;
			var newRectPosition = _rectTransform.anchoredPosition + (Vector2)movePositionDiff;
			
			// Calculate adjustment to keep window fully within screen bounds
			var newClampedPosition = new Vector2(
					Mathf.Clamp(newRectPosition.x, -maxXPosition, maxXPosition),
					Mathf.Clamp(newRectPosition.y, -maxYPosition, maxYPosition)
				);
			var clampedDifference = newRectPosition - newClampedPosition;

			// Apply adjustment to keep window on screen
			newRectPosition -= clampedDifference;
			newMovePosition -= (Vector3)clampedDifference;

			_rectTransform.anchoredPosition = newRectPosition;

			// Store new resize posiiton for next frame...
			_lastMovePosition = newMovePosition;

		}

		/// <summary>
		/// Adjusts input position by canvas scale factor to get accurate position values.
		/// </summary>
		/// <returns></returns>
		private Vector3 GetCanvasSpaceInputPosition() => UnityEngine.Input.mousePosition / _parentCanvas.scaleFactor;

		#endregion
#endif
	}
}