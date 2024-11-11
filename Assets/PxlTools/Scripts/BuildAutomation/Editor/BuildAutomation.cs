using System.Collections.Generic;
using System.IO;
using System.Text;
using PxlTools.BuildAutomation.Defines;
using PxlTools.BuildAutomation.Data;
using PxlTools.BuildAutomation.Utils;
using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEngine;
using PxlTools.CommandLine.Runtime;
using Unity.Plastic.Newtonsoft.Json;

namespace PxlTools.BuildAutomation.Editor
{
	public static class BuildAutomation
	{
		#region EditorMenuItemMethods

		/// <summary>
		/// In Editor menu item to run a standalone android build using the same build flow the bitrise automation will.
		/// </summary>
		[MenuItem("PxlTools/Build/Standalone/Android")]
		private static void Menu_BuildAndroidStandalone()
		{
			var buildPath = EditorUtility.OpenFolderPanel("Build Destination", string.Empty, string.Empty);
			// Empty path - window was cancelled so cancel build.
			if (string.IsNullOrWhiteSpace(buildPath))
				return;

			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Generate scene paths array for build options
			var scenePaths = GetScenePaths(config, config.IncludeDemoScenes);

			// Set as standalone android project and setup build options
			EditorUserBuildSettings.exportAsGoogleAndroidProject = false;
			var buildOptions = GetBuildPlayerOptions(buildPath, BuildTarget.Android, scenePaths, config.IsDevelopmentBuild, config.AdditionalScriptingDefines);

			// For standalone we specify the file extension output within path so amend
			buildOptions.locationPathName += $"/{config.BuildName}.apk";

			// Trigger build
			Build(buildOptions, config.ReportName);
		}

		/// <summary>
		/// In Editor menu item to run a standalone iOS build using the same build flow the bitrise automation will.
		/// </summary>
		[MenuItem("PxlTools/Build/Standalone/iOS")]
		private static void Menu_BuildiOSStandalone()
		{
			var buildPath = EditorUtility.OpenFolderPanel("Build Destination", string.Empty, string.Empty);
			// Empty path - window was cancelled so cancel build.
			if (string.IsNullOrWhiteSpace(buildPath))
				return;

			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Generate scene paths array, buildoptions and then start build.
			var scenePaths = GetScenePaths(config, config.IncludeDemoScenes);
			var buildOptions = GetBuildPlayerOptions(buildPath, BuildTarget.iOS, scenePaths, config.IsDevelopmentBuild,config.AdditionalScriptingDefines);
			
			// Set up final output name
			buildOptions.locationPathName += $"/{config.BuildName}";
			
			Build(buildOptions, config.ReportName);
		}

		/// <summary>
		/// In Editor menu item to run a standalone WebGL build using the same build flow the bitrise automation will.
		/// </summary>
		[MenuItem("PxlTools/Build/Standalone/WebGL")]
		private static void Menu_BuildWebGLStandalone()
		{
			var buildPath = EditorUtility.OpenFolderPanel("Build Destination", string.Empty, string.Empty);

			// Empty path - window was cancelled so cancel build.
			if (string.IsNullOrWhiteSpace(buildPath))
				return;

			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Generate scene paths array, buildoptions and then start build.
			var scenePaths = GetScenePaths(config, config.IncludeDemoScenes);
			var buildOptions = GetBuildPlayerOptions(buildPath, BuildTarget.WebGL, scenePaths, config.IsDevelopmentBuild, config.AdditionalScriptingDefines);

			// Set up final output name
			buildOptions.locationPathName += $"/{config.BuildName}";

			Build(buildOptions, config.ReportName);
		}

		/// <summary>
		/// In Editor menu item to run a Export Android Library build using the same build flow the bitrise automation will.
		/// </summary>
		[MenuItem("PxlTools/Build/Export Android Library")]
		private static void Menu_ExportAndroid()
		{
			var buildPath = EditorUtility.OpenFolderPanel("Build Destination", string.Empty, string.Empty);
			
			// Empty path - window was cancelled so cancel build.
			if (string.IsNullOrWhiteSpace(buildPath))
				return;

			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Generate scene paths array.
			var scenePaths = GetScenePaths(config, config.IncludeDemoScenes);

			// Set as exported android project and setup build options
			EditorUserBuildSettings.exportAsGoogleAndroidProject = true;
			var buildOptions = GetBuildPlayerOptions(buildPath, BuildTarget.Android, scenePaths, config.IsDevelopmentBuild, config.AdditionalScriptingDefines);

			// Trigger build
			Build(buildOptions, config.ReportName);
		}

