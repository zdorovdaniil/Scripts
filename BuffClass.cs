using UnityEngine;

[CreateAssetMenu(fileName = "BuffClass", menuName = "Project/BuffClass", order = 7)]
public class BuffClass : ScriptableObject
{
    public int BuffId;
    public Buff Buff;
    public float Duration;
    public float Time;
    public int Value;
    public Sprite Icon;
    public GameObject EffectOnUse;
    public GameObject EffectOnUsing;
}
public enum Buff
{
    Nothing,
    Heal, // моментальное восстановление HP
    Healing,  // восстановление HP по 0,1 сек
    MinusDamage, // 
    ArmorClass, // 
    AttackWeapon, // 
    MoveSpeed, // 
    KickStrenght,
    MaxHp,
    MinusDamagePercent,
    AttackIgnoreArmor,
    MaxAttack,
}
