using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[RequireComponent (typeof(HealthSystem))]
public class DroneAI : MonoBehaviour
{
    private enum States
    {
        IDLE =0,
        PATROL,
        ALERT,
        CHASE,
        ATTACK,
        HIT,
        DIE
    }
    FSM<States> m_brain;
    
    public float m_MinDistanceToAlert = 5.0f;
    public float m_MinDistanceToAttack = 3.0f;
    public float m_MaxDistanceToAttack = 7.0f;
    public float m_MaxDistanceToPatrol = 15.0f;
    public float m_ConeAngle = 60.0f;
    public float m_RootSpeed = 10f;
    public LayerMask m_CollisionLayerMask;
    public List<Transform> m_PatrolWaypoints;
    public GameObject m_particles;
    NavMeshAgent m_NavMeshAgent;
    int m_CurrentWaypointId = 0;
    float m_DistanceToPlayer;//Al usarlo en diferentes funciones pasamos ha ahcerla miembro
    States m_PreviouState;
    public GameObject m_BulletPrefab;
    public float m_Candence = 0.75f;
    float m_CurrentCandence = 0.75f;
    public Transform m_UIAnchor;
    LifeBarEnemyPosition m_LifeBar;
    HealthSystem m_hp;
    public RectTransform m_HUDRectTransform;
    public HealthBarEnemy m_DroneLife;
    private float m_IncrementRoot;
    private float m_initialRoot;

    AudioSource m_AudioSource;
    public AudioClip m_AudioShoot;


    // Start is called before the first frame update
    private void Awake()
    {
        m_hp = GetComponent<HealthSystem>();
        m_NavMeshAgent = GetComponent<NavMeshAgent>();
        m_AudioSource = GetComponent<AudioSource>();
    }
    void Start()
    {
        CreateLifeBar();
        Init();
    }
    private void OnEnable()
    {
        m_hp.OnDeath += OnDeath;
    }
    private void OnDisable()
    {
        m_hp.OnDeath -= OnDeath;
    }

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        UnityEditor.Handles.color = Color.green;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, m_MinDistanceToAlert);
        Handles.Label(new Vector3(0, 0,10), "m_MinDistanceToAlert green");
        UnityEditor.Handles.color = Color.yellow;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, m_MaxDistanceToAttack);
        Handles.Label(new Vector3(0, 0, 20), "m_MaxDistanceToAttack yellow");
        UnityEditor.Handles.color = Color.magenta;
        UnityEditor.Handles.DrawWireDisc(transform.position, transform.up, m_MaxDistanceToPatrol);
        Handles.Label(new Vector3(0, 0, 30), "m_MaxDistanceToAttack magenta");

    }
