using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthSystem : MonoBehaviour, IDamageTaker
{
    public float m_maxHealth = 100f;
    public float m_maxShield = 50f;
    public bool m_invencible = false;
    public bool m_Shield = false;

    float m_currentHealth =100;
    float m_currentShield;
    public float m_InvecibleTime = 0.5f;

    public Action<float> OnHit;
    public Action<float> OnDamageShield;
    public Action OnDeath;
    public Action OnRevive;
    // Start is called before the first frame update
    void Start()
    {
        m_currentHealth = m_maxHealth;
        m_currentShield = m_maxShield;
    }

    public virtual void TakeDamage(float amount)
    {
        if (!m_invencible)
        {
            if(m_Shield && m_currentShield > 0)
            {
                float l_shieldDamage = amount * 0.75f;
                float l_HealthDamage= amount * 0.25f;
                if (m_currentShield- l_shieldDamage >= 0)
                {
                    m_currentShield -= l_shieldDamage;
                    OnDamageShield?.Invoke(m_currentShield / m_maxShield);
                }
                else
                {
                    l_shieldDamage= m_currentShield - m_currentShield;
                    l_HealthDamage += l_shieldDamage;
                    m_currentShield = 0;
                    OnDamageShield?.Invoke(0);
                }
                m_currentHealth -= l_HealthDamage;
            }
            else
            {
                m_currentHealth -= amount;
                if (m_currentHealth > m_maxHealth) m_currentHealth = m_maxHealth;
                if (m_currentHealth <= 0.0f) OnDeath?.Invoke();
            }
            OnHit?.Invoke(m_currentHealth / m_maxHealth);
            if(m_currentHealth>0)
                StartCoroutine(InvecibleTime());
        }
    }
    IEnumerator InvecibleTime()
    {
        m_invencible = true;
        yield return new WaitForSeconds(m_InvecibleTime);
        m_invencible = false;
    }
    public virtual void Die()
    {
        m_currentHealth = 0;
        OnDeath?.Invoke();
    }
    public void ResetHealth()
    {
        m_currentHealth = m_maxHealth;
        OnHit?.Invoke(m_currentHealth / m_maxHealth);
    }

    public void AddHealth(float amount)
    {
        m_currentHealth += amount;
        if (m_currentHealth > m_maxHealth)
            m_currentHealth = m_maxHealth;
        OnHit?.Invoke(m_currentHealth / m_maxHealth);
    }
    public void RestartHealthSystem()
    {
        
        m_currentHealth = m_maxHealth;
        m_currentShield = m_maxShield;
        OnRevive?.Invoke();
        OnHit?.Invoke(m_currentHealth / m_maxHealth);
        OnDamageShield?.Invoke(m_currentShield / m_maxShield);
        
    }
    public void AddShield(float amount)
    {
        m_currentShield += amount;
        if (m_currentShield > m_maxShield)
            m_currentShield = m_maxShield;
        OnDamageShield?.Invoke(m_currentShield / m_maxShield);
    }

    public float GetCurrentHealth()
    {
        return m_currentHealth;
    }

    public float GetMaxHealth()
    {
        return m_maxHealth;
    }

    public float GetCurrentShield()
    {
        return m_currentShield;
    }

    public float GetMaxShield()
    {
        return m_maxShield;
    }

}
