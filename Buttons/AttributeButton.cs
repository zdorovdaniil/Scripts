using UnityEngine;
using TMPro;
public class AttributeButton : MonoBehaviour
{
    [SerializeField] private AttributeStat _attribute;
    [SerializeField] private TMP_Text _nameAttribute;
    [SerializeField] private TMP_Text _descruption;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _maxLevel;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Transform _buttonUp;
    public bool IsAskConfirm;

    public void GetReport(ReportType reportType)
    {
        if (reportType == ReportType.Accept)
        {
            UpAttribute();
        }
    }
    private void UpAttribute()
    {
        if (!_playerStats.UpAttribute(_attribute.Attr.Id)) { MsgBoxUI.Instance.ShowAttention("not enought attribute points"); }
        CharacterUI.Instance.UpdateButtons();
        GlobalSounds.Instance.SAttributeUp();
    }
    public void ClickUpAttribute()
    {
        if (IsAskConfirm)
        { MsgBoxUI.Instance.Show(this.gameObject, "upgrade attribute", "do you really want to spend 1 attribute point to upgrade attribute?"); }
        else UpAttribute();
    }
    public void SetText(PlayerStats playerStats, AttributeStat attribute, bool isAsk)
    {
        _attribute = attribute;
        _playerStats = playerStats;
        IsAskConfirm = isAsk;
        if (_attribute.IsAvaibleToLevelUp(_playerStats.GetPointStat)) _buttonUp.gameObject.SetActive(true);
        else { _buttonUp.gameObject.SetActive(false); }
        _nameAttribute.text = _attribute.Attr.Name;
        _level.text = _attribute.Level.ToString();
        _descruption.text = _attribute.Attr.Description;
        _maxLevel.text = _attribute.Attr.MaxLevel.ToString();

    }
}
