using UnityEngine;
using UnityEngine.Events;
using TMPro;

public class CounterButtons : MonoBehaviour
{
    private enum CounterType { None, ChangeDungeonLevel, }
    [SerializeField] private CounterType _counterType = CounterType.None;
    [SerializeField] private TMP_Text _curValueText; public void SetText(string text) => _curValueText.text = text;
    [SerializeField] private UnityEvent _eventOnChangeValue;
    [SerializeField] private int _value; public int GetValue => _value;

    /// <summary>
    /// Функция, меняет параметр
    /// </summary>
    /// <param name="step"> если spet отрицательный то кнопка будет уменьшать значение на заданное число</param>
    public void ChangeCounter(int step)
    {
        if (_counterType == CounterType.ChangeDungeonLevel)
        {
            ProcessCommand.SetDungeonLevel(ProcessCommand.GetDungeonLevel + step);
            _value = ProcessCommand.GetDungeonLevel;
            if (!PhotonNetwork.offlineMode) SelectDungeonUI.Instance.photonView.RPC("SetUIFromRules", PhotonTargets.AllBuffered, (int)_value);
            else SelectDungeonUI.Instance.SetUIFromRules(_value);
        }
        _eventOnChangeValue.Invoke();
        UpdateUI();
    }
    public void UpdateUI()
    {
        string textForUI = "";
        if (_counterType == CounterType.ChangeDungeonLevel)
        {
            textForUI = ProcessCommand.GetDungeonLevel.ToString();
        }
        _curValueText.text = textForUI;
    }
}
