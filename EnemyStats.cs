﻿using System.Collections;
using System.Collections.Generic;
using stats; // Пространство имен stats из скрипта Stats
using TMPro;
using UnityEngine;

public class EnemyStats : Photon.MonoBehaviour
{
    private Stats stats;
    public float curHP; // Количество жизней нынешние
    public bool IsDeath; // Отвечает за уничтожение
    private bool isTakeDamage; //для ограниечения получаемого урона в секунду
    // от выбранного типа противника зависят его статы
    [SerializeField] public Enemy enemyTupe;
    public RoomControl BelongRoom; // принадлежит комнате
    // UI для отображения уровня, названия и здоровья
    public TextMeshPro lvlUI;
    public TextMeshPro NameEnemyUI;
    public TextMeshPro HealthEnemyUI;
    public Transform EnemyGUI;
    [SerializeField] Transform _mapPoint;
    private EnemyController enemyController;
    private Sound _sound;

    // обновление состояний для отображения UI
    private void UpdateConditionStatsUI()
    {
        if (lvlUI != null)
        {
            lvlUI.SetText("lvl - " + stats.Level.ToString());
            NameEnemyUI.SetText(enemyTupe.Name);
            HealthEnemyUI.SetText(curHP.ToString() + " / " + stats.HP);
        }
    }
    void Start()
    {
        int level = GameManager.Instance.GetDungeonLevel;
        stats = new Stats(level, 0, enemyTupe.Strenght(level), enemyTupe.Agility(level), enemyTupe.Endurance(level), enemyTupe.Speed(level));
        curHP = stats.HP;
        enemyController = gameObject.GetComponent<EnemyController>();
        _sound = GetComponent<Sound>();
        stats.attackWeapon = enemyTupe.AttackWeapon(level);
        stats.armorEquip = enemyTupe.ArmorClass(level);
        stats.recount();
        enemyController.SetActionDoing(enemyTupe.enemyType);
        isTakeDamage = true;
        UpdateConditionStatsUI();
    }
    public bool TakeDamage(float value, bool isAbsoluteHit = false, float kickStrenght = 1f)
    {
        if (isTakeDamage == true)
        {
            isTakeDamage = false;
            float takeDamage;
            int blockedDamage;
            int maxBlockDamage = Mathf.FloorToInt(stats.armor / 5);
            int kickChance = 0;
            StartCoroutine(resetDamageGet());
            if (isAbsoluteHit == false)
            {
                takeDamage = Mathf.Floor((value - stats.minusDMG) - Random.Range(0, maxBlockDamage));
            }
            else
            {
                takeDamage = value;
            }
            blockedDamage = Mathf.FloorToInt(value - takeDamage);
            string deathMessage = ".";
            if (takeDamage >= curHP) deathMessage = ", he is death.";
            LogUI.Instance.Loger("You hit " + enemyTupe.Name + " <color=red>" + takeDamage + " dmg</color>, <color=teal>" + blockedDamage + " armor</color>" + deathMessage);
            if (takeDamage >= 0)
            {
                if (_sound != null) _sound.StartSound(SoundType.Hit);
                if (PhotonNetwork.offlineMode != true) { photonView.RPC("MinusDamage", PhotonTargets.All, (float)takeDamage); }
                else curHP -= takeDamage;
                if (enemyTupe.enemyType != EnemyFightingType.DistanceCrosser)
                {
                    kickChance = Mathf.FloorToInt((100 - (1 + Mathf.Sqrt(stats.armor / takeDamage) + 10)));
                    int random = Random.Range(0, 100);
                    _sound.StartSound(SoundType.Step);
                    if (random < kickChance)
                    {
                        if (PhotonNetwork.offlineMode) enemyController.KickBack(kickStrenght);
                        else photonView.RPC("SendKickBack", PhotonTargets.All, (float)kickStrenght);
                    }
                }
                else
                {
                    if (!CheckIsDeath())
                    {
                        Vector3 vector3 = BelongRoom.GetRandomTeleportPointRoom();
                        float[] newPos = new float[3];
                        newPos[0] = vector3.x; newPos[1] = vector3.y; newPos[2] = vector3.z;
                        // Далее отправляю по сети массив newPos 
                        if (!PhotonNetwork.offlineMode) photonView.RPC("SendPosition", PhotonTargets.AllBuffered, (float[])newPos);
                        else SendPosition(newPos);
                    }
                }
            }
            Debug.Log("AllDMG: " + value + " . Hit: " + takeDamage + ". Armor: " + stats.armor + ". KickChance: " + kickChance + ". KickStrenght: " + kickStrenght);
            // умирает ли ИИ
            if (CheckIsDeath()) return true;
            else return false;

        }
        UpdateHP();
        return false;
    }
    [PunRPC]
    private void SendPosition(float[] newPos)
    {
        this.transform.position = new Vector3(newPos[0], newPos[1], newPos[2]);
        enemyController.LookAtActiveTarget();
    }
    [PunRPC]
    private void SendKickBack(float kickStrenght)
    {
        enemyController.KickBack(kickStrenght);
    }

    [PunRPC]
    public void MinusDamage(float damage)
    {
        curHP -= damage;
        UpdateHP();
    }
    private bool CheckIsDeath()
    {
        if (curHP <= 0)
        {
            if (IsDeath) return true;
            else
            {
                curHP = 0;
                if (PhotonNetwork.offlineMode != true) { photonView.RPC("DeathEnemy", PhotonTargets.AllBuffered); }
                else DeathEnemy();
                return true;
            }
        }
        else return false;
    }
    public void UpdateHP()
    {
        if (curHP > stats.HP)
        {
            curHP = stats.HP;
        }
        CheckIsDeath();
        UpdateConditionStatsUI();
    }
    public int GetLevel()
    {
        return stats.Level;
    }
    [PunRPC]
    private void DeathEnemy()
    {
        if (!IsDeath)
        {
            SpawnDrop();
            IsDeath = true;
            if (BelongRoom != null) BelongRoom.DefeatEnemyInRoom();
            if (_mapPoint != null) _mapPoint.gameObject.SetActive(false);
            if (EnemyGUI != null) EnemyGUI.gameObject.SetActive(false);
            _sound.StartSound(SoundType.Death);
            enemyController.Death();
        }
    }
    private void SpawnDrop()
    {
        if (PhotonNetwork.isMasterClient)
        {
            LootDrop lootDrop = GetComponent<LootDrop>();
            lootDrop.CreateDrop();
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
    public Stats GetStats()
    {
        if (stats != null) return stats;
        else return null;
    }
    // обнуление возможности полуения урона
    private IEnumerator resetDamageGet()
    {
        yield return new WaitForSecondsRealtime(enemyTupe.timeResetHit);
        {
            isTakeDamage = true;
        }
    }
}
