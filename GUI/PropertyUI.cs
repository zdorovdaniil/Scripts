using UnityEngine;
using TMPro;

// скрипт берет из сохранения данные о денежном состоянии персонажа и отображает на экран в обьекты 
public class PropertyUI : MonoBehaviour
{
    public static PropertyUI instance;
    [SerializeField] private TextMeshProUGUI _coinsText;
    [SerializeField] private TextMeshProUGUI _gemsText;
    [SerializeField] private TextMeshProUGUI _playerName;
    [SerializeField] private int _coins;
    [SerializeField] private int _gems;
    [SerializeField] private string _nickName;
    public int GetCoins => _coins;
    public int GetGems => _gems;

    private void Awake() { instance = this; }
    public void UpdateProperty()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        _coins = PlayerPrefs.GetInt(id + "_slot_curGOLD");
        _gems = PlayerPrefs.GetInt(id + "_slot_curGEMS");
        _nickName = PlayerPrefs.GetString(id + "_slot_nickName");
        UpdateUI();
    }
    public void MinusCoins(int value)
    {
        UpdateProperty();
        _coins -= value;
        SaveCurProperty();
        UpdateUI();
    }
    public void SaveCurProperty()
    {
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_curGOLD", _coins);
        PlayerPrefs.SetInt(id + "_slot_curGEMS", _gems);
    }
    public void AddCoins(int value) => SetCoins(_coins + value);
    public void SetCoins(int value)
    {
        _coins = value;
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_curGOLD", _coins);

        UpdateUI();
    }
    public void SetGems(int value)
    {
        _gems = value;
        int id = PlayerPrefs.GetInt("activeSlot");
        PlayerPrefs.SetInt(id + "_slot_curGEMS", _gems);

        UpdateUI();
    }
    private void UpdateUI()
    {
        _coinsText.text = _coins.ToString();
        _gemsText.text = _gems.ToString();
        if (_playerName != null) { _playerName.text = _nickName; }
    }

}
