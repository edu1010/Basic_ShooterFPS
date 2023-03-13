using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
    public enum DoorType
    {
        KEY,
        AUTHOMATIC,
        SHOOTING_GALLERY_FINISHED
    }
    public DoorType m_DoorType;

    public GameObject m_Key;

    Animator m_Animator;

    public ShootingGallery m_ShootingGallery;

    AudioSource m_SoundOpenDoor;
    bool m_FirstTime = true;

    void Start()
    {
        m_Animator = GetComponent<Animator>();
        m_Animator.SetBool("Open", false);
        m_Animator.SetBool("Close", false);

        m_SoundOpenDoor = GetComponent<AudioSource>();
    }

    public bool PlayerGotTheKey()
    {
        return m_Key.GetComponent<KeyItem>().GetIfKeyWasPicked();
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            switch (m_DoorType)
            {
                case DoorType.KEY:
                    if (PlayerGotTheKey())
                    {
                        OpenDoor();
                    }
                    break;
                case DoorType.AUTHOMATIC:
                    OpenDoor();
                    break;
                case DoorType.SHOOTING_GALLERY_FINISHED:
                    if (m_ShootingGallery.GetShootingGalleryStateGame() == ShootingGallery.StateGame.WIN)
                    {
                        OpenDoor();
                    }
                    break;
            }
        }
        else if(other.tag == "Enemy" && m_DoorType == DoorType.AUTHOMATIC)
        {
            OpenDoor();
        }
    }
    private void OnTriggerStay(Collider other)
    {
        if (other.tag == "Player")//Necesario para que la puerta de lava se abra tambien al coger la llave y crear el efecto
        {
            
            if (m_DoorType == DoorType.KEY)
            {
                if (PlayerGotTheKey() && m_FirstTime)
                {
                    m_SoundOpenDoor.Play();
                    OpenSilentDoor();
                    m_FirstTime = false;
                }
            }
            
        }


    }

    private void OnTriggerExit(Collider other)
    {
        CloseDoor();
        m_FirstTime = true;
    }

    void OpenDoor()
    {
        m_SoundOpenDoor.Play();

        m_Animator.SetBool("Open", true);
        m_Animator.SetBool("Close", false);
    }

    void CloseDoor()
    {
        m_SoundOpenDoor.Play();

        m_Animator.SetBool("Open", false);
        m_Animator.SetBool("Close", true);
    }
    void OpenSilentDoor()
    {
        m_Animator.SetBool("Open", true);
        m_Animator.SetBool("Close", false);
    }
}
