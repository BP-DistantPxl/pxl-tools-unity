using System;
using System.Text;
using System.Linq;
using System.Globalization;
using PxlTools.BuildAutomation.Defines;

namespace PxlTools.BuildAutomation.Data
{
	public class BuildVersionData
	{
		#region Variables

		public static BuildVersionData CurrentVersionData;

		public DateTimeOffset BuildTime { get; set; }
		public string GitBranch { get; set; }
		public string GitStatus { get; set; }
		public string GitSHA { get; set; }

		private static readonly StringBuilder _stringBuilder = new StringBuilder();

		#endregion

		#region PublicStaticMethods

		public static void Initialise()
		{
#if UNITY_EDITOR
			CurrentVersionData = new();
#else
			var info = Resources.Load<TextAsset>(BuildAutomationDefines.Versioning.VERSION_INFO_FILE_NAME);

			if (info == null)
			{
				CurrentVersionData = new();
				Debug.LogError(BuildAutomationDefines.Validation.VERSION_INFO_MISSING_ERROR);
			}
			else
			{
				CurrentVersionData = JsonConvert.DeserializeObject<BuildVersionData>(info.text);
				Debug.Log($"{BuildAutomationDefines.Validation.VERSION_INFO_SUCCESS}\n\n{info.text}");
			}
#endif
		}

		public static BuildVersionData NowFromStatus(string gitStatus, string sha)
		{
			var splitStatus = gitStatus.Split('\n');
			var info = new BuildVersionData
			{
				BuildTime = DateTimeOffset.Now,
				GitSHA = sha
			};

			if (splitStatus.Length == 0)
				info.GitStatus = BuildAutomationDefines.Validation.GIT_STATUS_UNKNOWN_ERROR;
			else if (splitStatus.Length == 1)
				info.GitStatus = splitStatus[0];
			else
			{
				info.GitBranch = splitStatus[0].Replace(BuildAutomationDefines.Versioning.GIT_STATUS_PREFIX_TO_REMOVE, "");
				info.GitStatus = splitStatus[1];
			}

			return info;
		}

		#endregion

		#region PublicVersionStringGetMethods

		public string GetVersionStringForCloud() => BuildTime.Date.ToString(BuildAutomationDefines.Versioning.DATE_FORMAT, CultureInfo.InvariantCulture) + "." + GitSHA;

		public string GetVersionStringForBuild()
		{
			var result = BuildTime.Date.ToString(BuildAutomationDefines.Versioning.DATE_FORMAT, CultureInfo.InvariantCulture);
			result += $".{GetNumbers(GitSHA ?? BuildAutomationDefines.Versioning.NUMBER_FORMAT)}";
			result = result.Substring(0, Math.Min(result.Length, 17));
			return result;
		}

		#endregion

		#region HelperMethods

		public StringBuilder GetStats()
		{
			_stringBuilder.Clear();
			_stringBuilder.AppendFormat($"<b>{BuildAutomationDefines.Versioning.GIT_BRANCH_LABEL}</b> {GitBranch ?? BuildAutomationDefines.Versioning.STATUS_UNKNOWN}\n");
			_stringBuilder.AppendFormat($"<b>{BuildAutomationDefines.Versioning.GIT_STATUS_LABEL}</b> {GitStatus ?? BuildAutomationDefines.Versioning.STATUS_UNKNOWN}\n");
			_stringBuilder.AppendFormat($"<b>{BuildAutomationDefines.Versioning.GIT_SHA_LABEL}</b> {GitSHA ?? BuildAutomationDefines.Versioning.STATUS_UNKNOWN}\n");
			_stringBuilder.AppendFormat($"<b>{BuildAutomationDefines.Versioning.BUILD_DATE_LABEL}</b> {BuildTime.Date.ToLongDateString()}\n");
			return _stringBuilder;
		}

		private static string GetNumbers(string input) => new string(input.Where(c => char.IsDigit(c)).ToArray());

		#endregion
	}
}