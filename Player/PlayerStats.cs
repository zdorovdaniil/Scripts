using stats;
using System.Collections;
using UnityEngine;

public class PlayerStats : Photon.MonoBehaviour
{
    public Stats stats;
    [SerializeField] private string _nickName; public void SetNickName(string text) => _nickName = text; public string GetNickName => _nickName;
    public bool IsDeath;
    private int PointStat; public int GetPointStat => PointStat;
    private int PointSkill; public int GetPointSkill => PointSkill; public void MinusPointSkill(int value = 1) { PointSkill -= value; }
    public float curHP; // Количество жизней персонаж нынешние
    public int curEXP; // Количество опыта
    private bool isTakeDamage; //для ограниечения получаемого урона в секунду

    private Inventory _inventory; public Inventory GetInventory => _inventory;
    private Vector3 _spawnPosition;
    private HumanEffects _playerEffects;
    [SerializeField] private ParticlePlaces _particlePlaces;
    private RoomControl curRoom; public void SetCurRoom(RoomControl room) { curRoom = room; }

    // Сетевые переменные
    private int _ping; public int GetPing => _ping;
    private bool server_IsDeath;
    private float server_curHP;
    private float server_HP;
    private int server_Ping;

    public void ReturnToRespawn()
    {
        float[] vector = ProcessCommand.ConvertVector3ToFloat(_spawnPosition);
        if (PhotonNetwork.offlineMode)
            TeleportTo(vector);
        else photonView.RPC("TeleportTo", PhotonTargets.AllBuffered, (float[])vector);
    }

