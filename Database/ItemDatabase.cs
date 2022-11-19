using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Скрипт при старте игры сразу сотритует предметы по типу
/// </summary>
[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Project/ItemDatabase", order = 4)]
public class ItemDatabase : ScriptableObject
{
    public List<Item> Items = new List<Item>();
    public List<ItemsOfType> ItemsType = new List<ItemsOfType>();
    [SerializeField] private int _lastId;
    [SerializeField] private int _identicalId;
    public void Initialize()
    {
        foreach (Item item in Items)
        {
            for (int i = 0; i < 8; i++)
            {
                ItemsOfType itemsOfTypes = new ItemsOfType();
                ItemsType.Add(itemsOfTypes);
            }
            int numType = (int)item.itemTupe;
            ItemsType[numType].Add(item);
            //Debug.Log("add to type: " + numType + " / " + "now count: " + ItemsType[numType].Count);
        }

        // код ниже находит одинаковые id в списке предметов, и выводин id что встречается 2 раза
#if UNITY_EDITOR
        int lastId = 0;
        int[] vectorIds = new int[Items.Count];
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Id >= lastId) lastId = Items[i].Id; ;
            vectorIds[i] = Items[i].Id;
        }
        foreach (Item item in Items)
        {
            if (CountMatches(item.Id) >= 2) _identicalId = item.Id;
        }
        _lastId = lastId;
        Debug.Log("ExeCode 1");

#endif
        int CountMatches(int Id)
        {
            int numMatches = 0;
            for (int i = 0; i < vectorIds.Length; i++)
            {
                if (Id == vectorIds[i]) numMatches += 1;
            }
            return numMatches;
        }
    }
    public List<Item> GetListItemsByType(ItemTupe type, int[] levelRange = null, bool forChest = false)
    {
        int numType = (int)type;
        List<Item> list = ItemsType[numType].ListItems;
        if (levelRange == null) return list;
        else
        {
            List<Item> newList = new List<Item>();
            foreach (Item item in list)
            {
                if (item.Level >= levelRange[0] && item.Level <= levelRange[1])
                {
                    if (forChest)
                    {
                        if (item.IsSpawnInChest) newList.Add(item);
                        else continue;
                    }
                    else newList.Add(item);
                }
                else continue;
            }
            return newList;
        }
    }
    public Item GetItem(int itemID)
    {
        foreach (var item in Items) { if (item != null && item.Id == itemID) return item; }
        return null;
    }
}
public class ItemsOfType
{
    private List<Item> Items = new List<Item>(); public List<Item> ListItems => Items;
    public int Count => Items.Count;
    public void Add(Item item) => Items.Add(item);
}