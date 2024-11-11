#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.Input.Runtime;
using UnityEngine;

namespace PxlTools.Demos.Input
{
	public class _InputDemoManager : MonoBehaviour
	{
		[SerializeField] private TextMeshProUGUI _inputsTextLog;
		[SerializeField] private TextMeshProUGUI _eventsTextLog;

		private void Start()
		{
			// Initialise touch managers.
			MultiTouchInputManager.Instance.Initialise(true);
			DesktopInputManager.Instance.Initialise(true);

			// Clear logs
			_inputsTextLog.text = "";
			_eventsTextLog.text = "";

			// Hook up all multi touch events.
			MultiTouchInputManager.Instance.OnTouchPressed.AddListener(OnTouchPress);
			MultiTouchInputManager.Instance.OnTouchReleased.AddListener(OnTouchRelease);
			MultiTouchInputManager.Instance.OnTouchDragged.AddListener(OnTouchDrag);
			MultiTouchInputManager.Instance.OnMultiTouchPressed.AddListener(OnMultiTouchPress);
			MultiTouchInputManager.Instance.OnMultiTouchReleased.AddListener(OnMultiTouchRelease);
			MultiTouchInputManager.Instance.OnMultiTouchDragged.AddListener(OnMultiTouchDrag);
			MultiTouchInputManager.Instance.OnPinched.AddListener(OnMultiTouchPinch);
			MultiTouchInputManager.Instance.OnRotated.AddListener(OnMultiTouchRotate);

			// Hook up all desktop input events.
			DesktopInputManager.Instance.OnPointerPressed.AddListener(OnPointerPress);
			DesktopInputManager.Instance.OnPointerReleased.AddListener(OnPointerRelease);
			DesktopInputManager.Instance.OnPointerDragged.AddListener(OnPointerDrag);
			DesktopInputManager.Instance.OnSHFTPointerPressed.AddListener(OnSHFTPointerPress);
			DesktopInputManager.Instance.OnSHFTPointerReleased.AddListener(OnSHFTPointerRelease);
			DesktopInputManager.Instance.OnSHFTPointerDragged.AddListener(OnSHFTPointerDrag);
			DesktopInputManager.Instance.OnCTRLPointerPressed.AddListener(OnCTRLPointerPress);
			DesktopInputManager.Instance.OnCTRLPointerReleased.AddListener(OnCTRLPointerRelease);
			DesktopInputManager.Instance.OnCTRLPointerDragged.AddListener(OnCTRLPointerDrag);
			DesktopInputManager.Instance.OnCMDPointerPressed.AddListener(OnCMDPointerPress);
			DesktopInputManager.Instance.OnCMDPointerReleased.AddListener(OnCMDPointerRelease);
			DesktopInputManager.Instance.OnCMDPointerDragged.AddListener(OnCMDPointerDrag);
			DesktopInputManager.Instance.OnScrollWheelChanged.AddListener(OnScrollWheelChange);

			UnityEngine.Input.simulateMouseWithTouches = false;
		}

		private void Update()
		{
			_inputsTextLog.text = "Touches:\n\n";
			_inputsTextLog.text += OutputActiveTouchDetails();
			_inputsTextLog.text += "\n\n\nMouse / Scroll:\n\n";
			_inputsTextLog.text += OutputActiveDesktopInputDetails();
			
		}

		private string OutputActiveTouchDetails()
		{
			var resultString = "";
			foreach (var touch in MultiTouchInputManager.Instance.ActiveTouches)
			{
				resultString += $"Touch {touch.Key} at {touch.Value.position}\n";
			}
			return resultString;
		}

		private string OutputActiveDesktopInputDetails()
		{
			var resultString = "";
			if (UnityEngine.Input.GetMouseButton(0))
				resultString += $"-Left mouse button  at {UnityEngine.Input.mousePosition}\n";
			if (UnityEngine.Input.GetMouseButton(1))
				resultString += $"-Right mouse button  at {UnityEngine.Input.mousePosition}\n";
			if (UnityEngine.Input.mouseScrollDelta.y != 0)
				resultString += $"-Mouse scroll wheel active at {UnityEngine.Input.mousePosition}\n";

			return resultString;
		}

		#region MultiTouchPublicEvents

