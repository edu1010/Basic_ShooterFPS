using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    Image m_HealthBar;
    Text m_HealthText;
    HealthSystem m_hp;
    private void Start()
    {
        m_hp = GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>();
        m_HealthBar = GetComponent<Image>();
        m_HealthText = GetComponentInChildren<Text>();
        m_hp.OnHit += SetValue;
    }
    private void Update()
    {
        m_HealthText.text = "" + GameController.GetGameController().GetPlayer().GetComponent<HealthSystem>().GetCurrentHealth();
    }
    private void OnEnable()
    {
        if(m_hp!=null)
            m_hp.OnHit += SetValue;
    }
    private void OnDisable()
    {
        m_hp.OnHit -= SetValue;
    }

    public void SetValue(float amount)
    {
        m_HealthBar.fillAmount = amount;
    }
}
