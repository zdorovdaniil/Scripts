using UnityEngine;
using UnityEngine.Events;

public enum TupeDamageZone
{
    Fire, Lava, Default, Arrow, Kill
}
public class DamageZone : MonoBehaviour
{
    [SerializeField]
    TupeDamageZone m_TupeDamageZone = TupeDamageZone.Default;
    public TupeDamageZone tupeDamageZone { get { return m_TupeDamageZone; } set { m_TupeDamageZone = value; } }

    // Игрок которому принадлежит данная DamageZone, если не указан то принадлежит окр. миру или ИИ
    public PlayerStats playerStats;
    // ИИ которому принадлежит данная DamageZone
    public EnemyStats enemyStats;
    public UnityEvent Event;
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Enemy")
        {
            EnemyStats otherEnemyStats = other.gameObject.GetComponent<EnemyStats>();
            // если damageZone от игрока попадает по ИИ то ему наносится урон
            if (otherEnemyStats != enemyStats && playerStats != null)
            {
                float damagePlayer = playerStats.stats.Damage();
                float kickStrenght = playerStats.stats.KickStrenght();
                bool isDies = false;
                isDies = otherEnemyStats.TakeDamage(damagePlayer, false, kickStrenght);
                if (isDies == true)
                {
                    DungeonStats.Instance.DefeatEnemy();
                    PlayerQuest.instance.UpdateProcessQuests(otherEnemyStats, null, "kill_enemy");
                    if (otherEnemyStats.enemyTupe.IsBoss)
                    {
                        PlayerQuest.instance.UpdateProcessQuests(null, null, "kill_boss");
                    }
                }
            }
        }
        if (other.tag == "Player")
        {
            // получение PlayerStats от другово игрока, по которому пришлось попадание
            PlayerStats otherPlayerStats = other.gameObject.GetComponent<PlayerStats>();

            if (enemyStats != null) // если владелец damageZone ИИ
            {
                if (!enemyStats.IsDeath) // если противник жив, то он наносит урон игроку
                {
                    float damageEnemy = enemyStats.GetStats().Damage();
                    otherPlayerStats.TakeDamage(damageEnemy, false, enemyStats.enemyTupe.Name);
                }
            }
            else
            {
                if (tupeDamageZone == TupeDamageZone.Default)
                    return;
                if (tupeDamageZone == TupeDamageZone.Fire)
                {
                    int damage = Random.Range(1, 3);
                    otherPlayerStats.TakeDamage(damage, true, "Fire");
                }
                if (tupeDamageZone == TupeDamageZone.Lava)
                {
                    int damage = Random.Range(5, 10);
                    otherPlayerStats.TakeDamage(damage, true, "Lava");
                }
                if (tupeDamageZone == TupeDamageZone.Arrow)
                {
                    if (enemyStats != null)
                    {
                        float damageEnemy = enemyStats.GetStats().Damage();
                        otherPlayerStats.TakeDamage(damageEnemy, false, enemyStats.enemyTupe.Name);
                    }
                    else
                    {
                        int damage = Random.Range(5, 10);
                        otherPlayerStats.TakeDamage(damage, false, "Arrow");
                    }
                }
                if (tupeDamageZone == TupeDamageZone.Kill)
                {
                    int damage = 500;
                    otherPlayerStats.TakeDamage(damage, true, "Trap");
                }
            }
        }

        if (other.tag == "BreakAbleItem")
        {
            Destroy(other.gameObject);
        }
    }
}