		#endregion

		#region CLIMethods

		// NOTE: See Bitrise Workflows for documentation on how the below commands are used or use below cmds as reference to run locally.

		//CMD: sudo /Applications/Unity/Hub/Editor/$DYNAMIC_UNITY_VERSION/Unity.app/Contents/MacOS/Unity -projectPath "$BITRISE_SOURCE_DIR" -buildTarget iOS -accept-apiupdate -batchmode -executeMethod PxlTools.BuildAutomation.Editor.BuildAutomation.Build_iOS_Standalone -quit -logFile "build_ios.log" -outputPath=$BITRISE_SOURCE_DIR/Builds/iOS -outputName=$STANDALONE_NAME-iOS_$BITRISE_BUILD_NUMBER -isDevelopmentBuild=$IS_DEVELOPMENT_BUILD -includeDemoScenes=$INCLUDE_DEMO_SCENES &
		public static void Build_iOS_Standalone() => BuildiOS(true);
		//CMD: sudo /Applications/Unity/Hub/Editor/$DYNAMIC_UNITY_VERSION/Unity.app/Contents/MacOS/Unity -projectPath "$BITRISE_SOURCE_DIR" -buildTarget iOS -accept-apiupdate -batchmode -executeMethod PxlTools.BuildAutomation.Editor.BuildAutomation.Build_iOS_Export -quit -logFile "build_ios.log" -outputPath=$BITRISE_SOURCE_DIR/Builds/iOS -isDevelopmentBuild=$IS_DEVELOPMENT_BUILD -includeDemoScenes=$INCLUDE_DEMO_SCENES &
		public static void Build_iOS_Export() => BuildiOS(false);
		//CMD: sudo /Applications/Unity/Hub/Editor/$DYNAMIC_UNITY_VERSION/Unity.app/Contents/MacOS/Unity -projectPath "$BITRISE_SOURCE_DIR" -buildTarget Android -accept-apiupdate -batchmode -executeMethod PxlTools.BuildAutomation.Editor.BuildAutomation.Build_Android_Standalone -quit -logFile "build_android.log" -outputPath=$BITRISE_SOURCE_DIR/Builds/Android -outputName=$STANDALONE_NAME-Android_$BITRISE_BUILD_NUMBER -isDevelopmentBuild=$IS_DEVELOPMENT_BUILD -includeDemoScenes=$INCLUDE_DEMO_SCENES &
		public static void Build_Android_Standalone() => BuildAndroid(true);
		//CMD: sudo /Applications/Unity/Hub/Editor/$DYNAMIC_UNITY_VERSION/Unity.app/Contents/MacOS/Unity -projectPath "$BITRISE_SOURCE_DIR" -buildTarget Android -accept-apiupdate -batchmode -executeMethod PxlTools.BuildAutomation.Editor.BuildAutomation.Build_Android_Export -quit -logFile "build_android.log" -outputPath=$BITRISE_SOURCE_DIR/Builds/Android -isDevelopmentBuild=$IS_DEVELOPMENT_BUILD -includeDemoScenes=$INCLUDE_DEMO_SCENES &
		public static void Build_Android_Export() => BuildAndroid(false);
		//CMD: sudo /Applications/Unity/Hub/Editor/$DYNAMIC_UNITY_VERSION/Unity.app/Contents/MacOS/Unity -projectPath "$BITRISE_SOURCE_DIR" -buildTarget webGL -accept-apiupdate -batchmode -executeMethod PxlTools.BuildAutomation.Editor.BuildAutomation.Build_WebGL_Standalone -quit -logFile "build_webgl.log" -outputPath=$BITRISE_SOURCE_DIR/Builds/WebGL -outputName=$STANDALONE_NAME-WebGL_$BITRISE_BUILD_NUMBER -isDevelopmentBuild=$IS_DEVELOPMENT_BUILD -includeDemoScenes=$INCLUDE_DEMO_SCENES &
		public static void Build_WebGL_Standalone() => BuildWebGL(true);

		#endregion

		#region BuildMethods

