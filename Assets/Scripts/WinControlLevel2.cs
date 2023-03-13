using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
public class WinControlLevel2 : MonoBehaviour
{
    public List<GameObject> m_Enemies;
    int m_EnemiesAlieve = 3;
    

    // Update is called once per frame
    void Update()
    {
        m_EnemiesAlieve = m_Enemies.Where(x => x.activeSelf == true).Count();
        if (m_EnemiesAlieve <= 0)
        {
            GameController.GetGameController().ShowWinHud();
        }
    }
}
