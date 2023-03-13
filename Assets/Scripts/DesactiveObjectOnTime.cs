using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DesactiveObjectOnTime : MonoBehaviour
{
    public float m_DesactiveTime = 2.0f;

    // Start is called before the first frame update
    void OnEnable()
    {
        StartCoroutine(CDesactiveObjectsOnTime());
    }

    IEnumerator CDesactiveObjectsOnTime()
    {
        yield return new WaitForSeconds(m_DesactiveTime);
        gameObject.SetActive(false);
    }
}
