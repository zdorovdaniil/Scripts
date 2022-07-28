using TMPro;
using UnityEngine;

public class AttributeButton : MonoBehaviour
{
    [SerializeField] private TMP_Text _nameAttribute;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private int _numAttribute;
    [SerializeField] private PlayerStats _selectedPlayerStats;
    [SerializeField] private Transform _buttonUp;
    public bool IsAskConfirm;

    public void GetReport(ReportType reportType)
    {
        if (reportType == ReportType.Accept)
        {
            SendUpAttribute();
        }
    }
    private void SendUpAttribute()
    {
        if (!_selectedPlayerStats.UpAttribute(_numAttribute)) { MsgBoxUI.Instance.ShowAttention("not enought attribute points"); }
        CharacterUI.Instance.UpdateButtons();
    }
    public void UpAttribute()
    {
        if (IsAskConfirm)
        { MsgBoxUI.Instance.Show(this.gameObject, "upgrade attribute", "do you really want to spend 1 attribute point to upgrade attribute?"); }
        else SendUpAttribute();
    }
    public void SetText(PlayerStats playerStats, bool isAsk)
    {
        _selectedPlayerStats = playerStats;
        IsAskConfirm = isAsk;
        if (_selectedPlayerStats.TryUpgradeAttrubute()) _buttonUp.gameObject.SetActive(true);
        else { _buttonUp.gameObject.SetActive(false); }
        _level.text = playerStats.GetLevelAttribute(_numAttribute).ToString();
    }
}
