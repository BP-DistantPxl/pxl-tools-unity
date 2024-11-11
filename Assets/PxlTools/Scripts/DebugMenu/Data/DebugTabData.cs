#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace PxlTools.DebugMenu.Data
{
	/// <summary>
	/// Data structure for debug menu tabs.
	/// Debug menu tabs act as layout grouping objects and have no further interaction.
	/// </summary>
	[Serializable]
	public class DebugTabData
	{
		public string Id => Title;
		public string Title;
		public List<string> HeaderIDs;
	}
}
#endif