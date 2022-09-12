using UnityEngine;
using UnityEngine.Events;

public class BulletMove : Photon.MonoBehaviour
{
    [SerializeField] private float _timeToDestruct = 5f;
    public int StartSpeed = 2;
    [SerializeField] private bool _destroyOnTrigger = true;
    [SerializeField] private bool _includeToObjectOnTrigger = true;
    [SerializeField] private EffectType _effectOnHit = EffectType.None;
    [SerializeField] private ParticleSystem _flyTrail;
    [SerializeField] private AudioSource _hitSound;
    private Rigidbody rb;
    private bool _isMove = true;
    [SerializeField] private UnityEvent EventOnHitTag;
    [SerializeField] private bool _isActivingOneTime = true;
    private bool _isActivatedEventOnHit = false;

    Vector3 PreviousStep;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("DestroyNow", _timeToDestruct);
        rb.velocity = transform.TransformDirection(Vector3.forward * StartSpeed);
        PreviousStep = gameObject.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collider" || other.tag == "Player" || other.tag == "DestroyObject")
        {
            if (!_isActivatedEventOnHit)
            {
                EventOnHitTag.Invoke();
                if (_isActivingOneTime) _isActivatedEventOnHit = true;
                HitEffects();
                HitToObject(other);
                if (_destroyOnTrigger)
                {
                    DestroyDamageZone();
                    this.enabled = false;
                }
            }
        }
    }
    private void DestroyDamageZone()
    {
        if (this.gameObject.GetComponent<DamageZone>())
        { Destroy(this.gameObject.GetComponent<DamageZone>()); }
    }
    private void HitToObject(Collider other)
    {
        rb.Sleep();
        _isMove = false;
        if (_includeToObjectOnTrigger) this.gameObject.transform.SetParent(other.transform);
        float timeToDestroy = 10f;
        if (other.tag != "Player") timeToDestroy = 30f;
        ProcessCommand.DestroyGameObjectDelay(this.gameObject, timeToDestroy);
    }
    private void FixedUpdate()
    {
        if (PhotonNetwork.isMasterClient && _isMove)
        {
            Quaternion CurrentStep = gameObject.transform.rotation;
            transform.LookAt(PreviousStep, transform.up);
            gameObject.transform.rotation = CurrentStep;
            PreviousStep = gameObject.transform.position;
        }
    }
    private void HitEffects()
    {
        if (_flyTrail)
        {
            _flyTrail.Stop();
            _flyTrail.transform.SetParent(null);
        }
        if (_hitSound)
        {
            _hitSound.Play();
            _hitSound.transform.SetParent(null);
        }
        if (_effectOnHit != EffectType.None)
        {
            GlobalEffects.Instance.CreateParticle(this.transform, _effectOnHit);
        }
    }
    private void DestroyNow()
    {
        Destroy(this.gameObject);
    }
}