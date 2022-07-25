using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "Item", menuName = "Project/Item", order = 0)]
[System.Serializable]
public class Item : ScriptableObject
{
    public int Id;
    public Sprite Icon;
    public string Name;
    public int MaxSizeSlot = 1;
    public int Level = 1;
    public int Attack = 0;
    public int Armor = 0;
    public int cost = 0;
    [Multiline(2)] public string Features;
    [Space]
    [SerializeField] ItemTupe m_ItemTupe = ItemTupe.Nothing;
    public ItemTupe itemTupe { get { return m_ItemTupe; } set { m_ItemTupe = value; } }

    [SerializeField] ArmorTupe m_ArmorTupe = ArmorTupe.Nothing;
    public ArmorTupe armorTupe { get { return m_ArmorTupe; } set { m_ArmorTupe = value; } }
    [Space]
    public GameObject PrefabItem;

    [Header("Buff And Improve")]
    public List<BuffClass> Buffs;
    public ImproveItem Improve = ImproveItem.Nothing;
    [Header("Item Requirements")]
    public int neededAttr = 0;
    public int neededSkill = 0;
    public int neededLevel = 0;

    public string nameAttr;
    public string nameSkill;
    [Header("Craft")]
    [SerializeField] private List<Item> _itemsForCraft = new List<Item>();
    public List<Item> GetItemsCraft => _itemsForCraft;
    [SerializeField] private int _craftDifficult; public int GetCraftDifficult => _craftDifficult;

    public static bool CheckReqirement(Item _item, PlayerStats playerStats)
    {
        if (_item.itemTupe == ItemTupe.Weapon && _item.neededAttr <= playerStats.stats.Strenght &&
            _item.neededSkill <= playerStats.stats.Skills[6].Level && _item.neededLevel <= playerStats.stats.Level)
        { return true; }
        else if (_item.itemTupe == ItemTupe.Armor
            && _item.neededSkill <= playerStats.stats.Skills[5].Level &&
            _item.neededAttr <= playerStats.stats.Strenght && _item.neededLevel <= playerStats.stats.Level)
        { return true; }
        else return false;
    }
    public static bool UsingItem(PlayerStats playerStats, InventorySlot slot, Inventory inv, ClothAdder clothAdder, PlayerUpdate playerUpdate)
    {
        bool SelectArmor(InventorySlot _slot, int numE, int numI)
        {
            if (inv.GetEquipSlot(numE) != null) { MsgBoxUI.Instance.ShowAttention("armor slot busy"); return false; }
            else if (CheckReqirement(_slot.item, playerStats))
            {
                inv.SetToEquipSlot(_slot, numE);
                inv.DeleteItemId(_slot.item.Id, 1);
                inv.UpdateClothVisible();
                LogUI.Instance.Loger("Equip " + _slot.item.armorTupe);
                return true;
            }
            else { MsgBoxUI.Instance.ShowAttention("the item does not match the characteristics of the player"); return false; }
        }

        if (slot.item.itemTupe == ItemTupe.Weapon)
        {
            if (inv.GetEquipSlot(0) != null) { MsgBoxUI.Instance.ShowInfo("weapon", "weapon not requiment player stats"); return false; }
            else if (CheckReqirement(slot.item, playerStats))
            {
                inv.SetToEquipSlot(slot, 0);
                inv.DeleteItemId(slot.item.Id, 1);
                inv.UpdateClothVisible();
                LogUI.Instance.Loger("Equip weapon");
                return true;
            }
            else { Debug.Log("not requiment player stats"); return false; }
        }
        else if (slot.item.itemTupe == ItemTupe.Armor)
        {
            if (CheckReqirement(slot.item, playerStats))
            {
                if (slot.item.armorTupe == ArmorTupe.Helmet) { if (SelectArmor(slot, 1, 25)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Tors) { if (SelectArmor(slot, 2, 26)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Pants) { if (SelectArmor(slot, 3, 27)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Shoes) { if (SelectArmor(slot, 4, 28)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Amulet) { if (SelectArmor(slot, 5, 29)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Ring) { if (SelectArmor(slot, 6, 30)) { return true; } else { return false; } }
            }
            else return false;
        }
        else if (slot.item.itemTupe == ItemTupe.Poison || slot.item.itemTupe == ItemTupe.Usable || slot.item.itemTupe == ItemTupe.Food)
        {
            inv.DeleteItemId(slot.item.Id, 1);
            foreach (BuffClass buffClass in slot.item.Buffs)
            {
                if (buffClass.Buff == Buff.Healing) { playerUpdate.UseHealPoison(slot.item); }
                if (buffClass.Buff == Buff.Heal) { playerStats.AddHP(buffClass.Value); }
                playerStats.stats.AddBuff(buffClass); playerStats.ChangeSpeed();
            }
            return true;
        }
        else if (slot.item.itemTupe == ItemTupe.Improve)
        {
            inv.DeleteItemId(slot.item.Id, 1);
            switch (slot.item.Improve)
            {
                case ImproveItem.Strenght: playerStats.UpAttribute(0, false); break;
                case ImproveItem.Agility: playerStats.UpAttribute(1, false); break;
                case ImproveItem.Endurance: playerStats.UpAttribute(2, false); break;
                case ImproveItem.Speed: playerStats.UpAttribute(3, false); break;
                case ImproveItem.Detection: playerStats.stats.Skills[0].LevelUp(); break;
                case ImproveItem.Leap: playerStats.stats.Skills[1].LevelUp(); break;
                case ImproveItem.Medicine: playerStats.stats.Skills[2].LevelUp(); break;
            }
            return true;
        }
        else { return false; }
        return false;
    }
}


public enum ArmorTupe
{
    Nothing, Helmet, Tors, Pants, Shoes, Amulet, Ring,
}
public enum ItemTupe
{
    Nothing,
    Food, // еда для восстановления HP
    Armor, // броня (шлемы,корпус,штаны,обувь)
    Weapon, // оружие
    Poison, // зелья восстановления и временно повышающие характеристики
    Usable,
    Improve, // предметы что повышают на всегда характиристики персонажа
    MonsterDrop, // предметы выпадаемые из монстров. Используются для крафта и повышения уровня персонажа
    Legandary // легендарные предметы
}
public enum ImproveItem
{
    Nothing, Strenght, Agility, Endurance, Speed, Detection, Medicine, Leap
}








