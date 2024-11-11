using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Debug = UnityEngine.Debug;
using PxlTools.CommandLine.Defines;

namespace PxlTools.CommandLine.Runtime
{
	public class CommandLineBridge
	{
		/// <summary>
		/// Runs a command process on current OS platforms command line and returns resulting output string.
		/// </summary>
		/// <param name="command">The string command to run.</param>
		/// <param name="workingDirectory">The working directory to run the command in.</param>
		/// <returns>Resulting output string from the finished command (if successful).</returns>
		public static string RunCommand(string command, string workingDirectory = "")
		{
			// Try to get process start info and return if null.
			var processStartInfo = TryGetProcessStartInfo(command, workingDirectory);
			if (processStartInfo == null) return null;

			// Prepare bool and start try catch finally process statement
			var result = "";
			try
			{
				// Run process and wait for resulting output
				var process = Process.Start(processStartInfo);
				process.Start();
				string outputString = process.StandardOutput.ReadToEnd();
				process.WaitForExit();

				// Remove command and store result
				result = outputString.Replace(command + Environment.NewLine, "");
			}
			catch (Exception exception)
			{
				// Flag exception as error.
				Debug.LogError(exception);
				result = null;
			}

			// Output additional log indicating success of command run process.
			if (string.IsNullOrEmpty(result))
				Debug.Log($"{command}\n{CommandLineDefines.Validation.CMD_RUN_FAILED_ERROR}");
			else
				Debug.Log($"{command}\n{CommandLineDefines.Validation.CMD_RUN_SUCCESS}");

			return result;
		}

		/// <summary>
		/// Generic method that looks for the result of passed in arg string contained within the command line.
		/// </summary>
		/// <param name="targetArg">The arg string / id name to look for.</param>
		/// <returns>The result if it exists, null if it doesnt.</returns>
		public static string TryGetCommandLineArg(string targetArg)
		{
			foreach (var arg in Environment.GetCommandLineArgs())
			{
				if (arg.Contains(targetArg))
				{
					return arg.Split('=')[1];
				}
			}
			return null;
		}

		/// <summary>
		/// Try to generate and return Process Start Info based on the current OS platform function is called from.
		/// </summary>
		/// <param name="command">The string command to run.</param>
		/// <param name="workingDirectory">The working directory to run the command in.</param>
		/// <returns>ProcessStartInfo for the current OS platofrm or null if on unsupported OS.</returns>
		public static ProcessStartInfo TryGetProcessStartInfo(string command, string workingDirectory = "")
		{
			if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
			{
				// On windows so use cmd.exe
				return new ProcessStartInfo
				{
					FileName = CommandLineDefines.Processes.WINDOWS_CMD_TOOL_NAME,
					Arguments = "/c " + command,
					UseShellExecute = false,
					CreateNoWindow = true,
					RedirectStandardOutput = true,
					WorkingDirectory = workingDirectory ?? "./"
				};
			}
			else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
			{
				// On unix so use shell bash
				return new ProcessStartInfo
				{
					FileName = CommandLineDefines.Processes.OSX_LINUX_CMD_TOOL_NAME,
					UseShellExecute = false,
					RedirectStandardOutput = true,
					Arguments = $"-c \"{command.Replace("\"", "\\\"")}\"",
					WorkingDirectory = workingDirectory ?? "./"
				};

			}
			else
			{
				Debug.LogError(CommandLineDefines.Validation.UNSUPPORTED_OS_ERROR);
				return null;
			}
		}
	}
}
