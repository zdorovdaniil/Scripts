using UnityEngine;

public class ParticlePlaces : MonoBehaviour
{
    [SerializeField] private Transform _hitParticlePlace; public Transform HitPlace => _hitParticlePlace;
    [SerializeField] private Transform _buffPlace; public Transform BuffPlace => _buffPlace;

}
