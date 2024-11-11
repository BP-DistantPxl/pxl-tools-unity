using System.Collections.Generic;
using PxlTools.Extensions.Events;
using UnityEngine;
using UnityEngine.EventSystems;

namespace PxlTools.Input.Runtime
{
	public class DesktopInputManager : MonoBehaviour
	{
		#region Variables

		public static DesktopInputManager Instance { get; private set; }

		public UnityEvent<Vector2> OnPointerPressed => _onPointerPressed;
		public UnityEvent<Vector2> OnPointerReleased => _onPointerReleased;
		public UnityEvent<Vector2, Vector2> OnPointerDragged => _onPointerDragged;
		public UnityEvent<Vector2, float> OnScrollWheelChanged => _onScrollWheelChanged;
		public UnityEvent<Vector2> OnCTRLPointerPressed => _onCTRLPointerPressed;
		public UnityEvent<Vector2> OnCTRLPointerReleased => _onCTRLPointerReleased;
		public UnityEvent<Vector2, Vector2> OnCTRLPointerDragged => _onCTRLPointerDragged;
		public UnityEvent<Vector2, float> OnCTRLScrollWheelChanged => _onCTRLScrollWheelChanged;
		public UnityEvent<Vector2> OnCMDPointerPressed => _onCMDPointerPressed;
		public UnityEvent<Vector2> OnCMDPointerReleased => _onCMDPointerReleased;
		public UnityEvent<Vector2, Vector2> OnCMDPointerDragged => _onCMDPointerDragged;
		public UnityEvent<Vector2, float> OnCMDScrollWheelChanged => _onCMDScrollWheelChanged;
		public UnityEvent<Vector2> OnSHFTPointerPressed => _onSHFTPointerPressed;
		public UnityEvent<Vector2> OnSHFTPointerReleased => _onSHFTPointerReleased;
		public UnityEvent<Vector2, Vector2> OnSHFTPointerDragged => _onSHFTPointerDragged;
		public UnityEvent<Vector2, float> OnSHFTScrollWheelChanged => _onSHFTScrollWheelChanged;

		// Input events
		private UnityEvent<Vector2> _onPointerPressed = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onPointerReleased = new UnityEvent<Vector2>();
		private UnityEvent<Vector2, Vector2> _onPointerDragged = new UnityEvent<Vector2, Vector2>();
		private UnityEvent<Vector2, float> _onScrollWheelChanged = new UnityEvent<Vector2, float>();
		private UnityEvent<Vector2> _onCTRLPointerPressed = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onCTRLPointerReleased = new UnityEvent<Vector2>();
		private UnityEvent<Vector2, Vector2> _onCTRLPointerDragged = new UnityEvent<Vector2, Vector2>();
		private UnityEvent<Vector2, float> _onCTRLScrollWheelChanged = new UnityEvent<Vector2, float>();
		private UnityEvent<Vector2> _onCMDPointerPressed = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onCMDPointerReleased = new UnityEvent<Vector2>();
		private UnityEvent<Vector2, Vector2> _onCMDPointerDragged = new UnityEvent<Vector2, Vector2>();
		private UnityEvent<Vector2, float> _onCMDScrollWheelChanged = new UnityEvent<Vector2, float>();
		private UnityEvent<Vector2> _onSHFTPointerPressed = new UnityEvent<Vector2>();
		private UnityEvent<Vector2> _onSHFTPointerReleased = new UnityEvent<Vector2>();
		private UnityEvent<Vector2, Vector2> _onSHFTPointerDragged = new UnityEvent<Vector2, Vector2>();
		private UnityEvent<Vector2, float> _onSHFTScrollWheelChanged = new UnityEvent<Vector2, float>();
		// General vars
		private bool _isEnabled = false;
		private bool _isMouseDown = false;
		private bool _isCMDHeld = false;
		private bool _isCTRLHeld = false;
		private bool _isSHFTHeld = false;
		private Vector2 _lastInputPosition;

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

