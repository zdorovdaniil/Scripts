using UnityEngine;
using TMPro;

/// <summary>
/// Скрипт отвечает за формирование скписка доступных из региона комнат, а также реализует функционал подключения к выбранной комнате
/// Раз в секунду формирует список комнат в регионе
/// </summary>
public class LobbyRoomUI : MonoBehaviour
{
    public static LobbyRoomUI instance; private void Awake() { instance = this; }
    [SerializeField] private Transform _roomListTrf;
    [SerializeField] private GameObject _roomButtonPrefab;
    [SerializeField] private TMP_Text _numRoomsInRegion;
    [SerializeField] private TMP_Text _regionName;
    [SerializeField] private TextMeshProUGUI _textNameRoom;
    [SerializeField] private TextMeshProUGUI _textNumPlayers;

    private RoomInfo _selectedRoomInfo;
    public RoomInfo GetRoomInfo => _selectedRoomInfo;
    private float _timer;

    private void FixedUpdate()
    {
        _timer += Time.deltaTime;
        if (_timer >= 1f) { _timer = 0; SetRoomsList(); }
    }
    public void ClearField()
    {
        _textNameRoom.text = "";
        _textNumPlayers.text = "";
    }

    public void SetRoomInfoField(RoomInfo roomInfo)
    {
        _selectedRoomInfo = roomInfo;
        _textNameRoom.text = _selectedRoomInfo.Name;
        _textNumPlayers.text = _selectedRoomInfo.PlayerCount.ToString() + " / " + _selectedRoomInfo.MaxPlayers.ToString();
    }
    public void SetRoomsList()
    {
        for (int i = 0; i < _roomListTrf.childCount; i++)
        {
            Destroy(_roomListTrf.GetChild(i).gameObject);
        }
        foreach (RoomInfo room in PhotonNetwork.GetRoomList())
        {
            Instantiate(_roomButtonPrefab, _roomListTrf).GetComponent<ButtonRoom>().SetUp(room);
        }
        _numRoomsInRegion.text = PhotonNetwork.GetRoomList().Length.ToString();
        _regionName.text = PhotonNetwork.CloudRegion.ToString();
    }
}
