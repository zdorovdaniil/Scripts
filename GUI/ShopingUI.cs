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
    [SerializeField] private List<ImproveUI> _improves;

    [SerializeField] private TextMeshProUGUI _buyModif;
    [SerializeField] private TextMeshProUGUI _sellModif;
    [SerializeField] private TextMeshProUGUI _tradeSkill;
    void Start() { instance = this; }
    public void FillShopSlotsUI()
    {
        List<InventorySlot> shopCatalog = new List<InventorySlot>(InventorySlot.CreateListInvSlots(_defaultitemsInShop));
        foreach (ImproveUI improveUI in _improves)
        {
            improveUI.SetUI();
            if (improveUI.Improve.GetMaxLvl <= improveUI.Improve.GetCurLvl)
            {
                foreach (Item item in improveUI.Improve.GetUnlockItems)
                {
                    InventorySlot itemShop = new InventorySlot(item, 1);
                    shopCatalog.Add(itemShop);
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

    public void SellSlot(Item item, int amount)
    {
        _inv.DeleteItemId(item.Id, amount);
        _inv.SaveItemsId();
        _invSlotsUI.FullSlots(_inv.GetItems);
        int moneyNow = PropertyUI.instance.GetCoins;
        int sellCost = Mathf.FloorToInt(item.cost * _playerStats.stats.GetSaleModif());
        PropertyUI.instance.SetCoins(moneyNow + (sellCost * amount));
        PropertyUI.instance.UpdateProperty();
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
        PropertyUI.instance.UpdateProperty();
    }
    public void SpawnReferenceGUI(InventorySlot slot, ReferenceButtonType buttonType)
    {
        ProcessCommand.ClearChildObj(_spawnReferenceGUI);
        ReferenceUI reference = Instantiate(_prefReferenceGUI, _spawnReferenceGUI).GetComponent<ReferenceUI>();
        reference.SetPlayerStats(_playerStats);
        reference.SetValueSlot(slot, buttonType);
    }
}
