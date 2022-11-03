using UnityEngine;
using TMPro;

/// <summary>
/// Скрипт отвечает за созданную игроком комнату, и передачу связанных с комнотой сетевых параметров
/// </summary>
public class SelectDungeonUI : Photon.MonoBehaviour
{
    public static SelectDungeonUI Instance; private void Awake() { Instance = this; }
    [SerializeField] private TMP_Text _1PlayerField;
    [SerializeField] private Transform _1PlayerLine;
    [SerializeField] private TMP_Text _2PlayerField;
    [SerializeField] private Transform _2PlayerLine;
    [SerializeField] private Transform _kick2PlayerButton;
    [SerializeField] private Transform _lobbyRoomPanel;
    [SerializeField] private Transform _selectRoomPanel;
    [SerializeField] private Transform[] _disableUIObjectsFor2Player;

    [SerializeField] private DungeonRules _rules;
    [SerializeField] private CounterButtons _dungeonLevelCounter;
    [SerializeField] private TMP_Text _numRooms;
    [SerializeField] private TMP_Text _addQuests;
    [SerializeField] private TMP_Text _enemyLevel;
    [SerializeField] private TMP_Text _modifSpawnItemsChest;


    [PunRPC]
    public void SetUIFromRules(int dungeonLevel)
    {
        _dungeonLevelCounter.SetText(dungeonLevel.ToString());
        _numRooms.text = _rules.NumRoomsFromLevel(dungeonLevel).ToString();
        _addQuests.text = _rules.AddQuestFromLevel(dungeonLevel).ToString();
        //_enemyLevel.text = _rules.EnemyLevel(dungeonLevel).ToString();
        int[] vector = new int[2]; vector = _rules.EnemyLevelRangeFromLevel(dungeonLevel);
        if (vector[0] == vector[1]) _enemyLevel.text = vector[0].ToString();
        else _enemyLevel.text = vector[0].ToString() + " - " + vector[1].ToString();
        _modifSpawnItemsChest.text = _rules.GetModifSpawnItemsChest(dungeonLevel).ToString();

    }
    private void Start()
    {
        OpenlostRoomsLobby();
        //_lobbyRoomPanel.gameObject.SetActive(false);
        //_selectRoomPanel.gameObject.SetActive(true);
    }
    public void ClearFields()
    {
        _1PlayerField.text = "";
        _1PlayerLine.gameObject.SetActive(false);
        _2PlayerField.text = "";
        _2PlayerLine.gameObject.SetActive(false);
        _kick2PlayerButton.gameObject.SetActive(false);
    }
    [PunRPC]
    public void ClearSecondNickName()
    {
        _2PlayerField.text = "";
        _kick2PlayerButton.gameObject.SetActive(false);
    }

    public void StartCreateRoom()
    {
        ClearFields();
        SwitchClientObjects(true);
        _1PlayerField.text = PhotonNetwork.playerName;
        _1PlayerLine.gameObject.SetActive(true);
        photonView.RPC("GetFirstNickName", PhotonTargets.OthersBuffered, (string)PhotonNetwork.playerName);
        OpenRoomLobby();
        SetUIFromRules(ProcessCommand.GetDungeonLevel);
    }
    public void SetSingleplayerRoom()
    {
        _1PlayerField.text = PlayerPrefs.GetString(ProcessCommand.CurActiveSlot + "_slot_nickName");
        OpenRoomLobby();
        SetUIFromRules(ProcessCommand.GetDungeonLevel);
    }
    private string _1PlayerNameForOtherPlayer;
    [PunRPC]
    public void GetFirstNickName(string value)
    {
        _1PlayerNameForOtherPlayer = value;
        _1PlayerField.text = value;
    }
    public void SetRoomInfo()
    {
        ClearFields();
        if (PhotonNetwork.isNonMasterClientInRoom)
        {
            SwitchClientObjects(false);
            _2PlayerField.text = PhotonNetwork.playerName;
            photonView.RPC("SetSecondPlayerName", PhotonTargets.All, (string)PhotonNetwork.playerName);
            _2PlayerLine.gameObject.SetActive(true);

        }
        OpenRoomLobby();
    }
    [PunRPC]
    public void SetSecondPlayerName(string name)
    {
        _2PlayerField.text = name;
        if (PhotonNetwork.isMasterClient)
        { _kick2PlayerButton.gameObject.SetActive(true); }
        else { _kick2PlayerButton.gameObject.SetActive(false); }
    }
    public void OpenRoomLobby()
    {
        _lobbyRoomPanel.gameObject.SetActive(true);
        _selectRoomPanel.gameObject.SetActive(false);
        ProcessCommand.SetDungeonLevel(1);
        _dungeonLevelCounter.UpdateUI();
    }
    public void OpenlostRoomsLobby()
    {
        ClearFields();
        _lobbyRoomPanel.gameObject.SetActive(false);
        _selectRoomPanel.gameObject.SetActive(true);
        ProcessCommand.SetDungeonLevel(1);
        _dungeonLevelCounter.UpdateUI();
    }
    private void SwitchClientObjects(bool status)
    {
        foreach (Transform obj in _disableUIObjectsFor2Player)
        { obj.gameObject.SetActive(status); }
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }

}
