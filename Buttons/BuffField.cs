using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffField : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _value;
    public void SetFields(BuffStat buffStat)
    {
        _icon.sprite = buffStat.BuffClass.Icon;
        int time = Mathf.FloorToInt(buffStat.Time);
        _time.text = time.ToString();
        _value.text = buffStat.BuffClass.Value.ToString();
    }
}
