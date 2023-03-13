using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthItem : Items
{
    public float m_HealthAmount;
    public override bool CanPick()
    {
        float l_CurrentHealth = GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().GetCurrentHealth();
        float l_MaxHealth = GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().GetMaxHealth();

        if (l_CurrentHealth == l_MaxHealth)
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
        gameObject.SetActive(false);
        GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().AddHealth(m_HealthAmount);
    }
}
