using UnityEngine;
using System.Collections.Generic;
[CreateAssetMenu(fileName = "Enemy", menuName = "Project/Enemy", order = 1)]
[System.Serializable]
public class Enemy : ScriptableObject
{
    public int Id;
    [SerializeField] EnemyFightingType m_EnemyType = EnemyFightingType.Melee;
    public EnemyFightingType enemyType { get { return m_EnemyType; } set { m_EnemyType = value; } }
    public Sprite Icon;
    public string Name;
    [Multiline(2)] public string Features;
    [Header("Stats")]
    [SerializeField] private float _multiplierPerLvl = 0.2f;
    [SerializeField] private int _coinsDrop; public int CoinsDrop(int level = 1) => CountMyltiplier(_coinsDrop, level);
    [SerializeField] private int _expGet; public int ExpGet(int level = 1) => CountMyltiplier(_expGet, level);
    [SerializeField] private int _strenght; public int Strenght(int level = 1) => CountAttribute(_strenght, level);
    [SerializeField] private int _agility; public int Agility(int level = 1) => CountAttribute(_agility, level);
    [SerializeField] private int _endurance; public int Endurance(int level = 1) => CountAttribute(_endurance, level);
    [SerializeField] private int _speed; public int Speed(int level = 1) => CountAttribute(_speed, level);
    [SerializeField] private int _attackWeapon; public int AttackWeapon(int level = 1) => CountAttribute(_attackWeapon, level);
    [SerializeField] private int _armorEquip; public int ArmorClass(int level = 1) => CountAttribute(_armorEquip, level);
    [SerializeField] private int _critChance = 2;
    [SerializeField] private int _critValue = 100;
    [SerializeField] private int _buffChance = 2;
    [Header("Fight")]
    // промежуток времени, через которое противник после получения урона может снова получить урон
    public float timeResetHit = 1f;
    // множитель к характеристикам, опыту за уничтожение за уровень
    [Header("Others")]
    public GameObject PrefabEnemy;
    public bool IsBoss;
    [SerializeField] private List<Item> RandomObjectsDrop = new List<Item>();
    public Item GetRandomItemInLoot => RandomObjectsDrop[Random.Range(0, RandomObjectsDrop.Count - 1)];
    [SerializeField] private List<Item> AlwaysObjectsDrop = new List<Item>(); public List<Item> GetAlwaysLoot => AlwaysObjectsDrop;
    [SerializeField] private List<BuffClass> RandomBuffAvaibles = new List<BuffClass>();
    public BuffClass GetRandomBuff => RandomBuffAvaibles[Random.Range(0, RandomBuffAvaibles.Count - 1)];

    private int CountAttribute(int attrubute, int level)
    {
        if (level > 1)
        {
            return Mathf.FloorToInt(((_multiplierPerLvl * level) + 1) * attrubute);
        }
        else return attrubute;
    }
    private int CountMyltiplier(int inputValue, int level)
    {
        return Mathf.FloorToInt(inputValue * _multiplierPerLvl * 100f);
    }
    public int[] GetCritForStats(int level)
    {
        int[] mass = new int[] { _critChance, _critValue };
        if (level > 1)
        {
            for (int i = 0; i < mass.Length; i++)
            { mass[i] = CountAttribute(mass[i], level); }
            return mass;
        }
        else return mass;
    }
    private int BaseMyltiplier(int inputValue, int level)
    {
        return Mathf.FloorToInt(inputValue * level);
    }
    public BuffClass IsStartBuff(int level)
    {
        int coeff = BaseMyltiplier(_buffChance, level);
        int random = ProcessCommand.RandomValue;
        Debug.Log(Name + "Buff chance: " + coeff.ToString() + " - " + random.ToString());
        if (coeff > random)
        {
            return GetRandomBuff;
        }
        else return null;
    }
}
public enum EnemyFightingType
{
    Melee, // противник ближнего боя
    Distance, // противник дальнего боя
    DistanceCrosser // противник дальнего боя, при приближении игрока к нему телепортируется в случайное место комнаты
}
