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


    [SerializeField] private int _strenght; public int Strenght(int level = 1) => CountAttribute(_strenght, level);
    [SerializeField] private int _agility; public int Agility(int level = 1) => CountAttribute(_agility, level);
    [SerializeField] private int _endurance; public int Endurance(int level = 1) => CountAttribute(_endurance, level);
    [SerializeField] private int _speed; public int Speed(int level = 1) => CountAttribute(_speed, level);
    [SerializeField] private int _attackWeapon; public int AttackWeapon(int level = 1) => CountAttribute(_attackWeapon, level);
    [SerializeField] private int _armorEquip; public int ArmorClass(int level = 1) => CountAttribute(_armorEquip, level);
    [SerializeField] private int _critChance;
    [SerializeField] private int _critValue;



    private int CountAttribute(int attrubute, int level)
    {
        if (level > 1)
        {
            return Mathf.FloorToInt(((_multiplierPerLvl * level) + 1) * attrubute);
        }
        else return attrubute;
    }
    public int[] GetCritForStats(int level)
    {
        int[] mass = new int[] { 2, 100 };
        if (level > 1)
        {
            for (int i = 0; i < mass.Length; i++)
            { mass[i] = mass[i] * level; }
            return mass;
        }
        else return mass;
    }
    // промежуток времени, через которое противник после получения урона может снова получить урон
    public float timeResetHit = 1f;
    // множитель к характеристикам за уровень
    [SerializeField] private float _multiplierPerLvl = 0.2f;

    [Multiline(2)] public string Features;
    public GameObject PrefabEnemy;
    public bool IsBoss;

    [SerializeField] private List<Item> RandomObjectsDrop = new List<Item>();
    public List<Item> GetRandomLoot => RandomObjectsDrop;
    public Item GetRandomItemInLoot() { int num = Random.Range(0, RandomObjectsDrop.Count - 1); return RandomObjectsDrop[num]; }
    [SerializeField] private List<Item> AlwaysObjectsDrop = new List<Item>();
    public List<Item> GetAlwaysLoot => AlwaysObjectsDrop;
    [SerializeField] private int CoinsDrop;
    public int GetCoinsDrop => CoinsDrop;

}
public enum EnemyFightingType
{
    Melee, // противник ближнего боя
    Distance, // противник дальнего боя
    DistanceCrosser // противник дальнего боя, при приближении игрока к нему телепортируется в случайное место комнаты
}
