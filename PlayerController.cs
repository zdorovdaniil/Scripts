using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : Photon.MonoBehaviour
{
    [SerializeField] private float _defaultSpeed; // скорость персонажа по стандарту
    [SerializeField] private float _curSpeedMove; // скорость персонажа изменяемая от прокачки
    [SerializeField] private float _speedRotation; // скорость поворота персонажа

    [SerializeField] private GameObject WeaponZone; // damage зона оружия

    private Animator anim;
    private CharacterController ch_control;
    [SerializeField] private AttackController att_control;

    private float gravityForce; // гравитация персонажа
    private Vector3 moveVector; // направление движения персонажа                                       
    private Vector3 direct; // направление поворота игрока

    private float _horizontal;
    private float _vertical;
    [SerializeField] private FixedJoystick _fixedJoystick;
    private float curSpeed;
    // параметры боя
    public bool isAttack;
    [SerializeField] private GameObject _particleObj;

    private void Awake()
    {
        _curSpeedMove = _defaultSpeed;
    }
    public void SpawnParticle(int isInverse = 0)
    {
        Vector3 spawnPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        GameObject obj = Instantiate(_particleObj, spawnPos, transform.rotation).gameObject;
        if (isInverse != 0) { obj.transform.Rotate(0, 0, 180); }
        StartCoroutine(DestroyParticle(obj));
    }
    private IEnumerator DestroyParticle(GameObject obj)
    {
        yield return new WaitForSecondsRealtime(0.6f);
        {
            Destroy(obj);
        }
    }
    void Start()
    {
        anim = GetComponent<Animator>();
        ch_control = GetComponent<CharacterController>();
    }

    public void FixedUpdate()
    {
        if ((photonView.isMine) || !(PhotonNetwork.isMasterClient || PhotonNetwork.isNonMasterClientInRoom))
        {
            PlayerMoving();
            GamingGravity();
        }
        else
        {
            return;
        }
        SwitchDamageZone(); // обновление DamageZone данного игрока

    }
    public void SetJerk()
    {
        anim.SetTrigger("Roll");
    }

    public void ChangeAlwaysSpeed(float value)
    {
        _curSpeedMove = value;
    }
    private void PlayerMoving()
    {
        moveVector = Vector3.zero;
        _horizontal = Mathf.Lerp(_horizontal, _fixedJoystick.Horizontal, Time.deltaTime * _curSpeedMove);
        _vertical = Mathf.Lerp(_vertical, _fixedJoystick.Vertical, Time.deltaTime * _curSpeedMove);
        curSpeed = Mathf.Lerp(curSpeed, _curSpeedMove, Time.deltaTime * 3);
        moveVector.x = _horizontal * curSpeed;
        moveVector.z = _vertical * curSpeed;
        // состояние покоя
        if (_fixedJoystick.Horizontal == 0 && _fixedJoystick.Horizontal == 0)
        {
            curSpeed = 0;
            _horizontal = 0;
            _vertical = 0;
            anim.SetBool("Run", false);
            anim.SetBool("Idle", true);
        }
        else // перемещение персонажа
        {
            anim.SetBool("Run", true);
            anim.SetBool("Idle", false);
            // поворот персонажа в сторону направления перемещения
            // поворот возможен только тогда, когда нет анимации атаки

            if ((Vector3.Angle(Vector3.forward, moveVector) > 1f || Vector3.Angle(Vector3.forward, moveVector) == 0))
            {
                // шаг вращения
                float singleStep = 0f;
                // скорость вращения персонажа в 4 раза меньше во время атаки
                if (anim.GetInteger("Attack") != 0)
                { singleStep = (_speedRotation + Time.deltaTime) / 4; }
                else
                { singleStep = _speedRotation + Time.deltaTime; }
                // поворот вектора впред в направлении цели на один шаг
                direct = Vector3.RotateTowards(transform.forward, moveVector, singleStep, 0.0f);
                // рисуем луч в направление поворота 
                Debug.DrawRay(transform.position, direct, Color.red);
                if (direct != Vector3.zero)
                    transform.rotation = Quaternion.LookRotation(direct);
            }

        }

        // расчет гравитации после поворота персонажа
        moveVector.y = gravityForce;
        // перемещение возможно если персонаж не атакует
        if (anim.GetInteger("Attack") == 0)
        {
            ch_control.Move(moveVector * Time.deltaTime);
        }
    }
    // просмотр статус активности damagezone от анимации
    public void TakeStatusDamageZoneFromAnim(int _status)
    {
        if (_status == 0) isAttack = false;
        else if (_status == 1) isAttack = true;
    }
    public void SwitchDamageZone()
    {
        WeaponZone.SetActive(isAttack);
    }

    public void PlayerAttack(int _numAttack)
    {
        anim.SetInteger("Attack", _numAttack);
    }
    public void PlayerStopAttack()
    {
        anim.SetInteger("Attack", 0);
    }
    public void PlayerDeath()
    {
        isAttack = false;
        att_control.UpPressAttack();
        if (anim != null) anim.SetBool("Death", true);
        _fixedJoystick.ResetPosition();
    }
    public void PlayerAlive()
    {
        anim.SetBool("Death", false);
        anim.SetBool("Idle", true);
        _fixedJoystick.ResetPosition();
    }
    public void ResetAnim()
    {
        anim.SetTrigger("Reset");
    }
    public void PlayerComboAttack()
    {
        anim.SetTrigger("Combo");
    }
    private void GamingGravity()
    {
        if (!ch_control.isGrounded)
        {
            gravityForce -= 20 * Time.deltaTime;
        }
        else gravityForce = -1f;
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        // если отсылаем данные
        if (stream.isWriting)
        {
            // отправляем данные
            stream.SendNext(isAttack);
        }
        else // иначе считываем состояние (когда объект не принадлежит нам)
        {
            // принимаем данные
            isAttack = (bool)stream.ReceiveNext();
            WeaponZone.SetActive(isAttack);
        }
    }
}
