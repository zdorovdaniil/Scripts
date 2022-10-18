using UnityEngine;

[CreateAssetMenu(fileName = "TextLanguage", menuName = "Project/TextLanguage", order = 9)]
public class TextLanguage : ScriptableObject
{
    [SerializeField] private string[] _textField; public string GetTextField(int idTextField) => _textField[idTextField];
}
