using System.Collections;
using UnityEngine;
using TMPro;

public class DungeonUI : MonoBehaviour
{
    [SerializeField] private Transform _tabPlayers;
    [SerializeField] private TMP_Text _1playerNameTMP;
    [SerializeField] private TMP_Text _1playerConditionTMP;
    [SerializeField] private TMP_Text _1PingTMP;
    [SerializeField] private TMP_Text _2playerNameTMP;
    [SerializeField] private TMP_Text _2playerConditionTMP;
    [SerializeField] private TMP_Text _2PingTMP;
    [SerializeField] private TMP_Text _timerTMP;
    [SerializeField] private TMP_Text _enemyiesDefeated;
    [SerializeField] private TMP_Text _dungeonLevelTMP;
    [SerializeField] private TMP_Text _roomsPassed;
    [SerializeField] private TMP_Text _chestsOpened;
    private bool _isUpdatingFields;
    private GameManager _gameManager;
    private DungeonStats _dungeonStats;

    public void ActivateUI(bool status)
    {
        this.gameObject.SetActive(status);
        if (_dungeonStats == null) { _dungeonStats = DungeonStats.Instance; }
        _gameManager = GameManager.Instance;
        if (status)
        {
            SetFieldsName();
            SetValuesInFields();
            StartCoroutine(UpdateTimer());
            if (PhotonNetwork.offlineMode != true)
            {
                StartCoroutine(UpdatePing());
            }
            GlobalSounds.Instance.SOpenWindow();
        }
        else
        {
            StopCoroutine(UpdateTimer());
            StopCoroutine(UpdatePing());
            GlobalSounds.Instance.SCloseWindow();
        }
    }
    private void SetValuesInFields()
    {
        DungeonStats dungeonStats = DungeonStats.Instance;
        _dungeonLevelTMP.text = _gameManager.GetDungeonLevel.ToString();
        _enemyiesDefeated.text = dungeonStats.curKills.ToString() + " of " + dungeonStats.numEnemyInDungeon.ToString();
        _roomsPassed.text = dungeonStats.passedRoom.ToString() + " of " + dungeonStats.numRoomsInDungeon.ToString();
        _chestsOpened.text = dungeonStats.numOpenChest.ToString() + " of " + dungeonStats.numChestsInDungeon.ToString();
    }
    private IEnumerator UpdateTimer()
    {
        yield return new WaitForSecondsRealtime(1f);
        {
            float time = _gameManager.GetTimeDungeonGoing;
            _timerTMP.text = Mathf.Floor(time / 60).ToString() + " : " + (time % 60).ToString();
        }
        StartCoroutine(UpdateTimer());
    }
    private IEnumerator UpdatePing()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        {
            PlayerStats playerStats1 = _gameManager.GetPlayerIndex(0);
            PlayerStats playerStats2 = _gameManager.GetPlayerIndex(1);
            _1playerConditionTMP.text = playerStats1.GetPing.ToString();
            if (playerStats2 != null) _2playerConditionTMP.text = playerStats2.GetPing.ToString();
        }
        StartCoroutine(UpdatePing());
    }
    private void SetFieldsName()
    {
        if (PhotonNetwork.offlineMode != true)
        {
            _tabPlayers.gameObject.SetActive(true);
            PlayerStats playerStats1 = _gameManager.GetPlayerIndex(0);
            PlayerStats playerStats2 = _gameManager.GetPlayerIndex(1);
            _1playerNameTMP.text = playerStats1.NickName;
            if (playerStats2 != null) _2playerNameTMP.text = playerStats2.NickName;
        }
        else
        {
            _tabPlayers.gameObject.SetActive(false);
        }
    }
}
