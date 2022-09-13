using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffField : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _value;
    public virtual void SetFields(BuffStat buffStat)
    {
        if (_icon)
        { _icon.sprite = buffStat.BuffClass.Icon; }
        else { _sprite.sprite = buffStat.BuffClass.Icon; }
        int time = Mathf.FloorToInt(buffStat.Time);
        _time.text = time.ToString();
        _value.text = buffStat.BuffClass.Value.ToString();
    }
}
