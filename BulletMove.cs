using UnityEngine;

public class BulletMove : Photon.MonoBehaviour
{
    [SerializeField] private float _timeToDestruct = 5f;
    public int StartSpeed = 2;
    [SerializeField] private ParticleSystem _flyTrail;
    [SerializeField] private AudioSource _hitSound;
    private Rigidbody rb;
    private bool _isMove = true;

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
        if (other.tag == "Collider" || other.tag == "Player")
        {
            HitEffects();
            HitToObject(other);
        }
    }
    private void HitToObject(Collider other)
    {
        rb.Sleep();
        _isMove = false;
        this.gameObject.transform.SetParent(other.transform);
        if (other.tag == "Player") Invoke("DestroyNow", 10f);
        else Invoke("DestroyNow", 30f);
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
    }
    private void DestroyNow()
    {
        HitEffects();
        Destroy(this.gameObject);
    }
}