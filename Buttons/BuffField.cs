using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BuffField : MonoBehaviour
{
    [SerializeField] private Image _icon;
    [SerializeField] private SpriteRenderer _sprite;
    [SerializeField] private TMP_Text _time;
    [SerializeField] private TMP_Text _value;
    [SerializeField] private TMP_Text _description;
    private BuffStat _buffStat;
    private RequirementField _requireComponent;

    public void SetFieldsBuff(BuffStat buffStat)
    {
        _buffStat = buffStat;
        if (_icon)
        { _icon.sprite = buffStat.BuffClass.Icon; }
        else { _sprite.sprite = buffStat.BuffClass.Icon; }
        int time = Mathf.FloorToInt(buffStat.Time);
        _time.text = time.ToString();
        if (_value) _value.text = buffStat.BuffClass.Value.ToString();
    }
    public void SetBuffInfoField(BuffStat buffStat)
    {
        SetFieldsBuff(buffStat);
        _description.text = "Buff " + buffStat.BuffClass.Description + " on value: " + _buffStat.BuffClass.Value.ToString();
    }
    public void SetFieldRequirement(RequirementField requireComponent)
    {
        _requireComponent = requireComponent;
        _time.text = requireComponent.Name;
        _value.text = requireComponent.Value;
        if (requireComponent.Sprite) { _icon.sprite = requireComponent.Sprite; }
    }
    public void ClickOnBuff()
    {
        MsgBoxUI.Instance.ShowInfo("Buff info", "Buff " + _buffStat.BuffClass.Buff.ToString() + " on value: " + _buffStat.BuffClass.Value.ToString() + " duration: " + _buffStat.BuffClass.Duration.ToString());
    }
    public void ClickOnRequirement()
    {
        MsgBoxUI.Instance.ShowInfo("Requirement info", "Requirement " + _requireComponent.Name + " needs: " + _requireComponent.Value);
    }
}
