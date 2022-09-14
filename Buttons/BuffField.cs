using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffField : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _value;
    private BuffStat _buffStat;
    public void SetFields(BuffStat buffStat)
    {
        _buffStat = buffStat;
        if (_icon)
        { _icon.sprite = buffStat.BuffClass.Icon; }
        else { _sprite.sprite = buffStat.BuffClass.Icon; }
        int time = Mathf.FloorToInt(buffStat.Time);
        _time.text = time.ToString();
        _value.text = buffStat.BuffClass.Value.ToString();
    }
    public void ClickOnBuff()
    {
        MsgBoxUI.Instance.ShowInfo("Buff info", "Buff " + _buffStat.BuffClass.Buff.ToString() + " on value: " + _buffStat.BuffClass.Value.ToString() + " duration: " + _buffStat.BuffClass.Duration.ToString());
    }
}
