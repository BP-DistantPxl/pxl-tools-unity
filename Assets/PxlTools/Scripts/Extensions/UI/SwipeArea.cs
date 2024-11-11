using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using Image = UnityEngine.UI.Image;

namespace PxlTools.Extensions.UI
{
	[RequireComponent(typeof(Image))]
	public class SwipeAreaEXT : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
	{
		#region Variables

		public UnityEvent OnSwipeLeft => _onSwipeLeft;
		public UnityEvent OnSwipeRight => _onSwipeRight;
		public UnityEvent OnSwipeUp => _onSwipeUp;
		public UnityEvent OnSwipeDown => _onSwipeDown;

		private enum SwipeAreaType
		{
			Horizontal,
			Vertical,
			Both
		}

		// Floats used to measure if swipe distance is greater than PERCENTAGE of swipe area
		// This makes behaviour identical on higher res devices
		[SerializeField] private RectTransform _rectTransform;
		[SerializeField] private SwipeAreaType _type;
		[SerializeField][Range(0.1f, 1.0f)] private float _areaWidthPercentRequired = 0.25f;
		[SerializeField][Range(0.1f, 1.0f)] private float _areaHeightPercentRequired = 0.25f;

		private UnityEvent _onSwipeLeft = new UnityEvent();
		private UnityEvent _onSwipeRight = new UnityEvent();
		private UnityEvent _onSwipeUp = new UnityEvent();
		private UnityEvent _onSwipeDown = new UnityEvent();
		private Vector2 _startPosition;
		private bool _isDragging = false;
		private float _horizontalDistanceTarget;
		private float _verticalDistanceTarget;

		#endregion

		#region IPointerMethods

		/// <summary>
		/// Event fired on pointer down registered on image component, calculates start point of pointer event.
		/// </summary>
		/// <param name="eventData">PointerEventData for the pointer down event.</param>
		public void OnPointerDown(PointerEventData eventData)
		{
			if (!_isDragging)
			{
				_isDragging = true;
				_horizontalDistanceTarget = _rectTransform.rect.width * _areaWidthPercentRequired;
				_verticalDistanceTarget = _rectTransform.rect.height * _areaHeightPercentRequired;
				RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out _startPosition);
			}
		}

		/// <summary>
		/// Event fired on pointer up registered on image component.
		/// </summary>
		/// <param name="eventData">PointerEventData for the pointer up event.</param>
		public void OnPointerUp(PointerEventData eventData) => _isDragging = false;

		/// <summary>
		/// Event fired on pointer dragged over image component. Calculates if swipe event has occurred or not.
		/// </summary>
		/// <param name="eventData">PointerEventData for the pointer drag event.</param>
		public void OnDrag(PointerEventData eventData)
		{
			if (_isDragging)
			{
				RectTransformUtility.ScreenPointToLocalPointInRectangle(_rectTransform, eventData.position, eventData.pressEventCamera, out var currentPosition);
				var swiped = CalculateSwipeEvents(currentPosition);
				if (swiped)
				{
					_isDragging = false;
				}
			}
		}

		#endregion

		#region SwipeEventHandlingMethods

		/// <summary>
		/// Runs swipe calculation depending on allowed swipe directions and triggers attached events.
		/// </summary>
		/// <param name="inputPosition">The position of the current input.</param>
		/// <returns>Whether an event was true (to prevent multiple event firings).</returns>
		private bool CalculateSwipeEvents(Vector2 inputPosition)
		{
			switch (_type)
			{
				case SwipeAreaType.Horizontal:
					if (VerifyHorizontalSwipe(inputPosition, out var horizontalEvent))
					{
						horizontalEvent?.Invoke();
						return true;
					}
					break;

				case SwipeAreaType.Vertical:
					if (VerifyVerticalSwipe(inputPosition, out var verticalEvent))
					{
						verticalEvent?.Invoke();
						return true;
					}
					break;

				case SwipeAreaType.Both:
					if (VerifyBothSwipe(inputPosition, out var bothEvent))
					{
						bothEvent?.Invoke();
						return true;
					}
					break;
			}

			return false;
		}


		/// <summary>
		/// Calculates if horizontal swipe event has been triggered.
		/// </summary>
		/// <param name="inputPosition">Current position of the user input.</param>
		/// <param name="result">The resulting unity event to trigger.</param>
		/// <returns>Bool result of calculation.</returns>
		private bool VerifyHorizontalSwipe(Vector2 inputPosition, out UnityEvent result)
		{
			// Get two positions to measure
			var posA = _startPosition;
			var posB = inputPosition;

			// Check movements normal to verify is going *enough* in the direction we want
			var dragDirection = (posB - posA).normalized;
			if (Mathf.Abs(dragDirection.x) > UIExtensionsDefines.SwipeArea.SWIPE_X_NORMAL_MINIMUM)
			{
				// Then check distance is greater than our threshold
				var dragDistance = Vector2.Distance(posA, posB);
				if (dragDistance >= _horizontalDistanceTarget)
				{
					result = dragDirection.x > 0 ? _onSwipeRight : _onSwipeLeft;
					return true;
				}
			}

			result = null;
			return false;
		}

		/// <summary>
		/// Calculates if vertical swipe event has been triggered.
		/// </summary>
		/// <param name="inputPosition">Current position of the user input.</param>
		/// <param name="result">The resulting unity event to trigger.</param>
		/// <returns>Bool result of calculation.</returns>
		private bool VerifyVerticalSwipe(Vector2 inputPosition, out UnityEvent result)
		{
			// Get two positions to measure
			var posA = _startPosition;
			var posB = inputPosition;

			// Check movements normal to verify is going *enough* in the direction we want
			var dragDirection = (posB - posA).normalized;
			if (Mathf.Abs(dragDirection.y) > UIExtensionsDefines.SwipeArea.SWIPE_Y_NORMAL_MINIMUM)
			{
				// Then check distance is greater than our threshold
				var dragDistance = Vector2.Distance(posA, posB);
				if (dragDistance >= _verticalDistanceTarget)
				{
					result = dragDirection.y > 0 ? _onSwipeUp : _onSwipeDown;
					return true;
				}
			}

			result = null;
			return false;
		}

		/// <summary>
		/// Calculates if either horizontal & vertical swipe event has been triggered.
		/// </summary>
		/// <param name="inputPosition">Current position of the user input.</param>
		/// <param name="result">The resulting unity event to trigger.</param>
		/// <returns>Bool result of calculation.</returns>
		private bool VerifyBothSwipe(Vector2 inputPosition, out UnityEvent result)
		{
			var dragNormal = (inputPosition - _startPosition).normalized;
			if (Mathf.Abs(dragNormal.x) >= Mathf.Abs(dragNormal.y))
			{
				// Swipe mostly horizontal so trigger Horizontal checks
				if (VerifyHorizontalSwipe(inputPosition, out result))
					return true;
			}
			else
			{
				// Swipe mostly vertical so trigger vertical checks
				if (VerifyVerticalSwipe(inputPosition, out result))
					return true;
			}

			result = null;
			return false;
		}

		#endregion
	}
}