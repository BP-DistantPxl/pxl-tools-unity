#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace PxlTools.DebugMenu.Data
{
	#region BaseOptionDataStructure

	/// <summary>
	/// Define our list of supported debug option "sub type"s. 
	/// Used to link type with inherited class and data structure.
	/// </summary>
	[Serializable]
	public enum OptionSubType
	{
		Slider,
		Toggle,
		Label,
		Button,
		DropDown,
		Log
	}

	/// <summary>
	/// Data structure for debug options at base level.
	/// SerializeReference used to allow inspector to generate custom fields on subtype change.
	/// </summary>
	[Serializable]
	public class DebugOptionData
	{
		public string Id;
		public OptionSubType Type;
		[SerializeReference] public DebugOptionSubData SubData;
	}


	/// <summary>
	/// Data structure for option sub types at base level.
	/// All debug option sub types need these fields.
	/// </summary>
	[Serializable]
	public class DebugOptionSubData
	{
		public string DisplayText;
		public string Description;
	}

	#endregion

	#region SubDataStructures

	/// <summary>
	/// Data structure for additional "slider" sub type vars.
	/// </summary>
	[Serializable]
	public class DebugOptionSliderData : DebugOptionSubData
	{
		public float Value;
		public float MinimumValue;
		public float MaximumValue;
		public UnityEvent<float> OnValueChanged = new UnityEvent<float>();

		/// <summary>
		/// Allows function to be passed and hooked up automatically, whether at runtime or in inspector.
		/// </summary>
		/// <param name="onValueChanged">Function to trigger when slider value changes, returns float result.</param>
		public DebugOptionSliderData(UnityAction<float> onValueChanged = null)
		{
			if (onValueChanged != null)
			{
				OnValueChanged.AddListener(onValueChanged);
			}
		}
	}

	/// <summary>
	/// Data structure for additional "toggle" sub type vars.
	/// </summary>
	[Serializable]
	public class DebugOptionToggleData : DebugOptionSubData
	{
		public bool Value;
		public UnityEvent<bool> OnValueChanged = new UnityEvent<bool>();

		/// <summary>
		/// Allows function to be passed and hooked up automatically, whether at runtime or in inspector.
		/// </summary>
		/// <param name="onValueChanged">Function to trigger when toggle value changes, returns bool result.</param>
		public DebugOptionToggleData(UnityAction<bool> onValueChanged = null)
		{
			if (onValueChanged != null)
			{
				OnValueChanged.AddListener(onValueChanged);
			}
		}
	}

	/// <summary>
	/// Data structure for additional "label" sub type vars.
	/// </summary>
	[Serializable]
	public class DebugOptionLabelData : DebugOptionSubData
	{
		public string Value;
		public float UpdateInterval;
		public UnityEvent<DebugOptionLabelData> OnLabelUpdated = new UnityEvent<DebugOptionLabelData>();

		/// <summary>
		/// Allows function to be passed and hooked up automatically, whether at runtime or in inspector.
		/// </summary>
		/// <param name="onLabelUpdated">Function to trigger on label update, returns all label data.</param>
		public DebugOptionLabelData(UnityAction<DebugOptionLabelData> onLabelUpdated = null)
		{
			if (onLabelUpdated != null)
			{
				OnLabelUpdated.AddListener(onLabelUpdated);
			}
		}
	}

	/// <summary>
	/// Data structure for additional "button" sub type vars.
	/// </summary>
	[Serializable]
	public class DebugOptionButtonData : DebugOptionSubData
	{
		public string ButtonText;
		public UnityEvent OnButtonPressed = new UnityEvent();

		/// <summary>
		/// Allows function to be passed and hooked up automatically, whether at runtime or in inspector.
		/// </summary>
		/// <param name="onButtonPressed">Function to trigger on button press, has no return data.</param>
		public DebugOptionButtonData(UnityAction onButtonPressed = null)
		{
			if (onButtonPressed != null)
			{
				OnButtonPressed.AddListener(onButtonPressed);
			}
		}
	}

	/// <summary>
	/// Data structure for additional "drop down" sub type vars.
	/// </summary>
	[Serializable]
	public class DebugOptionDropDownData : DebugOptionSubData
	{
		public List<string> Options;
		public int Value;
		public UnityEvent<int> OnValueChanged = new UnityEvent<int>();

		/// <summary>
		/// Allows function to be passed and hooked up automatically, whether at runtime or in inspector.
		/// </summary>
		/// <param name="onValueChanged">Function to trigger on drop down value change, returns enum int value.</param>
		public DebugOptionDropDownData(UnityAction<int> onValueChanged = null)
		{
			if (onValueChanged != null)
			{
				OnValueChanged.AddListener(onValueChanged);
			}
		}
	}

	/// <summary>
	/// Data structure for additional "log" sub type vars.
	/// </summary>
	[Serializable]
	public class DebugOptionLogData : DebugOptionSubData
	{
		public struct LogEntry
		{
			public string LogString;
			public string StackTrace;
			public LogType LogType;
			public Sprite LogIcon;
			public Color LogColor;
		}

		public List<LogEntry> Entries;
		public float UpdateInterval;
		public UnityEvent<DebugOptionLogData> OnLogUpdated = new UnityEvent<DebugOptionLogData>();

		/// <summary>
		/// Allows function to be passed and hooked up automatically, whether at runtime or in inspector.
		/// </summary>
		/// <param name="onLogUpdated">Function to trigger on log update, returns all log data.</param>
		public DebugOptionLogData(UnityAction<DebugOptionLogData> onLogUpdated = null)
		{
			if (onLogUpdated != null)
			{
				OnLogUpdated.AddListener(onLogUpdated);
			}
		}
	}

	#endregion
}
#endif