﻿using System.Collections.Generic;
using UnityEngine;

namespace stats
{
    public class Stats
    {
        // Основные СТАТы персонажа
        public int Level; // Уровень персонажа, влияет на уровень сложности
        public float HP;// Максимальное здоровье персонажа, зависит от Выносливости
        public int EXP;// Необходимое количество очков опыта для повышения

        // Данные параметры зависят от характеристик и надетых предметов на персонажа
        public float critChance;
        public float critChanceEquip;
        public float critChanceSkill;
        public float critValue;
        public float critValueEquip;
        public float critValueSkill;

        public float attack; // Атака получается от Скорости и Силы и экипированного оружия
        public float attackWeapon; // урон экипированного оружия
        public float attackSkill; // урон от навыка владения оружием
        public float minDMG; // минимальный урон
        public float maxDMG; // максимальный урон
        public float armor; // Класс брони - зависит от Ловкости,Выносливости и экипированной брони
        public float armorEquip; // Экипированная броня 
        public float armorSkill; // броня от навыка ношения брони 
        public float minusDMG; // Уменьшение урона
        public float moveSpeedAttr; // скорость перемещения от аттрибутов
        public float moveSpeed; // Скорость движения персонажа

        // Параметры зависят от бафа игрока
        public int buffCritChance;
        public int buffCritValue;
        public int buffAttack;
        public int buffDefence;
        public int buffMinusDMG;
        public int buffSpeed;
        public int buffKickStrenght;
        public int buffMaxBlock;
        public int buffGainExp;

        public List<AttributeStat> Attributes = new List<AttributeStat>(); // список аттрибутов
        public List<Skill> Skills = new List<Skill>(); // список навыков 
        public List<BuffStat> ActiveBuffes = new List<BuffStat>(); // список активных баффов

