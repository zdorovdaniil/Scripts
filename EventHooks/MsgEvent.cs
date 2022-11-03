using UnityEngine;

public class MsgEvent : MonoBehaviour
{

    [SerializeField] private TextLocalize _textLocalize;
    [SerializeField] private bool _isInfo = true;
    public void Show()
    {
        if (_isInfo) MsgBoxUI.Instance.ShowInfo(_textLocalize);
        else
        {
            MsgBoxUI.Instance.ShowAttention(_textLocalize);
        }
    }
    void Start()
    {

    }

}