		public void OnTouchPress(Vector2 position) => _eventsTextLog.text = $"-On Touch Pressed. Pos: {position} \n" + _eventsTextLog.text;
		public void OnTouchRelease(Vector2 position) => _eventsTextLog.text = $"-On Touch Released. Pos: {position} \n" + _eventsTextLog.text;
		public void OnTouchDrag(Vector2 position, Vector2 delta) => _eventsTextLog.text = $"-On Touch Dragged. Pos: {position} Delta: {delta} \n" + _eventsTextLog.text;
		public void OnMultiTouchPress(Vector2 center) => _eventsTextLog.text = $"-On Multitouch Pressed. Center: {center} \n" + _eventsTextLog.text;
		public void OnMultiTouchRelease(Vector2 center) => _eventsTextLog.text = $"-On Multitouch Released. Center: {center} \n" + _eventsTextLog.text;
		public void OnMultiTouchDrag(Vector2 center, Vector2 delta) => _eventsTextLog.text = $"-On Multitouch Dragged. Center: {center} Delta: {delta} \n" + _eventsTextLog.text;
		public void OnMultiTouchPinch(Vector2 center, float averageDistance, float deltaDistance) => _eventsTextLog.text = $"-On Multitouch Pinched. Center: {center} Average Dist: {averageDistance} Delta Dist: {deltaDistance} \n" + _eventsTextLog.text;
		public void OnMultiTouchRotate(Vector2 center, float totalRotation, float deltaRotation) => _eventsTextLog.text = $"-On Multitouch Rotation. Center: {center} Delta Rot: {deltaRotation} \n" + _eventsTextLog.text;

		#endregion

		#region DesktopInputPublicEvents

		public void OnPointerPress(Vector2 position) => _eventsTextLog.text = $"-On Mouse Pressed. Pos: {position} \n" + _eventsTextLog.text;
		public void OnPointerRelease(Vector2 position) => _eventsTextLog.text = $"-On Mouse Released. Pos: {position} \n" + _eventsTextLog.text;
		public void OnPointerDrag(Vector2 position, Vector2 delta) => _eventsTextLog.text = $"-On Mouse Dragged. Pos: {position} Delta: {delta} \n" + _eventsTextLog.text;
		public void OnCTRLPointerPress(Vector2 position) => _eventsTextLog.text = $"-On CTRL + Mouse Pressed. Pos: {position} \n" + _eventsTextLog.text;
		public void OnCTRLPointerRelease(Vector2 position) => _eventsTextLog.text = $"-On CTRL + Mouse Released. Pos: {position} \n" + _eventsTextLog.text;
		public void OnCTRLPointerDrag(Vector2 position, Vector2 delta) => _eventsTextLog.text = $"-On CTRL + Mouse Dragged. Pos: {position} Delta: {delta} \n" + _eventsTextLog.text;
		public void OnCMDPointerPress(Vector2 position) => _eventsTextLog.text = $"-On CMD + Mouse Pressed. Pos: {position} \n" + _eventsTextLog.text;
		public void OnCMDPointerRelease(Vector2 position) => _eventsTextLog.text = $"-On CMD + Mouse Released. Pos: {position} \n" + _eventsTextLog.text;
		public void OnCMDPointerDrag(Vector2 position, Vector2 delta) => _eventsTextLog.text = $"-On CMD + Mouse Dragged. Pos: {position} Delta: {delta} \n" + _eventsTextLog.text;
		public void OnSHFTPointerPress(Vector2 position) => _eventsTextLog.text = $"-On SHFT + Mouse Pressed. Pos: {position} \n" + _eventsTextLog.text;
		public void OnSHFTPointerRelease(Vector2 position) => _eventsTextLog.text = $"-On SHFT + Mouse Released. Pos: {position} \n" + _eventsTextLog.text;
		public void OnSHFTPointerDrag(Vector2 position, Vector2 delta) => _eventsTextLog.text = $"-On SHFT + Mouse Dragged. Pos: {position} Delta: {delta} \n" + _eventsTextLog.text;
		public void OnScrollWheelChange(Vector2 position, float delta) => _eventsTextLog.text = $"-On Mouse Scroll Wheel changed. Pos: {position} Scroll Delta: {delta} \n" + _eventsTextLog.text;

		#endregion
	}
}
#endif