using UnityEngine;

[CreateAssetMenu(fileName = "BuffClass", menuName = "Project/BuffClass", order = 7)]
public class BuffClass : ScriptableObject
{
    public int BuffId;
    public Buff Buff;
    public float Duration;
    public int Value;
    public Sprite Icon;
    public GameObject EffectOnUse;
    public GameObject EffectOnUsing;
    public float EffectLifetime;
    [Multiline(2)] public string Description;
}
public class BuffStat
{
    public BuffClass BuffClass;
    public float Time;
    public BuffStat(BuffClass buffClass)
    {
        BuffClass = buffClass;
        Time = buffClass.Duration;
    }
    public void FullTime()
    {
        Time = BuffClass.Duration;
    }
    public bool DoingBuff()
    {
        Time = Time - 0.25f;
        if (Time <= 0) return false;
        else return true;
    }
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
    CritChance,
    CritValue,
    MaxBlock,
    GainExp // модификатор получения опыта
}
