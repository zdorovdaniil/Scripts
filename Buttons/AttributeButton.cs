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
    private CharacterUI _characterUI;
    private bool _isAskConfirm;
    private bool _isSavePlStats;

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
        _playerStats.UpdateArmor();
        _playerStats.CheckStatusEquip();
        _characterUI.UpdateButtons();
        GlobalSounds.Instance.SAttributeUp();
        if (_isSavePlStats) _playerStats.SaveStatsToSlot(ProcessCommand.CurActiveSlot, _playerStats.stats);
    }
    public void ClickUpAttribute()
    {
        if (_isAskConfirm)
        { MsgBoxUI.Instance.Show(this.gameObject, "upgrade attribute", "do you really want to spend 1 attribute point to upgrade attribute?"); }
        else UpAttribute();
    }
    public void SetText(PlayerStats playerStats, AttributeStat attribute, bool isAsk, bool isSaving, CharacterUI characterUI)
    {
        _characterUI = characterUI;
        _attribute = attribute;
        _playerStats = playerStats;
        _isAskConfirm = isAsk;
        _isSavePlStats = isSaving;
        if (_attribute.IsAvaibleToLevelUp(_playerStats.GetPointStat)) _buttonUp.gameObject.SetActive(true);
        else { _buttonUp.gameObject.SetActive(false); }
        _nameAttribute.text = _attribute.Attr.Name;
        _level.text = _attribute.Level.ToString();
        _descruption.text = _attribute.Attr.Description;
        _maxLevel.text = _attribute.Attr.MaxLevel.ToString();

    }
}
