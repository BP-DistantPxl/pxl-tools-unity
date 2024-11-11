namespace PxlTools.CommandLine.Defines
{
	public static class CommandLineDefines
	{
		/// <summary>
		/// Vars used in running processes on a per platform basis.
		/// </summary>
		public class Processes
		{
			public const string WINDOWS_CMD_TOOL_NAME = "cmd.exe";
			public const string OSX_LINUX_CMD_TOOL_NAME = "/bin/bash";
		}

		/// <summary>
		/// Vars used throughout validation during command line processes.
		/// </summary>
		public class Validation
		{
			public const string UNSUPPORTED_OS_ERROR = "Attempted to run command on unsupported OS, please implement additional support or change OS Platform";
			public const string CMD_RUN_FAILED_ERROR = "Command failed to complete successfully. Please see flagged exception above.";
			public const string CMD_RUN_SUCCESS = "Command completed successfully. Output parsed and returned.";
		}
	}
}
