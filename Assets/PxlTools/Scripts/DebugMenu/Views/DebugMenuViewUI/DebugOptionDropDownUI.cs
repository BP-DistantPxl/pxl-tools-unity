#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using UnityEngine;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugOptionDropDownUI : DebugOptionUI
	{
		#region Variables

		[SerializeField] private TMP_Dropdown _dropdown;

		private DebugOptionDropDownData _subData;

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Populates drop down debug option UI components with passed in data.
		/// </summary>
		/// <param name="data">Drop down data to populate option UI.</param>
		public override void SetData(DebugOptionData data)
		{
			// Store sub data in case of future reference and set initial toggle value visuals
			_subData = data.SubData as DebugOptionDropDownData;

			_dropdown.ClearOptions();
			_dropdown.AddOptions(_subData.Options);
			_dropdown.SetValueWithoutNotify(_subData.Value);
			_dropdown.onValueChanged.AddListener(OnDropDownChanged);
			
			// Assign title & description & other universal values in base
			base.SetData(data);
		}

		/// <summary>
		/// On user changing value, update ui and trigger linked function.
		/// </summary>
		/// <param name="result">New int value of drop down selected option.</param>
		public void OnDropDownChanged(int result)
		{
			// Update subdata value
			_subData.Value = result;
			// Call options on change event
			_subData.OnValueChanged?.Invoke(result);
		}

		#endregion
	}
}
#endif