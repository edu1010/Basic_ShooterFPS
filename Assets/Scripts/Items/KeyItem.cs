using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyItem : Items
{
    public bool m_KeyPicked = false;
   
    public override bool CanPick()
    {
        return true; 
    }

    public override void Pick()
    {
        m_KeyPicked = true;
        gameObject.SetActive(false);
        
    }

    public bool GetIfKeyWasPicked()
    {
        return m_KeyPicked;
    }
}
