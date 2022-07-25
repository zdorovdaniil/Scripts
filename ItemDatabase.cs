using System.Collections.Generic;
using UnityEngine;
[CreateAssetMenu]
public class ItemDatabase : ScriptableObject
{
    public List<Item> Items = new List<Item>();
    public List<Item> GetListItems => Items;

    // Получение предмета из БД по заданному ID
    public Item GetItem(int itemID)
    {
        foreach (var item in Items)
        {
            if (item != null && item.Id == itemID) return item;

        }
        return null;
    }
    // Получение из БД предметов заданного типа
    public List<Item> GetListItemsByType(ItemTupe type)
    {
        List<Item> newList = new List<Item>();
        foreach (Item item in Items)
        {
            if (item.itemTupe == type) newList.Add(item);
        }
        return newList;
    }
}