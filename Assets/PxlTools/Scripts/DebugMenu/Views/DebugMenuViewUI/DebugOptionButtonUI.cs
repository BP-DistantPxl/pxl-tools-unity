#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using UnityEngine;
using UnityEngine.UI;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugOptionButtonUI : DebugOptionUI
	{
		#region Variables

		[SerializeField] private Button _button;
		[SerializeField] private TextMeshProUGUI _text;

		private DebugOptionButtonData _subData;

		#endregion

		#region PublicFunctionalityMethods

		public override void SetData(DebugOptionData data)
		{
			// Store data and set up initial UI values & events
			_subData = data.SubData as DebugOptionButtonData;
			_text.text = _subData.ButtonText;
			_button.onClick.AddListener(OnButtonPressed);

			// Assign title & description & other universal values in base
			base.SetData(data);
		}

		/// <summary>
		/// On pressed, triggers linked function.
		/// </summary>
		public void OnButtonPressed() => _subData.OnButtonPressed?.Invoke();

		#endregion
	}
}
#endif