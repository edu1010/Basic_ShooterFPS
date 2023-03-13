using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallRun : MonoBehaviour
{
    public Transform m_PitchControllerTransform;
    Vector3[] m_directions = new Vector3[]{
        Vector3.right,
        Vector3.right+Vector3.forward,
        Vector3.forward,
        Vector3.left+Vector3.forward,
        Vector3.left
    };
    public float m_WallJumpSpeed=20;
    RaycastHit[] hits;
    public bool m_IsWallRunning;
    bool m_WasWallRunning;
    public float m_IsWallDistance;
    public LayerMask m_WhatIsWallRun;
    public FPSPlayerController m_FPSPlayer;
    Vector3 m_lastWallNormal;
    Quaternion m_targetRotation;
    float timer = 0.0f;
    bool isGrounded() => m_FPSPlayer.m_OnGround;
   
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void Update()
    {
        if(!m_IsWallRunning && m_WasWallRunning && !isGrounded())
        {
            //m_FPSPlayer.m_IsWallJumping = true;
        }
        else if(isGrounded())
        {
            m_FPSPlayer.m_IsWallJumping = false;
            timer = 0f;
        }
        if (m_IsWallRunning)
        {
            timer += Time.deltaTime;
            
        }
        
        if (m_IsWallRunning && !m_WasWallRunning)
        {
            m_FPSPlayer.m_VerticalSpeed = 0;
            timer = 0f;
        }
        m_WasWallRunning = m_IsWallRunning;


    }
    // Update is called once per frame
    void LateUpdate()
    {
        if (CanWallRun())
        {
            hits = new RaycastHit[m_directions.Length];
            bool l_IsWallRunning = false;//Lo pondremos a true si esta tocando la pard, usamos una variable local para no resetear la variable miembro y que de un falso positivo
            for (int i = 0; i < m_directions.Length; i++)
            {
                //Ponemos la direcion en global
                Vector3 dir = transform.TransformDirection(m_directions[i]);
                Physics.Raycast(transform.position, dir, out hits[i], m_IsWallDistance, m_WhatIsWallRun);
                if (hits[i].collider != null)
                {
                    Debug.DrawRay(transform.position, dir * hits[i].distance, Color.green);
                    RotatePlayer(m_directions[i]);
                    m_lastWallNormal = hits[i].normal;
                    m_FPSPlayer.m_GravityMultiplayer = 0.2f;
                    if(!m_WasWallRunning && m_IsWallRunning)
                    {
                        m_FPSPlayer.m_VerticalSpeed = 0;
                    }
                    l_IsWallRunning = true;
                }
                else
                {
                    Debug.DrawRay(transform.position, dir * m_IsWallDistance, Color.red);
                }
            }
            m_IsWallRunning = l_IsWallRunning;
            bool l_ResetRot = true;
            for (int i = 0; i < hits.Length; i++)
            {
                if (hits[i].collider != null)
                {
                    l_ResetRot = false;
                }
            }
            if (l_ResetRot)
            {
                ResetPlayerRotatation();
            }
            if (Input.GetKeyDown(m_FPSPlayer.m_JumpKeyCode))
            {
               // WallJump();
            }

        }
        else    { 
            ResetPlayerRotatation();
            m_FPSPlayer.m_GravityMultiplayer = m_FPSPlayer.m_MAxGravityMultiplayer;
            m_IsWallRunning = false;
        }
        if(m_PitchControllerTransform.localRotation != m_targetRotation)
        {
            
            m_PitchControllerTransform.localRotation = Quaternion.Lerp(m_PitchControllerTransform.localRotation, m_targetRotation, 0.05f);
        }
        
        
    }
    public Vector3 WallJump()//Enviamos la normal de la última pared para el salto
    {
        float l_MovementX = m_lastWallNormal.x ;
        //m_FPSPlayer.m_CharacterController.Move(l_Movement);
        return m_lastWallNormal;
    }
    bool CanWallRun()
    {
        return !isGrounded();
    }
    void RotatePlayer(Vector3 dir)
    {
        if (dir == Vector3.left)
        {
            m_targetRotation = Quaternion.Euler(m_PitchControllerTransform.localRotation.eulerAngles.x, 0.0f, -30);
           
           
        }
        if (dir == Vector3.right)
        {
            m_targetRotation = Quaternion.Euler(m_PitchControllerTransform.localRotation.eulerAngles.x, 0.0f, 30);
        }

    }
    void ResetPlayerRotatation()
    {
        m_targetRotation = Quaternion.Euler(m_PitchControllerTransform.localRotation.eulerAngles.x, 0.0f, 0.0f);
        m_FPSPlayer.m_GravityMultiplayer=m_FPSPlayer.m_MAxGravityMultiplayer;
    }
}
