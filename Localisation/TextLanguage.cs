using UnityEngine;

[CreateAssetMenu(fileName = "TextLanguage", menuName = "Project/TextLanguage", order = 9)]
public class TextLanguage : ScriptableObject
{
    [SerializeField] private int _languageId; public int LanguageId => _languageId;
    [SerializeField] private string[] _textField; public string GetTextField(int idTextField) => _textField[idTextField];
}
