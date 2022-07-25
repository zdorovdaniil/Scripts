using System.Collections.Generic;
using UnityEngine;

public class DungeonObjects : MonoBehaviour
{
    public static DungeonObjects Instance; private void Awake() { Instance = this; }
    [SerializeField] private List<Chunk> _roomsDungeon = new List<Chunk>();
    [SerializeField] private List<Chest> _chestsDungeon = new List<Chest>(); 
    [SerializeField] private List<Enemy> _enemyDungeon = new List<Enemy>();

    public int GetNumChestsInDungeon => _chestsDungeon.Count;
    private DungeonStats _dungeonStats;

	private void Start()
	{
        _dungeonStats = DungeonStats.Instance;
    }
	public void AddChunk(Chunk chunk)
    {
        _roomsDungeon.Add(chunk);
        _dungeonStats.numRoomsInDungeon = _roomsDungeon.Count;
    }
    public void AddChest(Chest chest)
    {
        _chestsDungeon.Add(chest);
        DungeonStats.Instance.numChestsInDungeon = GetNumChestsInDungeon;
    }
    public void UpdateParametrsRooms()
    {
        foreach (Chunk chunk in _roomsDungeon)
        {
            chunk.gameObject.GetComponent<RoomControl>().UpdateParametrs();
        }
    }
    public void NetworkSendData()
    {
        foreach (Chunk chunk in _roomsDungeon)
        {
            chunk.SendAllThisChunkData();
            chunk.SendUpdatePortals();
        }
    }
    public void UpdatePortalsInRooms()
    {
        foreach (Chunk chunk in _roomsDungeon)
        {
            chunk.UpdatePortals();
        }
    }
    public Chest ReturnRandomClosedChest()
    {
        List<Chest> closedChests = new List<Chest>();
        foreach (Chest chest in _chestsDungeon)
        {
            if (!chest.IsOpened) closedChests.Add(chest);
            else continue;
        }
        if (closedChests.Count <= 0) return null;
        else
        {
            int randomValue = Random.Range(0, closedChests.Count);
            return closedChests[randomValue];
        }
    }

        
}
