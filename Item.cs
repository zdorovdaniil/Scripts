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
    [SerializeField] private ItemRequirement _itemRequirement; public ItemRequirement GetItemRequirement => _itemRequirement;
    [Header("Craft")]
    [SerializeField] private List<Item> _itemsForCraft = new List<Item>();
    public List<Item> GetItemsCraft => _itemsForCraft;
    [SerializeField] private int _craftDifficult; public int GetCraftDifficult => _craftDifficult;

    public bool CheckReqirement(PlayerStats playerStats)
    {
        return _itemRequirement.CheckReqirement(playerStats);
    }
    private void SoundOnEquip(Item item)
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
        bool SelectArmor(InventorySlot _slot, int numE, int numI)
        {
            if (inv.GetEquipSlot(numE) != null) { MsgBoxUI.Instance.ShowAttention("armor slot busy"); return false; }
            else
            {
                inv.SetToEquipSlot(_slot, numE);
                inv.DeleteItemId(_slot.item.Id, 1);
                inv.UpdateClothVisible();
                LogUI.Instance.Loger("Equip " + _slot.item.armorTupe);
                GlobalSounds.Instance.SEquipArmor();
                return true;
            }
        }
        if (slot.item.itemTupe == ItemTupe.Weapon)
        {
            if (inv.GetEquipSlot(0) != null) { MsgBoxUI.Instance.ShowInfo("weapon", "weapon not requiment player stats"); return false; }
            else if (_itemRequirement.CheckReqirement(playerStats))
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
            inv.DeleteItemId(slot.item.Id, 1);
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








