using UnityEngine;

public class ChangeLanguageUI : MonoBehaviour
{
    [SerializeField] private int _languageId;
    public void ClickOnLanguageButton()
    {
        Settings.CurLanguage = _languageId;
        Settings.Instance.SendUpdateLanguage();
        Settings.SaveSettings();
    }
}
