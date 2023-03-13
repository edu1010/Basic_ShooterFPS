using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthScreenShootingGallery : HealthSystem
{
    [SerializeField] ShootingGallery m_ShootingGallery;

    private void Start()
    {
        
    }
    public override void TakeDamage(float amount)
    {
        //base.TakeDamage(amount);
        Debug.Log("salud");
        if (m_ShootingGallery.GetShootingGalleryStateGame() != ShootingGallery.StateGame.PLAYING)
        {
            m_ShootingGallery.SetShootingGalleryStateGame(ShootingGallery.StateGame.PLAYING);
            m_ShootingGallery.ResetGallery();
        }
    }
}
