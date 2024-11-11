using PxlTools.BuildAutomation.Data;
using PxlTools.BuildAutomation.Defines;
using UnityEngine;

namespace PxlTools.BuildAutomation.Utils
{
	public static class BuildAutomationUtils
	{

		/// <summary>
		/// Finds BuildAutomationConfig scriptable object within project and outputs validation errors if none or too many exist.
		/// </summary>
		/// <returns>The BuildAutomationCofig to use if present in the correct folder.</returns>
		public static BuildAutomationConfig GetBuildAutomationConfig()
		{
			// Get scriptable object build config
			var configs = Resources.LoadAll<BuildAutomationConfig>("");
			if (configs == null || configs.Length <= 0)
			{
				Debug.LogError(BuildAutomationDefines.Validation.CONFIG_MISSING_ERROR);
				return null;
			}
			else if (configs.Length > 1)
				Debug.LogError(BuildAutomationDefines.Validation.CONFIG_MULTIPLES_ERROR);
			else
				Debug.Log(BuildAutomationDefines.Validation.CONFIG_LOAD_SUCCESS);

			return configs[0];
		}
	}
}

