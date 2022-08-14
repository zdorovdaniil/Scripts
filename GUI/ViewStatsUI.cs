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
    private void Awake()
    {
        Instance = this;
    }
    public void UpdateUI()
    {
        StartCoroutine(UpdateFields());
    }
    private IEnumerator UpdateFields()
    {
        yield return new WaitForSecondsRealtime(0.05f);
        {
            if (_dungeonStats == null) { _dungeonStats = DungeonStats.Instance; }
            _strenghText.text = _playerStats.stats.Attributes[0].Level.ToString();
            _enduranceText.text = _playerStats.stats.Attributes[1].Level.ToString();
            _agilityText.text = _playerStats.stats.Attributes[2].Level.ToString();
            _speedText.text = _playerStats.stats.Attributes[3].Level.ToString();

            _attackText.text = _playerStats.stats.attack.ToString();
            _attackEquipText.text = _playerStats.stats.attackWeapon.ToString();
            _attackAttrText.text = _playerStats.stats.GetAttackAttr().ToString();
            _attackSkillText.text = _playerStats.stats.attackSkill.ToString();
            _attackBuffText.text = _playerStats.stats.buffAttack.ToString();

            _critChanceText.text = _playerStats.stats.critChance.ToString() + " %";
            _critChanceEquipText.text = _playerStats.stats.critChanceEquip.ToString() + " %";
            _critChanceSkillText.text = _playerStats.stats.critChanceSkill.ToString() + " %";
            _critChanceBuffText.text = _playerStats.stats.buffCritChance.ToString() + " %";

            _critValueText.text = _playerStats.stats.critValue.ToString() + " %";
            _critValueEquipText.text = _playerStats.stats.critValueEquip.ToString() + " %";
            _critValueSkillText.text = _playerStats.stats.critValueSkill.ToString() + " %";
            _critValueBuffText.text = _playerStats.stats.buffCritValue.ToString() + " %";

            _defenceText.text = _playerStats.stats.armor.ToString();
            _defenceEquipText.text = _playerStats.stats.armorEquip.ToString();
            _defenceAttrText.text = _playerStats.stats.GetDefenceAttr().ToString();
            _defenceSkillText.text = _playerStats.stats.armorSkill.ToString();
            _defenceBuffText.text = _playerStats.stats.buffDefence.ToString();

            _moveSpeedText.text = _playerStats.stats.moveSpeed.ToString();
            _moveSpeedAttrText.text = _playerStats.stats.moveSpeedAttr.ToString();
            _moveSpeedBuffText.text = _playerStats.stats.buffSpeed.ToString();

            _healthText.text = _playerStats.stats.HP.ToString();
            _regenValue.text = _playerStats.stats.GetAddHP().ToString() + " HP";
            _timeRegen.text = _playerStats.stats.GetTimeRegenHP().ToString() + " SEC";

            _blockText.text = _playerStats.stats.minusDMG.ToString() + " to " + _playerStats.stats.GetMaxBlockDamage().ToString();
            _damageText.text = _playerStats.stats.minDMG.ToString() + " to " + _playerStats.stats.maxDMG.ToString();
            _kickStrenght.text = _playerStats.stats.KickStrenght().ToString();


            _curLvlText.text = _playerStats.stats.Level.ToString();
            string curExp = _playerStats.curEXP.ToString();
            string needExp = PlayerLeveling.instance.GetHeedExp(_playerStats.stats.Level).ToString();
            _curExpText.text = curExp + " / " + needExp;

            _allRoomText.text = _dungeonStats.allPassRoom.ToString();
            _allKillsText.text = _dungeonStats.allKills.ToString();
            _allOpenChestText.text = _dungeonStats.allOpenChest.ToString();
        }
    }
    public void UpdateViewStatsUI(PlayerStats playerStats)
    {
        _playerStats = playerStats;
        _playerStats.stats.recount();
        StartCoroutine(UpdateFields());

    }

}
