using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuffDatabase", menuName = "Project/BuffDatabase", order = 8)]
public class BuffDatabase : ScriptableObject
{
    public List<BuffClass> Buffs = new List<BuffClass>();
    public List<BuffClass> GetBuffs => Buffs;

    // Получение предмета из БД по заданному ID
    public BuffClass GetBuff(int buffID)
    {
        foreach (BuffClass buff in Buffs)
        {
            if (buff != null && buff.BuffId == buffID) return buff;

        }
        return null;
    }
}