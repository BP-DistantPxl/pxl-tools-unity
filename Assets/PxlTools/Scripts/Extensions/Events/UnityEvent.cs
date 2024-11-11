using UnityEngine.Events;

namespace PxlTools.Extensions.Events
{
	/// <summary>
	/// Extended version of built in UnityEvents, gives access to whether there are currently active listeners, and what they are.
	/// USECASE: Input manager optimisation - do not check any input options that have no listeners whilst still supporting touchscreen and mouse on same device
	/// </summary>
	public class UnityEvent : UnityEngine.Events.UnityEvent
	{
		public int ActiveListeners => _activeListeners;

		private int _activeListeners = 0;

		public new void AddListener(UnityAction listener)
		{
			_activeListeners++;
			base.AddListener(listener);
		}

		public new void RemoveListener(UnityAction listener)
		{
			_activeListeners--;
			base.RemoveListener(listener);
		}

		public new void RemoveAllListeners()
		{
			_activeListeners = 0;
			base.RemoveAllListeners();
		}
	}

	public class UnityEvent<T0> : UnityEngine.Events.UnityEvent<T0>
	{
		public int ActiveListeners => _activeListeners;

		private int _activeListeners = 0;

		public new void AddListener(UnityAction<T0> listener)
		{
			_activeListeners++;
			base.AddListener(listener);
		}

		public new void RemoveListener(UnityAction<T0> listener)
		{
			_activeListeners--;
			base.RemoveListener(listener);
		}

		public new void RemoveAllListeners()
		{
			_activeListeners = 0;
			base.RemoveAllListeners();
		}
	}

	public class UnityEvent<T0, T1> : UnityEngine.Events.UnityEvent<T0, T1>
	{
		public int ActiveListeners => _activeListeners;

		private int _activeListeners = 0;

		public new void AddListener(UnityAction<T0, T1> listener)
		{
			_activeListeners++;
			base.AddListener(listener);
		}

		public new void RemoveListener(UnityAction<T0, T1> listener)
		{
			_activeListeners--;
			base.RemoveListener(listener);
		}

		public new void RemoveAllListeners()
		{
			_activeListeners = 0;
			base.RemoveAllListeners();
		}
	}

	public class UnityEvent<T0, T1, T2> : UnityEngine.Events.UnityEvent<T0, T1, T2>
	{
		public int ActiveListeners => _activeListeners;

		private int _activeListeners = 0;

		public new void AddListener(UnityAction<T0, T1, T2> listener)
		{
			_activeListeners++;
			base.AddListener(listener);
		}

		public new void RemoveListener(UnityAction<T0, T1, T2> listener)
		{
			_activeListeners--;
			base.RemoveListener(listener);
		}

		public new void RemoveAllListeners()
		{
			_activeListeners = 0;
			base.RemoveAllListeners();
		}
	}

	public class UnityEvent<T0, T1, T2, T3> : UnityEngine.Events.UnityEvent<T0, T1, T2, T3>
	{
		public int ActiveListeners => _activeListeners;

		private int _activeListeners = 0;

		public new void AddListener(UnityAction<T0, T1, T2, T3> listener)
		{
			_activeListeners++;
			base.AddListener(listener);
		}

		public new void RemoveListener(UnityAction<T0, T1, T2, T3> listener)
		{
			_activeListeners--;
			base.RemoveListener(listener);
		}

		public new void RemoveAllListeners()
		{
			_activeListeners = 0;
			base.RemoveAllListeners();
		}
	}
}