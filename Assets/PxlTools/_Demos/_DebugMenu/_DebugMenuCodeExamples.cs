#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Linq;
using PxlTools.Core.Runtime;
using PxlTools.DebugMenu.Data;
using UnityEngine;

namespace PxlTools.Demos.DebugMenu
{
	public class _DebugMenuCodeExamples : MonoBehaviour
	{
		#region Variables

		public enum DropDownOptions
		{
			Example,
			Example2,
			Example3,
			Example4
		}

		private DebugOptionData _dropDownOptionData;
		private DropDownOptions _currentDropDownOption = DropDownOptions.Example;

		#endregion

		public void GenerateOptions()
		{
			// Create debug options
			CreateButton();
			CreateLabel();
			CreateSliderAndToggle();

			// Store drop down option data to show example of removal in OnDestroy();
			_dropDownOptionData = CreateDropDown();
			PxlToolsManager.Instance.DebugMenuManager?.AddOption(_dropDownOptionData, "Runtime Tab", "Dropdown Header");

		}

		#region CreateDebugOptionMethods

		/// <summary>
		/// Example code to create a single button option and add it to the debug menu.
		/// </summary>
		private void CreateButton()
		{
			DebugOptionData buttonOptionData = new DebugOptionData()
			{
				Id = "tst_button",
				Type = OptionSubType.Button,
				SubData = new DebugOptionButtonData(OnButtonTriggered)
				{
					DisplayText = "Button Option",
					Description = "Button description"
				}
			};

			PxlToolsManager.Instance.DebugMenuManager?.AddOption(buttonOptionData, "Runtime Tab", "Runtime Header");
		}

		/// <summary>
		/// Example code to create a single label option and add it to the debug menu.
		/// </summary>
		private void CreateLabel()
		{
			DebugOptionData labelOptionData = new DebugOptionData()
			{
				Id = "tst_label",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnLabelUpdate)
				{
					DisplayText = "Label",
					Description = "Label description",
					UpdateInterval = 1.0f
				}
			};

			PxlToolsManager.Instance.DebugMenuManager?.AddOption(labelOptionData);
		}

		/// <summary>
		/// Example code to create a slider option and toggle option and add both to the debug menu.
		/// </summary>
		private void CreateSliderAndToggle()
		{
			DebugOptionData toggleOptionData = new DebugOptionData()
			{
				Id = "tst_toggle",
				Type = OptionSubType.Toggle,
				SubData = new DebugOptionToggleData(OnToggled)
				{
					DisplayText = "Toggle",
					Value = true
				}
			};

			DebugOptionData sliderOptionData = new DebugOptionData()
			{
				Id = "tst_slider",
				Type = OptionSubType.Slider,
				SubData = new DebugOptionSliderData(OnSliderChanged)
				{
					DisplayText = "Slider",
					Description = "Slides",
					MinimumValue = 0.0f,
					MaximumValue = 10.0f,
					Value = 5.0f
				}
			};

			PxlToolsManager.Instance.DebugMenuManager?.AddOptions(new DebugOptionData[] { toggleOptionData, sliderOptionData }, "Runtime Tab", "Toggle and Slider");
		}

		/// <summary>
		/// Example code to create a dropdown option and return it.
		/// </summary>
		/// <returns>DebugOptionData containing the data for a drop down option.</returns>
		private DebugOptionData CreateDropDown()
		{
			DebugOptionData dropDownOptionData = new DebugOptionData()
			{
				Id = "tst_dropdown",
				Type = OptionSubType.DropDown,
				SubData = new DebugOptionDropDownData(OnDropDownChanged)
				{
					DisplayText = "Dropdown",
					Description = "Limited selector list",
					Options = Enum.GetNames(typeof(DropDownOptions)).ToList(),
					Value = (int)_currentDropDownOption
				}
			};

			return dropDownOptionData;
		}

		#endregion

		#region PublicEventTriggers

		/// <summary>
		/// Event fired when button is pressed.
		/// </summary>
		public void OnButtonTriggered()
		{
			Debug.Log("Button triggered");
		}

		/// <summary>
		/// Event fired when label "updates".
		/// </summary>
		/// <param name="labelData">Label data set to be updated as needed.</param>
		public void OnLabelUpdate(DebugOptionLabelData labelData)
		{
			// We manually specify the new label value here.
			labelData.Value = UnityEngine.Random.Range(0, 100).ToString();
			Debug.Log("Label update triggered");
		}

		/// <summary>
		/// Event fired when toggle value is changed.
		/// </summary>
		/// <param name="value">The new bool result of the toggle.</param>
		public void OnToggled(bool value)
		{
			Debug.Log("Toggle triggered " + value);
		}

		/// <summary>
		/// Event fired when slider value is changed.
		/// </summary>
		/// <param name="value">The new float value of the slider.</param>
		public void OnSliderChanged(float value)
		{
			Debug.Log("Slider changed to " + value);
		}

		/// <summary>
		/// Event fired when drop down value is changed.
		/// </summary>
		/// <param name="value">The new int value of the drop downs selected index.</param>
		public void OnDropDownChanged(int value)
		{
			_currentDropDownOption = (DropDownOptions)value;
			Debug.Log("Dropdown changed to " + _currentDropDownOption);
		}

		#endregion

		private void OnDestroy()
		{
			// Removes drop down option using the data as reference. Automatically will delete the header group as it will now be empty.
			PxlToolsManager.Instance.DebugMenuManager?.RemoveOption(_dropDownOptionData);

			// Removes any options contained in the runtime tabs runtime header (in this case the button)
			PxlToolsManager.Instance.DebugMenuManager?.RemoveHeaderByTitle("Runtime Tab", "Runtime Header");

			// Removes any options contained in the runtime tab, makes above unnecessary but is an example.
			PxlToolsManager.Instance.DebugMenuManager?.RemoveTab("Runtime Tab");

			// NOTE: Tabs and headers will automatically delete themselves as they become empty so you can just remove by option if wanted.
			
			// At this point, one option will remain, the label, because it had no tab or header assigned, it remains in the default "unsorted" header that is always present if not empty.
		}
	}
}
#endif