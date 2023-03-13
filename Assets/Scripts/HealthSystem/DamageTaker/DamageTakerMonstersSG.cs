using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(HealthSystem))]
public class DamageTakerMonstersSG : MonoBehaviour
{
    [SerializeField] 
    HealthSystem m_hp;
    ShootingGallery m_shootingGallery;
    public GameObject m_DeathParticlePrefab;
    public PoolExplosions m_Pool;

    public void Start()
    {
        m_shootingGallery = GetComponentInParent<ShootingGallery>();
        m_hp.OnDeath += Die;
    }

    public void Die()
    {
        m_shootingGallery.AddPoints();
        m_shootingGallery.OneTargetLess();
        m_Pool.CreateExplosionParticle(transform.position);
        gameObject.SetActive(false);
    }
}
