using UnityEngine;
using System.Collections.Generic;
using TMPro;

public class ShopingUI : MonoBehaviour
{
    public static ShopingUI instance;
    private PlayerStats _playerStats;
    public void SetPlayerStats(PlayerStats playerStats) { _playerStats = playerStats; }
    [SerializeField] private List<Item> _defaultitemsInShop;
    [SerializeField] private Inventory _inv;
    [SerializeField] private Transform _spawnReferenceGUI;
    [SerializeField] private GameObject _prefReferenceGUI;
    [SerializeField] private SlotsUI _invSlotsUI;
    [SerializeField] private SlotsUI _shopSlotsUI;
    [SerializeField] private List<Improve> _improves;

    [SerializeField] private TextMeshProUGUI _buyModif;
    [SerializeField] private TextMeshProUGUI _sellModif;
    [SerializeField] private TextMeshProUGUI _tradeSkill;
    private int _curShopType = 0;
    void Start() { instance = this; }
    public void FillShopSlotsUI()
    {
        List<InventorySlot> shopCatalog = new List<InventorySlot>();
        foreach (Improve improve in _improves)
        {
            if (improve.GetMaxLvl <= improve.GetCurLvl)
            {
                foreach (Item item in _defaultitemsInShop)
                {
                    InventorySlot itemShop = new InventorySlot(item, 1);
                    if (CheckItemToShopCatalog(itemShop)) shopCatalog.Add(itemShop); else { continue; }
                }
                foreach (Item item in improve.GetUnlockItems)
                {
                    InventorySlot itemShop = new InventorySlot(item, 1);
                    if (CheckItemToShopCatalog(itemShop)) shopCatalog.Add(itemShop); else { continue; }
                }
            }
            else continue;
        }
        _invSlotsUI.FullSlots(_inv.GetItems);
        _shopSlotsUI.FullSlots(shopCatalog);
        _buyModif.text = _playerStats.stats.GetBuyModif().ToString();
        _sellModif.text = _playerStats.stats.GetSaleModif().ToString();
        _tradeSkill.text = _playerStats.stats.Skills[3].Level.ToString();
    }
    private bool CheckItemToShopCatalog(InventorySlot slot)
    {
        switch (_curShopType)
        {
            case 0: if (checkType(ItemTupe.Weapon) || checkType(ItemTupe.Armor)) { return true; } else { return false; }
            case 1: if (checkType(ItemTupe.Poison)) { return true; } else { return false; }
            case 2: if (checkType(ItemTupe.Food) || checkType(ItemTupe.Usable) || checkType(ItemTupe.Nothing)) { return true; } else { return false; }
            case 3: if (checkType(ItemTupe.MonsterDrop)) { return true; } else { return false; }
        }
        bool checkType(ItemTupe type)
        {
            if (slot.item.itemTupe == type) { return true; } else return false;
        }
        return true;
    }

    public void SetShopType(int value)
    {
        _curShopType = value;
        FillShopSlotsUI();
    }
    public void SellSlot(InventorySlot slot, int amount)
    {
        _inv.DeleteSlot(slot, amount);
        _inv.SaveItemsId();
        _invSlotsUI.FullSlots(_inv.GetItems);
        int moneyNow = PropertyUI.instance.GetCoins;
        int sellCost = Mathf.FloorToInt(slot.item.cost * _playerStats.stats.GetSaleModif());
        PropertyUI.instance.SetCoins(moneyNow + (sellCost * amount));
        Debug.Log("Sell: " + sellCost);
    }
    public void BuyItem(Item item)
    {
        float moneyNow = PropertyUI.instance.GetCoins;
        float buyCost = 0;
        buyCost = (item.cost * _playerStats.stats.GetBuyModif());
        Debug.Log("BuyCost: " + buyCost);
        if (moneyNow >= buyCost)
        {
            bool isAddedToInv = _inv.AddItems(item, 1);
            if (isAddedToInv)
            {
                moneyNow -= buyCost;
                PropertyUI.instance.SetCoins(Mathf.FloorToInt(moneyNow));
                _inv.SaveItemsId();
                _invSlotsUI.FullSlots(_inv.GetItems);
                GlobalSounds.Instance.SBuySell();
            }
            else { MsgBoxUI.Instance.ShowAttention("Inventory full"); }
        }
        else { MsgBoxUI.Instance.ShowAttention("Not enought money"); }
    }
    public void SpawnReferenceGUI(InventorySlot slot, ReferenceButtonType buttonType)
    {
        ProcessCommand.ClearChildObj(_spawnReferenceGUI);
        ReferenceUI reference = Instantiate(_prefReferenceGUI, _spawnReferenceGUI).GetComponent<ReferenceUI>();
        reference.SetPlayerStats(_playerStats);
        reference.SetValueSlot(slot, buttonType);
    }
}
