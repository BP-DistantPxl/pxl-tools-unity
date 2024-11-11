using System.Collections.Generic;
using PxlTools.Extensions.Events;
using UnityEngine;
using PxlTools.Input.Defines;
using UnityEngine.EventSystems;

namespace PxlTools.Input.Runtime
{
	public class MultiTouchInputManager : MonoBehaviour
	{
		#region Variables

		public static MultiTouchInputManager Instance { get; private set; }

		public Dictionary<int, Touch> ActiveTouches => _touchList;
		public UnityEvent<Vector2> OnTouchPressed => _onTouchPressed;
		public UnityEvent<Vector2> OnTouchReleased => _onTouchReleased;
		public UnityEvent<Vector2> OnTouchTapped => _onTouchTapped;
		public UnityEvent<Vector2> OnTouchHeld => _onTouchHeld;
		public UnityEvent<Vector2, Vector2> OnTouchDragged => _onTouchDragged;
		public UnityEvent<Vector2> OnMultiTouchPressed => _onMultiTouchPressed;
		public UnityEvent<Vector2> OnMultiTouchReleased => _onMultiTouchReleased;
		public UnityEvent<Vector2> OnMultiTouchTapped => _onMultiTouchTapped;
		public UnityEvent<Vector2> OnMultiTouchHeld => _onMultiTouchHeld;
		public UnityEvent<Vector2, Vector2> OnMultiTouchDragged => _onMultiTouchDragged;
		public UnityEvent<Vector2, float, float> OnPinched => _onPinched;
		public UnityEvent<Vector2, float, float> OnRotated => _onRotated;

		// Single touch events
		private UnityEvent<Vector2> _onTouchPressed = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onTouchReleased = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onTouchTapped = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onTouchHeld = new UnityEvent<Vector2>();
		private UnityEvent<Vector2, Vector2> _onTouchDragged = new UnityEvent<Vector2, Vector2>();
		// Multi touch events
		private UnityEvent<Vector2> _onMultiTouchPressed = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onMultiTouchReleased = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onMultiTouchTapped = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onMultiTouchHeld = new UnityEvent<Vector2>();
		private UnityEvent<Vector2, Vector2> _onMultiTouchDragged = new UnityEvent<Vector2, Vector2>();
		private UnityEvent<Vector2, float, float> _onPinched = new UnityEvent<Vector2, float, float>();
		private UnityEvent<Vector2, float, float> _onRotated = new UnityEvent<Vector2, float, float>();
		// General vars
		private bool _isEnabled = false;
		private bool _wasTouchAdded = false;
		private bool _wasTouchRemoved = false;
		private Dictionary<int, Touch> _touchList = new Dictionary<int, Touch>();

		#endregion

		#region PublicFunctionalityMethods

		public void Initialise(bool isEnabled = false)
		{
			// Set enable value
			SetEnabled(isEnabled);
		}

		public void SetEnabled(bool isEnabled) => _isEnabled = isEnabled;

		#endregion

		#region MonoBehaviourMethods

		private void Awake()
		{
			// Set up singleton
			if (Instance != null && Instance != this)
			{
				Destroy(this.gameObject);
				return;
			}
			else
				Instance = this;
		}

		private void Update()
		{
			if (!_isEnabled) return;

            UpdateTouches();
			EvaluateTouchCoreEvents();
			EvaluateTouchGestureEvents();
			// TODO: Evaluate tap events based on time touch is down?
		}

		#endregion

		#region TouchHandlingMethods

		/// <summary>
		/// Keeps track of active touches, added and removed touches.
		/// </summary>
		private void UpdateTouches()
		{
			// Reset bool vars
			_wasTouchAdded = false;
			_wasTouchRemoved = false;

			// Iterate through touches
			foreach (var touch in UnityEngine.Input.touches)
			{
				if (touch.phase == TouchPhase.Began && !IsPointerOverUIObject(touch))
					AddTouch(touch);
				else if (touch.phase == TouchPhase.Stationary || touch.phase == TouchPhase.Moved)
					UpdateTouch(touch);
				else if (touch.phase == TouchPhase.Canceled || touch.phase == TouchPhase.Ended)
					RemoveTouch(touch);
			}
		}

		/// <summary>
		/// Adds touch to tracking list.
		/// </summary>
		/// <param name="touch">The touch to add to tracking list.</param>
		private void AddTouch(Touch touch)
		{
			// Update touch if already exist, or add new
			if (_touchList.TryGetValue(touch.fingerId, out var trackedTouch))
				_touchList[touch.fingerId] = touch;
			else
			{
				_touchList.Add(touch.fingerId, touch);
				_wasTouchAdded = true;
			}
		}