    [PunRPC]
    public void TeleportTo(float[] pos)
    {
        GlobalSounds.Instance.PTeleportPlayer(this.transform);
        GlobalEffects.Instance.CreateParticle(this.transform, EffectType.TeleportTrail);
        this.transform.position = new Vector3(pos[0], pos[1], pos[2]);
        Debug.Log("Teleport To: " + pos[0] + ":" + pos[1] + ":" + pos[2]);
        GlobalEffects.Instance.CreateParticle(this.transform, EffectType.Teleport);
    }
    public void SetInventory(Inventory inv) { _inventory = inv; _inventory.SetInvSlotsFromItemsIDs(); CheckStatusEquip(); UpdateArmor(); }
    private void Awake() { SetStats(); }
    private void Start()
    {
        _playerEffects = GetComponent<HumanEffects>();
        _inventory = GetComponent<Inventory>();
        isTakeDamage = true;
        ChangeSpeed();
        _spawnPosition = transform.position;
        PlayerLeveling.Instance.SetPlayerStats(this);
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
            PointStat = PlayerPrefs.GetInt(_id + "_slot_point");
            PointSkill = PlayerPrefs.GetInt(_id + "_slot_pointSkill");
            curEXP = PlayerPrefs.GetInt(_id + "_slot_curEXP");
            // создание нового экземпляра stats из загруженных данных
            stats = new Stats(_lvl, _exp);
            DungeonStats.Instance.LoadDungeonStats();
            foreach (Skill skill in BasePrefs.instance.AvaibleSkills)
            {
                skill.Load();
            }
            stats.SetPlayerAttributes();
            curHP = stats.HP;
        }
        else
        {
            New_Game(_id);
        }
        stats.Skills = BasePrefs.instance.AvaibleSkills;
        stats.recount();
        HealPlayer();
    }
    // получение урона
    public void TakeDamage(float value, bool isAbsoluteHit = false, string hitName = "", float critValue = 0f, float critChance = 0f)
    {
        if (isTakeDamage == true)
        {
            isTakeDamage = false;
            StartCoroutine(ResetDamageGet());
            bool isCrit = false;
            float takeDamage;
            float maxBlockDamage = stats.GetMaxBlockDamage();
            int random = Random.Range(0, 100);

            if (random < critChance)
            {
                isAbsoluteHit = true;
                takeDamage = Mathf.Floor((value * (critValue / 100)) * 100.00f) * 0.01f;
                isCrit = true;
                GlobalSounds.Instance.PCrit(transform);
            }
            else if (isAbsoluteHit == false)
            {
                takeDamage = Mathf.Floor((value - stats.minusDMG) - Random.Range(0, maxBlockDamage));
            }
            else
            {
                takeDamage = Mathf.Floor(value);
            }
            if (takeDamage >= 0)
            {
                curHP -= takeDamage;
                gameObject.GetComponent<Sound>().StartSound(SoundType.Hit);
                GlobalEffects.Instance.CreateParticle(_particlePlaces.HitPlace, EffectType.Hit);
            }
            float blockedDamage = Mathf.Floor(value - takeDamage);
            string HitString = "";
            if (isCrit) HitString = " crit you!"; else { HitString = " hit you"; }
            Debug.Log("Crit % = " + critChance + " Crit Value = " + critValue);
            LogUI.Instance.Loger(hitName + HitString + " <color=red>" + takeDamage + " dmg</color>, <color=teal>" + blockedDamage + "</color> armor");
            UpdateHP();
        }
        IEnumerator ResetDamageGet()
        {
            yield return new WaitForSecondsRealtime(0.2f);
            {
                isTakeDamage = true;
            }
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
        stats = new Stats(1, 800);
        stats.SetPlayerAttributes();
        AddStandartPoints();
        curHP = 30;
        curEXP = 0;
        foreach (Skill skill in BasePrefs.instance.AvaibleSkills)
        { skill.Reset(); }
        PlayerPrefs.SetInt(_id + "_for_new_game", 1);
        ProcessCommand.SetDungeonLevel(1);
        ProcessCommand.SetMaxDungeonLevel(1);
        DungeonStats.Instance.ResetDungeonStats();
        SaveStatsToSlot(_id, stats);
    }
    // Обновление уровня в stats
    public void PlayerLevelUp()
    {
        GlobalSounds.Instance.SLevelUp();
        stats.lvlUP();
        curEXP = 0;
        PointStat += 3;
        PointSkill += 3;
        HealPlayer();
    }
    // начисление опыта
    public void GainExperience(int _value)
    {
        int withMod = stats.ModifGainExp(_value);
        Debug.Log("Gain exp: " + _value + " Total with modifaed: " + withMod);
        curEXP += withMod;
        if (curEXP >= stats.EXP) GlobalSounds.Instance.SAwaibleLvlUp();
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
            stats.Attributes[i_attr].Level += 1;
            if (isMinusPointStat == true)
            { PointStat -= 1; }
            if (i_attr == 2) { HealPlayer(); }
            ChangeSpeed();
            CheckStatusEquip();
            stats.recount();
            return true;
        }
        else
        { return false; }
    }
    public int GetLevelAttribute(int i_attr)
    {
        return stats.Attributes[i_attr].Level;
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
    public void AddBuffPlayer(int id)
    {
        BuffDatabase buffDatabase = BasePrefs.instance.GetBuffDatabase;
        BuffClass buff = buffDatabase.GetBuff(id);
        if (buff != null)
        {
            stats.AddBuff(buff);
            _playerEffects.ActivateEffect(buff);
        }
        else Debug.Log("Null Buff");
        Debug.Log("Add buff: " + id.ToString() + "with id: " + buff.BuffId);
        ChangeSpeed();
    }

    // понижение атрибута
    public bool DownAttribute(int i_attr)
    {
        if (stats.Attributes[i_attr].Level > 10)
        {
            stats.Attributes[i_attr].Level -= 1;
            stats.recount();
            return true;
        }
        else return false;
    }
    // Обновление атаки в stats
    public void CheckStatusEquip()
    {
        if (_inventory)
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
    }
    private void DeathPlayer()
    {
        IsDeath = true;
        stats.ResetAllBuff();
        if (_playerEffects) _playerEffects.DiactivateEffects();
        ChangeSpeed();
        DungeonStats.Instance.AddDeath();
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
                ReturnToRespawn();
            }
        }
    }
    // обновление брони в stats
    public void UpdateArmor()
    {
        if (_inventory)
        {
            int armor = 0;
            int critValue = 0;
            int critChance = 0;
            int equipModif = 0;
            for (int i = 0; i < 7; i++)
            {
                InventorySlot eqArmor = _inventory.GetEquipSlot(i);
                if (eqArmor == null) continue;
                armor += eqArmor.item.Armor;
                critChance += eqArmor.item.CritChance;
                critValue += eqArmor.item.CritValue;
                if (eqArmor.item.armorTupe != ArmorTupe.Amulet || eqArmor.item.armorTupe != ArmorTupe.Ring)
                    equipModif += 1;
            }
            stats.SetArmorEquip(armor, equipModif);
            stats.SetCritEquip(critChance, critValue);
        }
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
            PlayerPrefs.SetInt(_slot + "_slot_point", PointStat);
            PlayerPrefs.SetInt(_slot + "_slot_pointSkill", PointSkill);
            PlayerPrefs.SetFloat(_slot + "_slot_curHP", curHP);
            PlayerPrefs.SetInt(_slot + "_slot_curEXP", curEXP);
            DungeonStats.Instance.SaveDungeonStats();
            // Сохранение навыков
            foreach (Skill skill in BasePrefs.instance.AvaibleSkills)
            {
                skill.Save();
            }
            stats.SaveAttributes();
        }
    }
}
public class InfoPlayerStats
{
    public string _strenghText;
    public string _enduranceText;
    public string _agilityText;
    public string _speedText;

