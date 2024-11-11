using System;

namespace PxlTools.Core.Defines
{
	public class PxlToolsDefines
	{
		/// <summary>
		/// Vars used as paths to tool prefabs in resources folder
		/// </summary>
		public class Resources
		{
			public const string DEBUG_MENU_PREFAB = "DebugMenu";
			public const string MULTITOUCH_INPUT_PREFAB = "MultiTouchInputManager";
			public const string DESKTOP_INPUT_PREFAB = "DesktopInputManager";
		}

		public class Data
		{
			[Flags]
			public enum ToolTypes
			{
				DebugMenu = 1 << 0,
				MultiTouchInputManager = 1 << 1,
				DesktopInputManager = 1 << 2
			}
		}

		/// <summary>
		/// Vars used when building tool manager debug logs
		/// </summary>
		public class Strings
		{
			public const string GET_TOOLS_LOG_PREFIX = "PxlTools Manager found the following tools in Scene Hierarchy:\n";
			public const string CREATE_TOOLS_LOG_PREFIX = "PxlTools Manager created the following tools:\n";
			public const string DELETE_TOOLS_LOG_PREFIX = "PxlTools Manager deleted the following tools:\n";
		}
	}
}