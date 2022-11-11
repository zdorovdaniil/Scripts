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
    [SerializeField] private bool _isSavingAfterChange;
    public int GetCoins => _coins;
    public int GetGems => _gems;

    private void Awake() { instance = this; }
    public void LoadProperty()
    {
        _coins = PlayerPrefs.GetInt(ProcessCommand.CurActiveSlot + "_slot_curGOLD");
        _gems = PlayerPrefs.GetInt(ProcessCommand.CurActiveSlot + "_slot_curGEMS");
        _nickName = PlayerPrefs.GetString(ProcessCommand.CurActiveSlot + "_slot_nickName");
        UpdateUI();
    }
    public void AddCoins(int value)
    {
        SetCoins(_coins + value);
    }
    public void MinusCoins(int value)
    {
        SetCoins(_coins - value);
    }
    public void SetCoins(int value)
    {
        _coins = value;
        CheckSave();
        UpdateUI();
    }
    public void SetGems(int value)
    {
        _gems = value;
        CheckSave();
        UpdateUI();
    }
    private void CheckSave()
    {
        if (_isSavingAfterChange) SaveCurProperty();
    }
    public void SaveCurProperty()
    {
        PlayerPrefs.SetInt(ProcessCommand.CurActiveSlot + "_slot_curGOLD", _coins);
        PlayerPrefs.SetInt(ProcessCommand.CurActiveSlot + "_slot_curGEMS", _gems);
    }
    private void UpdateUI()
    {
        _coinsText.text = _coins.ToString();
        _gemsText.text = _gems.ToString();
        if (_playerName != null) { _playerName.text = _nickName; }
    }

}
