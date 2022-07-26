using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class JerkButton : MonoBehaviour
{
    public static JerkButton Instance; private void Awake() { Instance = this; }
    [SerializeField] private PlayerController _playerController;
    [SerializeField] private GameObject _buttonObject;
    [SerializeField] private TMP_Text _timer;

    private float _timer;
    public void Update(PlayerStats playerStats)
    {
        _timer += 0.2f;
        _buttonObject.SetActive(true);
        int curJerkLevel = playerStats.stats.Skills[1].Level;
        float time = 7.5f - (0.5 * curJerkLevel);

        if (_timer <= time)
        {
            _timer.text = (time - _timer).ToString();
        }
        else return;
    }
    public void Click()
    {
        if (_timer >= time)
        {
            _timer = 0;
            _playerController.SetJerk();
        }
        else return;

    }


}
