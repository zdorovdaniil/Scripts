using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerLeveling : MonoBehaviour
{
    public static PlayerLeveling Instance; private void Awake() { Instance = this; }

    private PlayerStats _plStats; public void SetPlayerStats(PlayerStats plS) => _plStats = plS;

    [Header("Player Leveling Requirements")]
    [SerializeField] private int _maxAvaibleLvl; public int MaxAvaibleLvl => _maxAvaibleLvl;
    public int[] LevelScore;
    public List<Item> ItemsFor3Level = new List<Item>();
    public List<Item> ItemsFor5Level = new List<Item>();
    public List<Item> ItemsFor7Level = new List<Item>();
    public List<Item> ItemsFor9Level = new List<Item>();
    public List<Item> ItemsFor11Level = new List<Item>();
    public List<Item> ItemsFor13Level = new List<Item>();
    public List<Item> ItemsFor15Level = new List<Item>();

    public List<InventorySlot> GetItemsForLeveling()
    {
        switch (_plStats.stats.Level)
        {
            case 2: { return InventorySlot.CreateListInvSlots(ItemsFor3Level); }
            case 4: { return InventorySlot.CreateListInvSlots(ItemsFor5Level); }
            case 6: { return InventorySlot.CreateListInvSlots(ItemsFor7Level); }
            case 8: { return InventorySlot.CreateListInvSlots(ItemsFor9Level); }
            case 10: { return InventorySlot.CreateListInvSlots(ItemsFor11Level); }
            case 12: { return InventorySlot.CreateListInvSlots(ItemsFor13Level); }
            case 14: { return InventorySlot.CreateListInvSlots(ItemsFor15Level); }
        }
        return null;
    }
    public bool IsMaxLevel()
    {
        if (_plStats.stats.Level >= _maxAvaibleLvl)
        { return true; }
        else return false;
    }
    public bool IsExpFull()
    {
        int curEXP = _plStats.curEXP;
        int curLvl = _plStats.stats.Level;
        if (curEXP >= LevelScore[curLvl - 1]) return true; else return false;
    }
    public int GetHeedExp(int level)
    { return LevelScore[level - 1]; }
    public void LevelUp()
    {
        _plStats.PlayerLevelUp();
    }

}
