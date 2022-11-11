using UnityEngine;
using UnityEngine.UI;
using TMPro;

[RequireComponent(typeof(PlayerStats))]
public class PlayerUpdate : MonoBehaviour
{
    private PlayerStats _playerStats;
    private PlayerLeveling _playerLeveling;
    /// <summary> /// время стандартного восстановления HP /// </summary>
    [SerializeField] private float _timeRegenHP = 1f;
    [SerializeField] private Item usingPoison;
    /// <summary> /// время пополнения здоровья от зелья /// </summary>
    [SerializeField] private float _timeRegenPoison = 0.1f;
    [SerializeField] private int numAddHPfromPoison = 0;
    [SerializeField] private TextMeshProUGUI _hpText;
    [SerializeField] private Slider _sliderHP;
    [SerializeField] private Slider _sliderEXP;
    [SerializeField] private GameObject _buffField;
    [SerializeField] private Transform _containBuffFields;
    // Buttons
    [SerializeField] private PercButton _jerkButton;
    [SerializeField] private PercButton _flyingSlashButton;
    private float needEXP = 1000;

    private void Start()
    {
        _playerStats = GetComponent<PlayerStats>();
        _sliderHP.minValue = 0;
        _playerLeveling = PlayerLeveling.Instance;
    }
    private void FixedUpdate()
    {
        CheckRegeneHP();
        UpdateUI();
    }

    private float _timerForRegenHP;
    private float _timerUpdateUI;
    private void PoisonRegenHP()
    {
        numAddHPfromPoison -= 1;
        _playerStats.curHP += 1;
        if (numAddHPfromPoison <= 0) { usingPoison = null; }
        _playerStats.UpdateHP();
    }
    private void DefaultRegenHP()
    {
        _timeRegenHP = _playerStats.stats.GetTimeRegenHP();
        _playerStats.AddHP(_playerStats.stats.GetAddHP());
        UpdateUI();
    }
    private void CheckRegeneHP()
    {
        _timerForRegenHP += Time.deltaTime;
        if (_timerForRegenHP >= _timeRegenHP)
        {
            _timerForRegenHP = 0f;
            DefaultRegenHP();
        }
        else if (numAddHPfromPoison <= 0) return;
        else if (usingPoison != null && _timerForRegenHP >= _timeRegenPoison)
        {
            _timerForRegenHP = 0f;
            PoisonRegenHP();
        }
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
    private float _timeToHeartBeat = 0f;
    private void CheckHeartBeat()
    {
        if (_playerStats.curHP <= _playerStats.stats.HP * 0.5f)
        {
            _timeToHeartBeat += 0.25f;
            if (_playerStats.curHP <= _playerStats.stats.HP * 0.25f)
            {
                // выполняется при 1/4 части ХП
                if (_timeToHeartBeat >= 0.5f) ExeSoundHeartBeat(); else { return; }
            }
        }
        else { return; }
    }
    private void ExeSoundHeartBeat()
    {
        Debug.Log("Hearth on " + _timeToHeartBeat.ToString());
        _timeToHeartBeat = 0;
        GlobalSounds.Instance.SHeartBeat();
    }
    private void UpdateUI()
    {
        _timerUpdateUI += Time.deltaTime;
        if (_timerUpdateUI >= 0.25f)
        {
            _timerUpdateUI = 0;
            UpdateHPSlider();
            UpdateBuffFields();
            CheckHeartBeat();
            _jerkButton.UpdateButton(_playerStats.stats.Skills[1].Level);
            _flyingSlashButton.UpdateButton(_playerStats.stats.Skills[6].Level);
            if (_playerLeveling) needEXP = _playerLeveling.GetHeedExp(_playerStats.stats.Level);
        }

    }
    private void UpdateBuffFields()
    {
        if (_playerStats.stats.ActiveBuffes.Count > 0)
        {
            ProcessCommand.ClearChildObj(_containBuffFields);
            foreach (BuffStat buffStat in _playerStats.stats.ActiveBuffes)
            {
                if (buffStat.DoingBuff())
                {
                    Instantiate(_buffField, _containBuffFields).GetComponent<BuffField>().SetFieldsBuff(buffStat);
                }
                else
                {
                    _playerStats.stats.ResetBuff(buffStat);
                    _playerStats.ChangeSpeed();
                    _playerStats.UpdateArmor();
                    break;
                }
            }
        }
        else
        {
            ProcessCommand.ClearChildObj(_containBuffFields);
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

}
