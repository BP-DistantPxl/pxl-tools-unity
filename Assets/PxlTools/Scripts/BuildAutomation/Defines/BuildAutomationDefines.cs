namespace PxlTools.BuildAutomation.Defines
{
	public static class BuildAutomationDefines
	{
		/// <summary>
		/// Vars used during build automation validation checks, output errors and success strings.
		/// </summary>
		public class Validation
		{
			public const string EXPECTED_CONFIG_PATH = "PxlTools-Unity/Creative/Builds/ScriptableObjects/Resources/";
			public const string EXPECTED_CONFIG_NAME = "BuildAutomationConfig";
			public const string CONFIG_LOAD_SUCCESS = "Successfully loaded BuildAutomationConfig scriptable object. Using config values unless command line overrides via Bitrise.";
			public const string BUILD_REPORT_EXTENSION = ".txt";
			public const string EXPECTED_VERSION_INFO_PATH = "Assets/PxlTools-Unity/Creative/BuildAutomation/DataAssets/Resources/";
			public const string VERSION_INFO_SUCCESS = "BuildVersionData file found successfully.";
			public const string GIT_STATUS_UNKNOWN_ERROR = "Failed to gather git status at build tiem.";
			public static readonly string CONFIG_MISSING_ERROR = $"Failed to find BuildAutomationConfig scriptable object. Please ensure config exists and is present in a Resources folder.\nExpected path: {EXPECTED_CONFIG_PATH}/{EXPECTED_CONFIG_NAME}";
			public static readonly string CONFIG_MULTIPLES_ERROR = $"Found too many BuildAutomationConfig scriptable objects. Please ensure only one config exists and is present in a Resources folder.\nExpected path: {EXPECTED_CONFIG_PATH}/{EXPECTED_CONFIG_NAME}";
			public static readonly string VERSION_INFO_MISSING_ERROR = $"Failed to find BuildVersionData file. Please verify file exists at {EXPECTED_VERSION_INFO_PATH}";
		}

		/// <summary>
		/// Vars used when getting from or sending info to command line.
		/// </summary>
		public class CommandLine
		{
			public const string INCLUDE_DEMO_SCENES_CMD = "-includeDemoScenes";
			public const string OUTPUT_PATH_CMD = "-outputPath";
			public const string OUTPUT_NAME_CMD = "-outputName";
			public const string IS_DEVELOPMENT_BUILD_CMD = "-isDevelopmentBuild";
			public const string RUN_GIT_STATUS_CMD = "git status";
			public const string RUN_GIT_SHA_CMD = "git rev-parse HEAD";
		}

		/// <summary>
		/// Vars used (mostly by BuildVersionData) to format and write build version json file.
		/// </summary>
		public class Versioning
		{
			public const string VERSION_INFO_FILE_NAME = "BuildVersionData";
			public const string VERSION_INFO_FILE_OUTPUT_PATH = "";
			public const string DATE_FORMAT = "yyyyMMdd";
			public const string NUMBER_FORMAT = "xx123xx";
			public const string STATUS_UNKNOWN = "Unknown";
			public const string GIT_BRANCH_LABEL = "GitBranch:";
			public const string GIT_STATUS_LABEL = "GitStatus:";
			public const string GIT_SHA_LABEL = "GitSHA:";
			public const string BUILD_DATE_LABEL = "BuildDate:";
			public const string GIT_STATUS_PREFIX_TO_REMOVE = "On branch ";
		}
	}
}

