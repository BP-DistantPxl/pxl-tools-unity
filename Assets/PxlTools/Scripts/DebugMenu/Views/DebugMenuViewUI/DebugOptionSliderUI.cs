#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using UnityEngine;
using UnityEngine.UI;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugOptionSliderUI : DebugOptionUI
	{
		#region Variables

		[SerializeField] private Slider _slider;
		[SerializeField] private TextMeshProUGUI _sliderValueLabel;

		private DebugOptionSliderData _subData;

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Populates slider debug option UI components with passed in data.
		/// </summary>
		/// <param name="data">Slider data to populate option UI.</param>
		public override void SetData(DebugOptionData data)
		{
			// Store sub data in case of future reference
			_subData = data.SubData as DebugOptionSliderData;

			// Set initial slider values & label
			_slider.minValue = _subData.MinimumValue;
			_slider.maxValue = _subData.MaximumValue;
			_slider.value = _subData.Value;
			_sliderValueLabel.text = _subData.Value.ToString("F2");

			_slider.onValueChanged.AddListener(OnSliderChanged);

			// Assign title & description & other universal values in base
			base.SetData(data);
		}

		/// <summary>
		/// On user changing value, update ui and trigger linked function.
		/// </summary>
		/// <param name="result">New float value of slider.</param>
		public void OnSliderChanged(float result)
		{
			// Update subdata value & display on label
			_subData.Value = result;
			_sliderValueLabel.text = _subData.Value.ToString("F2");

			// Call options on change event
			_subData.OnValueChanged?.Invoke(result);
		}

		#endregion
	}
}
#endif