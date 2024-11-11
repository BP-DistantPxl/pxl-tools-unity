using System;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace PxlTools.Extensions.UI
{
	public class Button : MonoBehaviour, IPointerClickHandler, IPointerDownHandler, IPointerUpHandler, IPointerEnterHandler, IPointerExitHandler
	{
		#region Variables

		[Serializable]
		public struct NamedImage
		{
			public string id;
			public Image image;
		}

		[Serializable]
		public struct NamedText
		{
			public string id;
			public TextMeshProUGUI text;
		}

		public RectTransform RectTransform => rectTransform;
		public Image Background => background;
		public UnityEvent OnClick => _onClick;
		public UnityEvent OnPressed => _onPressed;
		public UnityEvent OnReleased => _onReleased;
		public UnityEvent OnEnter => _onEnter;
		public UnityEvent OnExit => _onExit;
		public UnityEvent OnDestroyed => _onDestroyed;
		public UnityEvent OnEnabled => _onEnabled;
		public UnityEvent OnDisabled => _onDisabled;
		public bool IsEnabled => button.enabled;

		[SerializeField] private UnityEngine.UI.Button button;
		[SerializeField] private RectTransform rectTransform;
		[SerializeField] private Image background;
		[SerializeField] private NamedImage[] imageElements;
		[SerializeField] private NamedText[] textElements;

		private UnityEvent _onClick = new UnityEvent();
		private UnityEvent _onPressed = new UnityEvent();
		private UnityEvent _onReleased = new UnityEvent();
		private UnityEvent _onEnter = new UnityEvent();
		private UnityEvent _onExit = new UnityEvent();
		private UnityEvent _onDestroyed = new UnityEvent();
		private UnityEvent _onEnabled = new UnityEvent();
		private UnityEvent _onDisabled = new UnityEvent();

		#endregion

		#region EventTriggerMethods

		public void OnPointerClick(PointerEventData eventData) { if (button != null && button.enabled) _onClick?.Invoke(); }
		public void OnPointerEnter(PointerEventData eventData) { if (button != null && button.enabled) _onEnter?.Invoke(); }
		public void OnPointerExit(PointerEventData eventData) { if (button != null && button.enabled) _onExit?.Invoke(); }
		public void OnPointerDown(PointerEventData eventData) { if (button != null && button.enabled) _onPressed?.Invoke(); }
		public void OnPointerUp(PointerEventData eventData) { if (button != null && button.enabled) _onReleased?.Invoke(); }

		#endregion

		#region PublicFunctionalityMethods

		/// <summary>
		/// Public function to set whether the button is enabled or not. Optional: Can trigger onEnabled/onDisabled event behaviour.
		/// </summary>
		/// <param name="enabled">New bool enabled value to set the button to.</param>
		/// <param name="ignoreEventBehaviour">Whether to invoke onEnabled/onDisabled event behaviour.</param>
		public void SetEnabled(bool enabled, bool ignoreEventBehaviour = false)
		{
			// If change in enabled
			if (enabled != button.enabled && !ignoreEventBehaviour)
			{
				if (enabled)
					_onEnabled?.Invoke();
				else
					_onDisabled?.Invoke();
			}

			button.enabled = enabled;
		}

		#endregion

		#region ComponentAccessMethods

		/// <summary>
		/// Generic helper method to get any text element on the button via id string.
		/// </summary>
		/// <param name="id">The string id of the text component required.</param>
		/// <returns>The text component that matches the id string if any do.</returns>
		public TextMeshProUGUI GetTextElement(string id)
		{
			foreach (var element in textElements)
			{
				if (id == element.id)
				{
					return element.text;
				}
			}

			Debug.LogError($"No text element of id {id} exists. Please ensure StylizedButtons textElements.id are named correctly in inspector");
			return null;
		}

		/// <summary>
		/// Generic helper method to get any text element on the button via index int.
		/// </summary>
		/// <param name="id">The int index of the text component required.</param>
		/// <returns>The text component that matches the index int if any do.</returns>
		public TextMeshProUGUI GetTextElement(int index)
		{
			if (index >= 0 && index < textElements.Length)
				return textElements[index].text;

			Debug.LogError($"No text element at index {index} exists. Please check StylizedButtons textElements count");
			return null;
		}

		/// <summary>
		/// Generic helper method to get any image element on the button via id string.
		/// </summary>
		/// <param name="id">The string id of the image component required.</param>
		/// <returns>The image component that matches the id string if any do.</returns>
		public Image GetImageElement(string id)
		{
			foreach (var element in imageElements)
			{
				if (id == element.id)
				{
					return element.image;
				}
			}

			Debug.LogError($"No image element of id {id} exists. Please ensure StylizedButtons imageElements.id are named correctly in inspector");
			return null;
		}

		/// <summary>
		/// Generic helper method to get any image element on the button via index int.
		/// </summary>
		/// <param name="id">The int index of the image component required.</param>
		/// <returns>The image component that matches the index int if any do.</returns>
		public Image GetImageElement(int index)
		{
			if (index >= 0 && index < imageElements.Length)
				return imageElements[index].image;

			Debug.LogError($"No image element at index {index} exists. Please check StylizedButtons imageElements count");
			return null;
		}
		#endregion

		public void OnDestroy()
		{
			// Additional onDestroy behaviour (usually, this is set up by ButtonAnimationManager to ensure animations are cleared when buttons are destroyed)
			_onDestroyed?.Invoke();
			// Remove all listeners
			_onClick?.RemoveAllListeners();
			_onEnter?.RemoveAllListeners();
			_onExit?.RemoveAllListeners();
			_onPressed?.RemoveAllListeners();
			_onReleased?.RemoveAllListeners();
			_onDestroyed?.RemoveAllListeners();
			_onDisabled?.RemoveAllListeners();
			_onEnabled?.RemoveAllListeners();
		}
	}
}