        /// <summary>
        /// Функция возвращает значение опыта, формируемое из аттрибутов и экипированного снаряжения
        /// </summary>
        /// <returns></returns>
        public int ExpStatsValues()
        {
            int exp = 0;
            foreach (AttributeStat attr in Attributes)
            {
                exp += attr.Level;
            }
            float otherStats = attackWeapon + (armorEquip / 3);
            exp += Mathf.FloorToInt(otherStats);
            return exp;
        }
        public void SetAttackWeapon(int value)
        {
            recount();
            if (value != 0) attackSkill = Mathf.Floor(((Skills[6].Level * 0.25f)) * 100.00f) * 0.01f; else attackSkill = 0;
            attackWeapon = Mathf.Floor((value) * 100.00f) * 0.01f;
            recount();
        }
        public void SetCritEquip(int critChanceE, int critValueE)
        {
            recount();
            critChanceEquip = critChanceE;
            critChanceSkill = Skills[6].Level * 1;
            critValueEquip = critValueE;
            critValueSkill = Skills[6].Level * 6;

            critChance = 2 + critChanceEquip + critChanceSkill + buffCritChance;
            critValue = 100 + critValueEquip + critValueSkill + buffCritValue;

            recount();
        }
        public int ModifGainExp(int value)
        {
            return Mathf.FloorToInt(value * (buffGainExp * 0.01f + 1));
        }
        public void SetArmorEquip(int value, int countModifire)
        {
            recount();
            if (countModifire != 0) armorSkill = Mathf.Floor(((Skills[5].Level * 0.2f * countModifire)) * 100.00f) * 0.01f; else armorEquip = 0;
            armorEquip = Mathf.Floor((value) * 100.00f) * 0.01f;
            recount();
        }
        public float GetBuyModif()
        {
            return ((0.1f * (20 - Skills[3].Level)) * 100.00f) * 0.01f;
        }
        public float GetSaleModif()
        {
            return (((((Skills[3].Level * 0.2f) + 5) * 0.2f) * 0.5f) * 100.00f) * 0.01f;
        }
        public float GetMaxBlockDamage()
        {
            return Mathf.Floor(((armor / 5) + buffMaxBlock) * 100.00f) * 0.01f;
        }
        public float GetTimeRegenHP()
        {
            return Mathf.Floor((10 - (Attributes[2].Level / 5)) * 100.00f) * 0.01f;
        }
        public int GetAddHP()
        {
            int skillMedicene = Skills[2].Level;
            int addHPvalue = 1;
            if (skillMedicene == 0) { return addHPvalue; }
            else if (skillMedicene >= 1 && skillMedicene <= 4) { addHPvalue = 2; }
            else if (skillMedicene >= 5 && skillMedicene <= 8) { addHPvalue = 3; }
            else if (skillMedicene >= 9) { addHPvalue = 4; }
            return addHPvalue;
        }
        public float Damage()
        {
            float d = Random.Range(minDMG, maxDMG);
            return Mathf.Floor(d * 100.00f) * 0.01f;
        }
        // сила отбрасывания от удара
        public float KickStrenght()
        {
            return Mathf.Floor((((Attributes[0].Level * 0.25f) + (buffKickStrenght * 0.1f) + 10f) * 0.1f) * 100.00f) * 0.01f;
        }
        public void AddBuff(BuffClass buffClass)
        {
            if (!IsContainsInActiveBuffs())
            {
                BuffStat buffStats = new BuffStat(buffClass);
                ActiveBuffes.Add(buffStats);
                SetValueBuff(buffClass.Buff, buffClass.Value);
                Debug.Log(buffClass.Value + " / " + buffStats.Time);
            }
            bool IsContainsInActiveBuffs()
            {
                bool isContain = false;
                foreach (BuffStat buffStat in ActiveBuffes)
                {
                    if (buffClass.BuffId == buffStat.BuffClass.BuffId)
                    {
                        buffStat.FullTime();
                        isContain = true;
                        break;
                    }
                }
                return isContain;
            }
        }
        public void ResetBuff(BuffStat buffStat, bool withRemove = true)
        {
            if (withRemove) ActiveBuffes.Remove(buffStat);
            SetValueBuff(buffStat.BuffClass.Buff, -buffStat.BuffClass.Value);
        }
        private void SetValueBuff(Buff buff, int value)
        {
            switch (buff)
            {
                case Buff.AttackWeapon: { buffAttack += value; } break;
                case Buff.ArmorClass: { buffDefence += value; } break;
                case Buff.MinusDamage: { buffMinusDMG += value; } break;
                case Buff.MoveSpeed: { buffSpeed += value; } break;
                case Buff.CritChance: { buffCritChance += value; } break;
                case Buff.CritValue: { buffCritValue += value; } break;
                case Buff.KickStrenght: { buffKickStrenght += value; } break;
                case Buff.MaxBlock: { buffMaxBlock += value; } break;
                case Buff.GainExp: { buffGainExp += value; } break;
            }
            recount();
        }
        public void ResetAllBuff()
        {
            foreach (BuffStat buffStat in ActiveBuffes)
            { ResetBuff(buffStat, false); }
            ActiveBuffes.Clear();
            recount();
        }
        // значение защиты от атрибутов
        public float GetDefenceAttr()
        { return Mathf.Floor((Attributes[1].Level / 5 + Attributes[2].Level / 5) * 100.00f) * 0.01f; }
        // значение атаки от атрибутов
        public float GetAttackAttr()
        { return Mathf.Floor((Attributes[0].Level / 5 + Attributes[3].Level / 5) * 100.00f) * 0.01f; }
        public Stats(int lvl, int exp)
        {
            Level = lvl;
            EXP = exp;
        }
        public void SaveAttributes()
        {
            foreach (AttributeStat attribute in Attributes)
            {
                attribute.Save();
            }
        }
        public void SetPlayerAttributes()
        {
            foreach (Attribut attribute in BasePrefs.instance.AvaibleAttributes)
            {
                AttributeStat attr = new AttributeStat(attribute);
                attr.Load();
                Attributes.Add(attr);
            }
        }
        public void SetEnemyAttributes(int[] attributeLevels)
        {
            for (int i = 0; i < attributeLevels.Length; i++)
            {
                AttributeStat attr = new AttributeStat(BasePrefs.instance.AvaibleAttributes[i], attributeLevels[i]);
                Attributes.Add(attr);
            }
        }
        public void SetEnemyCrits(int[] critMassiv)
        {
            critChance = critMassiv[0];
            critValue = critMassiv[1];
        }

        //функция вызываемая при повышении уровня
        public void lvlUP()
        {
            if (Level <= PlayerLeveling.Instance.LevelScore.Length)
            {
                Level += 1;
                PlayerQuest.instance.UpdateProcessQuests(null, null, "level_up");
                EXP = PlayerLeveling.Instance.LevelScore[Level - 1];
            }
        }

        //функция пересчета урона для игрока
        public void newArmDmg()
        {
            attack = ((GetAttackAttr() + attackWeapon + attackSkill + buffAttack) * 100.00f) * 0.01f;
            armor = Mathf.Floor((GetDefenceAttr() + armorEquip + armorSkill + buffDefence) * 100.00f) * 0.01f;

            minDMG = attack;
            maxDMG = Mathf.Floor((attack + (attack / 5)) * 100.00f) * 0.01f;

            minusDMG = Mathf.Floor(((armor / 10) + buffMinusDMG) * 100.00f) * 0.01f;
            moveSpeedAttr = Mathf.Floor((Attributes[3].Level * 0.2f) * 100.00f) * 0.01f;
            moveSpeed = Mathf.Floor((moveSpeedAttr + buffSpeed + 8) * 100.00f) * 0.01f;
        }

        //Пересчет кол-ва максимальных жизней
        public void newhp()
        {
            HP = Mathf.Floor((Attributes[2].Level * 3f) * 100.00f) * 0.01f;
        }
        public void recount()
        {
            //Считаем урон
            newArmDmg();
            //считаем кол-во жизни
            newhp();
        }

    }

}