			UpdateModifierInputs();
			EvaluateInputEvents();
			// TODO: Evaluate click events based on time input is down?
		}

		#endregion

		#region PointerHandlingMethods

		/// <summary>
		/// Checks modifier keys for up or down values.
		/// </summary>
		private void UpdateModifierInputs()
		{
			// Evaluate if keyboard modifier buttons are held down
			_isCTRLHeld = UnityEngine.Input.GetKey(KeyCode.LeftControl) || UnityEngine.Input.GetKey(KeyCode.RightControl);
			_isCMDHeld = UnityEngine.Input.GetKey(KeyCode.LeftCommand) || UnityEngine.Input.GetKey(KeyCode.RightCommand);
			_isSHFTHeld = UnityEngine.Input.GetKey(KeyCode.LeftShift) || UnityEngine.Input.GetKey(KeyCode.RightShift);
		}

		/// <summary>
		/// Run event evaluations depending on which modifier keys are or aren't held down.
		/// </summary>
		private void EvaluateInputEvents()
		{
			// Depemnding on modifier keys held, evaluate and trigger related events
			if (_isCTRLHeld)
				EvaluateCTRLEvents();
			else if (_isCMDHeld)
				EvaluateCMDEvents();
			else if (_isSHFTHeld)
				EvaluateSHFTEvents();
			else
				EvaluateUnmodifiedEvents();
		}

		/// <summary>
		/// Evaluates and triggers unmodified input events due to no modifier key being held.
		/// </summary>
		private void EvaluateUnmodifiedEvents()
		{
			// Store current position and scroll delta
			var inputPosition = (Vector2)UnityEngine.Input.mousePosition;
			var scrollDeltaY = UnityEngine.Input.mouseScrollDelta.y;

			// Check for mouse down, drag and up events
			if (UnityEngine.Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
			{
				_isMouseDown = true;
				_lastInputPosition = inputPosition;
				OnPointerPress(inputPosition);
			}			
			else if (UnityEngine.Input.GetMouseButton(0) && inputPosition != _lastInputPosition && _isMouseDown)
			{
				OnPointerDrag(inputPosition, inputPosition - _lastInputPosition);
				_lastInputPosition = inputPosition;
			}
			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				_isMouseDown = false;
				OnPointerRelease(inputPosition);
			}
				
			// Check for mouse scroll wheel event and trigger
			if (scrollDeltaY != 0 && !IsPointerOverUIObject())
				OnScrollWheelChange(inputPosition,scrollDeltaY);
		}

		/// <summary>
		/// Evaluates and triggers CTRL modified input events due to CTRL key being held.
		/// </summary>
		private void EvaluateCTRLEvents()
		{
			// Store current position and scroll delta
			var inputPosition = (Vector2)UnityEngine.Input.mousePosition;
			var scrollDeltaY = UnityEngine.Input.mouseScrollDelta.y;

			// Check for mouse down, drag and up events, trigger unmodified if no listeners exist for CTRL key.
			if (UnityEngine.Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
			{
				_isMouseDown = true;
				_lastInputPosition = UnityEngine.Input.mousePosition;
				if (_onCTRLPointerPressed.ActiveListeners != 0)
					OnCTRLPointerPress(inputPosition);
				else
					OnPointerPress(inputPosition);
			}
			else if (UnityEngine.Input.GetMouseButton(0) && inputPosition != _lastInputPosition && _isMouseDown)
			{
				if (_onCTRLPointerDragged.ActiveListeners != 0)
					OnCTRLPointerDrag(inputPosition, inputPosition - _lastInputPosition);
				else
					OnPointerDrag(inputPosition, inputPosition - _lastInputPosition);

				_lastInputPosition = inputPosition;
			}
			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				_isMouseDown = false;
				if (_onCTRLPointerReleased.ActiveListeners != 0)
					OnCTRLPointerRelease(inputPosition);
				else
					OnPointerPress(inputPosition);
			}

			// Check for mouse scroll wheel event, trigger unmodified if no listeners exist for CTRL key.
			if (scrollDeltaY != 0)
			{
				if (_onCTRLScrollWheelChanged.ActiveListeners != 0)
					OnCTRLScrollWheelChange(inputPosition,scrollDeltaY);
				else
					OnScrollWheelChange(inputPosition,scrollDeltaY);
			}
		}

