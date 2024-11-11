using UnityEngine;
using UnityEngine.Events;

namespace PxlTools.Common.BaseClasses
{
	/// <summary>
	/// Base class used for all UI view scripts.
	/// Common behaviour for initialise, open and close.
	/// </summary>
	public class PxlView : MonoBehaviour
	{
		#region Variables

		public UnityEvent OnClose => _onClose;

		protected UnityEvent _onClose = new UnityEvent();
		protected bool _isViewOpen = false;
		protected bool _isAnimating = false;

		#endregion

		#region VirtualMethods

		public virtual void Initialise(object data = null) { }
		public virtual void Open(bool forceInstant = false) { }
		public virtual void Close(bool forceInstant = false) { }

		#endregion
	}
}