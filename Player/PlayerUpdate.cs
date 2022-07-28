using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats))]
public class PlayerUpdate : MonoBehaviour
{
    private PlayerStats _playerStats;
    private PlayerEffects _playerEffects;
    [SerializeField] private float timerRegenHP = 1f; // колво времени на еденицу регенирации
    [SerializeField] private Item usingPoison;
    [SerializeField] private float timeRegenPoison = 0.1f;
    [SerializeField] private int numAddHPfromPoison = 0;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Slider _sliderHP;
    [SerializeField] private Slider _sliderEXP;
    [SerializeField] private GameObject _buffField;
    [SerializeField] private Transform _containBuffFields;
    // Buttons
    [SerializeField] private JerkButton _jerkButton;
    [SerializeField] private List<BuffClass> _buffObjParticles = new List<BuffClass>();

    private float needEXP = 1000;

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _playerEffects = GetComponent<PlayerEffects>();
        _sliderHP.minValue = 0;
    }

    private void UpdateHPSlider()
    {
        float nowHP = _playerStats.curHP;
        float maxHP = _playerStats.stats.HP;
        float curEXP = _playerStats.curEXP;
        _hpText.text = nowHP.ToString() + " / " + maxHP.ToString();
        _sliderHP.maxValue = maxHP;
        _sliderHP.value = nowHP;
        _sliderEXP.maxValue = needEXP;
        _sliderEXP.value = curEXP;
    }
    private float _timerRegenHP;
    private float _timerUpdateUI;

    private void FixedUpdate()
    {
        RegeneHP();
        _timerUpdateUI += Time.deltaTime;
        if (_timerUpdateUI >= 0.25f)
        {
            _timerUpdateUI = 0;
            UpdateHPSlider();
            UpdateBuffFields();
            UpdateJerkButton();
        }
    }
    private void UpdateJerkButton()
    {
        if (_playerStats.stats.Skills[1].Level > 0)
        {
            _jerkButton.UpdateButton();
        }
    }
    private void UpdateBuffFields()
    {
        if (_playerStats.stats.ActiveBuffes.Count > 0)
        {
            ProcessCommand.ClearChildObj(_containBuffFields);
            foreach (BuffClass buffClass in _playerStats.stats.ActiveBuffes)
            {
                buffClass.Time -= 0.25f;
                Instantiate(_buffField, _containBuffFields).GetComponent<BuffField>().SetFields(buffClass);

                if (buffClass.Time <= 0.25f)
                {
                    _playerStats.stats.ResetBuff(buffClass);
                    _playerStats.ChangeSpeed();
                    break;
                }
                else
                {

                }
            }
        }
        else
        {
            ProcessCommand.ClearChildObj(_containBuffFields);
        }

    }
    private void RegeneHP()
    {
        _timerRegenHP += Time.deltaTime;
        if (_timerRegenHP >= timerRegenHP)
        {
            _timerRegenHP = 0f;
            DefaultRegenHP();
            PlayerLeveling playerLeveling = PlayerLeveling.instance;
            if (playerLeveling != null)
                needEXP = playerLeveling.GetHeedExp(_playerStats.stats.Level);
        }
        else if (numAddHPfromPoison <= 0) return;
        else if (usingPoison != null && _timerRegenHP >= timeRegenPoison)
        {
            _timerRegenHP = 0f;
            PoisonRegenHP();
        }
    }

    public void UseHealPoison(Item item)
    {
        usingPoison = item;
        foreach (BuffClass buffClass in item.Buffs)
        {
            if (buffClass.Buff == Buff.Healing)
                numAddHPfromPoison = buffClass.Value;
        }
    }
    private void PoisonRegenHP()
    {
        numAddHPfromPoison -= 1;
        _playerStats.curHP += 1;
        if (numAddHPfromPoison <= 0) { usingPoison = null; }
        _playerStats.UpdateHP();
    }
    private void DefaultRegenHP(int _regenLvl = 0)
    {
        int endurance = _playerStats.stats.Endurance;
        int skillMedicene = _playerStats.stats.Skills[2].Level;
        int addHP = 0;
        timerRegenHP = Mathf.Floor(8 - (endurance / 10));

        if (skillMedicene >= 0 && skillMedicene < 7)
        {
            addHP = 1;
        }
        if (skillMedicene >= 7 && skillMedicene < 14)
        {
            addHP = 2;
        }
        if (skillMedicene >= 14)
        {
            addHP = 3;
        }
        _playerStats.AddHP(addHP);
        _playerStats.UpdateHP();
    }
}
