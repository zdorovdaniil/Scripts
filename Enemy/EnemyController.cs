using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;


public class EnemyController : MonoBehaviour
{
    public GameObject EnemySkin;
    public GameObject Arrow;
    public Action actionDoing; // переменная содержит метод конфигуриции поведения противника, 
    public bool isDeath;
    private bool isKecked;
    private Vector3 startPos;
    [Header("Parametrs of Enemy")]
    [SerializeField] private float timeToResetKick = 1f; // время через которое противник придет в себя после попадания в него удара
    [SerializeField] private Vector2 RandomValueBetweenAttack = new Vector2(1.3f, 1.3f);
    [SerializeField] private float timeBetweenAttack; // время между сериями атак
    [SerializeField] private float attackDuration = 1.2f;
    [SerializeField] private float speedRotation = 5f; // скорость поворота к игроку
    [SerializeField] private float speedMoving; // скорость движения
    [SerializeField] private float lookRadius; // радиус видимости игроков
    [SerializeField] private float attackRadius; // радиус с которого притивник начинает атаковать
    [SerializeField] private float longAttackRadius = 6f; // радиус с которого притивник начинает атаковать дальней атакой
    [SerializeField] private int countAttackAnim = 1; // количество анимаций атаки
    [SerializeField] private bool isAvaibleLongAttack = false; // доступна ли анимация для длиннрой атаки
    private Transform activeTarget; // Цель с которой враг взаимодействует
    private PlayerStats activePlStats; // PlayerStats цели с которой враг взаимодействует
    private PlayerStats pl1;
    private PlayerStats pl2;
    private Transform target_1;
    private Transform target_2;
    [SerializeField] private float distanceToPlayer; // расстояние до игрока

    [SerializeField] private List<DamageZone> damageZone = new List<DamageZone>();
    [SerializeField] private float timer;
    [SerializeField] private bool isFirstAttack;
    [SerializeField] private bool isControlLocal; // задает контроль над поведением противника

    private GameManager gameManager;
    private Animator anim;
    private NavMeshAgent agent;

