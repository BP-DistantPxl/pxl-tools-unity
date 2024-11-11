#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using UnityEngine;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugHeaderUI : MonoBehaviour
	{
		#region Variables

		public DebugHeaderData Data { get; private set; }

		public RectTransform OptionsHolder => _optionsHolder;

		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _description;
		[SerializeField] private RectTransform _optionsHolder;

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Populates generic header UI components with passed in data.
		/// </summary>
		/// <param name="data">Header data to populate header UI.</param>
		public void SetData(DebugHeaderData data)
		{
			Data = data;
			_title.text = data.Title;
			_description.text = data.Description;
		}

		#endregion
	}
}
#endif