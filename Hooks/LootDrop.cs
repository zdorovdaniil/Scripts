using UnityEngine;
using System.Collections.Generic;

public class LootDrop : MonoBehaviour
{
    public bool isDroped = false;
    [SerializeField] private GameObject PrefDrop;
    [SerializeField] private List<Item> _itemsInLootDrop = new List<Item>();

    [SerializeField] private bool _isDependOnDungeonLevel = true;
    [SerializeField] private int _valueOneDrop;
    [SerializeField] private int _valueMinDrop;
    [SerializeField] private int _valueMaxDrop;
    [SerializeField] private float _multiplierPerLvl = 0.25f;

    public void CreateDrop()
    {
        if (!isDroped)
        {
            int countSpawnItems = 0;
            int countWillSpawn;
            int dungeonLevel;
            dungeonLevel = _isDependOnDungeonLevel ? GameManager.Instance.GetDungeonLevel : 1;
            int valueMaxDrop = Mathf.FloorToInt(_multiplierPerLvl * 100 * dungeonLevel) + _valueMaxDrop - 25;
            int totalDrop = Random.Range(_valueMinDrop, valueMaxDrop);
            countWillSpawn = Mathf.FloorToInt(totalDrop / _valueOneDrop);
            if (countWillSpawn <= 0) return;
            Enemy enemy = GetComponent<EnemyStats>().enemyTupe;
            GameObject drop = GameManager.SpawnDrop(transform, PrefDrop);
            CollectItems collectItems = drop.GetComponent<CollectItems>();

            for (int i = 0; i < countWillSpawn; i++)
            {
                Item item = enemy ? enemy.GetRandomItemInLoot() : GetRandomItemInLootDrop();
                collectItems.AddItem(item);
                countSpawnItems += 1;
            }
            Debug.Log("Cost: " + _valueOneDrop + ". Random: (" + _valueMinDrop + ":" + valueMaxDrop + "). Value:" + totalDrop + ". CountDroped: " + countWillSpawn);
        }
        else Debug.Log("Drop has already happened");
    }


    public Item GetRandomItemInLootDrop() { int num = Random.Range(0, _itemsInLootDrop.Count - 1); return _itemsInLootDrop[num]; }
}
