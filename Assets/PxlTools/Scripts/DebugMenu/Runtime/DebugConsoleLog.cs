#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System.Collections.Generic;
using PxlTools.Core.Runtime;
using PxlTools.DebugMenu.Data;
using PxlTools.DebugMenu.Defines;
using UnityEngine;

namespace PxlTools.DebugMenu.Runtime
{
	public class DebugConsoleLog : DebugOptionCollection
	{
		#region Variables 

		[Header("Assets:")]
		[SerializeField] private Sprite _errorIcon;
		[SerializeField] private Sprite _warningIcon;
		[SerializeField] private Sprite _logIcon;

		private bool _isConsoleLogEnabled = false;
		private List<DebugOptionLogData.LogEntry> _logEntries = new List<DebugOptionLogData.LogEntry>();

		#endregion

		#region InheritedMethods

		public override void Initialise()
		{
			AddConsoleLogControlOptions();
			AddConsoleLogOption();

			Debug.Log(string.Format(DebugMenuDefines.Strings.Common.TAB_ADDED, DebugMenuDefines.Strings.ConsoleLog.TAB_NAME));
		}

		#endregion

		#region DebugMenuPopulationMethods

		private void AddConsoleLogControlOptions() => PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetConsoleLogControlDatas(), DebugMenuDefines.Strings.ConsoleLog.TAB_NAME, DebugMenuDefines.Strings.Common.TOOLS_HEADER_NAME, true);
		private void AddConsoleLogOption() => PxlToolsManager.Instance.DebugMenuManager?.AddOption(GetConsoleLogData(), DebugMenuDefines.Strings.ConsoleLog.TAB_NAME, DebugMenuDefines.Strings.ConsoleLog.WINDOW_HEADER_NAME, true);

		#endregion

		#region PublicEventMethods

		/// <summary>
		/// Toggles whether the console log is enabled and hooks or unhooks into application log messages
		/// </summary>
		/// <param name="value">The bool value that the toggle has changed to.</param>
		public void OnEnableLogToggleChanged(bool value)
		{
			_isConsoleLogEnabled = value;

			if (_isConsoleLogEnabled)
			{
				Application.logMessageReceived += OnLogReceived;
				Debug.LogWarning(DebugMenuDefines.Strings.ConsoleLog.LOG_ENABLED);
			}
			else
			{
				Application.logMessageReceived -= OnLogReceived;
				Debug.LogWarning(DebugMenuDefines.Strings.ConsoleLog.LOG_DISABLED);
			}
		}

		/// <summary>
		/// Event fired when copy log button is clicked. Copies entire console log string to device copy buffer.
		/// </summary>
		public void OnCopyLogButtonClicked()
		{
			// Set up string
			var infoString = $"{DebugMenuDefines.Strings.ConsoleLog.LOG_ENABLED.ToUpper()}\n";

			foreach (var entry in _logEntries)
				infoString += $"{entry.LogString}\n{entry.StackTrace}\n\n";

			GUIUtility.systemCopyBuffer = infoString;
		}

		/// <summary>
		/// Clears tracking list which in turn will clear the console log when next update runs.
		/// </summary>
		public void OnClearLogButtonClicked() => _logEntries.Clear();

		/// <summary>
		/// Event fired whenever application receives any log entry.
		/// </summary>
		/// <param name="logString">Body of text string of the log.</param>
		/// <param name="stackTrace">The string of the logs stack trace.</param>
		/// <param name="type">The log type.</param>
		public void OnLogReceived(string logString, string stackTrace, LogType type)
		{
			if (!_isConsoleLogEnabled) return;

			// Create log entry data and add to tracking list.
			var logEntry = new DebugOptionLogData.LogEntry()
			{
				LogString = logString,
				StackTrace = stackTrace,
				LogType = type,
				LogIcon = GetLogIcon(type),
				LogColor = GetLogColor(type)
			};

			_logEntries.Add(logEntry);
		}

		/// <summary>
		/// Event fired when log runs update, grabs log entries and applies to dataset.
		/// </summary>
		/// <param name="logData">The dataset that is attached to the log ui elements and must be updated.</param>
		public void OnConsoleLogUpdate(DebugOptionLogData logData)
		{
			// Send list of log entries to the log UI.
			logData.Entries = new List<DebugOptionLogData.LogEntry>(_logEntries);
		}

		#endregion

		#region DebugDataGeneratingMethods

		/// <summary>
		/// Creates data sets for a collection of debug option toggles and buttons that control the console log behaviour.
		/// </summary>
		/// <returns>Array of debug options (toggles / buttons) to be displayed on the debug menu.</returns>
		private DebugOptionData[] GetConsoleLogControlDatas()
		{
			DebugOptionData logEnableToggle = new DebugOptionData()
			{
				Id = "tgl_logEnable",
				Type = OptionSubType.Toggle,
				SubData = new DebugOptionToggleData(OnEnableLogToggleChanged)
				{
					DisplayText = "Enable Log?",
					Description = "May cause performance impact",
					Value = _isConsoleLogEnabled
				}
			};

			DebugOptionData logCopyButton = new DebugOptionData()
			{
				Id = "btn_logCopy",
				Type = OptionSubType.Button,
				SubData = new DebugOptionButtonData(OnCopyLogButtonClicked)
				{
					DisplayText = "Copy Log Entries",
					Description = "",
					ButtonText = "Copy to Device"
				}
			};

			DebugOptionData logClearButton = new DebugOptionData()
			{
				Id = "btn_logClear",
				Type = OptionSubType.Button,
				SubData = new DebugOptionButtonData(OnClearLogButtonClicked)
				{
					DisplayText = "Clear Log",
					Description = "",
					ButtonText = "Clear"
				}
			};

			return new DebugOptionData[]
			{
				logEnableToggle,
				logCopyButton,
				logClearButton
			};
		}

		/// <summary>
		/// Creates a data set for a console log debug option.
		/// </summary>
		/// <returns>The created DebugOptionData for a log subtype.</returns>
		private DebugOptionData GetConsoleLogData()
		{
			DebugOptionData consoleLog = new DebugOptionData()
			{
				Id = "log_consoleLog",
				Type = OptionSubType.Log,
				SubData = new DebugOptionLogData(OnConsoleLogUpdate)
				{
					DisplayText = "Console",
					UpdateInterval = 0.0f,
					Entries = null
				}
			};

			return consoleLog;
		}

		#endregion

		#region HelperMethods

		/// <summary>
		/// Gets the sprite icon to use depending on the entry type.
		/// </summary>
		/// <param name="type">The log entry type.</param>
		/// <returns>The sprite icon associated with the entry type.</returns>
		private Sprite GetLogIcon(LogType type)
		{
			switch (type)
			{
				case LogType.Log:
					return _logIcon;

				case LogType.Warning:
					return _warningIcon;

				case LogType.Error:
				case LogType.Exception:
				case LogType.Assert:
					return _errorIcon;

				default:
					return _logIcon;
			}
		}

		/// <summary>
		/// Gets the text color to use depending on the entry type.
		/// </summary>
		/// <param name="type">The log entry type.</param>
		/// <returns>The text color associated with the entry type.</returns>
		private Color GetLogColor(LogType type)
		{
			switch (type)
			{
				case LogType.Log:
					return DebugMenuDefines.Colors.LOG;

				case LogType.Warning:
					return DebugMenuDefines.Colors.WARNING;

				case LogType.Error:
				case LogType.Exception:
				case LogType.Assert:
					return DebugMenuDefines.Colors.ERROR;

				default:
					return DebugMenuDefines.Colors.LOG;
			}
		}

		#endregion
	}
#endif
}