		/// <summary>
		/// Generates all build options and settings from configs and or command line args and triggers iOS build.
		/// </summary>
		/// <param name="isStandalone">Whether the iOS build should be standalone or not (although iOS is always standalone, this var impacts the output location and so is required).</param>
		public static void BuildiOS(bool isStandalone = false)
		{
			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Check command line for output path and fallback to default if none passed in
			var buildPath = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.OUTPUT_PATH_CMD);
			if (buildPath == null)
			{
				if (isStandalone)
					buildPath = Path.Combine(config.StandaloneBuildOutputPath, config.IOSPathAppend);
				else
					buildPath = Path.Combine(config.ExportBuildOutputPath, config.IOSPathAppend);
			}

			// Validate output directory and prep build time info
			EnsureDirectoryExists(buildPath);
			WriteBuildVersionData();

			// Try to get command prompt override for include demo scenes. If no cmd, use config as default, otherwise try to use cmd value.
			var includeDemoScenes = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.INCLUDE_DEMO_SCENES_CMD);
			if (includeDemoScenes != null)
			{
				if (includeDemoScenes == "true")
					config.SetIncludeDemoScenes(true);
				else if (includeDemoScenes == "false")
					config.SetIncludeDemoScenes(false);
			}

			// Get scene paths, always main scenes, and if chosen, demo scenes.
			var scenePaths = GetScenePaths(config, config.IncludeDemoScenes);

