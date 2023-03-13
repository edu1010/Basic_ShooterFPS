using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DamageTakerPlayer : MonoBehaviour
{
    [SerializeField] HealthSystem hp;
    private void OnEnable()
    {
        hp.OnDeath += Die;
        hp.OnRevive += Revive;
    }
    private void OnDisable()
    {
        hp.OnDeath -= Die;
        hp.OnRevive -= Revive;
    }

    public void Die()
    {
        GameController.GetGameController().ShowDeathHud();
        hp.OnDeath -= Die;
        Debug.Log("veces");
    }
    public void Revive()
    {
        hp.OnDeath += Die;
    }
}
