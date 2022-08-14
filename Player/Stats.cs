using System.Collections.Generic;
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
        public List<AttributeStat> Attributes = new List<AttributeStat>(); // список аттрибутов
        public List<Skill> Skills = new List<Skill>(); // список навыков 
        public List<BuffClass> ActiveBuffes = new List<BuffClass>(); // список активных баффов


        public void SetAttackWeapon(int value)
        {
            if (value != 0) attackSkill = Mathf.Floor(((Skills[6].Level * 0.2f)) * 100.00f) * 0.01f; else attackSkill = 0;
            attackWeapon = Mathf.Floor((value) * 100.00f) * 0.01f;
        }
        public void SetCritEquip(int critChanceE, int critValueE)
        {
            critChanceEquip = critChanceE;
            critChanceSkill = Skills[6].Level * 2;
            critValueEquip = critValueE;
            critValueSkill = Skills[6].Level * 4;

            critChance = 2 + critChanceEquip + critChanceSkill + buffCritChance;
            critValue = 100 + critValueEquip + critValueSkill + buffCritValue;
        }
        public void SetArmorEquip(int value, int countModifire)
        {
            if (countModifire != 0) armorSkill = Mathf.Floor(((Skills[5].Level * 0.2f * countModifire)) * 100.00f) * 0.01f; else armorEquip = 0;
            armorEquip = Mathf.Floor((value) * 100.00f) * 0.01f;
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
            return Mathf.Floor((armor / 5) * 100.00f) * 0.01f;
        }
        public float GetTimeRegenHP()
        {
            return Mathf.Floor((8 - (Attributes[2].Level / 10)) * 100.00f) * 0.01f;
        }
        public int GetAddHP()
        {
            int skillMedicene = Skills[2].Level;
            if (skillMedicene >= 0 && skillMedicene < 7)
            {
                return 1;
            }
            else if (skillMedicene >= 7 && skillMedicene < 14)
            {
                return 2;
            }
            else if (skillMedicene >= 14)
            {
                return 3;
            }
            return 1;
        }
        public float Damage()
        {
            float d = Random.Range(minDMG, maxDMG);
            return Mathf.Floor(d * 100.00f) * 0.01f;
        }
        // сила отбрасывания от удара
        public float KickStrenght()
        {
            return Mathf.Floor((((Attributes[0].Level * 0.3f) + (buffKickStrenght * 0.5f) + 10f) * 0.1f) * 100.00f) * 0.01f;
        }
        public void AddBuff(BuffClass buffClass)
        {
            if (ActiveBuffes.Contains(buffClass))
            {
                buffClass.Time = buffClass.Duration;
            }
            else
            {
                ActiveBuffes.Add(buffClass);
                buffClass.Time = buffClass.Duration;
                SetValueBuff(buffClass.Buff, buffClass.Value);
                Debug.Log(buffClass.Value + " / " + buffClass.Time);
            }
        }
        public void ResetBuff(BuffClass buffClass)
        {
            ActiveBuffes.Remove(buffClass);
            SetValueBuff(buffClass.Buff, -buffClass.Value);
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
            }
            recount();
        }
        public void ResetAllBuff()
        {
            ActiveBuffes.Clear();
            buffAttack = 0;
            buffDefence = 0;
            buffMinusDMG = 0;
            buffSpeed = 0;
            buffKickStrenght = 0;
            recount();
        }
        // значение защиты от атрибутов
        public float GetDefenceAttr()
        { return Mathf.Floor((Attributes[1].Level / 5 + Attributes[2].Level / 5) * 100.00f) * 0.01f; }
        // значение атаки от атрибутов
        public float GetAttackAttr()
        { return Mathf.Floor((Attributes[1].Level / 5 + Attributes[3].Level / 5) * 100.00f) * 0.01f; }
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

        //функция вызываемая при повышении уровня
        public void lvlUP()
        {
            if (Level <= PlayerLeveling.instance.LevelScore.Length)
            {
                Level += 1;
                PlayerQuest.instance.UpdateProcessQuests(null, null, "level_up");
                EXP = PlayerLeveling.instance.LevelScore[Level - 1];
            }
        }

        //функция пересчета урона для игрока
        public void newArmDmg()
        {
            attack = (GetAttackAttr() + attackWeapon + attackSkill + buffAttack);
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
