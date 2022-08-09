using UnityEngine;

public class BulletMove : Photon.MonoBehaviour
{
    public float TimeToDestruct = 1f;
    public int StartSpeed = 2;
    [SerializeField] private ParticleSystem _flyTrail;
    public GameObject particleHit;
    private Rigidbody rb;

    Vector3 PreviousStep;
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        Invoke("DestroyNow", TimeToDestruct);

        rb.velocity = transform.TransformDirection(Vector3.forward * StartSpeed);
        PreviousStep = gameObject.transform.position;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Collider")
        {
            DestroyNow();
        }
    }
    void FixedUpdate()
    {
        if (PhotonNetwork.isMasterClient)
        {
            Quaternion CurrentStep = gameObject.transform.rotation;

            transform.LookAt(PreviousStep, transform.up);
            RaycastHit hit = new RaycastHit();
            if (Physics.Raycast(PreviousStep, transform.TransformDirection(Vector3.back), out hit) && (hit.transform.gameObject != gameObject))
            {
                //Instantiate(particleHit, hit.point, Quaternion.FromToRotation(Vector3.up, hit.normal));
            }
            gameObject.transform.rotation = CurrentStep;
            PreviousStep = gameObject.transform.position;
        }
    }
    void DestroyNow()
    {
        _flyTrail.Stop();
        _flyTrail.SetParent(null);
        Destroy(this.gameObject);
    }
}