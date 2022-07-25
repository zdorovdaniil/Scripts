using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class CraftingUI : MonoBehaviour
{
    public static CraftingUI instance;
    [SerializeField] private ContainSlot _selectedContainSlot;

    [SerializeField] private List<Item> _defaultAvaibleCrafts = new List<Item>();
    [SerializeField] private SlotsUI _inlockedItemsToCraft;
    [SerializeField] private List<ImproveUI> _improves;
    [SerializeField] private SlotsUI _secondSlotsUI;
    [SerializeField] private Inventory _inv;

    private InventorySlot SelectedSlot;
    private List<Item> craftItems = new List<Item>();
    private List<InventorySlot> collection = new List<InventorySlot>();

    [SerializeField] private Transform _buttonCraft;
    [SerializeField] private TextMeshProUGUI _nameSelectedSlot;
    [SerializeField] private TextMeshProUGUI _curCraftSkillText;
    [SerializeField] private TextMeshProUGUI _craftDifficultText;
    [SerializeField] private TextMeshProUGUI _chanceCraftText;
    private PlayerStats _playerStats;
    private int _craftSkill;
    private int _chanceToCraft;
    private int craftDifficult;
    private void Start()
    {
        instance = this;
        _buttonCraft.gameObject.SetActive(false);
    }
    public void ClearCraftingUI()
    {
        _secondSlotsUI.ClearSlots();
        _selectedContainSlot.ClearClot();
    }
    public void LoadCraftingUI()
    {
        _craftSkill = _playerStats.stats.Skills[4].Level;
        _curCraftSkillText.text = _craftSkill.ToString();
        List<Item> items = new List<Item>(_defaultAvaibleCrafts);
        foreach (ImproveUI improve in _improves)
        {
            improve.SetUI();
            if (improve.Improve.GetCurLvl > 1)
            {
                items.AddRange(improve.Improve.GetUnlockItems);
            }
        }
        _inlockedItemsToCraft.FullSlots(InventorySlot.CreateListInvSlots(items));
    }
    public void FillSecondUI(InventorySlot _Slot)
    {
        craftDifficult = _Slot.item.GetCraftDifficult;
        _chanceToCraft = ProcessCommand.ChanceToCraft(craftDifficult, _craftSkill);

        _craftDifficultText.text = craftDifficult.ToString();
        _chanceCraftText.text = _chanceToCraft.ToString();

        _buttonCraft.gameObject.SetActive(false);
        _selectedContainSlot.UpdateSlotInfo(_Slot, null, ContainerType.None);
        SelectedSlot = _Slot;
        _nameSelectedSlot.text = SelectedSlot.item.Name.ToString();
        craftItems = SelectedSlot.item.GetItemsCraft;
        collection = InventorySlot.CreateListInvSlots(craftItems);
        int count = 0;
        for (int i = 0; i < collection.Count; i++)
        {
            int haveCount = _inv.GetCountItemsWithId(collection[i].item.Id);
            int needCount = collection[i].amount;
            if (haveCount >= needCount)
            { count += 1; }
        }
        if (count >= collection.Count)
        {
            _buttonCraft.gameObject.SetActive(true);
        }
        _secondSlotsUI.FullSlots(collection);
    }

    public void CraftSelectedSlot()
    {
        if (!_inv.IsFull())
        {
            if (TryToCraftItem())
            {
                _inv.AddItems(SelectedSlot.item);
                MsgBoxUI.Instance.ShowInfo("info", "item created successfully");
            }
            else
            {
                MsgBoxUI.Instance.ShowInfo("info", "unsuccessful craft. Items used for craft are lost");
            }
            DeletingItemsInInv();
            _inv.SaveItemsId();
            ClearCraftingUI();
            FillSecondUI(SelectedSlot);
        }
        else
        {
            MsgBoxUI.Instance.ShowInfo("info", "inventory is full");
        }
    }
    private bool TryToCraftItem()
    {
        int random = Random.Range(0, 100);
        if (random < _chanceToCraft) return true;
        else return false;
    }
    private void DeletingItemsInInv()
    {
        foreach (InventorySlot slot in collection)
        {
            _inv.DeleteItemId(slot.item.Id, slot.amount);
        }
    }
    public void SetPlayerStats(PlayerStats playerStats)
    {
        _playerStats = playerStats;
    }
}
