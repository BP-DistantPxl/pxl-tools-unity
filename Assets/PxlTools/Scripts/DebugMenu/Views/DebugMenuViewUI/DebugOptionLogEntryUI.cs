#if DEVELOPMENT_BUILD || UNITY_EDITOR
using TMPro;
using PxlTools.DebugMenu.Data;
using UnityEngine;
using UnityEngine.UI;

public class DebugOptionLogEntryUI : MonoBehaviour
{
    #region Variables

    public LogType EntryType => _entryType;

	[SerializeField] private Image _icon;
    [SerializeField] private TextMeshProUGUI _text;

    private LogType _entryType;

	#endregion

    /// <summary>
    /// Called by DebugOptionLogUI upon generation of new log entry. Applies visuals and text to UI.
    /// </summary>
    /// <param name="entry">Log Entry data set used to populate Entry UI.</param>
	public void Initialise(DebugOptionLogData.LogEntry entry)
    {
        // Store entry data
        _entryType = entry.LogType;
        
        // Use entry values to apply visuals
        _text.text = $"{entry.LogString}\n{entry.StackTrace}";
        _text.color = entry.LogColor;
        _icon.sprite = entry.LogIcon;
    }
}
#endif