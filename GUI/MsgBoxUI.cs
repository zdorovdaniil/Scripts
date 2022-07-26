using UnityEngine;
using TMPro;
using System.Collections;

public class MsgBoxUI : MonoBehaviour
{
    public static MsgBoxUI Instance;
    [SerializeField] private TMP_Text _title;
    [SerializeField] private TMP_Text _description;
    [SerializeField] private ReportType _report;
    [SerializeField] private GameObject _workingObj;
    [SerializeField] private Transform _buttonAccept;
    [SerializeField] private Transform _buttonCancel;
    [SerializeField] private Transform _buttonOkey;
    [SerializeField] private Transform _box;
    private string _reportStatus;
    private void Start() { Instance = this; }
    public void Show(GameObject obj, string title, string description, string reportStatus = "", bool isOnlyAcceptButton = false)
    {
        _box.gameObject.SetActive(true);
        CloseButtons();
        _buttonAccept.gameObject.SetActive(true);
        if (!isOnlyAcceptButton) _buttonCancel.gameObject.SetActive(true);
        _workingObj = obj;
        _title.text = title;
        _description.text = description;
        _report = ReportType.Nothing;
        _reportStatus = reportStatus;
        GlobalSounds.Instance.SOpenWindow();
    }
    public void ShowAttention(string description)
    {
        _title.text = TextBase.Field(1);
        _description.text = description;
        ActivateOKButton();
        GlobalSounds.Instance.SAttention();
    }
    public void ShowAttention(TextLocalize textLocalize)
    {
        _title.text = TextBase.Field(1);
        _description.text = textLocalize.Text();
        ActivateOKButton();
    }
    public void ShowInfo(TextLocalize textLocalize)
    {
        _title.text = TextBase.Field(0);
        _description.text = textLocalize.Text();
        ActivateOKButton();
    }
    /*public void ShowFullInfo(TextLocalize title, TextLocalize description)
    {
        _title.text = title.Text();
        _description.text = description.Text();
        ActivateOKButton();
    }*/
    public void ShowInfo(string title, string description)
    {
        _title.text = title;
        _description.text = description;
        ActivateOKButton();
    }
    private void ActivateOKButton()
    {
        _box.gameObject.SetActive(true);
        StartCoroutine(ActiveBox());
        CloseButtons();
        _buttonOkey.gameObject.SetActive(true);
        GlobalSounds.Instance.SOpenWindow();
    }
    public void Okey()
    {
        _box.gameObject.SetActive(false);
        GlobalSounds.Instance.SCloseWindow();
    }
    public void Accept()
    {
        if (_workingObj != null)
        {
            _report = ReportType.Accept;
            if (_workingObj.GetComponent<AttributeButton>())
            { _workingObj.GetComponent<AttributeButton>().GetReport(_report); }
            else if (_workingObj.GetComponent<SkillButton>())
            { _workingObj.GetComponent<SkillButton>().GetReport(_report); }
            else if (_workingObj.GetComponent<SettingsUI>())
            { _workingObj.GetComponent<SettingsUI>().GetReport(_reportStatus); }
            else if (_workingObj.GetComponent<ProfileSlot>())
            { _workingObj.GetComponent<ProfileSlot>().GetReport(_report); }
            else if (_workingObj.GetComponent<ImproveUI>())
            { _workingObj.GetComponent<ImproveUI>().GetReport(_report); }
            else if (_workingObj.GetComponent<GameManager>())
            { _workingObj.GetComponent<GameManager>().GetReport(_reportStatus); }

        }
        _box.gameObject.SetActive(false);
        GlobalSounds.Instance.SCloseWindow();
    }
    public void Cancel()
    {
        if (_workingObj != null)
        {
            _report = ReportType.Cancel;
        }
        _box.gameObject.SetActive(false);
        GlobalSounds.Instance.SCloseWindow();
    }
    private void CloseButtons()
    {
        _buttonAccept.gameObject.SetActive(false);
        _buttonCancel.gameObject.SetActive(false);
        _buttonOkey.gameObject.SetActive(false);
    }
    public IEnumerator ActiveBox()
    {
        yield return new WaitForSecondsRealtime(0.1f);
        {
            _box.gameObject.SetActive(true);
        }
    }
}


public enum ReportType
{
    Nothing,
    Accept,
    Cancel
}