		/// <summary>
		/// Updates existing touch data.
		/// </summary>
		/// <param name="touch">The touch to update data of.</param>
		private void UpdateTouch(Touch touch)
		{
			// Update touch if already exist, or add new
			if (_touchList.TryGetValue(touch.fingerId, out var trackedTouch))
				_touchList[touch.fingerId] = touch;
		}

		/// <summary>
		/// Removes touch from tracking list.
		/// </summary>
		/// <param name="touch">The touch to remove from tracking list.</param>
		private void RemoveTouch(Touch touch)
		{
			// Remove touch from dictionary if it is present
			if (_touchList.TryGetValue(touch.fingerId, out var trackedTouch))
				_touchList.Remove(touch.fingerId);

			_wasTouchRemoved = true;
		}

		/// <summary>
		/// Evaluates and triggers events for core touch - press, release, hold.
		/// </summary>
		private void EvaluateTouchCoreEvents()
		{
			if (_touchList.Count == 1)
			{
				// Trigger on presse
				if (_wasTouchAdded)
					OnTouchPress(_touchList[0].position);
				// Trigger on release
				if (_wasTouchRemoved)
					OnTouchRelease(_touchList[0].position);
				// Trigger held
				if (!_wasTouchAdded && !_wasTouchRemoved)
					OnTouchHold(_touchList[0].position);
			}
			else if (_touchList.Count > 1)
			{
				var centerPositions = GetTouchesCenterPositions();

				// Trigger on press
				if (_wasTouchAdded)
					OnMultiTouchPress(centerPositions[0]);
				// Trigger on release
				if (_wasTouchRemoved)
					OnMultiTouchRelease(centerPositions[0]);
				// Trigger held
				if (!_wasTouchAdded && !_wasTouchRemoved)
					OnMultiTouchHold(centerPositions[0]);
			}
		}

		/// <summary>
		/// Evaluates and triggers events for gestures - drag, pinch, rotate
		/// </summary>
		private void EvaluateTouchGestureEvents()
		{
			if (_touchList.Count == 1)
				OnTouchDrag(_touchList[0].position, _touchList[0].deltaPosition);
			else if (_touchList.Count > 1)
			{
				// Grab center point, delta center and evaluate average distances and delta distances
				var centerPositions = GetTouchesCenterPositions();
				var distanceAverages = GetTouchesAverageDistances(centerPositions[0], out var isMultiTouchDrag);
					
				// Touches were close enough to be a drag event
				if (isMultiTouchDrag)
					OnMultiTouchDrag(centerPositions[0], centerPositions[1]);

				// Distance delta changed so it could be a pinch event
				if (distanceAverages[1] != 0.0f)
					OnPinch(centerPositions[0], distanceAverages[0], distanceAverages[1]);

				// Calculate rotation value of frame and rotation delta
				var rotationValues = GetTouchesRotations(centerPositions[0]);

				// Rotation delta changes so it could be a rotate event
				if (rotationValues[1] != 0.0f)
					OnRotate(centerPositions[0], rotationValues[0], rotationValues[1]);
			}
		}

		#endregion

		#region HelperMathsMethods

		/// <summary>
		/// Gets the center position from an array of positions.
		/// </summary>
		/// <returns>The center position of all passed positions.</returns>
		private Vector2[] GetTouchesCenterPositions()
		{
			// Set up initial values
			var positionTotal = Vector2.zero;
			var positionDeltaTotal = Vector2.zero;

			// Calculate totals from touches
			foreach (var touch in _touchList.Values)
			{
				positionTotal += touch.position;
				positionDeltaTotal += touch.deltaPosition;
			}

			// Divide result by total touches for average center
			return new Vector2[] { positionTotal / _touchList.Count, positionDeltaTotal / _touchList.Count };
		}

		/// <summary>
		/// Gets the average distance to the center position from an array of positions.
		/// </summary>
		/// <param name="centerPosition">The position to measure distance to.</param>
		/// <param name="isMultiTouchDrag">Bool out var establishing if multi touch dragging is true.</param>
		/// <returns>Floats representing the average distance, and delta distance change to center position.</returns>
		private float[] GetTouchesAverageDistances(Vector2 centerPosition, out bool isMultiTouchDrag)
		{
			// Reset bool
			isMultiTouchDrag = true;

			// Set up initial values
			var distanceTotal = 0.0f;
			var oldDistanceTotal = 0.0f;
			foreach (var touch in _touchList.Values)
			{
				// Get distances and increment totals
				var distance = Vector2.Distance(centerPosition, touch.position);
				distanceTotal += distance;
				oldDistanceTotal += Vector2.Distance(centerPosition, touch.position - touch.deltaPosition);

				// If any finger is too far from center, do not consider this a drag event
				if (distance > InputDefines.Config.MAX_MULTITOUCH_DRAG_SEPERATION)
					isMultiTouchDrag = false;
			}

			return new float[] { distanceTotal / _touchList.Count, (distanceTotal - oldDistanceTotal) / _touchList.Count };
		}

