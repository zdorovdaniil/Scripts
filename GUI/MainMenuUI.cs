using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class MainMenuUI : MonoBehaviour
{
    public static MainMenuUI Instance; private void Awake() { Instance = this; }
    [SerializeField] private GameObject Profile;
    [SerializeField] private GameObject SelectProfile;
    [SerializeField] private GameObject DangeonOptions;
    [SerializeField] private GameObject CreateNewCharacter;
    [SerializeField] private GameObject Storage;
    [SerializeField] private GameObject Settings;
    [SerializeField] private GameObject _craftingMenu;
    [SerializeField] private GameObject _shopingMenu;
    [SerializeField] private Inventory _inv;
    [SerializeField] private Dungeon _dungeon;
    [SerializeField] private CharacterUI _characterUI;
    [SerializeField] private ShopingUI _shopingUI;
    [SerializeField] private SettingsUI _settingsUI;
    [SerializeField] private PropertyUI _propertyUI;
    [SerializeField] private List<ProfileSlot> _profileSlots;
    private PlayerStats plStats;

    [SerializeField] private List<Text> NumRooms = new List<Text>(); // GUI выбора колво комнат
    private int numRooms = 30; // Стандартное количество комнат

    private void Start()
    {
        CreateNewCharacter.SetActive(true);
        Profile.SetActive(false);
        if (PlayerPrefs.HasKey("numRooms"))
        {
            numRooms = PlayerPrefs.GetInt("numRooms");
        }
        foreach (Text _numRooms in NumRooms)
        {
            _numRooms.text = numRooms.ToString();
        }
        SelectProfile.SetActive(true);
        CreateNewCharacter.SetActive(false);
    }
    public void CloseAllPanels()
    {
        CreateNewCharacter.SetActive(false);
        DangeonOptions.SetActive(false);
        Storage.SetActive(false);
        SelectProfile.SetActive(false);
        _craftingMenu.SetActive(false);
        _shopingMenu.SetActive(false);
        Settings.SetActive(false);
    }
    public void ClickShoping()
    {
        CloseAllPanels();
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
        CloseAllPanels();
        CreatePlayerStats();
        CreateNewCharacter.SetActive(true);
        Profile.SetActive(true);
        _propertyUI.UpdateProperty();
        _dungeon.CheckPlayerName();
    }
    public void ClickSelSlot()
    {
        CloseAllPanels();
        _inv.LoadItemsId();
        _inv.SetInvSlotsFromItemsIDs();
        _propertyUI.UpdateProperty();
        _dungeon.CheckPlayerName();
        Profile.SetActive(true);
        DangeonOptions.SetActive(true);
    }
    public void AppExit()
    {
        Application.Quit();
    }
    public void ClickDunOpts()
    {
        CloseAllPanels();
        DangeonOptions.SetActive(true);
    }
    public void ClickCraftingMenu()
    {
        CloseAllPanels();
        _craftingMenu.SetActive(true);
        CraftingUI craftingUI = _craftingMenu.GetComponent<CraftingUI>();
        craftingUI.SetPlayerStats(plStats);
        craftingUI.LoadCraftingUI();
    }
    public void ClickStorage()
    {
        CloseAllPanels();
        Storage.SetActive(true);
        Storage storage = Storage.GetComponent<Storage>();
        storage.PlStats = plStats;
        storage.LoadingStorage();
        storage.FillStorageSlotsUI();
    }
    public void CreatePlayerStats()
    {
        plStats = this.gameObject.AddComponent<PlayerStats>();
        plStats.SetStats();
        _propertyUI.UpdateProperty();
        _characterUI.SetPlayerStats(plStats);
        _settingsUI.SetPlayerStats(plStats);
        _characterUI.UpdateButtons();
    }
    public void ClickSettings()
    {
        CloseAllPanels();
        Settings.SetActive(true);
    }
    public void SaveAttributesToSlot()
    {
        int _slot = PlayerPrefs.GetInt("activeSlot");
        plStats.SaveStatsToSlot(_slot, plStats.stats);
    }
    public void ChangeRooms(bool isUp)
    {
        if (isUp && numRooms < 90) { numRooms += 5; }
        if (!isUp && numRooms > 30) { numRooms -= 5; }
        foreach (Text _numRooms in NumRooms)
        {
            _numRooms.text = numRooms.ToString();
        }
        PlayerPrefs.SetInt("numRooms", numRooms);
    }
    public void ClickChangeSlot()
    {
        UpdateProfileSlots();
        Profile.SetActive(false);
        _dungeon.DisconnecteFromServer();
        CloseAllPanels();
        Destroy(this.GetComponent<PlayerStats>());
        Destroy(this.GetComponent<PlayerStats>());
        Destroy(this.GetComponent<PlayerStats>());
        SelectProfile.SetActive(true);
    }
    public void UpdateProfileSlots()
    {
        foreach (ProfileSlot slot in _profileSlots) { slot.OpenStatsToSlot(); }
    }
}
