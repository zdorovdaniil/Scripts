using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SkillButton : MonoBehaviour
{
    [SerializeField] private Skill _skill;
    [SerializeField] private Image _imageSkill;
    [SerializeField] private TMP_Text _nameSkill;
    [SerializeField] private TMP_Text _level;
    [SerializeField] private TMP_Text _maxLevel;
    [SerializeField] private TMP_Text _needForUp;
    [SerializeField] private TMP_Text _descruption;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private Transform _buttonUp;
    public bool IsAskConfirm;

    public void GetReport(ReportType reportType)
    {
        if (reportType == ReportType.Accept)
        {
            SkillLevelUp();
        }
    }
    public void SkillLevelUp()
    {
        _playerStats.MinusPointSkill(_skill.SpendToLeveling());
        _skill.LevelUp();
        CharacterUI.Instance.UpdateButtons();
        GlobalSounds.Instance.SSkillUp();
    }
    public void ClickUpSkill()
    {
        if (IsAskConfirm)
        { MsgBoxUI.Instance.Show(this.gameObject, "upgrade skill", "do you really want spend skill point to upgrade a skill?"); }
        else SkillLevelUp();
    }
    public void SetText(PlayerStats playerStats, Skill skill, bool isAsk)
    {
        _skill = skill;
        _playerStats = playerStats;
        IsAskConfirm = isAsk;
        if (_skill.IsAvaibleToLevelUp(_playerStats.GetPointSkill)) _buttonUp.gameObject.SetActive(true);
        else { _buttonUp.gameObject.SetActive(false); }
        _nameSkill.text = skill.Name;
        _level.text = skill.Level.ToString();
        _imageSkill.sprite = skill.Icon;
        _descruption.text = skill.Description;
        _maxLevel.text = skill.MaxLevel.ToString();
        _needForUp.text = skill.SpendToLeveling().ToString();
    }
}
