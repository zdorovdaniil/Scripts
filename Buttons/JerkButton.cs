using UnityEngine;
using TMPro;

public class JerkButton : MonoBehaviour
{
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private PlayerStats _playerStats;
    [SerializeField] private GameObject _buttonObject;
    [SerializeField] private TMP_Text _timerTMP;

    private float _timer = 0;
    private float _timeForReUse;
    private void Start()
    {
        _buttonObject.SetActive(false);
    }

    public void UpdateButton(int jerkSkill)
    {
        if (jerkSkill > 0)
        {
            _timer += 0.25f;
            _buttonObject.SetActive(true);
            _timeForReUse = Mathf.Floor(8f - (0.5f * jerkSkill));
            if (_timer <= _timeForReUse)
            {
                _timerTMP.text = (_timeForReUse - _timer).ToString();
            }
            else _timerTMP.text = "";
        }
    }
    public void ClickJerk()
    {
        if (_timer >= _timeForReUse && _playerStats.stats.Skills[1].Level >= 1)
        {
            _timer = 0;
            _playerController.SetJerk();
        }
        else return;
    }
}
