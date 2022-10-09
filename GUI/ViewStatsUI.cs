using System.Collections;
using TMPro;
using UnityEngine;


public class ViewStatsUI : MonoBehaviour
{
    public static ViewStatsUI Instance;
    [Header("PlayerAttributes")]
    [SerializeField] private TMP_Text _strenghText;
    [SerializeField] private TMP_Text _enduranceText;
    [SerializeField] private TMP_Text _agilityText;
    [SerializeField] private TMP_Text _speedText;
    [Header("PlayerStatCombat")]
    [SerializeField] private TMP_Text _attackText;
    [SerializeField] private TMP_Text _attackEquipText;
    [SerializeField] private TMP_Text _attackAttrText;
    [SerializeField] private TMP_Text _attackSkillText;
    [SerializeField] private TMP_Text _attackBuffText;

    [SerializeField] private TMP_Text _critValueText;
    [SerializeField] private TMP_Text _critValueEquipText;
    [SerializeField] private TMP_Text _critValueSkillText;
    [SerializeField] private TMP_Text _critValueBuffText;

    [SerializeField] private TMP_Text _critChanceText;
    [SerializeField] private TMP_Text _critChanceEquipText;
    [SerializeField] private TMP_Text _critChanceSkillText;
    [SerializeField] private TMP_Text _critChanceBuffText;
    //
    [SerializeField] private TMP_Text _defenceText;
    [SerializeField] private TMP_Text _defenceEquipText;
    [SerializeField] private TMP_Text _defenceAttrText;
    [SerializeField] private TMP_Text _defenceSkillText;
    [SerializeField] private TMP_Text _defenceBuffText;
    //
    [SerializeField] private TMP_Text _moveSpeedText;
    [SerializeField] private TMP_Text _moveSpeedAttrText;
    [SerializeField] private TMP_Text _moveSpeedBuffText;

    [SerializeField] private TMP_Text _regenValue;
    [SerializeField] private TMP_Text _timeRegen;
    [SerializeField] private TMP_Text _blockText;
    [SerializeField] private TMP_Text _damageText;
    [SerializeField] private TMP_Text _kickStrenght;
    [SerializeField] private TMP_Text _healthText;

    [SerializeField] private TMP_Text _curLvlText;
    [SerializeField] private TMP_Text _curExpText;

    [Header("All Stats")]
    [SerializeField] private TMP_Text _allRoomText;
    [SerializeField] private TMP_Text _allKillsText;
    [SerializeField] private TMP_Text _allOpenChestText;
    private DungeonStats _dungeonStats;
    private PlayerStats _playerStats;
    private InfoPlayerStats _infoStats;
    private void Awake()
    {
        Instance = this;
    }
    public void UpdateUI()
    {
        StartCoroutine(UpdateFields(0));
    }
    private IEnumerator UpdateFields(float time)
    {
        yield return new WaitForSecondsRealtime(time);
        {
            Debug.Log("Delay:" + time);
            if (_dungeonStats == null) { _dungeonStats = DungeonStats.Instance; }
            _strenghText.text = _infoStats._strenghText;
            _enduranceText.text = _infoStats._enduranceText;
            _agilityText.text = _infoStats._agilityText;
            _speedText.text = _infoStats._speedText;

            _attackText.text = _infoStats._attackText;
            _attackEquipText.text = _infoStats._attackEquipText;
            _attackAttrText.text = _infoStats._attackAttrText;
            _attackSkillText.text = _infoStats._attackSkillText;
            _attackBuffText.text = _infoStats._attackBuffText;

            _critChanceText.text = _infoStats._critChanceText;
            _critChanceEquipText.text = _infoStats._critChanceEquipText;
            _critChanceSkillText.text = _infoStats._critChanceSkillText;
            _critChanceBuffText.text = _infoStats._critChanceBuffText;

            _critValueText.text = _infoStats._critValueText;
            _critValueEquipText.text = _infoStats._critValueEquipText;
            _critValueSkillText.text = _infoStats._critValueSkillText;
            _critValueBuffText.text = _infoStats._critValueBuffText;

            _defenceText.text = _infoStats._defenceText;
            _defenceEquipText.text = _infoStats._defenceEquipText;
            _defenceAttrText.text = _infoStats._defenceAttrText;
            _defenceSkillText.text = _infoStats._defenceSkillText;
            _defenceBuffText.text = _infoStats._defenceBuffText;

            _moveSpeedText.text = _infoStats._moveSpeedText;
            _moveSpeedAttrText.text = _infoStats._moveSpeedAttrText;
            _moveSpeedBuffText.text = _infoStats._moveSpeedBuffText;

            _healthText.text = _infoStats._healthText;
            _regenValue.text = _infoStats._regenValue;
            _timeRegen.text = _infoStats._timeRegen;

            _blockText.text = _infoStats._blockText;
            _damageText.text = _infoStats._damageText;
            _kickStrenght.text = _infoStats._kickStrenght;


            _curLvlText.text = _infoStats._curLvlText;
            _curExpText.text = _infoStats._curExpText;

            _allRoomText.text = _dungeonStats.allPassRoom.ToString();
            _allKillsText.text = _dungeonStats.allKills.ToString();
            _allOpenChestText.text = _dungeonStats.allOpenChest.ToString();
        }
    }

    public void UpdateViewStatsUI(PlayerStats playerStats)
    {
        _infoStats = new InfoPlayerStats(playerStats);
        playerStats.stats.recount();
        playerStats.UpdateArmor();
        playerStats.CheckStatusEquip();

        StartCoroutine(UpdateFields(0.05f));
        StartCoroutine(UpdateFields(0.1f));

    }

}
