using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffField : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _value;
    public void SetFields(BuffClass buffClass)
    {
        _icon.sprite = buffClass.Icon;
        int time = Mathf.FloorToInt(buffClass.Time);
        _time.text = time.ToString();
        _value.text = buffClass.Value.ToString();
    }
}