    public string _attackText;
    public string _attackEquipText;
    public string _attackAttrText;
    public string _attackSkillText;
    public string _attackBuffText;

    public string _critChanceText;
    public string _critChanceEquipText;
    public string _critChanceSkillText;
    public string _critChanceBuffText;

    public string _critValueText;
    public string _critValueEquipText;
    public string _critValueSkillText;
    public string _critValueBuffText;

    public string _defenceText;
    public string _defenceEquipText;
    public string _defenceAttrText;
    public string _defenceSkillText;
    public string _defenceBuffText;

    public string _moveSpeedText;
    public string _moveSpeedAttrText;
    public string _moveSpeedBuffText;

    public string _healthText;
    public string _regenValue;
    public string _timeRegen;

    public string _blockText;
    public string _damageText;
    public string _kickStrenght;


    public string _curLvlText;
    public string _curExpText;
    public InfoPlayerStats(PlayerStats plstast)
    {
        _strenghText = plstast.stats.Attributes[0].Level.ToString();
        _enduranceText = plstast.stats.Attributes[1].Level.ToString();
        _agilityText = plstast.stats.Attributes[2].Level.ToString();
        _speedText = plstast.stats.Attributes[3].Level.ToString();

        _attackText = plstast.stats.attack.ToString();
        _attackEquipText = plstast.stats.attackWeapon.ToString();
        _attackAttrText = plstast.stats.GetAttackAttr().ToString();
        _attackSkillText = plstast.stats.attackSkill.ToString();
        _attackBuffText = plstast.stats.buffAttack.ToString();

        _critChanceText = plstast.stats.critChance.ToString() + " %";
        _critChanceEquipText = plstast.stats.critChanceEquip.ToString() + " %";
        _critChanceSkillText = plstast.stats.critChanceSkill.ToString() + " %";
        _critChanceBuffText = plstast.stats.buffCritChance.ToString() + " %";

        _critValueText = plstast.stats.critValue.ToString() + " %";
        _critValueEquipText = plstast.stats.critValueEquip.ToString() + " %";
        _critValueSkillText = plstast.stats.critValueSkill.ToString() + " %";
        _critValueBuffText = plstast.stats.buffCritValue.ToString() + " %";

        _defenceText = plstast.stats.armor.ToString();
        _defenceEquipText = plstast.stats.armorEquip.ToString();
        _defenceAttrText = plstast.stats.GetDefenceAttr().ToString();
        _defenceSkillText = plstast.stats.armorSkill.ToString();
        _defenceBuffText = plstast.stats.buffDefence.ToString();

        _moveSpeedText = plstast.stats.moveSpeed.ToString();
        _moveSpeedAttrText = plstast.stats.moveSpeedAttr.ToString();
        _moveSpeedBuffText = plstast.stats.buffSpeed.ToString();

        _healthText = plstast.stats.HP.ToString();
        _regenValue = plstast.stats.GetAddHP().ToString() + " HP";
        _timeRegen = plstast.stats.GetTimeRegenHP().ToString() + " SEC";

        _blockText = plstast.stats.minusDMG.ToString() + " to " + plstast.stats.GetMaxBlockDamage().ToString();
        _damageText = plstast.stats.minDMG.ToString() + " to " + plstast.stats.maxDMG.ToString();
        _kickStrenght = plstast.stats.KickStrenght().ToString();


        _curLvlText = plstast.stats.Level.ToString();
        string curExp = plstast.curEXP.ToString();
        string needExp = PlayerLeveling.Instance.GetHeedExp(plstast.stats.Level).ToString();
        _curExpText = curExp + " / " + needExp;

    }
}

