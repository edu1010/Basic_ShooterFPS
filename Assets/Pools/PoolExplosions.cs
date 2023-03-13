using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolExplosions : MonoBehaviour
{
    public PoolElements m_poolElements;
    public GameObject m_Particles;

    // Start is called before the first frame update
    void Start()
    {
        m_poolElements = new PoolElements(7, transform, m_Particles);
    }

    public void CreateExplosionParticle(Vector3 pos)
    {
        GameObject l_Particles = m_poolElements.GetNextElement();
        l_Particles.SetActive(true);
        l_Particles.transform.position = pos;
        l_Particles.transform.rotation = Quaternion.identity;
    }
}
