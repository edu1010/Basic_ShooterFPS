using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSPlayerController : MonoBehaviour,IRestart
{

    float m_Yaw;
    float m_Pitch;
    [Header("Camera")]
    public float m_YawRotationalSpeed = 360.0f;
    public float m_PitchRotationalSpeed = 180.0f;
    public float m_MinPitch = -80.0f;
    public float m_MaxPitch = 50.0f;
    public Transform m_PitchControllerTransform;
    public bool m_InvertedYaw = false;
    public bool m_InvertedPitch = true;
    [Header("Controls")]
    public CharacterController m_CharacterController;
    public float m_Speed = 10.0f;
    public KeyCode m_LeftKeyCode = KeyCode.A;
    public KeyCode m_RightKeyCode = KeyCode.D;
    public KeyCode m_UpKeyCode = KeyCode.W;
    public KeyCode m_DownKeyCode = KeyCode.S;
    public KeyCode m_ReloadCode = KeyCode.R;
    [Header("Gravity")]

    //gravedad
    public float m_VerticalSpeed = 0.0f;
    float m_OldVerticalSpeed = 0.0f;//verticalSpeed de hace un frame
    public bool m_OnGround = false;
    private bool m_isJumping;
    public WallRun m_WallRun;

    [Header("Run and jump")]
    //x(sin(yaw),0,cos yaw)
    public KeyCode m_RunKeyCode = KeyCode.LeftShift;
    public KeyCode m_JumpKeyCode = KeyCode.Space;
    public float m_FastSpeedMultiplier = 1.2f;
    public float m_JumpSpeed = 10.0f;
    float m_time = 0.0f;
    public float m_GravityMultiplayer = 1.2f;
    public float m_MAxGravityMultiplayer = 4f;
    bool m_IncreaseGravity = false;

    [Header("Disparo")]
    public LayerMask m_ShootLayerMask;
    public GameObject m_HitParticle;
    public GameObject m_bullet;
    public PoolElements m_AmmoPool;
    public int m_MaxBulletsInCharger = 30;
    public int m_CurrentBullets = 30;
    int m_CurrentChargers = 0;
    int m_MaxChargers = 10;
    public float m_Cadence = 0.75f;
    float m_CurrentCadence= 0f;
    
    [Header("Debug")]
    //DEBUG
    public KeyCode m_DebugLockAngleKeyCode = KeyCode.I;
    public KeyCode m_DebugLockKeyCode = KeyCode.O;
    public KeyCode m_DebugSeeBulletTrajectory= KeyCode.Z;
    bool m_AngleLocked = false;
    bool m_AimLocked = true;
    public bool m_DebugBullet = false;

    [Header("Animation")]
    [SerializeField] Animator m_Animator;
    private IEnumerator m_CorutineStopAnim;

    [Header("Audio")]
    AudioSource m_AudioSource;
    public AudioSource m_AudioSourceWalk;//Creamos 2 para que puedan haber sonidos simultaneos
    //public AudioClip m_AudioSteps;
    public AudioClip m_AudioShoot;
    public AudioClip m_AudioReload;
    public AudioClip m_AudioItemPicked;
    public AudioClip m_AudioWalk1;
    public AudioClip m_AudioWalk2;
    bool m_SoundFinish = true;

    Vector3 l_Movement = Vector3.zero;
    [Header("WallJump")]
    public bool m_IsWallJumping = false;

   [SerializeField] float m_friction = 1f;
   [SerializeField] float m_WallMaxJumpTime = 1f;
    IEnumerator m_resetWallJump;
    Vector3 m_dir;

    [Header("Rest")]
    List<Transform> m_Checkpoints;
    int m_CurrentCheckpoint = 0;




    void Awake()
    {
        m_Yaw=transform.rotation.eulerAngles.y;
        m_Pitch=m_PitchControllerTransform.localRotation.eulerAngles.x;
        m_CharacterController=GetComponent<CharacterController>();
        GameController.GetGameController().SetPlayer(this);

        m_Animator.SetBool("Shoot", false);
        m_Animator.SetBool("Reload", false);

        m_AudioSource = GetComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

        m_resetWallJump = ResetWallJump();
        m_Checkpoints = GameController.GetGameController().GetLevelData().GetCheckpointsList();
        m_AmmoPool = new PoolElements(10, transform, m_HitParticle);
        
    }
    void Update()
    {
        if (GameController.GetGameController().GetGameStates() == GameStates.PLAY)
        {
#if UNITY_EDITOR
            if (Input.GetKeyDown(m_DebugLockAngleKeyCode))
                m_AngleLocked = !m_AngleLocked;
            if (Input.GetKeyDown(m_DebugLockKeyCode))
            {
                if (Cursor.lockState == CursorLockMode.Locked)
                    Cursor.lockState = CursorLockMode.None;
                else
                    Cursor.lockState = CursorLockMode.Locked;
                m_AimLocked = Cursor.lockState == CursorLockMode.Locked;
            }
            if (Input.GetKeyDown(m_DebugSeeBulletTrajectory))
            {
                m_DebugBullet = !m_DebugBullet;
            }
#endif
            //ROTACION CAMARA input
            float l_MouseAxisY = Input.GetAxis("Mouse Y");
            float l_MouseAxisX = Input.GetAxis("Mouse X");
#if UNITY_EDITOR
            if (m_AngleLocked)
            {
                l_MouseAxisY = 0.0f;
                l_MouseAxisX = 0.0f;
            }
#endif

            //m_pitch es x en mru x = x0+v*t
            m_Pitch += l_MouseAxisY * m_PitchRotationalSpeed * Time.deltaTime * (m_InvertedPitch ? -1.0f : 1.0f);
            m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

            m_Yaw += l_MouseAxisX * m_YawRotationalSpeed * Time.deltaTime * (m_InvertedYaw ? -1.0f : 1.0f);

            transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0);
            m_PitchControllerTransform.localRotation = Quaternion.Euler(m_Pitch, 0.0f, m_PitchControllerTransform.localRotation.eulerAngles.z);

            //MOVIMIENTO
            if (!m_IsWallJumping)//Si esta saltando desde la pared no reiniciamos el movimiento, para que sigue hacia la otra pared
                l_Movement = Vector3.zero;
            l_Movement = Vector3.zero;

            float l_YawInRadians = m_Yaw * Mathf.Deg2Rad;
            float l_Yaw90InRadians = (m_Yaw + 90.0f) * Mathf.Deg2Rad;
            Vector3 l_Forward = new Vector3(Mathf.Sin(l_YawInRadians), 0.0f, Mathf.Cos(l_YawInRadians));
            Vector3 l_Right = new Vector3(Mathf.Sin(l_Yaw90InRadians), 0.0f, Mathf.Cos(l_Yaw90InRadians));
            if (!m_IsWallJumping)
            {
                if (Input.GetKey(m_UpKeyCode))
                    l_Movement += l_Forward;
                else if (Input.GetKey(m_DownKeyCode))
                    l_Movement = -l_Forward;
                if (Input.GetKey(m_RightKeyCode))
                    l_Movement += l_Right;
                else if (Input.GetKey(m_LeftKeyCode))
                    l_Movement -= l_Right;
                l_Movement.Normalize();

            }
            l_Movement = l_Movement * Time.deltaTime * m_Speed;
            if (m_OnGround && l_Movement != Vector3.zero && m_SoundFinish)
            {
                m_SoundFinish = false;
                StartCoroutine(FinishSound());
                int l_Random = Random.Range(0, 2);
                m_AudioSourceWalk.PlayOneShot(l_Random == 0 ? m_AudioWalk1 : m_AudioWalk2);
            }

            //IMPLEMENTACION DE GRAVEDAD

            if (!m_OnGround && (m_VerticalSpeed == 0 || (m_OldVerticalSpeed < 0 && m_VerticalSpeed > 0)))
            {
                m_IncreaseGravity = true;
            }
        ;
            if (m_isJumping && m_WallRun.m_IsWallRunning)
            {
                StopCoroutine(m_resetWallJump);
                m_IsWallJumping = true;
                StartCoroutine(m_resetWallJump);

                l_Movement = Vector3.zero;

                m_dir = l_Forward + m_WallRun.WallJump();
                m_dir.Normalize();
                m_VerticalSpeed = 10;
            }
            //v = v0 +a*t
            m_VerticalSpeed += Physics.gravity.y * Time.deltaTime * (m_IncreaseGravity ? m_GravityMultiplayer : 1.0f);
            l_Movement.y = m_VerticalSpeed * Time.deltaTime;


            //comprobamos si estamos corriendo
            float l_SpeedMultiplier = 1.0f;
            if (Input.GetKey(m_RunKeyCode))
            {
                l_SpeedMultiplier = m_FastSpeedMultiplier;
            }

            if (!m_IsWallJumping)
            {
                l_Movement *= Time.deltaTime * m_Speed * l_SpeedMultiplier;
                m_friction = 1;
            }
            else
            {
                m_friction -= Time.deltaTime;
                m_friction = Mathf.Max(m_friction, 0.7f);
                l_Movement += m_dir * m_Speed * l_SpeedMultiplier * m_friction * Time.deltaTime;
            }
            CollisionFlags l_CollisionFlags = m_CharacterController.Move(l_Movement);//Macara binaria para saber como hemos chocado, por arriba abajo
            if ((l_CollisionFlags & CollisionFlags.Below) != 0)//Colisiona con el suelo
            {
                m_OnGround = true;
                if (!m_WallRun.m_IsWallRunning)
                    m_isJumping = false;
                m_VerticalSpeed = 0.0f;
                m_time = Time.time;
            }
            else
            {
                if (Time.time - m_time > 0.3)
                {
                    m_OnGround = false;
                }
            }

            //Cuando toca el techo caiga
            if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
                m_VerticalSpeed = 0.0f;

            if (Input.GetKeyDown(m_JumpKeyCode))
            {
                if (m_OnGround)
                {
                    Debug.Log("salta");
                    m_VerticalSpeed = m_JumpSpeed;
                }
                m_isJumping = true;
            }
            else
            {
                m_isJumping = false;
            }

            m_OldVerticalSpeed = m_VerticalSpeed;

            if (CanShoot() && (Input.GetMouseButtonDown(0) || Input.GetMouseButton(0)))
            {
                m_AudioSource.PlayOneShot(m_AudioShoot);
                m_Animator.SetBool("Shoot", true);
                Shoot();
            }
            else
            {
                if (m_Animator.GetBool("Shoot"))
                {
                    m_CorutineStopAnim = TurnOffAnimation("Shoot", 0.3f);
                    StartCoroutine(m_CorutineStopAnim);
                }

            }
            if (CanReload() && Input.GetKey(m_ReloadCode))
            {
                m_AudioSource.PlayOneShot(m_AudioReload);
                m_Animator.SetBool("Reload", true);
                Reload();
                m_CorutineStopAnim = TurnOffAnimation("Reload", 1.35f);
                StartCoroutine(m_CorutineStopAnim);
            }
            else
            {

            }
           
            m_CurrentCadence -= Time.deltaTime;
        }
    }
    bool CanShoot()
    {
        if (m_CurrentBullets > 0 && m_CurrentCadence <= 0)
            return true;
        else
            return false;
    }
    void Shoot()
    {
        m_CurrentCadence = m_Cadence; 
        Ray l_Ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0.0f));
        var l_bullet = m_bullet.GetComponent<Bullet>();
        l_bullet.m_BulletDebug = m_DebugBullet;
        l_bullet.m_dir = l_Ray.direction;
        l_bullet.transform.position = Camera.main.transform.position;
        m_CurrentBullets--;
        Instantiate(m_bullet);
    }

    bool CanReload()
    {
        if (m_CurrentChargers > 0 && m_Animator.GetBool("Reload")==false)
            return true;
        else
            return false;
    }
    void Reload()
    {
        m_CurrentChargers--;
        m_CurrentBullets = m_MaxBulletsInCharger;
    }
    public void CreateShootHitParticle(Vector3 HitPosition, Vector3 Normal)
    {
        GameObject l_Particles = m_AmmoPool.GetNextElement();
        l_Particles.SetActive(true);
        l_Particles.transform.SetParent(null);
        l_Particles.transform.position = HitPosition;
        l_Particles.transform.rotation = Quaternion.LookRotation(Normal);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
           Items l_item = other.GetComponent<Items>();
           if (l_item.CanPick())
           {
                m_AudioSource.PlayOneShot(m_AudioItemPicked,0.2f);     
                l_item.Pick();
           }
        }
        if (other.tag == "Checkpoint")
        {
            SetCurrentCheckpoint(other.gameObject.transform);
        }



    }

    IEnumerator ResetWallJump()
    {
        //m_GravityMultiplayer = m_MAxGravityMultiplayer;
        yield return new WaitForSeconds(m_WallMaxJumpTime);
        m_IsWallJumping = false;
    }

    public int GetCurrentBulletsInCharger()
    {
        return m_CurrentBullets;
    }

    public int GetCurrentChargers()
    {
        return m_CurrentChargers ;
    }
    public int GetMaxChargers()
    {
        return m_MaxChargers;
    }

    public void AddCharger(int amount)
    {
        m_CurrentChargers += amount;
    }

    public void Restart()
    {
        m_CharacterController.enabled = false;
        transform.position = m_Checkpoints[m_CurrentCheckpoint].position;
        
        transform.rotation = m_Checkpoints[m_CurrentCheckpoint].rotation;
        m_CharacterController.enabled = true;
        transform.GetComponent<HealthSystem>().RestartHealthSystem();
        
    }
    IEnumerator TurnOffAnimation(string Anim, float time)
    {
        yield return new WaitForSeconds(time);
        m_Animator.SetBool(Anim, false);

    }
    public void SetCurrentCheckpoint(Transform Checkpoint)
    {
        for (int i = 0; i < m_Checkpoints.Count; i++)
        {
            if(Checkpoint == m_Checkpoints[i])
            {
                m_CurrentCheckpoint = i;
            }
        }
    }
    public void SetDebugPointer(bool angleLocked)
    {
        m_AngleLocked = angleLocked;
        m_AimLocked = angleLocked;
    }
    IEnumerator FinishSound()
    {
        yield return new WaitForSeconds(0.5f);
        m_SoundFinish = true;

    }
}
