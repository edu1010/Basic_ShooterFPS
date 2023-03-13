using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(HealthSystem))]
public class DamageTakerDrone : MonoBehaviour
{
    [SerializeField]
    HealthSystem m_hp;
    //public GameObject m_DeathParticlePrefab;
    Animator m_Animator;
    private IEnumerator m_CorutineDesactiveDrone;
    public GameObject m_Item;
    public float m_Probability = 0.5f;
    public bool m_DropKey=false;
    AudioSource m_AudioSource;
    public AudioClip m_AudioDie;

    public void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_AudioSource = GetComponent<AudioSource>();
        m_hp.OnDeath += Die;
    }

    public void Die()
    {
        m_AudioSource.PlayOneShot(m_AudioDie);
        m_Animator.SetBool("Die", true);
        m_CorutineDesactiveDrone = DesactiveDrone(2.0f);
        StartCoroutine(m_CorutineDesactiveDrone);
    }

    IEnumerator DesactiveDrone(float time)
    {
        float l_random = Random.value;
        yield return new WaitForSeconds(time);
        if(l_random <= m_Probability)
        {
            if (!m_DropKey)
            {
                Instantiate(m_Item, transform.position + Vector3.up, Quaternion.identity);
            }
            else
            {
                m_Item.transform.position = transform.position + Vector3.up;
            }
            
        }
        gameObject.SetActive(false);
    }
}
