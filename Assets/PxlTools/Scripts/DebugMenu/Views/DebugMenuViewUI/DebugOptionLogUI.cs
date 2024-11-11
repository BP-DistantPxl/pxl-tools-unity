#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System.Collections.Generic;
using PxlTools.DebugMenu.Data;
using PxlTools.DebugMenu.Defines;
using PxlTools.Extensions.UI;
using UnityEngine;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugOptionLogUI : DebugOptionUI
	{
		#region Variables

		[Header("Components:")]
		[SerializeField] private Button _commentsButton;
		[SerializeField] private Button _warningsButton;
		[SerializeField] private Button _errorsButton;
		[SerializeField] private RectTransform _logEntryHolder;
		[Header("Prefabs:")]
		[SerializeField] private DebugOptionLogEntryUI _logEntryPrefab;

		private DebugOptionLogData _subData;
		private float _timeSinceLastUpdate;
		private List<DebugOptionLogEntryUI> _entryInstances = new List<DebugOptionLogEntryUI>();
		private bool _isCommentsEnabled = true;
		private bool _isWarningsEnabled = true;
		private bool _isErrorsEnabled = true;

		#endregion

		#region MonobehaviourMethods

		/// <summary>
		/// Check each update whether enough time has passed that label needs updating.
		/// </summary>
		private void Update()
		{
			if (_subData == null) return;
			// Less than 0 means remain static label
			if (_subData.UpdateInterval < 0.0f) return;

			// Increment timer and update if needed
			_timeSinceLastUpdate += Time.deltaTime;
			if (_timeSinceLastUpdate >= _subData.UpdateInterval)
			{
				_timeSinceLastUpdate = 0.0f;
				OnUpdateLog();
			}
		}

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Populates log debug option UI components with passed in data and hooks up additional button events.
		/// </summary>
		/// <param name="data">Log data to populate option UI.</param>
		public override void SetData(DebugOptionData data)
		{
			// Store sub data in case of future reference
			_subData = data.SubData as DebugOptionLogData;

			// Initialize timer and set up filter button events
			_timeSinceLastUpdate = 0.0f;
			_commentsButton.OnClick.AddListener(OnCommentButtonClicked);
			_warningsButton.OnClick.AddListener(OnWarningButtonClicked);
			_errorsButton.OnClick.AddListener(OnErrorButtonClicked);

			// Assign title & description & other universal values in base
			base.SetData(data);
		}

		/// <summary>
		/// Calls linked log function to update logs entries list value.
		/// List is then used to populate the UI.
		/// </summary>
		public void OnUpdateLog()
		{
			// Call event - MUST ASSIGN VALUE VARIABLE OF _SUBDATA
			_subData.OnLogUpdated?.Invoke(_subData);

			if (_subData.Entries == null) return;

			// Entries must have been cleared so delete all and rebuild if needed
			if (_subData.Entries.Count < _entryInstances.Count)
				DeleteLogEntryUIs();

			// Update log entries from _subData.Entries
			for (var i = _entryInstances.Count; i < _subData.Entries.Count; i++)
				GenerateLogEntryUI(_subData.Entries[i]);
		}

		/// <summary>
		/// Event fired when comment button filter is clicked. Enables or disables comment entries in the UI.
		/// </summary>
		public void OnCommentButtonClicked()
		{
			_isCommentsEnabled = !_isCommentsEnabled;

			foreach (var entryInstance in _entryInstances)
			{
				if (entryInstance.EntryType == LogType.Log)
					entryInstance.gameObject.SetActive(_isCommentsEnabled);
			}

			_commentsButton.Background.color = _isCommentsEnabled ? DebugMenuDefines.Colors.ENABLED_FILTER : DebugMenuDefines.Colors.DISABLED_FILTER;
		}

		/// <summary>
		/// Event fired when warning button filter is clicked. Enables or disables warning entries in the UI.
		/// </summary>
		public void OnWarningButtonClicked()
		{
			_isWarningsEnabled = !_isWarningsEnabled;

			foreach (var entryInstance in _entryInstances)
			{
				if (entryInstance.EntryType == LogType.Warning)
					entryInstance.gameObject.SetActive(_isWarningsEnabled);
			}

			_warningsButton.Background.color = _isWarningsEnabled ? DebugMenuDefines.Colors.ENABLED_FILTER : DebugMenuDefines.Colors.DISABLED_FILTER;
		}

		/// <summary>
		/// Event fired when error button filter is clicked. Enables or disables error entries in the UI.
		/// </summary>
		public void OnErrorButtonClicked()
		{
			_isErrorsEnabled = !_isErrorsEnabled;

			foreach (var entryInstance in _entryInstances)
			{
				var type = entryInstance.EntryType;
				if (type == LogType.Error || type == LogType.Assert || type == LogType.Exception)
					entryInstance.gameObject.SetActive(_isErrorsEnabled);
			}

			_errorsButton.Background.color = _isErrorsEnabled ? DebugMenuDefines.Colors.ENABLED_FILTER : DebugMenuDefines.Colors.DISABLED_FILTER;

		}

		#endregion

		#region LogEntryHandlingMethods

		/// <summary>
		/// Generates and initialises a new instance of log entry prefab.
		/// </summary>
		/// <param name="entry">The entry data to populate the new log entry prefab instance with.</param>
		private void GenerateLogEntryUI(DebugOptionLogData.LogEntry entry)
		{
			// Create and initialise new entry, then add to tracker list.
			var newLogEntry = Instantiate(_logEntryPrefab, _logEntryHolder);
			newLogEntry.Initialise(entry);
			_entryInstances.Add(newLogEntry);

			// Hide or display new entry depending on active filters
			switch (entry.LogType)
			{
				case LogType.Log:
					newLogEntry.gameObject.SetActive(_isCommentsEnabled);
					break;

				case LogType.Warning:
					newLogEntry.gameObject.SetActive(_isWarningsEnabled);
					break;

				case LogType.Error:
				case LogType.Exception:
				case LogType.Assert:
					newLogEntry.gameObject.SetActive(_isErrorsEnabled);
					break;
			}
		}

		/// <summary>
		/// Deletes all log entry instances and clears tracking list.
		/// </summary>
		private void DeleteLogEntryUIs()
		{
			// Destroy all entry holder game objects to clear visible log
			for (var i = 0; i < _entryInstances.Count; i++)
				Destroy(_entryInstances[i].gameObject);

			// Clear entry tracking list
			_entryInstances.Clear();
		}

		#endregion
	}
}
#endif