using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShieldBar : MonoBehaviour
{
    Image m_ShieldBar;
    Text m_ShieldText;
    HealthSystem m_hp;
    private void Start()
    {
        m_hp = GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>();
        m_ShieldBar = GetComponent<Image>();
        m_ShieldText = GetComponentInChildren<Text>();
        m_hp.OnDamageShield += SetValue;
    }
    private void Update()
    {
        m_ShieldText.text = "" + GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().GetCurrentShield();
    }
    private void OnEnable()
    {
        if (m_hp != null)
            m_hp.OnDamageShield += SetValue;
    }
    private void OnDisable()
    {
        m_hp.OnDamageShield -= SetValue;
    }

    public void SetValue(float amount)
    {
        m_ShieldBar.fillAmount = amount;
    }
}
