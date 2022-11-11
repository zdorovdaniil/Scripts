using UnityEngine;

[CreateAssetMenu(fileName = "TextLocalize", menuName = "Project/TextLocalize", order = 1)]
public class TextLocalize : ScriptableObject
{
    public string Text()
    {
        //Debug.Log("Language: " + TextBase.CurLanguage);
        return _fieldsLanguageText[TextBase.CurLanguage];
    }
    [SerializeField] private string[] _fieldsLanguageText = new string[2];

}
