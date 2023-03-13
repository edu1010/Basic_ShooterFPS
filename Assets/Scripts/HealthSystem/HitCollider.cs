using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitCollider : MonoBehaviour
{
    public enum THitColliderType
    {
        HEAD,
        HELIX,
        BODY
    }
    public THitColliderType m_HitColliderType;
    public GameObject m_Drone;
    HealthSystem m_DroneEnemyHP;
    DroneAI m_DroneAi;
    private void Start()
    {
        m_DroneEnemyHP = m_Drone.GetComponent<HealthSystem>();
        m_DroneAi = m_Drone.GetComponent<DroneAI>();
    }
    public void Hit()
    {
        switch (m_HitColliderType)
        {
            case THitColliderType.HEAD:
                m_DroneEnemyHP.TakeDamage(20f);
                break;
            case THitColliderType.HELIX:
                m_DroneEnemyHP.TakeDamage(15f);
                break;
            case THitColliderType.BODY:
                m_DroneEnemyHP.TakeDamage(10f);
                break;
        }
        m_DroneAi.SetStateToHit();
    }
}
