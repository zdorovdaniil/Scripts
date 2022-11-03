using System.Collections.Generic;
using UnityEngine;

// скрипт задает условия и воспроизводит действия при заходе и выхода в подземелье
// а также содержит в себе информацию для создания подземелья 
[CreateAssetMenu(fileName = "DungeonRules", menuName = "Project/DungeonRules", order = 4)]
public class DungeonRules : ScriptableObject
{
    [SerializeField] private List<Item> _itemsDeleteOnExiting; public List<Item> GetDeletingItems => _itemsDeleteOnExiting;
    [SerializeField] private int[] _numRoomsDependDunLevel = { 30, 40, 50, 60, 70 }; public int NumRoomsFromLevel(int dunLevel) => _numRoomsDependDunLevel[dunLevel - 1];
    [SerializeField] private int[] _additionalQuestsPerLevel = { 4, 5, 6, 7, 8 }; public int AddQuestFromLevel(int dunLevel) => _additionalQuestsPerLevel[dunLevel - 1];
    [SerializeField] private int[,] _enemyesLevel = { { 1, 1, 2, 3, 3 }, { 1, 2, 3, 4, 5 } };
    public int[] EnemyLevelRangeFromLevel(int dunLevel) { dunLevel -= 1; int[] vector = new int[2]; vector[0] = _enemyesLevel[0, dunLevel]; vector[1] = _enemyesLevel[1, dunLevel]; return vector; }
    public int GetModifSpawnItemsChest(int dunLevel)
    {
        return Mathf.FloorToInt((dunLevel * 5) + 45); ;
    }
    public int EnemyLevel(int dunLevel)
    {
        dunLevel -= 1;
        // +1 Обязательно, так как последний аргумент Random не эключает это число
        //int level = Random.Range(_enemyesLevel[dunLevel, 0], _enemyesLevel[dunLevel, 1] + 1);
        int level = Random.Range(_enemyesLevel[0, dunLevel], _enemyesLevel[1, dunLevel] + 1);
        Debug.Log("Generate Level " + level + " beetween " + _enemyesLevel[0, dunLevel] + " / " + _enemyesLevel[1, dunLevel]);
        return level;
    }

}
