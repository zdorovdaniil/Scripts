public static class TextBase
{
    private static TextLanguage _curTextLanguage; public static void SetTextLanguage(TextLanguage language) => _curTextLanguage = language;
    public static string Field(int i) => _curTextLanguage.GetTextField(i);

}
