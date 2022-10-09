using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GUIControl : MonoBehaviour
{
    public static GUIControl Instance;
    public GameObject Player;
    // GUI элементы
    public GameObject PlayerUIObject;
    public GameObject MapPicture;
    public GameObject Joystick;
    public GameObject ChestButton;
    public GameObject UseButton;
    public GameObject AttackButton;
    public GameObject ExitButton;
    public GameObject ButMain;
    public GameObject ChestPanel;
    public GameObject BigMap;
    public GameObject QuestPanel;
    public GameObject DeathWindow;
    [SerializeField] private Camera minMapCamera;
    [SerializeField] private QuestUI _questUI;
    [SerializeField] private Text fpsText;
    private GameManager _gameManager;
    private PlayerStats _playerStats;
    private Inventory _inventory;
    public int fps;

    private IEnumerator Start()
    {
        _gameManager = FindObjectOfType<GameManager>();
        Instance = this;
        SwitchAllPanels(true);
        minMapCamera.gameObject.SetActive(false);
        yield return new WaitForSecondsRealtime(0.2f);
        {
            _playerStats = Player.GetComponent<PlayerStats>();
            _inventory = Player.GetComponent<Inventory>();
            SwitchAllPanels(false);
        }
        CloseBigMap();
        OpenDefaultPanel();
    }
    private void Update()
    {
        fps = (int)(1f / Time.unscaledDeltaTime);
        fpsText.text = fps.ToString();
    }
    public void OpenDefaultPanel()
    {
        Joystick.SetActive(true); ButMain.SetActive(true); AttackButton.SetActive(true);
    }
    public void ClickViewAds()
    {
        InterstitialAd.S.ShowAd();
    }
    public void RewardPlayer()
    {
        //_playerStats.GainGold(200);
    }
    public void CheckPlayerReborn(bool ignoreCheck = false)
    {
        if (InterstitialAd.S.IsAdViewed)
        {
            InterstitialAd.S.IsAdViewed = false;
            DeathWindow.SetActive(false);
            _playerStats.IsDeath = false;
            _playerStats.curHP = 10;
            PlayerController plControl = Player.GetComponent<PlayerController>();
            plControl.PlayerAlive();
            plControl.ResetAnim();
            OpenDefaultPanel();
        }
        else
        {
            InterstitialAd.S.ShowAd();
        }
    }
    public void SwitchAllPanels(bool status)
    {
        PlayerUIObject.SetActive(status);
        ExitButton.SetActive(status);
        Joystick.SetActive(status);
        AttackButton.SetActive(status);
        ButMain.SetActive(status);
        ChestPanel.SetActive(status);
        BigMap.SetActive(status);
        QuestPanel.SetActive(status);
        DeathWindow.SetActive(status);
        UseButton.SetActive(status);
    }
    public void ClickDungeonUp()
    {
        _gameManager.CheckDungeonLevel();
    }
    public void OpenQuests()
    {
        QuestPanel.SetActive(true);
        PlayerQuest Plquest = Player.GetComponent<PlayerQuest>();
        _questUI.FillListsOfQuest(Plquest);
        GlobalSounds.Instance.SOpenWindow();
    }
    public void CloseQuests()
    {
        QuestPanel.SetActive(false);
        GlobalSounds.Instance.SCloseWindow();
    }
    public void CloseMapPicture()
    {
        MapPicture.SetActive(false);
        minMapCamera.gameObject.SetActive(false);
        GlobalSounds.Instance.SButtonClick();
    }
    public void OpenMapPicture()
    {
        MapPicture.SetActive(true);
        CloseBigMap();
        minMapCamera.gameObject.SetActive(true);
        GlobalSounds.Instance.SButtonClick();
    }
    public void CloseBigMap()
    {
        _gameManager.SwitchAllMapCamera(false);
    }
    public void GainEXP(int _value)
    {
        PlayerStats LinkPlayerStats = Player.GetComponent<PlayerStats>();
        LinkPlayerStats.GainExperience(_value);
    }
    public void On_Off_ButtonChest(bool _status)
    {
        ChestButton.SetActive(_status);
    }

    public void GameSave()
    {
        DungeonQuests.Instance.SaveQuestsValue();
        PlayerStats LinkPlayerStats = Player.GetComponent<PlayerStats>();
        Inventory inv = Player.GetComponent<Inventory>();
        inv.DeleteCollectionItems(_gameManager.GetRules.GetDeletingItems);
        LinkPlayerStats.SaveStatsToSlot(ProcessCommand.CurActiveSlot, LinkPlayerStats.stats);
        inv.SaveItemsId();
    }
    public void LoadSceneMenu()
    {
        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}

