using TMPro;
using UnityEngine;

/// <summary>
/// Скрипт выводит информацию с Improve на экран
/// </summary>
public class ImproveUI : MonoBehaviour
{
    public Improve _improve;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _curLvl;
    [SerializeField] private TMP_Text _maxLvl;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private Transform _buttonLvlUp;
    [SerializeField] private Transform _isMaxLvlText;
    public void SetImprove(Improve improve)
    {
        _improve = improve;
        UpdateUI();
    }
    public void UpdateUI()
    {
        _improve.LoadImprove();
        _description.text = _improve.GetDescription;
        _curLvl.text = _improve.GetCurLvl.ToString();
        _maxLvl.text = _improve.GetMaxLvl.ToString();
        _isMaxLvlText.gameObject.SetActive(false);
        if (_improve.IsLvlUp()) _cost.text = _improve.GetCurCost().ToString();
        else { _isMaxLvlText.gameObject.SetActive(true); _cost.text = "-"; }
        IsLvlUp();
    }
    public void IsLvlUp()
    {
        if (_improve.IsLvlUp() && PropertyUI.instance.GetCoins >= _improve.GetCurCost())
        { _buttonLvlUp.gameObject.SetActive(true); }
        else { _buttonLvlUp.gameObject.SetActive(false); }
    }
    public void ClickLvlUp()
    {
        GlobalSounds.Instance.SButtonClick();
        if (_improve.IsLvlUp())
        {
            MsgBoxUI.Instance.Show(this.gameObject, "improve", "do you really want to spend coins on ipgrades");
        }
        UpdateUI();
    }
    public void GetReport(ReportType report)
    {
        if (report == ReportType.Accept) LvlUp();
        UpdateUI();
    }
    private void LvlUp()
    {
        PropertyUI.instance.MinusCoins(_improve.GetCurCost());
        _improve.LvlUp();
    }
}


