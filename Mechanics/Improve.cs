using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт отвечает за создание хранения множества Improve открывающих различные предметы или увеличивает вместимость склада
/// </summary>
[CreateAssetMenu(fileName = "Improve", menuName = "Project/Improve", order = 5)]
public class Improve : ScriptableObject
{
    [SerializeField] private int _id;
    [SerializeField] private int _maxLvl = 2; public int GetMaxLvl => _maxLvl;
    [SerializeField] private int _curLvl = 1; public int GetCurLvl => _curLvl;
    [SerializeField] private int _startCost = 1000;
    [SerializeField] private float _costModified = 1.25f;
    [SerializeField] private TextLocalize _description; public string GetDescription => _description.Text();
    [SerializeField] private List<Item> _unlockItems = new List<Item>();
    public List<Item> GetUnlockItems => _unlockItems;

    //
    [SerializeField] private List<int> _lvlValues = new List<int>();
    public List<int> GetLvlValues => _lvlValues;
    public bool IsLvlUp()
    {
        if (_maxLvl <= _curLvl) return false;
        else return true;
    }
    public void LvlUp()
    {
        _curLvl += 1;
        SaveImprove();
    }
    public int GetCurCost()
    {
        return Mathf.FloorToInt(_startCost + (_curLvl * _costModified * 100));
    }
    public void LoadImprove()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        if (PlayerPrefs.HasKey(id + "_slot_" + _id + "_improveLvl"))
        {
            _curLvl = PlayerPrefs.GetInt(id + "_slot_" + _id + "_improveLvl");
        }
        else { _curLvl = 1; }
    }
    public void SaveImprove()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_" + _id + "_improveLvl", _curLvl);
    }
}