    private void Start()
    {
        startPos = this.gameObject.transform.position;
        anim = EnemySkin.GetComponent<Animator>();
        gameManager = FindObjectOfType<GameManager>();
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speedMoving;
        if (!SetTargets()) isDeath = true;
        else isDeath = false;
        NewTimeBetweenAttack();
    }
    private void NewTimeBetweenAttack()
    { timeBetweenAttack = UnityEngine.Random.Range(RandomValueBetweenAttack.x, RandomValueBetweenAttack.y); }
    public void SetActionDoing(EnemyFightingType type)
    {
        if (PhotonNetwork.isMasterClient || PhotonNetwork.offlineMode) isControlLocal = true;
        else
        {
            actionDoing = Inaction; isControlLocal = false; return;
        }
        if (type == EnemyFightingType.Distance || type == EnemyFightingType.DistanceCrosser)
            actionDoing = EnemyDoingDistance;
        else actionDoing = EnemyDoingMelee;
    }
    private void Inaction()
    { return; }
    private bool SetTargets()
    {
        if (gameManager.GetPlayerIndex(0) != null)
        {
            target_1 = gameManager.GetPlayerIndex(0).gameObject.transform;
            pl1 = gameManager.GetPlayerIndex(0).gameObject.GetComponent<PlayerStats>();
        }
        if (gameManager.GetPlayerIndex(1) != null)
        {
            target_2 = gameManager.GetPlayerIndex(1).gameObject.transform;
            pl2 = gameManager.GetPlayerIndex(1).gameObject.GetComponent<PlayerStats>();
        }
        else
        {
            target_2 = null;
            pl2 = null;
        }
        if (!target_1 && !target_2)
        { return false; }
        else return true;
    }
    private float distanceToSpawn;
    private float distanceToPlayer1;
    private float distanceToPlayer2;
    public void LookAtActiveTarget()
    {
        this.transform.LookAt(activeTarget);
    }
    private void SelectActiveTarget()
    {
        // дистанция до спавна
        if (startPos != null) distanceToSpawn = Vector3.Distance(startPos, transform.position);
        // дистанция до 1 противника
        distanceToPlayer1 = Vector3.Distance(target_1.position, transform.position);
        // если мультиплеер т.е. на карте присутствуют 2 игрока
        if (target_2 != null)
        {
            distanceToPlayer2 = Vector3.Distance(target_2.position, transform.position);
            if (distanceToPlayer1 < distanceToPlayer2 && pl1.IsDeath == false)
            {
                activeTarget = target_1;
                activePlStats = pl1;
            }
            else if (pl2.IsDeath == false)
            {
                activeTarget = target_2;
                activePlStats = pl2;
            }
        }
        else if (pl1.IsDeath == false)
        {
            activeTarget = target_1;
            activePlStats = pl1;
        }
    }
    private float timerTick;
    private void FixedUpdate()
    {
        if (!isDeath)
        {
            if (isKecked != true)
            {
                anim.SetBool("Hit", false);
                // установление ближней цели к противнику
                SelectActiveTarget();
                // перемещение, поворот, атака
                if (isControlLocal) actionDoing.Invoke();
            }
            else
            {
                anim.SetInteger("Attack", 0);
                anim.SetBool("Hit", true);
            }
        }
    }
    private void EnemyDoingDistance()
    {
        // определение расстояния между противником и игроком
        distanceToPlayer = Vector3.Distance(activeTarget.position, transform.position);
        if (distanceToPlayer <= attackRadius && activePlStats.IsDeath == false)
        {
            agent.SetDestination(transform.position);
            // Атака цели
            if (!EnemyAttack()) anim.SetBool("Idle", true); ;
            // Поворот к цели
            FaceToTarget();
            anim.SetBool("Run", false);
        }
        else if (distanceToPlayer <= lookRadius && activePlStats.IsDeath == false)
        {
            isFirstAttack = true;
            anim.SetInteger("Attack", 0);
            anim.SetBool("Run", true);
            anim.SetBool("Idle", false);
            // установление направления передвижения для противника в сторону игрока
            agent.SetDestination(activeTarget.position);
        }
        else if (distanceToSpawn > 3f && startPos != null)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Idle", false);
            if (startPos != null) agent.SetDestination(startPos);
        }
        else
        {
            anim.SetBool("Run", false);
            anim.SetBool("Idle", true);
        }
    }

    // действие противника при сражении в ближнем бою
    private void EnemyDoingMelee()
    {
        // определение расстояния между противником и игроком
        distanceToPlayer = Vector3.Distance(activeTarget.position, transform.position);
        if (distanceToPlayer <= lookRadius && activePlStats.IsDeath == false)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Idle", false);
            // установление направления передвижения для противника в сторону игрока
            if (anim.GetInteger("Attack") == 0)
            {
                agent.SetDestination(activeTarget.position);
            }
            if (distanceToPlayer <= attackRadius + longAttackRadius && isAvaibleLongAttack && isFirstAttack)
            {
                EnemyAttack();
            }
            else
            // если расстояние до игрока меньше дистанции атаки
            if (distanceToPlayer <= attackRadius)
            {
                // Атака цели
                EnemyAttack();
                // Поворот к цели, тогда когда персонаж не атакует
                if (anim.GetInteger("Attack") == 0)
                {
                    FaceToTarget();
                }
                anim.SetBool("Run", false);
            }
            else
            {
                // если игрок отойдет чуть подальше от противника, то первая атака снова доступна
                if (distanceToPlayer >= attackRadius * 1.25f)
                    isFirstAttack = true;
                anim.SetInteger("Attack", 0);
            }
        }
        else if (distanceToSpawn > 4f && startPos != null)
        {
            anim.SetBool("Run", true);
            anim.SetBool("Idle", false);
            if (startPos != null) agent.SetDestination(startPos);
        }
        else
        {
            anim.SetBool("Run", false);
            anim.SetBool("Idle", true);
        }
    }


    // Отталкивает объект относительно точки с указанной силой
    // Отдача от удара
    public void KickBack(float strenghKich = 1)
    {
        StopCoroutine(ResetKick());
        isKecked = true;
        StartCoroutine(ResetKick());
        Vector3 posOnKick = transform.position;
        // Определяем в каком направлении должен отлететь объект от активной цели
        if (agent.isActiveAndEnabled)
        {
            Vector3 pushDirection = posOnKick - activeTarget.position;
            // Перемещение 
            agent.Move(pushDirection * strenghKich);
            // Толкаем объект в нужном направлении с силой pushPower
            agent.SetDestination(transform.position);
        }

    }

    private bool EnemyAttack()
    {
        timer += Time.deltaTime;
        if (isFirstAttack == true)
        {
            timer = 0;
            int numAttack = 0;
            if (!isAvaibleLongAttack) { numAttack = UnityEngine.Random.Range(1, countAttackAnim + 1); }
            else { numAttack = -1; }
            SetAnimEnemyAttack(numAttack);
            isFirstAttack = false;
            return true;
        }
        else if (timer > timeBetweenAttack)
        {
            timer = 0;
            int numAttack = UnityEngine.Random.Range(1, countAttackAnim + 1);
            SetAnimEnemyAttack(numAttack);
            return true;
        }
        else return false;
    }
    private void FaceToTarget()
    {
        // Полуение направления к цели
        Vector3 direction = (activeTarget.position - transform.position).normalized;
        // Получения вращения, где указывается эта цель
        Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0, direction.z));
        // Обновление вращения противника в полученное направление поворота
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * speedRotation);
    }
    // при выборе данного объекта в редакторе unity, будет нарисован объект
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        // рисование сферы с радисом lookRadius
        Gizmos.DrawWireSphere(transform.position, lookRadius);
    }
    public void Death()
    {
        isDeath = true;
        SwitchDamageZone(false);
        StartCoroutine(EnableComponents());
        StopCoroutine(ResetAttack());
        anim.SetInteger("Attack", 0);
        anim.SetBool("Death", true);
        GetComponent<Collider>().enabled = false;
    }
    public void SwitchDamageZone(bool status)
    {
        foreach (DamageZone zone in damageZone)
        {
            if (zone.gameObject != null)
                zone.gameObject.SetActive(status);
        }
    }

    // воспроизведение анимации атаки
    private void SetAnimEnemyAttack(int _numAttack)
    {
        NewTimeBetweenAttack();
        StopCoroutine(ResetAttack());
        anim.SetInteger("Attack", _numAttack);
        StartCoroutine(ResetAttack());
    }
    public void FlyArrow()
    {
        GameObject arrowObj = GameManager.SpawnArrowIn(transform, Arrow);
        if (arrowObj)
        {
            arrowObj.gameObject.transform.position = new Vector3(arrowObj.transform.position.x, arrowObj.transform.position.y + 2.25f, arrowObj.transform.position.z);
            DamageZone damageZone = arrowObj.GetComponent<DamageZone>();
            EnemyStats enemyStats = GetComponent<EnemyStats>();
            damageZone.enemyStats = enemyStats;
        }
    }
    private IEnumerator ResetAttack()
    {
        yield return new WaitForSecondsRealtime(attackDuration);
        {
            anim.SetInteger("Attack", 0);
        }
    }
    private IEnumerator EnableComponents()
    {
        yield return new WaitForSecondsRealtime(2f);
        {
            agent.enabled = false;
            this.enabled = false;
        }
    }
    private IEnumerator ResetKick()
    {
        yield return new WaitForSecondsRealtime(timeToResetKick);
        {
            isKecked = false;
        }
    }
}
