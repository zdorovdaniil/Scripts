using TMPro;
using UnityEngine;

public class ImproveUI : MonoBehaviour
{
    public Improve Improve;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private TMP_Text _curLvl;
    [SerializeField] private TMP_Text _maxLvl;
    [SerializeField] private TMP_Text _cost;
    [SerializeField] private Transform _buttonLvlUp;
    public void SetUI()
    {
        Improve.LoadImprove();
        _description.text = Improve.GetDescription;
        _curLvl.text = Improve.GetCurLvl.ToString();
        _maxLvl.text = Improve.GetMaxLvl.ToString();
        if (Improve.IsLvlUp()) _cost.text = Improve.GetCurCost().ToString();
        else _cost.text = "max level";
        IsLvlUp();
    }
    public void IsLvlUp()
    {
        if (Improve.IsLvlUp() && PropertyUI.instance.GetCoins >= Improve.GetCurCost())
        { _buttonLvlUp.gameObject.SetActive(true); }
        else { _buttonLvlUp.gameObject.SetActive(false); }
    }
    public void ClickLvlUp()
    {
        if (Improve.IsLvlUp())
        {
            MsgBoxUI.Instance.Show(this.gameObject, "improve", "do you really want to spend coins on ipgrades");
        }
        SetUI();
    }
    public void GetReport(ReportType report)
    {
        if (report == ReportType.Accept) LvlUp();
    }
    private void LvlUp()
    {
        PropertyUI.instance.MinusCoins(Improve.GetCurCost());
        Improve.LvlUp();
    }
}


