using DG.Tweening;
using System.Collections.Generic;
using PxlTools.Extensions.UI;
using UnityEngine;

namespace PxlTools.Demos.Extensions
{
	public class _SwipeAreaDemoManager : MonoBehaviour
	{
		#region Variables

		[SerializeField] private SwipeAreaEXT _horizontalSwipeArea;
		[SerializeField] private RectTransform _horizontalPanelHolder;
		[SerializeField] private List<RectTransform> _horizontalSwipePanels;
		[SerializeField] private SwipeAreaEXT _verticalSwipeArea;
		[SerializeField] private RectTransform _verticalPanelHolder;
		[SerializeField] private List<RectTransform> _verticalSwipePanels;
		[SerializeField] private SwipeAreaEXT _multiSwipeArea;
		[SerializeField] private RectTransform _multiPanelHolder;
		[SerializeField] private List<RectTransform> _multiSwipeRow1Panels;
		[SerializeField] private List<RectTransform> _multiSwipeRow2Panels;

		private int _horizontalPanelIndex = 0;
		private int _verticalPanelIndex = 0;
		private int _multiPanelHorizontalIndex = 0;
		private int _multiPanelVerticalIndex = 0;

		#endregion

		void Start()
		{
			// Hook up horizontal swipes
			_horizontalSwipeArea.OnSwipeLeft.AddListener(OnHorizontalLeft);
			_horizontalSwipeArea.OnSwipeRight.AddListener(OnHorizontalRight);
			// Hook up vertical swipes
			_verticalSwipeArea.OnSwipeUp.AddListener(OnVerticalUp);
			_verticalSwipeArea.OnSwipeDown.AddListener(OnVerticalDown);
			// Hook up all direction swipes
			_multiSwipeArea.OnSwipeLeft.AddListener(OnMultiLeft);
			_multiSwipeArea.OnSwipeRight.AddListener(OnMultiRight);
			_multiSwipeArea.OnSwipeUp.AddListener(OnMultiUp);
			_multiSwipeArea.OnSwipeDown.AddListener(OnMultiDown);
		}

		#region HorizontalSwipeAreaMethods

		public void OnHorizontalLeft()
		{
			if (_horizontalPanelIndex >= _horizontalSwipePanels.Count - 1) return;

			_horizontalPanelIndex++;
			SetHorizontalSwipeAreaTarget();
		}

		public void OnHorizontalRight()
		{
			if (_horizontalPanelIndex <= 0) return;

			_horizontalPanelIndex--;
			SetHorizontalSwipeAreaTarget();
		}

		private void SetHorizontalSwipeAreaTarget()
		{
			var targetPos = _horizontalSwipePanels[_horizontalPanelIndex].anchoredPosition.x * -1.0f;
			_horizontalPanelHolder.DOAnchorPosX(targetPos, 0.5f);
		}

		#endregion

		#region VerticalSwipeAreaMethods

		public void OnVerticalUp()
		{
			if (_verticalPanelIndex >= _verticalSwipePanels.Count - 1) return;

			_verticalPanelIndex++;
			SetVerticalSwipeAreaTarget();
		}

		public void OnVerticalDown()
		{
			if (_verticalPanelIndex <= 0) return;

			_verticalPanelIndex--;
			SetVerticalSwipeAreaTarget();
		}

		private void SetVerticalSwipeAreaTarget()
		{
			var targetPos = (_verticalSwipePanels[_verticalPanelIndex].anchoredPosition.y) * -1.0f;
			_verticalPanelHolder.DOAnchorPosY(targetPos, 0.5f);
		}

		#endregion

		#region MultiDirectionSwipeAreaMethods

		public void OnMultiLeft()
		{
			if (_multiPanelHorizontalIndex >= _multiSwipeRow1Panels.Count - 1) return;

			_multiPanelHorizontalIndex++;
			SetMultiDirectionSwipeAreaTarget();
		}

		public void OnMultiRight()
		{
			if (_multiPanelHorizontalIndex <= 0) return;

			_multiPanelHorizontalIndex--;
			SetMultiDirectionSwipeAreaTarget();
		}

		public void OnMultiUp()
		{
			if (_multiPanelVerticalIndex >= 1) return;

			_multiPanelVerticalIndex++;
			SetMultiDirectionSwipeAreaTarget();
		}

		public void OnMultiDown()
		{
			if (_multiPanelVerticalIndex <= 0) return;

			_multiPanelVerticalIndex--;
			SetMultiDirectionSwipeAreaTarget();
		}

		private void SetMultiDirectionSwipeAreaTarget()
		{
			var targetPos = _multiPanelVerticalIndex == 0 ? _multiSwipeRow1Panels[_multiPanelHorizontalIndex].anchoredPosition : _multiSwipeRow2Panels[_multiPanelHorizontalIndex].anchoredPosition;
			targetPos *= -1.0f;
			_multiPanelHolder.DOAnchorPosX(targetPos.x, 0.5f);
			_multiPanelHolder.DOAnchorPosY(targetPos.y, 0.5f);
		}

		#endregion
	}
}
