using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldItem : Items
{
    public float m_ShieldAmount = 50.0f;

    public override bool CanPick()
    {
        float l_CurrentShield = GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().GetCurrentShield();
        float l_MaxShield = GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().GetMaxShield();

        if (l_CurrentShield == l_MaxShield)
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
        GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().AddShield(m_ShieldAmount);
    }
}