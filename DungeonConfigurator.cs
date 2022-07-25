using UnityEngine;

[CreateAssetMenu(fileName = "DungeonConfigurator", menuName = "Project/DungeonConfigurator", order = 3)]
public class DungeonConfigurator : ScriptableObject
{
    [SerializeField] private float _factorEndRooms;
    [SerializeField] private float _factorBeetweedRooms;
    [SerializeField] private float _factorStandartRooms;
    [SerializeField] private float _factorBossRooms;
    [SerializeField] private float _factorBigRooms;
    public int _countRooms = 0; public void SetCountRooms(int count) { _countRooms = count; }
    public float _percentEndRooms;
    public float _percentBeetweedRooms;
    public float _percentStandartRooms;
    public float _percentBossRooms;
    public float _percentBigRooms;

    public void DefinePercents()
    {
        if (_countRooms != 0)
        {
            float total = _factorEndRooms + _factorBeetweedRooms + _factorStandartRooms + _factorBossRooms + _factorBigRooms;
            _percentEndRooms = (_factorEndRooms * 100) / total;
            _percentBeetweedRooms = (_factorBeetweedRooms * 100) / total;
            _percentStandartRooms = (_factorStandartRooms * 100) / total;
            _percentBossRooms = (_factorBossRooms * 100) / total;
            _percentBigRooms = (_factorBigRooms * 100) / total;

        }
    }
    public RoomType GetRoomType()
    {
        int randomValue = Random.Range(0, 100);
        float sum = 0;

        if (randomValue < _percentEndRooms) { return RoomType.End; }
        else sum += _percentEndRooms;
        if (randomValue < _percentBeetweedRooms + sum) { return RoomType.BeetweenRooms; }
        else sum += _percentBeetweedRooms;
        if (randomValue < _percentStandartRooms + sum) { return RoomType.Default; }
        else sum += _percentStandartRooms;
        if (randomValue < _percentBossRooms + sum) { return RoomType.MiniBoss; }
        else sum += _percentBossRooms;
        if (randomValue < _percentBigRooms + sum) { return RoomType.Big; }
        else return RoomType.Default;
    }
}