		/// <summary>
		/// Evaluates and triggers CMD modified input events due to CMD key being held.
		/// </summary>
		private void EvaluateCMDEvents()
		{
			// Store current position and scroll delta
			var inputPosition = (Vector2)UnityEngine.Input.mousePosition;
			var scrollDeltaY = UnityEngine.Input.mouseScrollDelta.y;

			// Check for mouse down, drag and up events, trigger unmodified if no listeners exist for CTRL key.
			if (UnityEngine.Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
			{
				_isMouseDown = true;
				_lastInputPosition = UnityEngine.Input.mousePosition;
				if (_onCMDPointerPressed.ActiveListeners != 0)
					OnCMDPointerPress(inputPosition);
				else
					OnPointerPress(inputPosition);
			}
			else if (UnityEngine.Input.GetMouseButton(0) && inputPosition != _lastInputPosition && _isMouseDown)
			{
				if (_onCMDPointerDragged.ActiveListeners != 0)
					OnCMDPointerDrag(inputPosition, inputPosition - _lastInputPosition);
				else
					OnPointerDrag(inputPosition, inputPosition - _lastInputPosition);

				_lastInputPosition = inputPosition;
			}
			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				_isMouseDown = false;
				if (_onCMDPointerReleased.ActiveListeners != 0)
					OnCMDPointerRelease(inputPosition);
				else
					OnPointerPress(inputPosition);
			}

			// Check for mouse scroll wheel event, trigger unmodified if no listeners exist for CTRL key.
			if (scrollDeltaY != 0)
			{
				if (_onCMDScrollWheelChanged.ActiveListeners != 0)
					OnCMDScrollWheelChange(inputPosition, scrollDeltaY);
				else
					OnScrollWheelChange(inputPosition, scrollDeltaY);
			}
		}

		/// <summary>
		/// Evaluates and triggers SHFT modified input events due to SHFT key being held.
		/// </summary>
		private void EvaluateSHFTEvents()
		{
			// Store current position and scroll delta
			var inputPosition = (Vector2)UnityEngine.Input.mousePosition;
			var scrollDeltaY = UnityEngine.Input.mouseScrollDelta.y;

			// Check for mouse down, drag and up events, trigger unmodified if no listeners exist for CTRL key.
			if (UnityEngine.Input.GetMouseButtonDown(0) && !IsPointerOverUIObject())
			{
				_isMouseDown = true;
				_lastInputPosition = UnityEngine.Input.mousePosition;
				if (_onSHFTPointerPressed.ActiveListeners != 0)
					OnSHFTPointerPress(inputPosition);
				else
					OnPointerPress(inputPosition);
			}
			else if (UnityEngine.Input.GetMouseButton(0) && inputPosition != _lastInputPosition && _isMouseDown)
			{
				if (_onSHFTPointerDragged.ActiveListeners != 0)
					OnSHFTPointerDrag(inputPosition, inputPosition - _lastInputPosition);
				else
					OnPointerDrag(inputPosition, inputPosition - _lastInputPosition);

				_lastInputPosition = inputPosition;
			}
			else if (UnityEngine.Input.GetMouseButtonUp(0))
			{
				_isMouseDown = false;
				if (_onSHFTPointerReleased.ActiveListeners != 0)
					OnSHFTPointerRelease(inputPosition);
				else
					OnPointerPress(inputPosition);
			}

			// Check for mouse scroll wheel event, trigger unmodified if no listeners exist for CTRL key.
			if (scrollDeltaY != 0)
			{
				if (_onSHFTScrollWheelChanged.ActiveListeners != 0)
					OnSHFTScrollWheelChange(inputPosition, scrollDeltaY);
				else
					OnScrollWheelChange(inputPosition, scrollDeltaY);
			}
		}

