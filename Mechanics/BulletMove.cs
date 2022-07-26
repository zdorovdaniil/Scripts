﻿using UnityEngine;
using UnityEngine.Events;

public enum Tags { Collider, Player, DestroyObject, Enemy }
public class BulletMove : Photon.MonoBehaviour
{
    [SerializeField] private bool _includeToObjectOnTrigger = true;
    [SerializeField] private EffectType _effectOnHit = EffectType.None;
    [SerializeField] private ParticleSystem _flyTrail;
    [SerializeField] private AudioSource _hitSound;
    [SerializeField] private UnityEvent _eventOnHit;
    [SerializeField] private bool _isActivingOneTime = true;
    [SerializeField] private Tags[] _activateOnTags;

    [Header("Phisics parametrs of Moving")]
    [SerializeField] private int _startSpeed = 2;
    [SerializeField] private float _timeToDestruct = 25f;
    private bool _isActivatedEventOnHit = false;
    private Rigidbody rb;
    private Vector3 _previousStep;
    private bool _isMove = true;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.velocity = transform.TransformDirection(Vector3.forward * _startSpeed);
        _previousStep = gameObject.transform.position;
        ProcessCommand.DestroyGameObjectDelay(this.gameObject, _timeToDestruct);
    }
    private void FixedUpdate()
    {
        if (PhotonNetwork.isMasterClient && _isMove)
        {
            Quaternion CurrentStep = gameObject.transform.rotation;
            transform.LookAt(_previousStep, transform.up);
            gameObject.transform.rotation = CurrentStep;
            _previousStep = gameObject.transform.position;
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (ProcessCommand.CheckTagInGameObject(other.gameObject.tag, _activateOnTags))
        {
            Debug.Log(this.gameObject.name + " triggered with: " + other.gameObject.name);
            if (!_isActivatedEventOnHit)
            {
                _eventOnHit.Invoke();
                if (_isActivingOneTime) _isActivatedEventOnHit = true;
                HitEffects();
                HitToObject(other);
            }
        }
    }
    private void HitToObject(Collider other)
    {
        rb.Sleep();
        _isMove = false;
        if (_includeToObjectOnTrigger)
        {
            HumanEffects humanEffects = other.GetComponent<HumanEffects>();
            if (humanEffects) humanEffects.AddParticleToContainer(this.gameObject);
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
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    { }
}