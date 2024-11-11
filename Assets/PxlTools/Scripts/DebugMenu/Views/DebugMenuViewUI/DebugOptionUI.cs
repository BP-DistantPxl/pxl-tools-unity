#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using UnityEngine;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugOptionUI : MonoBehaviour
	{
		#region Variables

		public DebugOptionData Data { get; private set; }

		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private TextMeshProUGUI _description;

		#endregion

		#region FunctionalityMethods

		/// <summary>
		/// Populates base debug option UI components with passed in data.
		/// Overridden by sub option types that inherit class.
		/// </summary>
		/// <param name="data">Option data to populate option UI.</param>
		public virtual void SetData(DebugOptionData data)
		{
			Data = data;
			_title.text = data.SubData.DisplayText;
			_description.text = data.SubData.Description;
		}

		#endregion
	}
}
#endif