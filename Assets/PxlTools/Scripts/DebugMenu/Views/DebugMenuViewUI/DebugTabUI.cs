#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using PxlTools.DebugMenu.Defines;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace PxlTools.DebugMenu.Views.UI
{
	public class DebugTabUI : MonoBehaviour
	{
		#region Variables

		public DebugTabData Data { get; private set; }
		public UnityEvent<DebugTabData> OnClicked => _onClicked;

		[SerializeField] private Image _backgroundImage;
		[SerializeField] private TextMeshProUGUI _title;
		[SerializeField] private Button _button;

		private Color _backgroundColor;
		private UnityEvent<DebugTabData> _onClicked = new UnityEvent<DebugTabData>();

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Initialises & populates generic tab UI components with passed in data.
		/// </summary>
		/// <param name="data">Tab data to populate tab UI.</param>
		public void SetData(DebugTabData data)
		{
			// Store data and update UI display.
			Data = data;
			_title.text = data.Title;

			// Set up initial vars and events.
			_backgroundColor = _backgroundImage.color;
			_backgroundColor.a = DebugMenuDefines.Animation.UNSELECTED_ALPHA;
			_button.onClick.AddListener(OnButtonPressed);
		}

		/// <summary>
		/// On pressed, triggers linked function.
		/// </summary>
		public void OnButtonPressed() => OnClicked?.Invoke(Data);

		#endregion

		#region VisualBehaviourMethods

		/// <summary>
		/// Sets alpha change to highlight tab when selected
		/// </summary>
		public void Highlight()
		{
			Color highlightColor = _backgroundColor;
			highlightColor.a = DebugMenuDefines.Animation.SELECTED_ALPHA;
			_backgroundImage.color = highlightColor;
		}

		/// <summary>
		/// Sets alpha change to unhighlight tab when deselected
		/// </summary>
		public void Unhighlight() => _backgroundImage.color = _backgroundColor;

		#endregion
	}
}
#endif