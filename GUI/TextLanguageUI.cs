using UnityEngine;
using TMPro;
public class TextLanguageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text _textObject;
    [SerializeField] private int _idTextField;
    public void SetTextUI()
    {
        Debug.Log("Set text: " + TextBase.Field(_idTextField));
        _textObject.text = TextBase.Field(_idTextField);
    }

}