			// Try to get command prompt override for is development build. If no cmd, use config as default, otherwise try to use cmd value.
			var developmentBuild = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.IS_DEVELOPMENT_BUILD_CMD);
			if (developmentBuild != null)
			{
				if (developmentBuild == "true")
					config.SetIsDevelopmentBuild(true);
				else
					config.SetIsDevelopmentBuild(false);

			}

			// NOTE: iOS is always built as standalone, to embed in react native app external modifier script must be used.
			var buildOptions = GetBuildPlayerOptions(buildPath, BuildTarget.iOS, scenePaths, config.IsDevelopmentBuild, config.AdditionalScriptingDefines);

			// For standalone we want more appropriate naming structure of the output build
			if (isStandalone)
			{
				// Check command line for output name and fallback to default if none passed in
				var buildName = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.OUTPUT_NAME_CMD);
				if (buildName == null)
					buildName = $"{config.BuildName}_{config.IOSPathAppend}";

				// Set up final output name
				buildOptions.locationPathName += $"/{buildName}";
			}

			// Trigger build
			Build(buildOptions, config.ReportName);
		}

		/// <summary>
		/// Generates all build options and settings from configs and or command line args and triggers Android build.
		/// </summary>
		/// <param name="isStandalone">Whether the Android build should be standalone or exported as a library.</param>
		public static void BuildAndroid(bool isStandalone = false)
		{
			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Check command line for output path and fallback to default if none passed in
			var buildPath = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.OUTPUT_PATH_CMD);
			if (buildPath == null)
			{
				if (isStandalone)
					buildPath = Path.Combine(config.StandaloneBuildOutputPath, config.AndroidPathAppend);
				else
					buildPath = Path.Combine(config.ExportBuildOutputPath, config.AndroidPathAppend);
			}

			// Validate output directory and prep build time info
			EnsureDirectoryExists(buildPath);
			WriteBuildVersionData();

			// Try to get command prompt override for include demo scenes. If no cmd, use config as default, otherwise try to use cmd value.
			var includeDemoScenes = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.INCLUDE_DEMO_SCENES_CMD);
			if (includeDemoScenes != null)
			{
				if (includeDemoScenes == "true")
					config.SetIncludeDemoScenes(true);
				else if (includeDemoScenes == "false")
					config.SetIncludeDemoScenes(false);
			}

			// Get scene paths, always main scenes, and if chosen, demo scenes.
			var scenePaths = GetScenePaths(config, config.IncludeDemoScenes);

			// Try to get command prompt override for is development build. If no cmd, use config as default, otherwise try to use cmd value.
			var developmentBuild = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.IS_DEVELOPMENT_BUILD_CMD);
			if (developmentBuild != null)
			{
				if (developmentBuild == "true")
					config.SetIsDevelopmentBuild(true);
				else
					config.SetIsDevelopmentBuild(false);

			}

			// Set as standalone or export android project and setup build options
			EditorUserBuildSettings.exportAsGoogleAndroidProject = !isStandalone;
			var buildOptions = GetBuildPlayerOptions(buildPath, BuildTarget.Android, scenePaths, config.IsDevelopmentBuild, config.AdditionalScriptingDefines);

			// For standalone we want more appropriate naming structure of the output build
			if (isStandalone)
			{
				// Check command line for output name and fallback to default if none passed in
				var buildName = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.OUTPUT_NAME_CMD);
				if (buildName == null)
					buildName = $"{config.BuildName}_{config.AndroidPathAppend}";

				// Set up final output name (for standalone android we specify the file extension output)
				buildOptions.locationPathName += $"/{buildName}.apk";
			}

			// Trigger build
			Build(buildOptions, config.ReportName);
		}

		/// <summary>
		/// Generates all build options and settings from configs and or command line args and triggers WebGL build.
		/// </summary>
		/// <param name="isStandalone">Whether the WebGL build should be standalone or not (although iOS is always standalone, this var impacts the output location and so is required).</param>
		public static void BuildWebGL(bool isStandalone = false)
		{
			// Try to get scriptable object config
			var config = BuildAutomationUtils.GetBuildAutomationConfig();
			if (config == null) return;

			// Check command line for output path and fallback to default if none passed in
			var buildPath = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.OUTPUT_PATH_CMD);
			if (buildPath == null)
			{
				if (isStandalone)
					buildPath = Path.Combine(config.StandaloneBuildOutputPath, config.WebGLPathAppend);
				else
					buildPath = Path.Combine(config.StandaloneBuildOutputPath, config.WebGLPathAppend);
			}

			// Validate output directory and prep build time info
			EnsureDirectoryExists(buildPath);
			WriteBuildVersionData();

			// Try to get command prompt override for include demo scenes. If no cmd, use config as default, otherwise try to use cmd value.
			var includeDemoScenes = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.INCLUDE_DEMO_SCENES_CMD);
			if (includeDemoScenes != null)
			{
				if (includeDemoScenes == "true")
					config.SetIncludeDemoScenes(true);
				else if (includeDemoScenes == "false")
					config.SetIncludeDemoScenes(false);
			}

			// Get scene paths, always main scenes, and if chosen, demo scenes.
			var scenePaths = GetScenePaths(config, config.IncludeDemoScenes);

			// Try to get command prompt override for is development build. If no cmd, use config as default, otherwise try to use cmd value.
			var developmentBuild = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.IS_DEVELOPMENT_BUILD_CMD);
			if (developmentBuild != null)
			{
				if (developmentBuild == "true")
					config.SetIsDevelopmentBuild(true);
				else
					config.SetIsDevelopmentBuild(false);

			}

			// NOTE: WebGL is always built as standalone, to embed in react native app external modifier script must be used.
			var buildOptions = GetBuildPlayerOptions(buildPath, BuildTarget.WebGL, scenePaths, config.IsDevelopmentBuild, config.AdditionalScriptingDefines);

			// For standalone we want more appropriate naming structure of the output build
			if (isStandalone)
			{
				// Check command line for output name and fallback to default if none passed in
				var buildName = CommandLineBridge.TryGetCommandLineArg(BuildAutomationDefines.CommandLine.OUTPUT_NAME_CMD);
				if (buildName == null)
					buildName = $"{config.BuildName}_{config.WebGLPathAppend}";

				// Set up final output name
				buildOptions.locationPathName += $"/{buildName}";
			}

			// Trigger build
			Build(buildOptions, config.ReportName);
		}

		/// <summary>
		/// Runs the build pipeline, outputs result and generates a report.
		/// </summary>
		/// <param name="options">The build player options used to create a build.</param>
		private static void Build(BuildPlayerOptions options, string reportName)
		{
			// Run build process
			BuildReport report = BuildPipeline.BuildPlayer(options);
			BuildSummary summary = report.summary;

			// Output result
			if (summary.result == BuildResult.Succeeded)
				Debug.Log("Build succeeded: " + summary.totalSize + " bytes");
			else if (summary.result == BuildResult.Failed)
				Debug.Log("Build failed");

			// Save build report
			OutputBuildReport(report, options.locationPathName, reportName);
		}

		#endregion

		#region BuildReportMethods

		/// <summary>
		/// Generates and saves build report to a text file once build is complete.
		/// </summary>
		/// <param name="report">The internally built report.</param>
		/// <param name="buildPath">Output path to save the text file report to.</param>
		private static void OutputBuildReport(BuildReport report, string buildPath, string reportName)
		{
			// Parse path for report
			var outputPath = GetBuildReportPath(buildPath, reportName);

			// Build string from report
			StringBuilder sb = new StringBuilder();
			foreach (var x in report.packedAssets)
			{
				if (x.contents != null)
				{
					foreach (var n in x.contents)
					{
						sb.AppendFormat("{0}\t{1}\n", n.sourceAssetPath, n.packedSize);
					}
				}
			}

			// Save to text file
			File.WriteAllText(outputPath, sb.ToString());
		}

		#endregion

		#region BuildVersionInfoMethods

		/// <summary>
		/// Generates BuildVersionData and outputs to json file at specified project path.
		/// </summary>
		public static void WriteBuildVersionData()
		{
			var path = Path.Combine(BuildAutomationDefines.Validation.EXPECTED_VERSION_INFO_PATH, BuildAutomationDefines.Versioning.VERSION_INFO_FILE_NAME);
			var gitStatus = CommandLineBridge.RunCommand(BuildAutomationDefines.CommandLine.RUN_GIT_STATUS_CMD);
			var gitSHA = CommandLineBridge.RunCommand(BuildAutomationDefines.CommandLine.RUN_GIT_SHA_CMD);
			var buildVersionData = BuildVersionData.NowFromStatus(gitStatus, gitSHA);
			var json = JsonConvert.SerializeObject(buildVersionData);
			PlayerSettings.bundleVersion = buildVersionData.GetVersionStringForBuild();
			File.WriteAllText(path, json);
		}

		#endregion

		#region HelperMethods

		/// <summary>
		/// Generates string array containing all the desired scene paths, by default this includes all MainScenes within the config, but optionally can include DemoScenes if requested by CLI or config.
		/// </summary>
		/// <param name="config">BuildAutomationConfig scriptable object to use as reference for config settings - holds main and demo scene names / paths.</param>
		/// <param name="includeDemoScenes">Whether or not to include non-release scenes for demo / dev purposes.</param>
		/// <returns>Resulting string array containing all scene paths, to be used by build options.</returns>
		private static string[] GetScenePaths(BuildAutomationConfig config, bool includeDemoScenes = false)
		{
			var scenePaths = new List<string>();

			// Add all main scenes
			foreach (var mainScene in config.MainScenes)
			{
				scenePaths.Add(mainScene.Path);
			}

			// Add all demo scenes if requested
			if (includeDemoScenes)
			{
				foreach (var demoScene in config.DemoScenes)
				{
					scenePaths.Add(demoScene.Path);
				}
			}

			return scenePaths.ToArray();
		}


		/// <summary>
		/// Generates and returns BuildPlayerOptions customised to the optional settings passed in.
		/// </summary>
		/// <param name="path">Output path for the build.</param>
		/// <param name="target">Build target device.</param>
		/// <param name="scenePaths">String array of all scenes to include.</param>
		/// <param name="additionalScriptingDefines">String array of all additional scriptable defines to include.</param>
		/// <returns></returns>
		private static BuildPlayerOptions GetBuildPlayerOptions(string path, BuildTarget target, string[] scenePaths, bool developmentBuild, string[] additionalScriptingDefines)
		{
			var options = new BuildPlayerOptions()
			{
				scenes = scenePaths,
				locationPathName = path,
				target = target,
				extraScriptingDefines = additionalScriptingDefines,
			};

			// Add development build on/off if cmd
			if (developmentBuild)
				options.options = BuildOptions.Development;

			return options;
		}


		/// <summary>
		/// Generates final build report path with file name and extension.
		/// </summary>
		/// <param name="path">The desired path to output the build report to.</param>
		/// <param name="reportName">The desired name of the build report.</param>
		/// <returns></returns>
		private static string GetBuildReportPath(string path, string reportName)
		{
			// If path has file name and doesn't end with folder directory...
			if (Path.GetExtension(path) != string.Empty)
			{
				// ...Isolate file name & extension from path...
				var splitString = path.Split('/');
				var charsToRemove = splitString[splitString.Length - 1].Length;
				// ...Increment length by one extra character to include '/' ...
				charsToRemove++;
				//... Remove from _path.
				path = path.Remove(path.Length - charsToRemove);
			}

			// Return result
			return path + reportName + BuildAutomationDefines.Validation.BUILD_REPORT_EXTENSION;
		}


		/// <summary>
		/// Creates directory at the path provided unless one already exists.
		/// </summary>
		/// <param name="path">The path to the directory that needs to exist</param>
		public static void EnsureDirectoryExists(string path)
		{
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
		}

		#endregion
	}
}