#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System;
using System.Collections.Generic;

namespace PxlTools.DebugMenu.Data
{
	/// <summary>
	/// Data structure for debug headers.
	/// Debug headers act as layout grouping objects and have no further interaction.
	/// </summary>
	[Serializable]
	public class DebugHeaderData
	{
		public string Id;
		public string Title;
		public string Description;
		public List<string> OptionIds;
	}
}
#endif