using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerStats))]
public class PlayerUpdate : MonoBehaviour
{
    private PlayerStats _playerStats;
    private PlayerLeveling _playerLeveling;
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
        _sliderHP.minValue = 0;
        _playerLeveling = PlayerLeveling.instance;
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
            _jerkButton.UpdateButton(_playerStats.stats.Skills[1].Level);
            if (_playerLeveling) needEXP = _playerLeveling.GetHeedExp(_playerStats.stats.Level);
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
        int endurance = _playerStats.stats.Attributes[2].Level;
        int addHP = _playerStats.stats.GetAddHP();
        timerRegenHP = _playerStats.stats.GetTimeRegenHP();
        _playerStats.AddHP(addHP);
        _playerStats.UpdateHP();
    }
}
