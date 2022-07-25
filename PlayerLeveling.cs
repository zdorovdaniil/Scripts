using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class PlayerLeveling : MonoBehaviour
{
    public static PlayerLeveling instance;
    [SerializeField] private SlotsUI _slotsLeveling;
    [SerializeField] private Inventory _inv;
    [SerializeField] private PlayerStats _plStats;
    [Header("Player Leveling Requirements")]
    public int[] LevelScore;
    public List<Item> ItemsFor3Level = new List<Item>();
    public List<Item> ItemsFor5Level = new List<Item>();
    public List<Item> ItemsFor7Level = new List<Item>();
    public List<Item> ItemsFor9Level = new List<Item>();
    [SerializeField] private Transform _buttonLevelUp;
    [SerializeField] private List<InventorySlot> collection = new List<InventorySlot>();
    [SerializeField] private TMP_Text _levelingText;

    private void Awake() {instance = this;}

    public void FillLevelingUI()
    {
        collection.Clear();
        _slotsLeveling.ClearSlots();
        int curEXP = _plStats.curEXP;
        int curLvl = _plStats.stats.Level;
        bool isExpFull = false;
        if (curEXP >= LevelScore[curLvl-1]) {isExpFull = true;}
        switch (curLvl)
        {
            case 2: { collection = InventorySlot.CreateListInvSlots(ItemsFor3Level); break;}
            case 4: { collection = InventorySlot.CreateListInvSlots(ItemsFor5Level); break;}
            case 6: { collection = InventorySlot.CreateListInvSlots(ItemsFor7Level); break;}
            case 8: { collection = InventorySlot.CreateListInvSlots(ItemsFor9Level); break;}
        }
        if (collection != null) 
        {
            _slotsLeveling.FullSlots(collection);
            if (isExpFull && _inv.CheckContainItemsInCollection(collection))
            {_buttonLevelUp.gameObject.SetActive(true);_levelingText.text = "";}
            else {_buttonLevelUp.gameObject.SetActive(false); _levelingText.text = "not enought exp or items";}
        }
        else
        {
            if (isExpFull){_buttonLevelUp.gameObject.SetActive(true);}
            else {_buttonLevelUp.gameObject.SetActive(false);}
        }
    }
    public int GetHeedExp(int level)
    { return LevelScore[level-1];}
    public void ClickLevelUp()
    {
        FillLevelingUI();
        if (_buttonLevelUp.gameObject.activeSelf)
        {
        _plStats.PlayerLevelUp();
        if (collection != null) 
        { foreach (InventorySlot slot in collection) 
        {_inv.DeleteItemId(slot.item.Id,slot.amount);}}
        collection.Clear();
        FillLevelingUI();
        PlayerUI.Instance.FillPlayerUI();
        }
    }
}
