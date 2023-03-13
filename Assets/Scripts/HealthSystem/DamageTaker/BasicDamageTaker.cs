using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(HealthSystem))]
public class BasicDamageTaker : MonoBehaviour
{
    [SerializeField]HealthSystem hp;

    private void OnEnable()
    {
        hp.OnDeath += Die;
    }

    public void Die()
    {
        gameObject.SetActive(false);
    }
}
