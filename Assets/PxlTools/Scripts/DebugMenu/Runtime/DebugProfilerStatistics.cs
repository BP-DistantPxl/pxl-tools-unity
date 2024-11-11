#if DEVELOPMENT_BUILD || UNITY_EDITOR
using System.Collections.Generic;
using PxlTools.Core.Runtime;
using PxlTools.DebugMenu.Data;
using PxlTools.DebugMenu.Defines;
using Unity.Profiling;
using UnityEngine;

namespace PxlTools.DebugMenu.Runtime
{
	public class DebugProfilerStatistics : DebugOptionCollection
	{
		#region Variables

		// Graphics profilers
		private ProfilerRecorder _mainThreadTimeRecorder;
		private ProfilerRecorder _renderThreadTimeRecorder;
		private ProfilerRecorder _setPassCallsRecorder;
		private ProfilerRecorder _drawCallsRecorder;
		private ProfilerRecorder _totalBatchesRecorder;
		private ProfilerRecorder _totalTrianglesRecorder;
		private ProfilerRecorder _totalVerticesRecorder;
		private ProfilerRecorder _totalShadowCastersRecorder;
		// Memory Profilers
		private ProfilerRecorder _totalReservedMemoryRecorder;
		private ProfilerRecorder _gcReservedMemoryRecorder;
		private ProfilerRecorder _systemUsedMemoryRecorder;
		// Other vars
		private bool _isProfilerEnabled = false;

		#endregion

		#region InheritedMethods

		public override void Initialise()
		{
			AddProfilerEnableToggleOption();
			AddGraphicsProfilerOptions();
			AddMemoryProfilerOptions();

			Debug.Log(string.Format(DebugMenuDefines.Strings.Common.TAB_ADDED, DebugMenuDefines.Strings.Profiler.TAB_NAME));
		}

		#endregion

		#region DebugMenuPopulationMethods

		private void AddProfilerEnableToggleOption() => PxlToolsManager.Instance.DebugMenuManager?.AddOption(GetProfilerEnableToggleData(), DebugMenuDefines.Strings.Profiler.TAB_NAME, DebugMenuDefines.Strings.Common.TOOLS_HEADER_NAME, true);	
		private void AddGraphicsProfilerOptions() => PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetGraphicsProfilerDatas(), DebugMenuDefines.Strings.Profiler.TAB_NAME, DebugMenuDefines.Strings.Profiler.GRAPHICS_HEADER_NAME, true);
		private void AddMemoryProfilerOptions() => PxlToolsManager.Instance.DebugMenuManager?.AddOptions(GetMemoryProfilerDatas(), DebugMenuDefines.Strings.Profiler.TAB_NAME, DebugMenuDefines.Strings.Profiler.MEMORY_HEADER_NAME, true);

		#endregion

		#region PublicEventMethods

		/// <summary>
		/// Event fired when enable profiling toggle value is changed, enables or disables profiler stats.
		/// </summary>
		/// <param name="value">The current bool value the toggle is set to.</param>
		public void OnEnableToggleChanged(bool value)
		{
			_isProfilerEnabled = value;

			if (_isProfilerEnabled)
			{
				EnableProfilerRecorders();
				Debug.LogWarning(DebugMenuDefines.Strings.Profiler.PROFILER_ENABLED);
			}
			else
			{
				DisableProfilerRecorders();
				Debug.LogWarning(DebugMenuDefines.Strings.Profiler.PROFILER_DISABLED);
			}
		}

		/// <summary>
		/// Resolution label continually updates even when profiler is off, updates value in UI and outputs warning if change occurs.
		/// </summary>
		/// <param name="labelData">The full data set of the label - used to change the label value.</param>
		public void OnResolutionLabelUpdate(DebugOptionLabelData labelData)
		{
			var currentResolutionText = $"{Screen.width} x {Screen.height}";
			if (labelData.Value != currentResolutionText)
			{
				Debug.LogWarning($"{DebugMenuDefines.Strings.Profiler.RESOLUTION_CHANGE_WARNING} {currentResolutionText}");
				labelData.Value = currentResolutionText;
			}
		}