		/// <summary>
		/// Gets the overall rotation from an array of touch positions based on direction from center position.
		/// </summary>
		/// <param name="centerPosition">The center position to measure directions rotation from.</param>
		/// <returns>Floats representing the total rotation and rotation delta.</returns>
		private float[] GetTouchesRotations(Vector2 centerPosition)
		{
			var rotationTotal = 0.0f;
			var lastRotationTotal = 0.0f;

			foreach (var touch in _touchList.Values)
			{
				// Calculate directions from center...
				var touchDirection = touch.position - centerPosition;
				var lastTouchDirection = (touch.position - touch.deltaPosition) - centerPosition;
				// ... get angles the direction is from straight up vector (aka bearing)...
				var touchAngle = Vector2.Angle(Vector2.up, touchDirection);
				var lastTouchAngle = Vector2.Angle(Vector2.up, lastTouchDirection);
				// ... find out if angles movement is negative or positive...
				var dotProduct = Vector2.Dot(Vector2.right, touchDirection);
				var lastDotProduct = Vector2.Dot(Vector2.right, lastTouchDirection);
				// ... add results to our rotations total
				rotationTotal = dotProduct < 0 ? rotationTotal + touchAngle : rotationTotal - touchAngle;
				lastRotationTotal = lastDotProduct < 0 ? lastRotationTotal + lastTouchAngle : lastRotationTotal - lastTouchAngle;
			}

			// Calculate rotation delta and limit to 360 degrees
			var deltaRotationAngle = rotationTotal - lastRotationTotal;
			while (deltaRotationAngle < 0.0f)
				deltaRotationAngle += 360.0f;
			while (deltaRotationAngle >= 360.0f)
				deltaRotationAngle -= 360.0f;

			// Limit rotation total to 360 degrees
			while (rotationTotal < 0.0f)
				rotationTotal += 360.0f;
			while (rotationTotal >= 360.0f)
				rotationTotal -= 360.0f;

			return new float[] { rotationTotal, deltaRotationAngle };
		}

		/// <summary>
		/// Prevents input behaviour triggering if UI is pressed.
		/// </summary>
		/// <param name="touch">The touch to check is over UI.</param>
		/// <returns>Whether the touch position is above UI or not.</returns>
		private bool IsPointerOverUIObject(Touch touch)
		{
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = touch.position;
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			return results.Count > 0;
		}

		#endregion

		#region EventTriggerMethods

		private void OnTouchPress(Vector2 position) => _onTouchPressed?.Invoke(position);
		private void OnTouchRelease(Vector2 position) => _onTouchReleased?.Invoke(position);
		private void OnTouchTap(Vector2 position) => _onTouchTapped?.Invoke(position);
		private void OnTouchHold(Vector2 position) => _onTouchHeld?.Invoke(position);
		private void OnTouchDrag(Vector2 centerPosition, Vector2 deltaPosition) => _onTouchDragged?.Invoke(centerPosition, deltaPosition);
		private void OnMultiTouchPress(Vector2 centerPosition) => _onMultiTouchPressed?.Invoke(centerPosition);
		private void OnMultiTouchRelease(Vector2 centerPosition) => _onMultiTouchReleased?.Invoke(centerPosition);
		private void OnMultiTouchTap(Vector2 centerPosition) => _onMultiTouchTapped?.Invoke(centerPosition);
		private void OnMultiTouchHold(Vector2 centerPosition) => _onMultiTouchHeld?.Invoke(centerPosition);
		private void OnMultiTouchDrag(Vector2 centerPosition, Vector2 deltaPosition) => _onMultiTouchDragged?.Invoke(centerPosition, deltaPosition);
		private void OnPinch(Vector2 centerPosition, float averageDistance, float deltaDistance) => _onPinched?.Invoke(centerPosition, averageDistance, deltaDistance);
		private void OnRotate(Vector2 centerPosition, float rotationTotal, float deltaRotation) => _onRotated?.Invoke(centerPosition, rotationTotal, deltaRotation);

		#endregion

		public void OnDestroy()
		{
			// Remove all single touch listeners
			_onTouchPressed?.RemoveAllListeners();
			_onTouchReleased?.RemoveAllListeners();
			_onTouchTapped?.RemoveAllListeners();
			_onTouchHeld?.RemoveAllListeners();
			_onTouchDragged?.RemoveAllListeners();

			// Remove all multi touch listeners
			_onMultiTouchPressed?.RemoveAllListeners();
			_onMultiTouchReleased?.RemoveAllListeners();
			_onMultiTouchTapped?.RemoveAllListeners();
			_onMultiTouchHeld?.RemoveAllListeners();
			_onMultiTouchDragged?.RemoveAllListeners();
			_onPinched?.RemoveAllListeners();
			_onRotated?.RemoveAllListeners();
		}
	}
}