using TMPro;
using UnityEngine;

public class ButtonRoom : MonoBehaviour
{
    // сохранение последней записанной комнате
    private RoomInfo _roomInfo;
    [SerializeField] private TMP_Text roomNameText;
    [SerializeField] private TMP_Text _amountPlayersText;

    public void SetUp(RoomInfo roomInfo)
    {
        _roomInfo = roomInfo;
        roomNameText.text = _roomInfo.Name;
        _amountPlayersText.text = _roomInfo.PlayerCount.ToString() + " / " + _roomInfo.MaxPlayers.ToString();
    }
    public void ClickOnButton()
    {
        LobbyRoom.instance.SetRoomInfoField(_roomInfo);
    }

}
