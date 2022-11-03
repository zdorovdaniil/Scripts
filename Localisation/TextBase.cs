using UnityEngine.Localization.Settings;
public static class TextBase
{
    private static TextLanguage _curTextLanguage; public static void SetTextLanguage(TextLanguage language) => _curTextLanguage = language;
    //public static int CurLanguage => _curTextLanguage.LanguageId;
    public static int CurLanguage => GetLocalizationCode();
    public static string Field(int i) => GetField(i);

    private static string GetField(int idField)
    {
        if (CurLanguage != _curTextLanguage.LanguageId) { _curTextLanguage = BasePrefs.instance.GetTextLanguage(CurLanguage); }
        return _curTextLanguage.GetTextField(idField);
    }

    // при добавлении в игру больше 2 языков, необходимо заменить данную конструкцию
    private static int GetLocalizationCode()
    {
        if (LocalizationSettings.SelectedLocale.Identifier.Code == "en") return 0;
        else return 1;
    }
}
