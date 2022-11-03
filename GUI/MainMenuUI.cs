using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance; private void Awake() { Instance = this; }
    [SerializeField] private GameObject SelectLanguagePanel;
    [SerializeField] private GameObject Profile;
    [SerializeField] private GameObject SelectProfile;
    [SerializeField] private GameObject DangeonOptions;
    [SerializeField] private GameObject CreateNewCharacter;
    [SerializeField] private GameObject Storage;
    [SerializeField] private GameObject SettingsPanel;
    [SerializeField] private GameObject _craftingMenu;
    [SerializeField] private GameObject _shopingMenu;
    [SerializeField] private GameObject _playerMenu;
    [SerializeField] private Inventory _inv;
    [SerializeField] private PlayerLevelingUI _playerLevelingUI;
    [SerializeField] private PlayerUI _playerUI;
    [SerializeField] private CharacterUI _characterUIStats;
    [SerializeField] private NetworkCreateGame _networkCreateGame;
    [SerializeField] private CharacterUI _characterUI;
    [SerializeField] private ShopingUI _shopingUI;
    [SerializeField] private SettingsUI _settingsUI;
    [SerializeField] private PropertyUI _propertyUI;
    [SerializeField] private List<ProfileSlot> _profileSlots;
    private PlayerStats plStats;

    //[SerializeField] private List<Text> NumRooms = new List<Text>(); // GUI выбора колво комнат
    private int numRooms = 30; // Стандартное количество комнат

    private void Start()
    {
        // для инициалзации Instance у объектов
        SwitchPanels(true);
        //
        SwitchPanels(false);
        Profile.SetActive(false);
        SelectLanguagePanel.SetActive(false);
        SelectProfile.SetActive(true);
        //FillUIRooms();
        if (!CheckHasSaving())
        {
            PlayerPrefs.SetInt("activeSlot", 1);
            ClickCreateNewCharacter();
            Settings.Instance.SetDefaultSettings();
            SelectLanguagePanel.SetActive(true);
        }
    }
    public void SwitchPanels(bool status)
    {
        CreateNewCharacter.SetActive(status);
        DangeonOptions.SetActive(status);
        Storage.SetActive(status);
        SelectProfile.SetActive(status);
        _craftingMenu.SetActive(status);
        _shopingMenu.SetActive(status);
        _playerMenu.SetActive(status);
        SettingsPanel.SetActive(status);
    }
    public void ClickPlayer()
    {
        ClickStorage();
        plStats.SetInventory(_inv);
        _characterUIStats.SetPlayerStats(plStats);
        _playerUI.SetPlayerStats(plStats);
        _playerUI.FillPlayerUI();
        _characterUIStats.UpdateButtons();
        _playerLevelingUI.FillLevelingUI();

        SwitchPanels(false);
        _playerMenu.SetActive(true);

    }
    public void ClickShoping()
    {
        SwitchPanels(false);
        _shopingMenu.SetActive(true);
        _shopingUI.SetPlayerStats(plStats);
    }
    public void ResetPlStats()
    {
        Destroy(this.GetComponent<PlayerStats>());
        CreatePlayerStats();
    }
    public void ClickCreateNewCharacter()
    {
        SwitchPanels(false);
        CreatePlayerStats();
        CreateNewCharacter.SetActive(true);
        Profile.SetActive(true);
        _propertyUI.UpdateProperty();
        _networkCreateGame.CheckPlayerName();
    }
    public void ClickSelSlot()
    {
        SwitchPanels(false);
        _inv.LoadItemsId();
        _inv.SetInvSlotsFromItemsIDs();
        _propertyUI.UpdateProperty();
        _networkCreateGame.CheckPlayerName();
        Profile.SetActive(true);
        DangeonOptions.SetActive(true);
    }
    public void AppExit()
    {
        // добавить запрос на подтверждение вдиалоговом окне
        Application.Quit();
    }
    public void ClickDunOpts()
    {
        SwitchPanels(false);
        DangeonOptions.SetActive(true);
    }
    public void ClickCraftingMenu()
    {
        SwitchPanels(false);
        _craftingMenu.SetActive(true);
        CraftingUI craftingUI = _craftingMenu.GetComponent<CraftingUI>();
        craftingUI.SetPlayerStats(plStats);
        craftingUI.LoadCraftingUI();
    }
    public void ClickStorage()
    {
        SwitchPanels(false);
        Storage.SetActive(true);
        Storage storage = Storage.GetComponent<Storage>();
        storage.SetPlayerStats(plStats);
        storage.FillStorageSlotsUI();
    }
    public void CreatePlayerStats()
    {
        plStats = this.gameObject.AddComponent<PlayerStats>();
        plStats.SetStats();
        _propertyUI.UpdateProperty();
        _characterUI.SetPlayerStats(plStats);
        _characterUI.UpdateButtons();
    }
    public void ClickSettings()
    {
        SwitchPanels(false);
        SettingsPanel.SetActive(true);
    }
    public void SaveAttributesToSlot()
    {
        int _slot = PlayerPrefs.GetInt("activeSlot");
        plStats.SaveStatsToSlot(_slot, plStats.stats);
    }
    public void ClickChangeSlot()
    {
        UpdateProfileSlots();
        Profile.SetActive(false);
        _networkCreateGame.DisconnecteFromServer();
        SwitchPanels(false);
        Destroy(this.GetComponent<PlayerStats>());
        Destroy(this.GetComponent<PlayerStats>());
        Destroy(this.GetComponent<PlayerStats>());
        SelectProfile.SetActive(true);
    }
    public void UpdateProfileSlots()
    {
        foreach (ProfileSlot slot in _profileSlots) { slot.SetMainMenuUI(this); slot.OpenStatsToSlot(); }
    }

    private bool CheckHasSaving()
    {
        bool isHas = false;
        for (int i = 0; i < _profileSlots.Count; i++)
        {
            if (PlayerPrefs.GetInt(i + "_slot_level") >= 1) isHas = true;
        }
        return isHas;
    }
}
