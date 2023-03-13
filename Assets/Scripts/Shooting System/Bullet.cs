using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public LayerMask m_ShootLayerMask;
    public GameObject m_HitParticle;
    public Ray m_RayDir;
    public Vector3 m_dir;
    public float m_speed = 10;
    public float m_gravity = -0.2f;
    LineRenderer m_lineRenderer;
    public bool m_BulletDebug=false;
    public bool m_ShowCompleteTrajectory = false;
    public string m_TagToDamage = "Enemy";
    // Start is called before the first frame update
    void Start()
    {
        if(m_BulletDebug)
            StartDebugBullet();

    }

    // Update is called once per frame
    void Update()
    {
        m_RayDir = new Ray(transform.position, m_dir);
        Debug.DrawRay(transform.position, m_dir * 1, Color.green);
        BulletTouchSomething();
    }
    private void LateUpdate()
    {
        if (m_BulletDebug)
            UpdateDebugBullet();
    }
    void StartDebugBullet()
    {
        GameObject l_gObject = new GameObject("DebugBullet");
        m_lineRenderer = l_gObject.AddComponent<LineRenderer>();
        DesactiveObjectOnTime l_destroy = l_gObject.AddComponent<DesactiveObjectOnTime>();
        l_destroy.m_DesactiveTime = this.GetComponent<DesactiveObjectOnTime>().m_DesactiveTime;

        m_lineRenderer.startWidth = 0.02f;
        m_lineRenderer.endWidth = 0.02f;
        m_lineRenderer.SetPosition(0, transform.position);
        m_lineRenderer.SetPosition(1, transform.position);
    }
    void UpdateDebugBullet()
    {
        if(!m_ShowCompleteTrajectory)
            m_lineRenderer.SetPosition(1, transform.position);
        else
        {
            m_lineRenderer.positionCount++;
            m_lineRenderer.SetPosition(m_lineRenderer.positionCount - 1, transform.position);
        }
    }
    void BulletTouchSomething()
    {
        Vector3 l_Movement = m_dir * m_speed * Time.deltaTime;
        l_Movement.y = l_Movement.y + m_gravity * Time.deltaTime;
        float l_distance = l_Movement.magnitude;
        l_Movement /= l_distance;
        RaycastHit l_RaycastHit;
        if (Physics.Raycast(transform.position  , transform.TransformDirection(m_dir), out l_RaycastHit, l_distance, m_ShootLayerMask.value))
        {
            Debug.DrawRay(transform.position, transform.TransformDirection(m_dir) * l_distance, Color.yellow);

            if (l_RaycastHit.collider.tag == m_TagToDamage)
            {
                if(m_TagToDamage == "Player")
                {
                    l_RaycastHit.transform.GetComponent<HealthSystem>().TakeDamage(10.0f);
                }
                else
                {
                    l_RaycastHit.collider.GetComponent<HitCollider>().Hit();
                    Debug.Log("LlamoHit");
                }
            }else if(l_RaycastHit.collider != null)
            {
                if(l_RaycastHit.collider.tag == "BasicEnemy")
                    {
                        l_RaycastHit.transform.GetComponent<HealthSystem>().TakeDamage(1.0f);
                    }
                if ((!(l_RaycastHit.collider.tag == "NoDecal")) && (!(l_RaycastHit.collider.tag == "Item")) && (!(l_RaycastHit.collider.tag == "BasicEnemy")))
                    CreateShootHitParticle(l_RaycastHit.point, l_RaycastHit.normal);
                gameObject.SetActive(false);
            }
            gameObject.SetActive(false);

        }
        else {
            Debug.DrawRay(transform.position, transform.TransformDirection(m_dir) * l_distance, Color.white);
            transform.position += l_Movement * l_distance;
        }
    }
    void CreateShootHitParticle(Vector3 HitPosition, Vector3 Normal)
    {
        GameController.GetGameController().GetPlayer().CreateShootHitParticle(HitPosition, Normal);
    }
}
