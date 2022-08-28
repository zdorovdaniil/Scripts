using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// скрипт задает условия и воспроизводит действия при заходе и выхода в подземелье
[CreateAssetMenu(fileName = "DungeonRules", menuName = "Project/DungeonRules", order = 4)]
[System.Serializable]
public class DungeonRules : ScriptableObject
{
    [SerializeField] private List<Item> _itemsDeleteOnExiting; public List<Item> GetDeletingItems => _itemsDeleteOnExiting;

}
