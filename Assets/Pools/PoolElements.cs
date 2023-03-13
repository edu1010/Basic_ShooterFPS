using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoolElements 
{
    List<GameObject> m_Elements;
    int m_CurrentElementId;
    
    public PoolElements(int Count, Transform Parent, GameObject Prefab)
    {
        m_Elements = new List<GameObject>();
        m_CurrentElementId = 0;

        for (int i = 0; i < Count; ++i)
        {
            GameObject l_Element = GameObject.Instantiate(Prefab, Parent);
            l_Element.SetActive(false);
            m_Elements.Add(l_Element);
        }
    }

    public GameObject GetNextElement()
    {
        GameObject l_Element = m_Elements[m_CurrentElementId];
        m_CurrentElementId++;
        if (m_CurrentElementId >= m_Elements.Count)
        {
            m_CurrentElementId = 0;
        }
        return l_Element;
    }
}
