#if DEVELOPMENT_BUILD || UNITY_EDITOR
using UnityEngine;

namespace PxlTools.DebugMenu.Runtime
{
	public class DebugOptionCollection : MonoBehaviour
	{
		public virtual void Initialise() { }
	}
}
#endif