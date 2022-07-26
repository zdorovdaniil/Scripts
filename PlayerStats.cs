using stats; // Пространство имен stats из скрипта Stats
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStats : Photon.MonoBehaviour
{
    [SerializeField] private bool _isUnDeath;
    public Stats stats;
    public string NickName;
    public int IdStats; // присваивается случайно при каждой игре
    public bool IsDeath;
    private int PointStat; public int GetPointStat => PointStat;
    private int PointSkill; public int GetPointSkill => PointSkill; public void MinusPointSkill(int value = 1) { PointSkill -= value; }
    public float curHP; // Количество жизней персонаж нынешние
    public int curEXP; // Количество опыта
    private bool isTakeDamage; //для ограниечения получаемого урона в секунду
    private int _ping; public int GetPing => _ping;
    // Сетевые переменные
    private bool server_IsDeath;
    private float server_curHP;
    private float server_HP;
    private int server_Ping;
    //
    private Inventory _inventory; public Inventory GetInventory => _inventory;
    private Vector3 _spawnPosition;
    private RoomControl curRoom; public void SetCurRoom(RoomControl room) { curRoom = room; }

    private void Awake() { SetStats(); }
    private void Start()
    {
        _inventory = GetComponent<Inventory>();
        IdStats = Random.Range(1, 1000);
        isTakeDamage = true;
        ChangeSpeed();
        _spawnPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }

    // обновление сетевых переменных
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.isWriting)
        {
            _ping = PhotonNetwork.networkingPeer.RoundTripTime;
            server_IsDeath = IsDeath;
            stream.SendNext(server_IsDeath);
            server_curHP = curHP;
            stream.SendNext(server_curHP);
            server_HP = stats.HP;
            stream.SendNext(server_HP);
            server_Ping = _ping;
            stream.SendNext(server_Ping);
        }
        if (stream.isReading)
        {
            server_IsDeath = (bool)stream.ReceiveNext();
            IsDeath = server_IsDeath;
            server_curHP = (float)stream.ReceiveNext();
            curHP = server_curHP;
            server_HP = (float)stream.ReceiveNext();
            stats.HP = server_HP;
            server_Ping = (int)stream.ReceiveNext();
            _ping = server_Ping;
        }
    }
    // задание игровых статов при загрузке
    public void SetStats()
    {
        int _id = PlayerPrefs.GetInt("activeSlot");
        if (PlayerPrefs.GetInt(_id + "_for_new_game") == 1)
        {
            // загрузка атрибутов из сохранения
            Debug.Log("Active slot" + _id);
            int _lvl = PlayerPrefs.GetInt(_id + "_slot_level");
            int _exp = (int)Mathf.Floor(PlayerPrefs.GetFloat(_id + "_slot_exp"));
            int _str = PlayerPrefs.GetInt(_id + "_slot_strenght");
            int _agi = PlayerPrefs.GetInt(_id + "_slot_agility");
            int _vit = PlayerPrefs.GetInt(_id + "_slot_endurance");
            int _speed = PlayerPrefs.GetInt(_id + "_slot_speed");
            PointStat = PlayerPrefs.GetInt(_id + "_slot_point");
            PointSkill = PlayerPrefs.GetInt(_id + "_slot_pointSkill");
            curEXP = PlayerPrefs.GetInt(_id + "_slot_curEXP");
            // создание нового экземпляра stats из загруженных данных
            stats = new Stats(_lvl, _exp, _str, _agi, _vit, _speed);
            DungeonStats.Instance.LoadDungeonStats();
            curHP = stats.HP;
            foreach (Skill skill in BasePrefs.instance.AvaibleSkills)
            {
                skill.LoadSkill();
            }
            stats.Skills = BasePrefs.instance.AvaibleSkills;
        }
        else
        {
            New_Game(_id);
        }

    }

    // получение урона
    public void TakeDamage(float _value, bool isAbsoluteHit = false, string hitName = "")
    {
        if (isTakeDamage == true && _isUnDeath == false)
        {
            isTakeDamage = false;
            StartCoroutine(resetDamageGet());
            float takeDamage;
            float blockedDamage;
            float maxBlockDamage = stats.GetMaxBlockDamage();

            if (isAbsoluteHit == false)
            {
                takeDamage = Mathf.Floor((_value - stats.minusDMG) - Random.Range(0, maxBlockDamage));
            }
            else
            {
                takeDamage = Mathf.Floor(_value);
            }
            blockedDamage = Mathf.Floor(_value - takeDamage);
            LogUI.Instance.Loger(hitName + " hit you <color=red>" + takeDamage + " dmg</color>, <color=teal>" + blockedDamage + "</color> armor");
            if (takeDamage >= 0)
            {
                curHP -= takeDamage;
                gameObject.GetComponent<Sound>().StartSound(SoundType.Hit);
            }
            UpdateHP();
        }
    }
    // обнуление возможности полуения урона
    private IEnumerator resetDamageGet()
    {
        yield return new WaitForSecondsRealtime(0.2f);
        {
            isTakeDamage = true;
        }
    }
    // задание параметров при новой игре
    public void AddStandartPoints()
    {
        PointStat = 10;
        PointSkill = 10;
    }
    private void New_Game(int _id)
    {
        Debug.Log("New Game");
        stats = new Stats(1, 800, 10, 10, 10, 10);
        AddStandartPoints();
        curHP = 30;
        curEXP = 0;
        foreach (Skill skill in BasePrefs.instance.AvaibleSkills)
        {
            skill.ResetSkill();
        }
        stats.Skills = BasePrefs.instance.AvaibleSkills;
        PlayerPrefs.SetInt(_id + "_for_new_game", 1);
        PlayerPrefs.SetInt(_id + "_slot_dungeonLevel", 1);
        DungeonStats.Instance.ResetDungeonStats();
        SaveStatsToSlot(_id, stats);
    }
    // Обновление уровня в stats
    public void PlayerLevelUp()
    {
        stats.lvlUP();
        curEXP = 0;
        PointStat += 3;
        PointSkill += 3;
        HealPlayer();
    }
    // начисление опыта
    public void GainExperience(int _value)
    {
        curEXP += _value;
    }
    // восстановление здоровья персонажа
    public void HealPlayer()
    {
        curHP = stats.HP;
        UpdateHP();
    }
    public void AddHP(int arg = 0)
    {
        if (IsDeath != true) { curHP += arg; }
        UpdateHP();
    }
    public bool TryUpgradeAttrubute()
    {
        if (PointStat > 0)
        { return true; }
        else
        { return false; }
    }
    // повышение атрибута(i_attr - порядковый номер атрибута)
    public bool UpAttribute(int i_attr, bool isMinusPointStat = true)
    {
        if (TryUpgradeAttrubute() || !isMinusPointStat)
        {
            if (i_attr == 0) { stats.Strenght += 1; } // повышение силы
            if (i_attr == 1) { stats.Agility += 1; } // ловкости
            if (i_attr == 2) { stats.Endurance += 1; }// живучести
            if (i_attr == 3) { stats.Speed += 1; } // скорости
            if (isMinusPointStat == true)
            { PointStat -= 1; }
            if (i_attr == 2) { HealPlayer(); }
            ChangeSpeed();
            stats.recount();
            return true;
        }
        else
        { return false; }
    }
    public int GetLevelAttribute(int i_attr)
    {
        int n = 0;
        if (i_attr == 0) { n = stats.Strenght; } // повышение силы
        if (i_attr == 1) { n = stats.Agility; } // ловкости
        if (i_attr == 2) { n = stats.Endurance; }// живучести
        if (i_attr == 3) { n = stats.Speed; } // скорости
        return n;
    }
    public void ChangeSpeed()
    {
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            float speed = stats.moveSpeed;
            playerController.ChangeAlwaysSpeed(speed);
        }
    }
    [PunRPC]
    public void AddBuff(int id)
    {
        BasePrefs basePrefs = BasePrefs.instance;
        BuffClass buff = basePrefs.GetBuffId(id);
        if (buff != null)
        {
            stats.AddBuff(buff);
        }
        else Debug.Log("Null Buff");
        Debug.Log("Add buff" + BasePrefs.instance.GetBuffId(id));
    }

    // понижение атрибута
    public bool DownAttribute(int i_attr)
    {
        if (i_attr == 0 && stats.Strenght > 10) { stats.Strenght -= 1; stats.recount(); return true; } // понижение силы
        if (i_attr == 1 && stats.Agility > 10) { stats.Agility -= 1; stats.recount(); return true; } // ловкости
        if (i_attr == 2 && stats.Endurance > 10) { stats.Endurance -= 1; stats.recount(); return true; }// живучести
        if (i_attr == 3 && stats.Speed > 10) { stats.Speed -= 1; stats.recount(); return true; } // скорости

        return false;
    }
    // Обновление атаки в stats
    public void CheckStatusEquip()
    {
        int attackWeapon = 0;
        if (_inventory.GetEquipSlot(0) != null)
        { attackWeapon += _inventory.GetEquipSlot(0).item.Attack; }
        if (_inventory.GetEquipSlot(5) != null)
        { attackWeapon += _inventory.GetEquipSlot(5).item.Attack; }
        if (_inventory.GetEquipSlot(6) != null)
        { attackWeapon += _inventory.GetEquipSlot(6).item.Attack; }
        stats.SetAttackWeapon(attackWeapon);
        stats.newArmDmg();
    }
    private void DeathPlayer()
    {
        IsDeath = true;
        stats.ResetAllBuff();
        ChangeSpeed();
        gameObject.GetComponent<Sound>().StartSound(SoundType.Death);
        PlayerController plControl = GetComponent<PlayerController>();
        GUIControl gUIControl = GUIControl.Instance;
        if (photonView.isMine)
        {
            if (gUIControl.DeathWindow.activeSelf == false)
            {
                gUIControl.SwitchAllPanels(false);
                plControl.PlayerDeath();
                StopCoroutine(DeathCont());
                StartCoroutine(DeathCont());
            }
        }
        if (curRoom != null) curRoom.DeathInRoom(this.gameObject); // отключение заблокированных проходов в комнате где умер игрок
        // включение окна послесмертия через несколько секунт после смерти
        IEnumerator DeathCont()
        {
            yield return new WaitForSecondsRealtime(2f);
            {
                gUIControl.DeathWindow.SetActive(true);
                this.transform.position = _spawnPosition;
            }
        }
    }
    // обновление брони в stats
    public void UpdateArmor()
    {
        int armor = 0;
        int equipModif = 0;
        for (int i = 1; i < 7; i++)
        {
            InventorySlot eqArmor = _inventory.GetEquipSlot(i);
            if (eqArmor == null) continue;
            armor += eqArmor.item.Armor;
            if (eqArmor.item.armorTupe != ArmorTupe.Amulet || eqArmor.item.armorTupe != ArmorTupe.Ring)
                equipModif += 1;
        }
        stats.SetArmorEquip(armor, equipModif);
        stats.newArmDmg();
    }
    // Обновление здоровья
    public void UpdateHP()
    {
        if (curHP > stats.HP)
        {
            curHP = stats.HP;
        }
        if (curHP <= 0 && IsDeath == false)
        {
            DeathPlayer();
        }
    }
    public void SaveStatsToSlot(int _slot, Stats _stats)
    {
        if (_stats != null)
        {
            PlayerPrefs.SetInt(_slot + "_for_new_game", 1);
            PlayerPrefs.SetInt(_slot + "_slot_level", _stats.Level);
            PlayerPrefs.SetFloat(_slot + "_slot_exp", _stats.EXP);
            PlayerPrefs.SetInt(_slot + "_slot_strenght", _stats.Strenght);
            PlayerPrefs.SetInt(_slot + "_slot_agility", _stats.Agility);
            PlayerPrefs.SetInt(_slot + "_slot_endurance", _stats.Endurance);
            PlayerPrefs.SetInt(_slot + "_slot_speed", _stats.Speed);
            PlayerPrefs.SetInt(_slot + "_slot_point", PointStat);
            PlayerPrefs.SetInt(_slot + "_slot_pointSkill", PointSkill);
            PlayerPrefs.SetFloat(_slot + "_slot_curHP", curHP);
            PlayerPrefs.SetInt(_slot + "_slot_curEXP", curEXP);
            DungeonStats.Instance.SaveDungeonStats();
            // Сохранение навыков
            foreach (Skill skill in BasePrefs.instance.AvaibleSkills)
            {
                skill.SaveSkill();
            }
        }
    }
}

