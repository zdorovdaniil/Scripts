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
    public int CritChance = 0;
    public int CritValue = 0;
    public int Attack = 0;
    public int Armor = 0;
    public int cost = 0;
    [SerializeField] private TextLocalize _descriptionText; public string Description() { return _descriptionText ? _descriptionText.Text() : "description id NULL"; }
    [Space]
    [SerializeField] ItemTupe m_ItemTupe = ItemTupe.Nothing;
    public ItemTupe itemTupe { get { return m_ItemTupe; } set { m_ItemTupe = value; } }

    [SerializeField] ArmorTupe m_ArmorTupe = ArmorTupe.Nothing;
    public ArmorTupe armorTupe { get { return m_ArmorTupe; } set { m_ArmorTupe = value; } }
    public bool IsSpawnInChest = true;
    [Space]
    public GameObject PrefabItem;

    [Header("Buff And Improve")]
    public List<BuffClass> Buffs;
    public ImproveItem Improve = ImproveItem.Nothing;
    [SerializeField] private ItemRequirement _itemRequirement; public ItemRequirement GetItemRequirement => _itemRequirement;
    [Header("Craft")]
    [SerializeField] private List<Item> _itemsForCraft = new List<Item>();
    public List<Item> GetItemsCraft => _itemsForCraft;
    [SerializeField] private int _craftDifficult; public int GetCraftDifficult => _craftDifficult;

    public bool CheckReqirement(PlayerStats playerStats)
    {
        return _itemRequirement.CheckReqirement(playerStats);
    }
    private void SoundOnUsing(Item item)
    {
        if (item.itemTupe == ItemTupe.Weapon) { GlobalSounds.Instance.SEquipWeapon(); }
        else if (item.itemTupe == ItemTupe.Armor)
        {
            if (item.armorTupe == ArmorTupe.Amulet) { GlobalSounds.Instance.SEquipAmulet(); }
            else if (item.armorTupe == ArmorTupe.Ring) { GlobalSounds.Instance.SEquipRing(); }
            else { GlobalSounds.Instance.SEquipArmor(); }
        }
        else if (item.itemTupe == ItemTupe.Poison) { GlobalSounds.Instance.SUsePoison(); }
        else if (item.itemTupe == ItemTupe.Food) { GlobalSounds.Instance.SUseFood(); }


    }
    public bool UsingItem(PlayerStats playerStats, InventorySlot slot, Inventory inv, PlayerUpdate playerUpdate)
    {
        SoundOnUsing(slot.item);
        bool SelectArmor(InventorySlot _slot, int numE, int numI)
        {
            if (inv.GetEquipSlot(numE) != null) { MsgBoxUI.Instance.ShowAttention("armor slot busy"); return false; }
            else
            {
                inv.SetToEquipSlot(_slot, numE);
                inv.DeleteSlot(_slot);
                inv.UpdateClothVisible();
                LogUI.Instance.Loger("Equip " + _slot.item.armorTupe);
                GlobalSounds.Instance.SEquipArmor();
                return true;
            }
        }
        if (slot.item.itemTupe == ItemTupe.Weapon)
        {
            if (inv.GetEquipSlot(0) != null) { MsgBoxUI.Instance.ShowInfo("weapon", "weapon slot are busy"); return false; }
            else if (_itemRequirement.CheckReqirement(playerStats))
            {
                inv.SetToEquipSlot(slot, 0);
                inv.DeleteSlot(slot);
                inv.UpdateClothVisible();
                LogUI.Instance.Loger("Equip weapon");
                return true;
            }
            else
            { MsgBoxUI.Instance.ShowAttention("weapon not requiment player stats"); return false; }
        }
        else if (slot.item.itemTupe == ItemTupe.Armor)
        {
            if (_itemRequirement.CheckReqirement(playerStats))
            {
                if (slot.item.armorTupe == ArmorTupe.Helmet) { if (SelectArmor(slot, 1, 25)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Tors) { if (SelectArmor(slot, 2, 26)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Pants) { if (SelectArmor(slot, 3, 27)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Shoes) { if (SelectArmor(slot, 4, 28)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Amulet) { if (SelectArmor(slot, 5, 29)) { return true; } else { return false; } }
                if (slot.item.armorTupe == ArmorTupe.Ring) { if (SelectArmor(slot, 6, 30)) { return true; } else { return false; } }
            }
            else { MsgBoxUI.Instance.ShowAttention("not requiment player stats"); return false; }
        }
        else if (slot.item.itemTupe == ItemTupe.Poison || slot.item.itemTupe == ItemTupe.Usable || slot.item.itemTupe == ItemTupe.Food)
        {
            inv.DeleteSlot(slot, 1);
            foreach (BuffClass buffClass in slot.item.Buffs)
            {
                if (buffClass.Buff == Buff.Healing) { playerUpdate.UseHealPoison(slot.item); }
                if (buffClass.Buff == Buff.Heal) { playerStats.AddHP(buffClass.Value); }
                playerStats.AddBuffPlayer(buffClass.BuffId);
            }
            return true;
        }
        else if (slot.item.itemTupe == ItemTupe.Improve)
        {
            if (inv) inv.DeleteSlot(slot, 1);
            switch (slot.item.Improve)
            {
                case ImproveItem.Strenght: playerStats.UpAttribute(0, false); break;
                case ImproveItem.Agility: playerStats.UpAttribute(1, false); break;
                case ImproveItem.Endurance: playerStats.UpAttribute(2, false); break;
                case ImproveItem.Speed: playerStats.UpAttribute(3, false); break;
                case ImproveItem.Detection: playerStats.stats.Skills[0].LevelUp(); break;
                case ImproveItem.Jerk: playerStats.stats.Skills[1].LevelUp(); break;
                case ImproveItem.Medicine: playerStats.stats.Skills[2].LevelUp(); break;
                case ImproveItem.Trade: playerStats.stats.Skills[3].LevelUp(); break;
                case ImproveItem.Craft: playerStats.stats.Skills[4].LevelUp(); break;
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
    Armor, // броня (шлемы,корпус,штаны,обувь,амулет,кольцо)
    Weapon, // оружие (мечи)
    Poison, // только зелья (восстановление HP и временно повышающие характеристики)
    Usable, // предметы что активируют скрипт события при использовании (открытие участка карты, баф на все подземелье)
    Improve, // предметы что повышают на всегда характиристики персонажа
    MonsterDrop, // предметы выпадаемые из монстров. Используются для крафта и повышения уровня персонажа
    Material, // предметы появляются в сундуках, и спавнятся в некоторых местах подземелья
    Jewel, // драгоценные камни и другие редкие предметы
    BookImprove, // книги, улучшающие скилл или атрибут
}
public enum ImproveItem
{
    Nothing, Strenght, Agility, Endurance, Speed, Detection, Jerk, Medicine, Trade, Craft,
}








