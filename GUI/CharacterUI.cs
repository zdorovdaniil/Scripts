using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class CharacterUI : MonoBehaviour
{
    public static CharacterUI Instance;
    [SerializeField] private PlayerStats _playerStats;
    public void SetPlayerStats(PlayerStats playerStats) { _playerStats = playerStats; }
    [SerializeField] private Transform _posSpawnAttrButtons;
    [SerializeField] private GameObject _prefabAttrButton;
    [SerializeField] private Transform _posSpawnSkillButtons;
    [SerializeField] private GameObject _prefabSkillButton;
    [SerializeField] private bool _isAskConfirm;
    [SerializeField] private TMP_Text _skillPointsT;
    [SerializeField] private TMP_Text _attributePointsT;


    public void UpdateButtons()
    {
        ProcessCommand.ClearChildObj(_posSpawnSkillButtons);
        ProcessCommand.ClearChildObj(_posSpawnAttrButtons);
        foreach (AttributeStat attribute in _playerStats.stats.Attributes)
        {
            Instantiate(_prefabAttrButton, _posSpawnAttrButtons).GetComponent<AttributeButton>().SetText(_playerStats, attribute, _isAskConfirm);
        }
        foreach (Skill skill in _playerStats.stats.Skills)
        {
            if (skill)
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
