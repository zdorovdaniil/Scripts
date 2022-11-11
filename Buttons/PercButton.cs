using UnityEngine;
using TMPro;

public enum PerkType { None, Jerc, FlyingSlash }
public class PercButton : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _buttonObject;
    [SerializeField] private TMP_Text _timerTMP;
    [SerializeField] private PerkType _perkType = PerkType.None;

    private float _timer = 0;
    private float _timeForReUse;
    private void Start()
    {
        if (_buttonObject) _buttonObject.SetActive(false);
    }

    public void UpdateButton(int skill)
    {
        if (skill > 0)
        {
            _timer += 0.25f;
            if (_buttonObject) _buttonObject.SetActive(true);
            _timeForReUse = PerkTimeReUse(_perkType);
            if (_timer <= _timeForReUse)
            {
                _timerTMP.text = (_timeForReUse - _timer).ToString();
            }
            else _timerTMP.text = "";
        }
        else
        {
            return;
        }
    }
    private float PerkTimeReUse(PerkType perkType)
    {
        switch (_perkType)
        {
            case PerkType.Jerc: return Mathf.Floor(8f - (0.5f * _playerStats.stats.Skills[1].Level));
            case PerkType.FlyingSlash: return Mathf.Floor(25f - (_playerStats.stats.Skills[6].Level * 0.5f)); // переделать
        }
        return 0;
    }
    private bool CheckTiming(PerkType perkType)
    {
        if (perkType == _perkType)
        {
            if (_timer >= _timeForReUse && PerkTimeReUse(perkType) >= 1)
            {
                _timer = 0;
                return true;
            }
            else return false;
        }
        else return false;

    }

    public void Click()
    {
        if (_buttonObject.activeSelf)
        {
            if (CheckTiming(PerkType.Jerc)) _playerController.SetJerk();
            if (CheckTiming(PerkType.FlyingSlash)) _playerController.SetFlyingSlash();
        }
    }
}
