using System.Collections;
using System.Collections.Generic;
using stats; // Пространство имен stats из скрипта Stats

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
    [SerializeField] private EnemyUI _enemyUI;
    [SerializeField] Transform _mapPoint;
    [SerializeField] private ParticlePlaces _particlePlaces;
    private EnemyController enemyController;
    private Sound _enemySounds;
    private HumanEffects _humanEffects;
    void Start()
    {
        int level = GameManager.Instance.GetDungeonLevel;
        stats = new Stats(level, 0);
        SetAttributesToStats(level);
        curHP = stats.HP;
        enemyController = gameObject.GetComponent<EnemyController>();
        _enemySounds = GetComponent<Sound>();
        _humanEffects = GetComponent<HumanEffects>();
        stats.attackWeapon = enemyTupe.AttackWeapon(level);
        stats.armorEquip = enemyTupe.ArmorClass(level);
        stats.recount();
        enemyController.SetActionDoing(enemyTupe.enemyType);
        isTakeDamage = true;
        _enemyUI.UpdateUI(this, stats);
        Heal();
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
            GlobalEffects.Instance.CreateParticle(_particlePlaces.HitPlace, EffectType.Hit);
            blockedDamage = Mathf.FloorToInt(value - takeDamage);
            string deathMessage = ".";
            if (takeDamage >= curHP) deathMessage = ", he is death.";
            LogUI.Instance.Loger("You hit " + enemyTupe.Name + " <color=red>" + takeDamage + " dmg</color>, <color=teal>" + blockedDamage + " armor</color>" + deathMessage);
            if (takeDamage >= 0)
            {
                if (_enemySounds != null) _enemySounds.StartSound(SoundType.Hit);
                if (PhotonNetwork.offlineMode != true) { photonView.RPC("MinusDamage", PhotonTargets.All, (float)takeDamage); }
                else MinusDamage(takeDamage);
                if (enemyTupe.enemyType != EnemyFightingType.DistanceCrosser)
                {
                    kickChance = Mathf.FloorToInt((100 - (1 + Mathf.Sqrt(stats.armor / takeDamage) + 10)));
                    int random = Random.Range(0, 100);
                    _enemySounds.StartSound(SoundType.Step);
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
        GlobalEffects.Instance.CreateParticle(this.transform, EffectType.TeleportTrail, -1f);
        this.transform.position = new Vector3(newPos[0], newPos[1], newPos[2]);
        enemyController.LookAtActiveTarget();
        GlobalEffects.Instance.CreateParticle(this.transform, EffectType.Teleport, -1.75f);
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
        _enemyUI.UpdateUI(this, stats);
    }
    public void Heal()
    {
        curHP = stats.HP;
        UpdateHP();
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
            Destroy(_enemyUI.gameObject);
            _enemySounds.StartSound(SoundType.Death);
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
    private void SetAttributesToStats(int level)
    {
        int[] levels = new int[] { enemyTupe.Strenght(level), enemyTupe.Agility(level), enemyTupe.Endurance(level), enemyTupe.Speed(level) };
        stats.SetEnemyAttributes(levels);
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
