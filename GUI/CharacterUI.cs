using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    public static CharacterUI Instance;
    [SerializeField] private PlayerStats _playerStats;
    public void SetPlayerStats(PlayerStats playerStats) { _playerStats = playerStats; }
    [SerializeField] private List<AttributeButton> _attributeButtons = new List<AttributeButton>(4);
    [SerializeField] private Transform _posSpawnSkillButtons;
    [SerializeField] private GameObject _prefabSkillButton;
    [SerializeField] private bool _isAskConfirm;
    [SerializeField] private TMP_Text _skillPointsT;
    [SerializeField] private TMP_Text _attributePointsT;


    public void UpdateButtons()
    {
        foreach (AttributeButton attrButton in _attributeButtons)
        {
            attrButton.SetText(_playerStats, _isAskConfirm);
        }
        for (int i = 0; i < _posSpawnSkillButtons.childCount; i++)
        {
            Destroy(_posSpawnSkillButtons.GetChild(i).gameObject);
        }
        foreach (Skill skill in _playerStats.stats.Skills)
        {
            if (skill != null)
                Instantiate(_prefabSkillButton, _posSpawnSkillButtons).GetComponent<SkillButton>().SetText(_playerStats, skill, _isAskConfirm);
        }
        UpdateFieldsUI();
    }
    public void UpdateFieldsUI()
    {
        _skillPointsT.text = _playerStats.GetPointSkill.ToString();
        _attributePointsT.text = _playerStats.GetPointStat.ToString();
    }

    private void Start() { Instance = this; }
}
