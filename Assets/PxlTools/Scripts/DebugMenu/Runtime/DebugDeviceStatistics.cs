#if DEVELOPMENT_BUILD || UNITY_EDITOR
using PxlTools.Core.Runtime;
using PxlTools.DebugMenu.Data;
using PxlTools.DebugMenu.Defines;
using UnityEngine;

namespace PxlTools.DebugMenu.Runtime
{
	public class DebugDeviceStatistics : DebugOptionCollection
	{
		#region InheritedMethods

		public override void Initialise()
		{
			AddGeneralInfoOptions();
			AddProcessingDebugOptions();
			AddGraphicsDebugOptions();
			AddCopyButtonOption();

			Debug.Log(string.Format(DebugMenuDefines.Strings.Common.TAB_ADDED, DebugMenuDefines.Strings.Device.TAB_NAME));
		}

		#endregion

		#region DebugMenuPopulationMethods

		private void AddGeneralInfoOptions() => PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetGeneralInfoDatas(), DebugMenuDefines.Strings.Device.TAB_NAME, DebugMenuDefines.Strings.Device.GENERAL_HEADER_NAME, true);
		private void AddProcessingDebugOptions() => PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetProcessingDatas(), DebugMenuDefines.Strings.Device.TAB_NAME, DebugMenuDefines.Strings.Device.CPU_HEADER_NAME, true);
		private void AddGraphicsDebugOptions() => PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetGraphicsDatas(), DebugMenuDefines.Strings.Device.TAB_NAME, DebugMenuDefines.Strings.Device.GPU_HEADER_NAME, true);
		private void AddCopyButtonOption() => PxlToolsManager.Instance.DebugMenuManager?.AddOption(GetCopyButtonData(), DebugMenuDefines.Strings.Device.TAB_NAME, DebugMenuDefines.Strings.Common.TOOLS_HEADER_NAME, true);

		#endregion

		#region PublicEventMethods

		/// <summary>
		/// Event fired when copy system info button is clicked. Copies entire device info string to device copy buffer.
		/// </summary>
		public void OnCopyButtonClicked()
		{
			// Set up string
			var infoString = $"{DebugMenuDefines.Strings.Device.TAB_NAME.ToUpper()}\n";

			// Add device general info labels to string
			infoString += $"\n{DebugMenuDefines.Strings.Device.GENERAL_HEADER_NAME.ToUpper()}\n";
			infoString += $"{DebugMenuDefines.Strings.Common.LOG_SEPERATOR.ToUpper()}\n";
			var generalInfoDatas = GetGeneralInfoDatas();
			foreach (var data in generalInfoDatas)
			{
				var subData = (DebugOptionLabelData)data.SubData;
				infoString += $"{subData.DisplayText}: {subData.Value}\n";
			}

			// Add device processing info labels to string
			infoString += $"\n{DebugMenuDefines.Strings.Device.CPU_HEADER_NAME.ToUpper()}\n";
			infoString += $"{DebugMenuDefines.Strings.Common.LOG_SEPERATOR.ToUpper()}\n";
			var processingDatas = GetProcessingDatas();
			foreach (var data in processingDatas)
			{
				var subData = (DebugOptionLabelData)data.SubData;
				infoString += $"{subData.DisplayText}: {subData.Value}\n";
			}

			// Add device gpu info labels to string
			infoString += $"\n{DebugMenuDefines.Strings.Device.GPU_HEADER_NAME.ToUpper()}\n";
			infoString += $"{DebugMenuDefines.Strings.Common.LOG_SEPERATOR.ToUpper()}\n";
			var graphicsDatas = GetGraphicsDatas();
			foreach (var data in graphicsDatas)
			{
				var subData = (DebugOptionLabelData)data.SubData;
				infoString += $"{subData.DisplayText}: {subData.Value}\n";
			}

			GUIUtility.systemCopyBuffer = infoString;
		}

		#endregion

		#region DebugDataGeneratingMethods

		/// <summary>
		/// Creates data sets for a collection of debug option labels (that do not update) that display general system info.
		/// </summary>
		/// <returns>Array of general info DebugOptionDatas to be displayed on the debug menu.</returns>
		private DebugOptionData[] GetGeneralInfoDatas()
		{
			DebugOptionData deviceModelLabel = new DebugOptionData()
			{
				Id = "lbl_deviceModel",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Model",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.deviceModel
				}
			};

			DebugOptionData deviceNameLabel = new DebugOptionData()
			{
				Id = "lbl_deviceName",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Name",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.deviceName
				}
			};

			DebugOptionData deviceTypeLabel = new DebugOptionData()
			{
				Id = "lbl_deviceType",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Type",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.deviceType.ToString()
				}
			};

			DebugOptionData deviceUIDLabel = new DebugOptionData()
			{
				Id = "lbl_deviceUID",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "UID",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.deviceUniqueIdentifier.ToString()
				}
			};

			DebugOptionData deviceOSLabel = new DebugOptionData()
			{
				Id = "lbl_deviceOS",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Operating System",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.operatingSystem.ToString()
				}
			};

			DebugOptionData deviceMemoryLabel = new DebugOptionData()
			{
				Id = "lbl_deviceMemory",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "System Memory",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.systemMemorySize.ToString() + "MB"
				}
			};

			DebugOptionData deviceResolutionLabel = new DebugOptionData()
			{
				Id = "lbl_deviceResolution",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Resolution",
					Description = "",
					UpdateInterval = -1.0f,
					Value = Screen.currentResolution.ToString()
				}
			};

			DebugOptionData deviceOrientationLabel = new DebugOptionData()
			{
				Id = "lbl_deviceOrientation",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Orientation",
					Description = "",
					UpdateInterval = -1.0f,
					Value = Screen.orientation.ToString()
				}
			};

			return new DebugOptionData[]
			{
				deviceModelLabel,
				deviceNameLabel,
				deviceTypeLabel,
				deviceUIDLabel,
				deviceOSLabel,
				deviceMemoryLabel,
				deviceResolutionLabel,
				deviceOrientationLabel
			};
		}

		/// <summary>
		/// Creates data sets for a collection of debug option labels (that do not update) that display processing system info.
		/// </summary>
		/// <returns>Array of processing info DebugOptionDatas to be displayed on the debug menu.</returns>
		private DebugOptionData[] GetProcessingDatas()
		{
			DebugOptionData processorTypeLabel = new DebugOptionData()
			{
				Id = "lbl_processorType",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Type",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.processorType
				}
			};

			DebugOptionData processorFrequencyLabel = new DebugOptionData()
			{
				Id = "lbl_processorFrequency",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Frequency",
					Description = "",
					UpdateInterval = -1.0f,
					Value = $"{SystemInfo.processorFrequency.ToString()}MHz"
				}
			};

			DebugOptionData processorCountLabel = new DebugOptionData()
			{
				Id = "lbl_processorCount",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Count",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.processorCount.ToString()
				}
			};

			return new DebugOptionData[]
			{
				processorTypeLabel,
				processorFrequencyLabel,
				processorCountLabel
			};
		}

		/// <summary>
		/// Creates data sets for a collection of debug option labels (that do not update) that display graphics system info.
		/// </summary>
		/// <returns>Array of graphics info DebugOptionDatas to be displayed on the debug menu.</returns>
		private DebugOptionData[] GetGraphicsDatas()
		{


			DebugOptionData graphicsNameLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsName",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Name",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsDeviceName
				}
			};

			DebugOptionData graphicsIDLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsID",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "ID",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsDeviceID.ToString()
				}
			};

			DebugOptionData graphicsTypeLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsType",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "API Type",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsDeviceType.ToString()
				}
			};

			DebugOptionData graphicsDeviceVersionLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsDeviceVersion",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Version",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsDeviceVersion
				}
			};

			DebugOptionData graphicsMemorySizeLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsMemory",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Memory",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsMemorySize.ToString() + "MB"
				}
			};

			DebugOptionData graphicsMultithreadedLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsMultithreaded",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Multithreaded?",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsMultiThreaded.ToString()
				}
			};

			DebugOptionData graphicsShaderLevelLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsShaderLevel",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Shader Level",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsShaderLevel.ToString()
				}
			};

			DebugOptionData graphicsVendorLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsVendor",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Vendor",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsDeviceVendor.ToString()
				}
			};

			DebugOptionData graphicsVendorIDLabel = new DebugOptionData()
			{
				Id = "lbl_graphicsVendorID",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData()
				{
					DisplayText = "Vendor ID",
					Description = "",
					UpdateInterval = -1.0f,
					Value = SystemInfo.graphicsDeviceVendorID.ToString()
				}
			};

			return new DebugOptionData[]
			{
				graphicsNameLabel,
				graphicsIDLabel,
				graphicsTypeLabel,
				graphicsDeviceVersionLabel,
				graphicsMemorySizeLabel,
				graphicsMultithreadedLabel,
				graphicsShaderLevelLabel,
				graphicsVendorLabel,
				graphicsVendorIDLabel
			};
		}

		/// <summary>
		/// Creates data set for a debug option button that copies all the system info to device for external use.
		/// </summary>
		/// <returns>Debug option data set for copy button.</returns>
		private DebugOptionData GetCopyButtonData()
		{
			DebugOptionData copySystemInfoButton = new DebugOptionData()
			{
				Id = "btn_copySystemInfo",
				Type = OptionSubType.Button,
				SubData = new DebugOptionButtonData(OnCopyButtonClicked)
				{
					DisplayText = "Copy All Info",
					Description = "",
					ButtonText = "Copy to Device"
				}
			};

			return copySystemInfoButton;
		}

		#endregion
	}
}
#endif