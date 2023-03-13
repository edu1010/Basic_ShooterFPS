using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AmmoHUD : MonoBehaviour
{
    Text m_Text;
    FPSPlayerController m_Player;

    void Start()
    {
        m_Text = GetComponentInChildren<Text>();
        m_Player = GameController.GetGameController().GetPlayer();
    }

    void Update()
    {
        m_Text.text = m_Player.GetCurrentBulletsInCharger() + " / " + m_Player.GetCurrentChargers();
    }

}
