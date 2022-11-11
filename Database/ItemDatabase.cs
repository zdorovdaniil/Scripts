using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemDatabase", menuName = "Project/ItemDatabase", order = 4)]
public class ItemDatabase : ScriptableObject
{
    public class ItemsOfType
    {
        private List<Item> Items = new List<Item>(); public List<Item> ListItems => Items;
        public int Count => Items.Count;
        public void Add(Item item) => Items.Add(item);
    }
    public List<Item> Items = new List<Item>();
    public List<ItemsOfType> ItemsType = new List<ItemsOfType>();
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
            Debug.Log("add to type: " + numType + " / " + "now count: " + ItemsType[numType].Count);

        }
    }

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
        int numType = (int)type;
        List<Item> newList = ItemsType[numType].ListItems;
        return newList;

        /*
        List<Item> newList = new List<Item>();
        foreach (Item item in Items)
        {
            if (item.itemTupe == type) newList.Add(item);
        }
        return newList;
        */
    }

}