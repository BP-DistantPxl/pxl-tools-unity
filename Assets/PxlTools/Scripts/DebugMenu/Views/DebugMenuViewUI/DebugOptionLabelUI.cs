#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using UnityEngine;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugOptionLabelUI : DebugOptionUI
	{
		#region Variables

		[SerializeField] private TextMeshProUGUI _label;

		private DebugOptionLabelData _subData;
		private float _timeSinceLastUpdate;

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
				OnUpdateLabel();
			}
		}

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Populates label debug option UI components with passed in data.
		/// </summary>
		/// <param name="data">Label data to populate option UI.</param>
		public override void SetData(DebugOptionData data)
		{
			// Store sub data in case of future reference and set initial label string value
			_subData = data.SubData as DebugOptionLabelData;
			_label.text = _subData.Value;

			// Initialize timer
			_timeSinceLastUpdate = 0.0f;

			// Assign title & description & other universal values in base
			base.SetData(data);
		}



		/// <summary>
		/// Calls linked label function to update label value.
		/// Value must be assigned by linked function to be displayed in UI.
		/// </summary>
		public void OnUpdateLabel()
		{
			// Call event - MUST ASSIGN VALUE VARIABLE OF _SUBDATA
			_subData.OnLabelUpdated?.Invoke(_subData);

			// Update label value & store time
			_label.text = _subData.Value;
		}

		#endregion
	}
}
#endif