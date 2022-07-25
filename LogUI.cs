using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogUI : MonoBehaviour
{
    public static LogUI Instance; private void Awake() { Instance = this; }
    [SerializeField] private List<Text> _logText = new List<Text>();
    private string _log;
    public void Loger(string text)
    {
        _log = ProcessCommand.ToTime(GameManager.Instance.GetTimeDungeonGoing) + " ";

        for (int i = 0; i < _logText.Count - 1;i++)
        {
            if (_logText[i + 1] != null) _logText[i].text = _logText[i + 1].text;
            else break;
        }
        _logText[_logText.Count-1].text = _log + text;
    }
}
