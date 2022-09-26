using System.Collections.Generic;
using UnityEngine;

public class SlotsUI : MonoBehaviour
{
    [SerializeField] private GameObject _prefabContainSlot;
    [SerializeField] private Transform _objContain;
    public List<Item> DefaultItemsInContainer = new List<Item>();
    [SerializeField] private ContainerType _containerType;
    [SerializeField] private Inventory _inv;
    [SerializeField] private FilterType _filterType;


    public void FullSlots(List<InventorySlot> invSlots)
    {
        ClearSlots();
        if (invSlots != null)
        {
            foreach (InventorySlot slot in invSlots)
            {
                if (slot != null)
                    Instantiate(_prefabContainSlot, _objContain).GetComponent<ContainSlot>().UpdateSlotInfo(slot, _inv, _containerType);
            }
        }
        else return;
    }
    public void ClearSlots()
    {
        for (int i = 0; i < _objContain.childCount; i++)
        {
            Destroy(_objContain.GetChild(i).gameObject);
        }
    }
    private List<InventorySlot> FilterInvList(FilterType filterType, List<InventorySlot> slots)
    {
        List<InventorySlot> filteredSlots = new List<InventorySlot>();
        if (filterType == FilterType.Level)
        {
            // находим максимум из листа
            int max = 0;
            foreach (InventorySlot slot in slots)
            {
                if (slot.item.Level >= max)
                { max = slot.item.Level; }
            }
            for (int i = 0; i < max + 1; i++)
            {
                foreach (InventorySlot slot in slots)
                {
                    if (slot.item.Level == i)
                        filteredSlots.Add(slot);
                }
            }
        }
        else filteredSlots = slots;
        return filteredSlots;
    }

}
public enum ContainerType
{
    None,
    Crafting,
    CraftingInfo,
    Shop,
    ShopInfo,
    Storage,
    StorageInfo,
    Equip,
    Inventory,
    Leveling,
    LevelingInfo,
    Chest,
    ChestInfo,
    Info
}
public enum FilterType
{
    None,
    ItemType,
    Level,
    Cost,
}