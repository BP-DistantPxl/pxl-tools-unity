#if DEVELOPMENT_BUILD || UNITY_EDITOR
using PxlTools.DebugMenu.Data;
using PxlTools.DebugMenu.Defines;
using UnityEngine;
using UnityEngine.UI;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugOptionToggleUI : DebugOptionUI
	{
		#region Variables

		[SerializeField] private Toggle _toggle;
		[SerializeField] private RectTransform _dotIcon;

		private DebugOptionToggleData _subData;

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Populates toggle debug option UI components with passed in data.
		/// </summary>
		/// <param name="data">Toggle data to populate option UI.</param>
		public override void SetData(DebugOptionData data)
		{
			// Store sub data in case of future reference and set initial toggle value visuals
			_subData = data.SubData as DebugOptionToggleData;
			_toggle.SetIsOnWithoutNotify(_subData.Value);
			Vector3 dotPosition = _dotIcon.anchoredPosition;
			dotPosition.x = _subData.Value ? DebugMenuDefines.Toggle.MAXIMUM_X_POSITION : DebugMenuDefines.Toggle.MINIMUM_X_POSITION;
			_dotIcon.anchoredPosition = dotPosition;

			// Hook up toggle change to option data change
			_toggle.onValueChanged.AddListener(OnToggleChanged);

			// Assign title & description & other universal values in base
			base.SetData(data);
		}

		/// <summary>
		/// On user changing value, update ui and trigger linked function.
		/// </summary>
		/// <param name="result">New bool value of toggle.</param>
		public void OnToggleChanged(bool result)
		{
			// Adjust the dot position
			Vector3 dotPosition = _dotIcon.anchoredPosition;
			dotPosition.x = result ? DebugMenuDefines.Toggle.MAXIMUM_X_POSITION : DebugMenuDefines.Toggle.MINIMUM_X_POSITION;
			_dotIcon.anchoredPosition = dotPosition;

			// Update subdata value
			_subData.Value = result;
			// Call options on change event
			_subData.OnValueChanged?.Invoke(result);
		}

		#endregion
	}
}
#endif