		#endregion

		#region HelperMethods

		/// <summary>
		/// Prevents input behaviour triggering if UI is pressed.
		/// </summary>
		/// <returns>Whether the input position is above UI or not.</returns>
		private bool IsPointerOverUIObject()
		{
			PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
			eventDataCurrentPosition.position = UnityEngine.Input.mousePosition;
			List<RaycastResult> results = new List<RaycastResult>();
			EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
			return results.Count > 0;
		}

		#endregion

		#region EventTriggerMethods

		private void OnPointerPress(Vector2 inputPosition) => _onPointerPressed?.Invoke(inputPosition);
		private void OnPointerRelease(Vector2 inputPosition) => _onPointerReleased?.Invoke(inputPosition);
		private void OnPointerDrag(Vector2 inputPosition, Vector2 deltaPosition) => _onPointerDragged?.Invoke(inputPosition, deltaPosition);
		private void OnScrollWheelChange(Vector2 inputPosition, float deltaValue) => _onScrollWheelChanged?.Invoke(inputPosition, deltaValue);
		private void OnCTRLPointerPress(Vector2 inputPosition) => _onCTRLPointerPressed?.Invoke(inputPosition);
		private void OnCTRLPointerRelease(Vector2 inputPosition) => _onCTRLPointerReleased?.Invoke(inputPosition);
		private void OnCTRLPointerDrag(Vector2 inputPosition, Vector2 deltaPosition) => _onCTRLPointerDragged?.Invoke(inputPosition, deltaPosition);
		private void OnCTRLScrollWheelChange(Vector2 inputPosition, float deltaValue) => _onCTRLScrollWheelChanged?.Invoke(inputPosition, deltaValue);
		private void OnCMDPointerPress(Vector2 inputPosition) => _onCMDPointerPressed?.Invoke(inputPosition);
		private void OnCMDPointerRelease(Vector2 inputPosition) => _onCMDPointerReleased?.Invoke(inputPosition);
		private void OnCMDPointerDrag(Vector2 inputPosition, Vector2 deltaPosition) => _onCMDPointerDragged?.Invoke(inputPosition, deltaPosition);
		private void OnCMDScrollWheelChange(Vector2 inputPosition, float deltaValue) => _onCMDScrollWheelChanged?.Invoke(inputPosition, deltaValue);
		private void OnSHFTPointerPress(Vector2 inputPosition) => _onSHFTPointerPressed?.Invoke(inputPosition);
		private void OnSHFTPointerRelease(Vector2 inputPosition) => _onSHFTPointerReleased?.Invoke(inputPosition);
		private void OnSHFTPointerDrag(Vector2 inputPosition, Vector2 deltaPosition) => _onSHFTPointerDragged?.Invoke(inputPosition, deltaPosition);
		private void OnSHFTScrollWheelChange(Vector2 inputPosition, float deltaValue) => _onSHFTScrollWheelChanged?.Invoke(inputPosition, deltaValue);
		
		#endregion

		public void OnDestroy()
		{
			// Remove all listeners
			_onPointerPressed?.RemoveAllListeners();
			_onPointerReleased?.RemoveAllListeners();
			_onPointerDragged?.RemoveAllListeners();
			_onCTRLPointerPressed?.RemoveAllListeners();
			_onCTRLPointerReleased?.RemoveAllListeners();
			_onCTRLPointerDragged?.RemoveAllListeners();
			_onCMDPointerPressed?.RemoveAllListeners();
			_onCMDPointerReleased?.RemoveAllListeners();
			_onCMDPointerDragged?.RemoveAllListeners();
			_onSHFTPointerPressed?.RemoveAllListeners();
			_onSHFTPointerReleased?.RemoveAllListeners();
			_onSHFTPointerDragged?.RemoveAllListeners();
			_onScrollWheelChanged?.RemoveAllListeners();
		}
	}
}

