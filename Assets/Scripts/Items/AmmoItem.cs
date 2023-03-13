using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoItem : Items, IRestart
{
    public int m_AmountOfChargers = 1;
    Animator m_Animator;
    public GameObject m_Weapon;
    private void Start()
    {
        m_Animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Vector3.Distance(GameController.GetGameController().GetPlayer().transform.position, transform.position) <= 5.0f)
        {
            m_Animator.SetBool("IsPlayerClose", true);
        }
        else
        {
            m_Animator.SetBool("IsPlayerClose", false);
        }
    }

    public override bool CanPick()
    {
        float l_CurrentChargers = GameController.GetGameController().GetPlayer().GetCurrentChargers();
        float l_MaxChargers = GameController.GetGameController().GetPlayer().GetMaxChargers();

        if (l_CurrentChargers == l_MaxChargers)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public override void Pick()
    {
        m_Weapon.SetActive(false);
        GameController.GetGameController().GetPlayer().AddCharger(m_AmountOfChargers);
        m_AmountOfChargers = 0;
    }

    public void Restart()
    {
        m_Weapon.SetActive(true);
        m_AmountOfChargers = 1;
    }
}
