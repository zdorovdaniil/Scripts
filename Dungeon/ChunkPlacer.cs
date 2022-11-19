using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChunkPlacer : MonoBehaviour
{
    // настройка шаблона появления комнат в подземелье
    [SerializeField] private DungeonConfigurator _dungeonConfig;
    [Range(9, 100)]
    public int NumRooms = 9; // задает количество комнат
    public int NumSpawnedRooms = 0; // количество появившихся комнат 
    public int NumFailSpawned = 0;
    public int TrySpawnRooms = 0; // попыток спавна комнат
    private Chunk[,] SpawnedRooms = new Chunk[11, 11]; // Для хранения комнат что появились на карте
    [SerializeField] private DungeonObjects _dungeonObjects; // Скрипт содержит объекты подземелья


    private GameManager _gameManager;
    private Vector3 _posStartSpawnRooms = new Vector3(0, 110, 0);
    private int _countEnemyesInRooms; public int GetCountEnemyes => _countEnemyesInRooms;
    public bool isMyltiplayer = false;

    public IEnumerator StartRespawnRooms(GameManager gameManager)
    {
        _gameManager = gameManager;
        if (_gameManager.IsCreatingDungeon)
        {
            NumRooms = DungeonStats.Instance.GetNumRooms;
            _dungeonConfig.SetCountRooms(NumRooms);
            _dungeonConfig.DefinePercents();
            RespawnFirstRoom();
            for (int i = 0; i < NumRooms; i++)
            {
                yield return new WaitForSecondsRealtime(0.025f);
                {
                    PlaceOneRoom();
                }
            }
            if (NumFailSpawned <= 0)
            {
                _dungeonObjects.UpdateParametrsRooms();
                _dungeonObjects.BuildNavMeshSurface();
                _gameManager.DungeonSuccessGenerated();
                UpdateDataInChunks();
            }
            else
            {
                _gameManager.DungeonFailGenerated();
            }
        }
        else
        {
            _dungeonObjects.BuildNavMeshSurface();
            _gameManager.DungeonSuccessGenerated();
        }
    }
    private void UpdateDataInChunks()
    {
        if (!PhotonNetwork.offlineMode)
        { _dungeonObjects.NetworkSendData(); }
    }
    private void RespawnFirstRoom()
    {
        GameObject _obj;
        Chunk startChunk = SelectChunkFromCollection(RoomType.Start);
        if (isMyltiplayer == true)
        {
            _obj = PhotonNetwork.Instantiate(startChunk.gameObject.name, startChunk.transform.position, startChunk.transform.rotation, 0);
        }
        else
        {
            _obj = Instantiate(startChunk.gameObject, startChunk.transform.position, startChunk.transform.rotation);
        }
        //_dungeonObjects.AddChunk(_obj.GetComponent<Chunk>());
        _obj.transform.SetParent(transform);
        SpawnedRooms[5, 5] = _obj.GetComponent<Chunk>();
    }

    private Chunk CreateRoomForMyltiplayer(Chunk _room)
    {
        Chunk newRoom; // переменная в которую запишется случайная комната
        GameObject _obj = PhotonNetwork.Instantiate(_room.gameObject.name, _posStartSpawnRooms, Quaternion.identity, 0);
        return newRoom = _obj.GetComponent<Chunk>();
    }
    private Chunk CreateRoomForSinglePlayer(Chunk _room)
    {
        Chunk newRoom; // переменная в которую запишется случайная комната
        GameObject _obj = Instantiate(_room.gameObject, _posStartSpawnRooms, Quaternion.identity);
        return newRoom = _obj.GetComponent<Chunk>();
    }
    private Chunk SelectChunkFromCollection(RoomType roomType)
    {
        //List<Chunk> chunks = new List<Chunk>();
        List<Chunk> chunks = _dungeonConfig.LishChunksOfType(roomType);
        /*foreach (Chunk chunk in AllSimpleRooms)
        {
            if (chunk.gameObject.GetComponent<RoomControl>().GetRoomType == roomType)
            { chunks.Add(chunk); }
        }*/
        int count = chunks.Count;
        int randomValue = Random.Range(0, count);
        if (chunks[randomValue] == null) Debug.Log("Null Chunk");
        return chunks[randomValue];
    }
    private void PlaceOneRoom()
    {
        // Получаем список мест, где можно спавнить комнаты
        HashSet<Vector2Int> FreePlaces = new HashSet<Vector2Int>();
        // Цикл проходит по всем клеткам сетки хранимая в SpawnedRooms
        for (int x = 0; x < SpawnedRooms.GetLength(0); x++)
        {
            for (int y = 0; y < SpawnedRooms.GetLength(1); y++)
            {
                if (SpawnedRooms[x, y] == null) continue;
                int maxX = SpawnedRooms.GetLength(0) - 1;
                int maxY = SpawnedRooms.GetLength(1) - 1;
                if (x > 0 && SpawnedRooms[x - 1, y] == null) FreePlaces.Add(new Vector2Int(x - 1, y));
                if (y > 0 && SpawnedRooms[x, y - 1] == null) FreePlaces.Add(new Vector2Int(x, y - 1));
                if (x < maxX && SpawnedRooms[x + 1, y] == null) FreePlaces.Add(new Vector2Int(x + 1, y));
                if (y < maxY && SpawnedRooms[x, y + 1] == null) FreePlaces.Add(new Vector2Int(x, y + 1));
            }
        }
        Chunk newRoom = null; // переменная в которую запишется случайная комната

        if (NumSpawnedRooms <= 5) // первые 5 комнат
        {
            // появление комнат 1 типа
            if (isMyltiplayer == true)
                newRoom = CreateRoomForMyltiplayer(SelectChunkFromCollection(RoomType.Default));
            else
                newRoom = CreateRoomForSinglePlayer(SelectChunkFromCollection(RoomType.Default));

        }
        else if (5 < NumSpawnedRooms && NumSpawnedRooms < NumRooms - 1 - _dungeonConfig.RoomsAppearOnce.Count)
        {
            if (isMyltiplayer == true)
                newRoom = CreateRoomForMyltiplayer(SelectChunkFromCollection(_dungeonConfig.GetRoomType()));
            else
                newRoom = CreateRoomForSinglePlayer(SelectChunkFromCollection(_dungeonConfig.GetRoomType()));
        }
        else if (NumSpawnedRooms >= NumRooms - 1 - _dungeonConfig.RoomsAppearOnce.Count && NumSpawnedRooms < NumRooms - 1)
        {
            if (isMyltiplayer == true)
                newRoom = CreateRoomForMyltiplayer(_dungeonConfig.RoomsAppearOnce[NumSpawnedRooms - (NumRooms - 1 - _dungeonConfig.RoomsAppearOnce.Count)]);
            else
                newRoom = CreateRoomForSinglePlayer(_dungeonConfig.RoomsAppearOnce[NumSpawnedRooms - (NumRooms - 1 - _dungeonConfig.RoomsAppearOnce.Count)]);
        }
        if (NumSpawnedRooms == NumRooms - 1)
        {
            if (isMyltiplayer == true)
                newRoom = CreateRoomForMyltiplayer(SelectChunkFromCollection(RoomType.Final));
            else
                newRoom = CreateRoomForSinglePlayer(SelectChunkFromCollection(RoomType.Final));
        }
        NumSpawnedRooms += 1;
        _countEnemyesInRooms += newRoom.gameObject.GetComponent<RoomControl>().GetCountEnemy;
        // помещение только что появившегнося объекта в объект environment
        newRoom.transform.SetParent(transform);

        newRoom.name = newRoom.name + "_" + NumSpawnedRooms; // название комнаты равно номеру комнаты
        TrySpawnRooms += 1;
        int limit = 500;
        //500 раз ищется свободное место, ставится и соединяется с соседними комнатами
        while (limit-- > 0)
        {
            Vector2Int position = FreePlaces.ElementAt(Random.Range(0, FreePlaces.Count));
            if (newRoom.IsSupportRotation)
            {
                int numRatations = Random.Range(0, 4);
                for (int i = 0; i < numRatations; i++)
                {
                    if (PhotonNetwork.offlineMode) { newRoom.RotateRandom(); }
                    else newRoom.SendAllRotation();
                }
            }

            if (ConnectRoom(newRoom, position)) // попытка соедениться к соседней комнате
            {
                // задается мировая позиция для комнаты
                newRoom.transform.position = new Vector3(position.x - 5, 0, position.y - 5) * 25;
                // задаем в массив комнат
                SpawnedRooms[position.x, position.y] = newRoom;
                //if (newRoom != null) _dungeonObjects.AddChunk(newRoom);
                newRoom.SetPositionCoordinate(position);
                // отключение тумана если включен чит
                if (!_gameManager.IsDisabledFog) newRoom.SwitchFog(true); else newRoom.SwitchFog(false);
                return;
            }
        }
        // неудачный спавн
        Debug.Log("Spawn Fail");
        Destroy(newRoom.gameObject); // удаляется комната,которой некуда присоедениться
        NumSpawnedRooms -= 1; // удаляется комната из списка появишихся
        NumFailSpawned += 1;
        _countEnemyesInRooms -= newRoom.gameObject.GetComponent<RoomControl>().GetCountEnemy;
    }

    private bool ConnectRoom(Chunk room, Vector2Int p)
    {
        int maxX = SpawnedRooms.GetLength(0) - 1;
        int maxY = SpawnedRooms.GetLength(1) - 1;
        // Список вариантов к чему подсоедениться
        List<Vector2Int> neighborsDoor = new List<Vector2Int>();
        List<Vector2Int> neighborsWall = new List<Vector2Int>();
        // Проверяется, что у комнаты есть дверь вверх, и что у комнаты сверху есть дверь вниз
        if (room.AllWallU != null && p.y < maxY && SpawnedRooms[p.x, p.y + 1]?.AllWallD != null)
        { neighborsWall.Add(Vector2Int.up); }
        if (room.AllWallD != null && p.y > 0 && SpawnedRooms[p.x, p.y - 1]?.AllWallU != null)
        { neighborsWall.Add(Vector2Int.down); }
        if (room.AllWallR != null && p.x < maxX && SpawnedRooms[p.x + 1, p.y]?.AllWallL != null)
        { neighborsWall.Add(Vector2Int.right); }
        if (room.AllWallL != null && p.x > 0 && SpawnedRooms[p.x - 1, p.y]?.AllWallR != null)
        { neighborsWall.Add(Vector2Int.left); }
        if (neighborsWall.Count == 0) return false;
        // Проверяется, что у комнаты есть вверху стена, и что у комнаты сверху есть стена снизу
        if (room.DoorU != null && p.y < maxY && SpawnedRooms[p.x, p.y + 1]?.DoorD != null)
        { neighborsDoor.Add(Vector2Int.up); }
        if (room.DoorD != null && p.y > 0 && SpawnedRooms[p.x, p.y - 1]?.DoorU != null)
        { neighborsDoor.Add(Vector2Int.down); }
        if (room.DoorR != null && p.x < maxX && SpawnedRooms[p.x + 1, p.y]?.DoorL != null)
        { neighborsDoor.Add(Vector2Int.right); }
        if (room.DoorL != null && p.x > 0 && SpawnedRooms[p.x - 1, p.y]?.DoorR != null)
        { neighborsDoor.Add(Vector2Int.left); }
        if (neighborsDoor.Count == 0) return false;
        // цикл проходиться по всем соседям коммнаты, делая соединение между ними
        for (int x = 0; x < neighborsDoor.Count; x++)
        {
            Vector2Int selectDirect = neighborsDoor[x]; // получаем направление куда подсоединяется текущая комната
            Chunk selectRoom = SpawnedRooms[p.x + selectDirect.x, p.y + selectDirect.y];
            StartCoroutine(room.CheckDoors(selectDirect, selectRoom));
            room.CheckType(selectDirect, selectRoom);
        }
        for (int x = 0; x < neighborsWall.Count; x++)
        {
            Vector2Int selectDirect = neighborsWall[x]; // получаем направление куда подсоединяется текущая комната
            Chunk selectRoom = SpawnedRooms[p.x + selectDirect.x, p.y + selectDirect.y];
            StartCoroutine(room.CheckWalls(selectDirect, selectRoom));
        }
        //Vector2Int selectDirect = neighbors[Random.Range(0,neighbors.Count)]; случайное соединение к одному из соседей
        return true;
    }
}