#endif
    // Update is called once per frame
    void Update()
    {
        m_DistanceToPlayer = Vector3.Distance(GameController.GetGameController().GetPlayer().transform.position, transform.position);
        m_brain.Update();
    }
    private void LateUpdate()
    {
        m_LifeBar.SetLifeBarEnemy(m_UIAnchor.position);
        if(!IsPlayerWatchingMe())
            m_LifeBar.DontShow();
    }
    void CreateLifeBar()
    {
        HealthBarEnemy l_HealthBar = GameObject.Instantiate(m_DroneLife.gameObject, m_HUDRectTransform.transform).GetComponent<HealthBarEnemy>();
        m_LifeBar = l_HealthBar.gameObject.GetComponent<LifeBarEnemyPosition>();
        l_HealthBar.m_hp = m_hp;
        l_HealthBar.gameObject.SetActive(true);
    }
    void Init()
    {   
        m_brain = new FSM<States>(States.IDLE);
        //OnEnter

        m_brain.SetOnEnter(States.IDLE, () => {
        });
        m_brain.SetOnEnter(States.PATROL, () => {
            m_NavMeshAgent.isStopped = false;

        });
        m_brain.SetOnEnter(States.ALERT, () => {
            m_IncrementRoot = 0.0f;
            m_initialRoot = transform.localRotation.eulerAngles.y;
        });
        m_brain.SetOnEnter(States.CHASE, () => {
            m_NavMeshAgent.isStopped = false;
        });
        m_brain.SetOnEnter(States.ATTACK, () => {
            m_NavMeshAgent.isStopped = true;
            m_CurrentCandence = 0;
        });
        m_brain.SetOnEnter(States.DIE, () => {

        });
        m_brain.SetOnEnter(States.HIT, () => {

        });

        //OnStay
        m_brain.SetOnStay(States.IDLE, () => {
            m_brain.ChangeState(States.PATROL);
        });
        m_brain.SetOnStay(States.PATROL, () => {
            if (HearsPlayer())
            {
                m_brain.ChangeState(States.ALERT);
            }
            if (!m_NavMeshAgent.hasPath && m_NavMeshAgent.pathStatus == NavMeshPathStatus.PathComplete)
            {
                MoveToNextPatrolPosition();
               
            }
            
        });   
        m_brain.SetOnStay(States.ALERT, () => {
            if (SeesPlayer2())
            {
                if( m_MaxDistanceToAttack < m_DistanceToPlayer)
                {
                    m_brain.ChangeState(States.ATTACK);
                }
                else 
                {
                    m_brain.ChangeState(States.CHASE);
                }
            }
            else
            { 
                m_IncrementRoot += Time.deltaTime * m_RootSpeed;
                transform.localRotation = Quaternion.Euler(transform.localRotation.eulerAngles.x, m_initialRoot + m_IncrementRoot, transform.localRotation.eulerAngles.z);
                if (m_IncrementRoot >= 360)
                {
                    if (HearsPlayer())
                    {
                        m_IncrementRoot = 0;
                    }
                    else
                    {
                        Debug.Log("SALIENDO_ALERT");
                        m_brain.ChangeState(States.PATROL);
                    }
                }
            }
              
        });   
        m_brain.SetOnStay(States.CHASE, () => {

            if (m_DistanceToPlayer < m_MaxDistanceToPatrol)
            {
                if(m_DistanceToPlayer> m_MinDistanceToAttack && m_DistanceToPlayer < m_MaxDistanceToAttack)
                {
                    m_brain.ChangeState(States.ATTACK);
                }
                else
                {
                    SetNextChasePosition();
                }
            }
            else
            {
                m_brain.ChangeState(States.PATROL);   
            }
        });  
        m_brain.SetOnStay(States.ATTACK, () => {
            transform.LookAt(GameController.GetGameController().GetPlayer().transform);
            m_CurrentCandence -= Time.deltaTime;
            if (SeesPlayer2())
            {
                if (m_CurrentCandence <= 0)
                {
                    StopCoroutine("TurnOffParticles");
                    m_particles.SetActive(true);
                    Shoot();
                    m_CurrentCandence = m_Candence;
                }
                else
                {
                    StartCoroutine("TurnOffParticles");
                }

            }
            else
            {
                //Si el dron no ve a player, debe buscarlo por lo que lo ponemos en estado de alerta. 
                m_brain.ChangeState(States.ALERT);
            }
            if (m_DistanceToPlayer > m_MaxDistanceToAttack)
            {
                m_brain.ChangeState(States.CHASE);
            }
            
        });  
        m_brain.SetOnStay(States.DIE, () => {
             
        });  
        m_brain.SetOnStay(States.HIT, () => {
            if(m_PreviouState == States.IDLE || m_PreviouState == States.PATROL)
            {
                m_brain.ChangeState(States.ALERT);
            }
            else
            {
                m_brain.ChangeState(m_PreviouState);
            }

        });
        //OnExit
        m_brain.SetOnExit(States.IDLE, () => {
            m_PreviouState = States.IDLE;
        });
        m_brain.SetOnExit(States.PATROL, () => {
            m_PreviouState = States.PATROL;
            m_NavMeshAgent.isStopped = true;
        });
        m_brain.SetOnExit(States.ALERT, () => {
            m_PreviouState = States.ALERT;
        });
        m_brain.SetOnExit(States.CHASE, () => {
            m_CurrentWaypointId = PatrolPositionNear();
            m_PreviouState = States.CHASE;
        });
        m_brain.SetOnExit(States.ATTACK, () => {
            m_PreviouState = States.ATTACK;
            m_particles.SetActive(false);
        });
        m_brain.SetOnExit(States.DIE, () => {
            m_PreviouState = States.DIE;
        });
        m_brain.SetOnExit(States.HIT, () => {
            m_PreviouState = States.HIT;

        });

    }


    void MoveToNextPatrolPosition()
    {
        m_NavMeshAgent.destination = m_PatrolWaypoints[m_CurrentWaypointId].position;
        m_NavMeshAgent.isStopped = false;
        ++m_CurrentWaypointId;
        if (m_CurrentWaypointId >= m_PatrolWaypoints.Count)
        {
            m_CurrentWaypointId = 0;
        }
    }
    int PatrolPositionNear()
    {
        Vector3 l_previus = Vector3.zero;
        int l_nearPosition = 0;
        for (int i = 0; i < m_PatrolWaypoints.Count; i++)
        {
            if(Vector3.Distance(transform.position, m_PatrolWaypoints[i].position) 
                < Vector3.Distance(transform.position, l_previus))
            {
                l_nearPosition = i;
            }
            l_previus = m_PatrolWaypoints[i].position;
        }
        return l_nearPosition;
    }
    void SetNextChasePosition()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position;
        Vector3 l_DirectionToPlayer = l_PlayerPosition - transform.position;
        l_DirectionToPlayer.y = 0;
        l_DirectionToPlayer.Normalize();
        Vector3 l_Destination = l_PlayerPosition - l_DirectionToPlayer * m_MinDistanceToAttack;
        m_NavMeshAgent.destination = l_Destination;
    }
    bool HearsPlayer()
    {
        if(m_DistanceToPlayer <= m_MinDistanceToAlert)
        {
            return true;
        }
        else 
        { 
            return false;
        }
        
    }
    bool IsPlayerWatchingMe()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position + Vector3.up * 1.6f;
        Vector3 l_EyesDronePosition = transform.position + Vector3.up * 1.6f;
        Vector3 l_Direction =  l_EyesDronePosition- l_PlayerPosition;
        float l_DistanceBetwenObjects = l_Direction.magnitude;
        l_Direction /= l_DistanceBetwenObjects;
        Ray l_ray = new Ray(l_PlayerPosition, l_Direction);
        Vector3 l_forward = transform.forward;
        l_forward.y = 0;
        l_forward.Normalize();
        l_Direction.y = 0;
        l_Direction.Normalize();
        if (!Physics.Raycast(l_ray, l_DistanceBetwenObjects, m_CollisionLayerMask.value))
        {
            Debug.DrawLine(l_EyesDronePosition, l_PlayerPosition, Color.red);
            return true;
        }
        else { return false; }
    }
    bool SeesPlayer2()
    {
        Vector3 l_PlayerPosition = GameController.GetGameController().GetPlayer().transform.position + Vector3.up * 1.6f;
        Vector3 l_EyesDronePosition = transform.position + Vector3.up * 1.6f;
        Vector3 l_Direction = l_PlayerPosition - l_EyesDronePosition;
        float l_DistanceToPlayer = l_Direction.magnitude;
        l_Direction /= l_DistanceToPlayer;
        Ray l_ray = new Ray(l_EyesDronePosition, l_Direction);
        Vector3 l_forward = transform.forward;
        l_forward.y = 0;
        l_forward.Normalize();
        l_Direction.y = 0;
        l_Direction.Normalize();
        if(l_DistanceToPlayer<m_MaxDistanceToPatrol
            && Vector3.Dot(l_forward,l_Direction)>= Mathf.Cos(m_ConeAngle * 0.5f * Mathf.Deg2Rad))
        {   
            if (!Physics.Raycast(l_ray, l_DistanceToPlayer, m_CollisionLayerMask.value))
            {
                Debug.DrawLine(l_EyesDronePosition, l_PlayerPosition, Color.red);
                return true;
            }
        }
        Debug.DrawLine(l_EyesDronePosition, l_PlayerPosition, Color.blue);
        return false;


    }
    void Shoot()
    {
        m_AudioSource.PlayOneShot(m_AudioShoot);
        Vector3 l_BulletDir = GameController.GetGameController().GetPlayer().transform.position - transform.position;
        l_BulletDir /= l_BulletDir.magnitude;
        var l_bullet = m_BulletPrefab.GetComponent<Bullet>();
        l_bullet.m_dir = l_BulletDir;
        l_bullet.transform.position = transform.position + new Vector3(0, 1.39f, 0);
        Instantiate(m_BulletPrefab);
    }
    public void SetStateToHit()
    {
        m_brain.ChangeState(States.HIT);
    }
    IEnumerator TurnOffParticles()
    {
        yield return new WaitForSeconds(0.5f);
        m_particles.SetActive(false);

    }
    void OnDeath()
    {
        m_brain.ChangeState(States.DIE);
    }
}
