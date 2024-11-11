#if DEVELOPMENT_BUILD || UNITY_EDITOR
using UnityEngine;

namespace PxlTools.DebugMenu.Defines
{
	public class DebugMenuDefines
	{
		/// <summary>
		/// Vars for input functionality
		/// </summary>
		public class Input
		{
			public const int REQUIRED_PRESSES = 5;
			public const float MAX_PRESS_TIME = 1.5f;
			public const float MINIMUM_WIDTH = 750.0f;
			public const float MINIMUM_HEIGHT = 450.0f;
		}

		/// <summary>
		/// Vars used for various animation functionality.
		/// </summary>
		public class Animation
		{
			public const float CLOSED_ALPHA = 0.0f;
			public const float OPEN_ALPHA = 1.0f;
			public const float FADE_DURATION = 0.25f;
			public const float UNSELECTED_ALPHA = 0.5f;
			public const float SELECTED_ALPHA = 1.0f;
		}

		/// <summary>
		/// Vars used by the debug option toggle sub type.
		/// </summary>
		public class Toggle
		{
			public const float MINIMUM_X_POSITION = -25.0f;
			public const float MAXIMUM_X_POSITION = 25.0f;
		}

		/// <summary>
		/// Vars for various UI coloring functionality
		/// </summary>
		public class Colors
		{
			public static readonly Color ERROR = Color.red;
			public static readonly Color WARNING = Color.yellow;
			public static readonly Color LOG = Color.white;
			public static readonly Color ENABLED_FILTER = Color.black;
			public static readonly Color DISABLED_FILTER = Color.white;
		}

		/// <summary>
		/// Vars used throughout debug menu architecture
		/// </summary>
		public class Strings
		{
			public struct Common
			{
				public const string TOOLS_HEADER_NAME = "Tools";
				public const string TAB_ADDED = "{0} (DebugOptionCollection) tab added to Debug Menu.";
				public const string LOG_SEPERATOR = "--------------------------------";
			}

			public struct MenuManager
			{
				public const string OPTION_EXISTS_ERROR = "Debug menu already contains option of ID: {0}. Discarding duplicate option.";
				public const string OPTION_DOESNT_EXIST_ERROR = "Debug menu does not contain option of ID: {0}. Unable to remove.";
			}

			public struct MenuView
			{
				public const string OPTION_EXISTS_ERROR = "Debug menu option of ID: {0} already exists. Duplicate discarded.";
				public const string TAB_EXISTS_ERROR = "Debug menu tab of ID: {0} already exists, merging headers and options into existing tab.";
				public const string HEADER_EXISTS_ERROR = "Debug menu header of ID: {0} already exists, merging options into existing header.";

			}

			public struct ConsoleLog
			{
				public const string TAB_NAME = "Console Log";
				public const string WINDOW_HEADER_NAME = "Log Window";
				public const string LOG_ENABLED = "DebugMenu Console Log Enabled. Performance may be impacted.";
				public const string LOG_DISABLED = "DebugMenu Console Log Disabled. Performance no longer impacted.";
			}

			public struct Device
			{
				public const string TAB_NAME = "Device";
				public const string GENERAL_HEADER_NAME = "General Info";
				public const string CPU_HEADER_NAME = "CPU Info";
				public const string GPU_HEADER_NAME = "GPU Info";
			}

			public struct Profiler
			{
				public const string TAB_NAME = "Profiler";
				public const string GRAPHICS_HEADER_NAME = "Graphics";
				public const string MEMORY_HEADER_NAME = "Memory";
				public const string PROFILER_ENABLED = "DebugMenu Profiler Enabled. Performance may be impacted.";
				public const string PROFILER_DISABLED = "DebugMenu Profiler Disabled. Performance no longer impacted.";
				public const string RESOLUTION_CHANGE_WARNING = "Game window resolution was changed. New resolution:";
				public const string MAIN_THREAD_RECORDER = "CPU Total Frame Time";
				public const string RENDER_THREAD_RECORDER = "CPU Render Thread Frame Time";
				public const string SET_PASS_RECORDER = "SetPass Calls Count";
				public const string DRAW_CALLS_RECORDER = "Draw Calls Count";
				public const string BATCHES_RECORDER = "Total Batches Count";
				public const string TRIS_RECORDER = "Triangles Count";
				public const string VERTS_RECORDER = "Vertices Count";
				public const string SHADOW_CASTER_RECORDER = "Shadow Casters Count";
				public const string TOTAL_MEMORY_RECORDER = "Total Reserved Memory";
				public const string GC_MEMORY_RECORDER = "GC Reserved Memory";
				public const string SYSTEM_USED_MEMORY_RECORDER = "System Used Memory";
			}

			public struct Scenes
			{
				public const string TAB_NAME = "Scenes";
				public const string MAIN_HEADER_NAME = "Main Scenes";
				public const string DEMO_HEADER_NAME = "Demo Scenes";
				public const string MAIN_PREFIX = "Main";
				public const string DEMO_PREFIX = "Demo";
				public const string MAIN_SCENE_LOADED = "Main Scene Loaded. Scene Name: {0} Scene Path: {1}";
				public const string DEMO_SCENE_LOADED = "Demo Scene Loaded. Scene Name: {0} Scene Path: {1}";
			}
		}
	}
}
#endif