		/// <summary>
		/// Events fired by associated label when label update time has passed.
		/// </summary>
		/// <param name="labelData">The full data set of the label - in this case, used to change the label value</param>
		public void OnFPSLabelUpdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{1.0f / (GetRecorderFrameAverage(_mainThreadTimeRecorder) * (1e-9f)):F1} FPS" : "...";
		public void OnMainThreadLabelUpdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{GetRecorderFrameAverage(_mainThreadTimeRecorder) * (1e-6f):F1} ms" : "...";
		public void OnRenderThreadLabelUpdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{GetRecorderFrameAverage(_renderThreadTimeRecorder) * (1e-6f):F1} ms" : "...";
		public void OnSetPassCallsLabelupdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_setPassCallsRecorder.LastValue}" : "...";
		public void OnDrawCallsLabelupdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_drawCallsRecorder.LastValue}" : "...";
		public void OnTotalBatchesLabelupdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_totalBatchesRecorder.LastValue}" : "...";
		public void OnTrianglesCountLabelupdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_totalTrianglesRecorder.LastValue * (1e-3f):F2} k" : "...";
		public void OnVerticesCountLabelupdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_totalVerticesRecorder.LastValue * (1e-3f):F2} k" : "...";
		public void OnTotalShadowCastersLabelUpdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_totalShadowCastersRecorder.LastValue}" : "...";
		public void OnTotalReservedMemoryLabelUpdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_totalReservedMemoryRecorder.LastValue / (1024 * 1024)} MB" : "...";
		public void OnGCReservedMemoryLabelUpdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_gcReservedMemoryRecorder.LastValue / (1024 * 1024)} MB" : "...";
		public void OnSystemUsedMemoryLabelUpdate(DebugOptionLabelData labelData) => labelData.Value = _isProfilerEnabled ? $"{_systemUsedMemoryRecorder.LastValue / (1024 * 1024)} MB" : "...";
		
		#endregion

		#region AdditionalFunctionalityMethods

		/// <summary>
		/// Begins profiler recorders to track performance.
		/// </summary>
		private void EnableProfilerRecorders()
		{
			_mainThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, DebugMenuDefines.Strings.Profiler.MAIN_THREAD_RECORDER, 15);
			_renderThreadTimeRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Internal, DebugMenuDefines.Strings.Profiler.RENDER_THREAD_RECORDER, 15);
			_setPassCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, DebugMenuDefines.Strings.Profiler.SET_PASS_RECORDER);
			_drawCallsRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, DebugMenuDefines.Strings.Profiler.DRAW_CALLS_RECORDER);
			_totalBatchesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, DebugMenuDefines.Strings.Profiler.BATCHES_RECORDER);
			_totalTrianglesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, DebugMenuDefines.Strings.Profiler.TRIS_RECORDER);
			_totalVerticesRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, DebugMenuDefines.Strings.Profiler.VERTS_RECORDER);
			_totalShadowCastersRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Render, DebugMenuDefines.Strings.Profiler.SHADOW_CASTER_RECORDER);

			_totalReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, DebugMenuDefines.Strings.Profiler.TOTAL_MEMORY_RECORDER);
			_gcReservedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, DebugMenuDefines.Strings.Profiler.GC_MEMORY_RECORDER);
			_systemUsedMemoryRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, DebugMenuDefines.Strings.Profiler.SYSTEM_USED_MEMORY_RECORDER);
		}

		/// <summary>
		/// Disposes of profiler recorders and stops tracking performance.
		/// </summary>
		private void DisableProfilerRecorders()
		{
			_mainThreadTimeRecorder.Dispose();
			_renderThreadTimeRecorder.Dispose();
			_setPassCallsRecorder.Dispose();
			_drawCallsRecorder.Dispose();
			_totalBatchesRecorder.Dispose();
			_totalTrianglesRecorder.Dispose();
			_totalVerticesRecorder.Dispose();
			_totalShadowCastersRecorder.Dispose();

			_totalReservedMemoryRecorder.Dispose();
			_gcReservedMemoryRecorder.Dispose();
			_systemUsedMemoryRecorder.Dispose();
		}

		/// <summary>
		/// Returns the average value from a collection of frame samples within a recorder.
		/// </summary>
		/// <param name="recorder">The average value between all stored samples.</param>
		/// <returns></returns>
		private double GetRecorderFrameAverage(ProfilerRecorder recorder)
		{
			var samplesCount = recorder.Capacity;
			if (samplesCount == 0)
				return 0;

			double r = 0;
			var samples = new List<ProfilerRecorderSample>(samplesCount);
			recorder.CopyTo(samples);
			for (var i = 0; i < samples.Count; i++)
				r += samples[i].Value;
			r /= samplesCount;

			return r;
		}

		#endregion

		#region DebugDataGeneratingMethods

		/// <summary>
		/// Creates data set for a debug option toggle that turns on / off the profiling recorders.
		/// </summary>
		/// <returns>DebugOptionData for the enable toggle option.</returns>
		private DebugOptionData GetProfilerEnableToggleData()
		{
			DebugOptionData profilerEnableToggle = new DebugOptionData()
			{
				Id = "tgl_profilerEnable",
				Type = OptionSubType.Toggle,
				SubData = new DebugOptionToggleData(OnEnableToggleChanged)
				{
					DisplayText = "Enable Profiler?",
					Description = "May cause performance impact",
					Value = false
				}
			};

			return profilerEnableToggle;
		}

		/// <summary>
		/// Creates data sets for a collection of debug option labels that track graphics profiler info.
		/// </summary>
		/// <returns>Array of graphics profiler DebugOptionDatas to be displayed on the debug menu.</returns>
		private DebugOptionData[] GetGraphicsProfilerDatas()
		{
			DebugOptionData profilerFPSLabel = new DebugOptionData()
			{
				Id = "lbl_profilerFPSThread",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnFPSLabelUpdate)
				{
					DisplayText = "FPS",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerMainThreadLabel = new DebugOptionData()
			{
				Id = "lbl_profilerCPUMainThread",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnMainThreadLabelUpdate)
				{
					DisplayText = "Main Thread",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerRenderThreadLabel = new DebugOptionData()
			{
				Id = "lbl_profilerRenderThread",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnRenderThreadLabelUpdate)
				{
					DisplayText = "Render Thread",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerDrawCallsLabel = new DebugOptionData()
			{
				Id = "lbl_profilerDrawCalls",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnDrawCallsLabelupdate)
				{
					DisplayText = "Draw Calls",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerTotalBatchesLabel = new DebugOptionData()
			{
				Id = "lbl_profilerTotalBatches",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnTotalBatchesLabelupdate)
				{
					DisplayText = "Saved by Batches",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerTotalTrianglesLabel = new DebugOptionData()
			{
				Id = "lbl_profilerTotalTriangles",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnTrianglesCountLabelupdate)
				{
					DisplayText = "Tris",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerTotalVerticesLabel = new DebugOptionData()
			{
				Id = "lbl_profilerTotalVertices",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnVerticesCountLabelupdate)
				{
					DisplayText = "Verts",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerResolutionLabel = new DebugOptionData()
			{
				Id = "lbl_profilerResolution",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnResolutionLabelUpdate)
				{
					DisplayText = "Resolution",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerSetPassCallsLabel = new DebugOptionData()
			{
				Id = "lbl_profilerSetPassCalls",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnSetPassCallsLabelupdate)
				{
					DisplayText = "SetPass Calls",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerTotalShadowCastersLabel = new DebugOptionData()
			{
				Id = "lbl_profilerTotalShadowCasters",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnTotalShadowCastersLabelUpdate)
				{
					DisplayText = "Shadow Casters",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			return new DebugOptionData[]
			{
				profilerFPSLabel,
				profilerMainThreadLabel,
				profilerRenderThreadLabel,
				profilerDrawCallsLabel,
				profilerTotalBatchesLabel,
				profilerTotalTrianglesLabel,
				profilerTotalVerticesLabel,
				profilerResolutionLabel,
				profilerSetPassCallsLabel,
				profilerTotalShadowCastersLabel
			};
		}

		/// <summary>
		/// Creates data sets for a collection of debug option labels that track memory profiler info.
		/// </summary>
		/// <returns>Array of memory profiler DebugOptionDatas to be displayed on the debug menu.</returns>
		private DebugOptionData[] GetMemoryProfilerDatas()
		{
			DebugOptionData profilerTotalReservedMemoryLabel = new DebugOptionData()
			{
				Id = "lbl_profilerTotalReservedMemoryThread",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnTotalReservedMemoryLabelUpdate)
				{
					DisplayText = "Total Reserved Memory",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerGCReservedMemoryLabel = new DebugOptionData()
			{
				Id = "lbl_profilerGCReservedMemory",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnGCReservedMemoryLabelUpdate)
				{
					DisplayText = "GC Reserved Memory",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			DebugOptionData profilerSystemUsedMemoryLabel = new DebugOptionData()
			{
				Id = "lbl_profilerSystemUsedMemory",
				Type = OptionSubType.Label,
				SubData = new DebugOptionLabelData(OnSystemUsedMemoryLabelUpdate)
				{
					DisplayText = "System Used Memory",
					Description = "",
					UpdateInterval = 0.2f
				}
			};

			return new DebugOptionData[]
			{
				profilerTotalReservedMemoryLabel,
				profilerGCReservedMemoryLabel,
				profilerSystemUsedMemoryLabel
			};
		}

		#endregion
	}
}
#endif