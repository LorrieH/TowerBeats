using UnityEngine;

public class TowerLaser : MonoBehaviour {
    
    [SerializeField]
    private Transform m_Target;
    [SerializeField]
    private GameObject m_ImpactArtPrefab;
    private GameObject m_ImpactArt;

    [SerializeField]
    public Transform ShootPos;
    private Vector3 m_Difference;
    private Vector3 m_TargetOffsetPoint = new Vector3(0.04f,0.47f,0);

    [SerializeField]
    private float m_RaySpeed = 3;
    private float m_IntervalValue;
    private float m_Distance;
    private float m_Offset;
    private float m_AnimCooldown = 1.1f;
    private float m_AnimCooldownCounter;
    private float m_RotationZ;
    private float m_XScaleOrginValue;
    private float m_XScaleModTimer;
    private float m_XScaleModValue;

    private Material m_Mat;

    private bool m_Pause;

    private void Awake()
    {
        PauseCheck.Pause += TogglePause;
    }
    
    void Start ()
    {
        if (m_ImpactArt == null)
        {
            m_ImpactArt = Instantiate(m_ImpactArtPrefab, new Vector3(9999999, 9999999, 1),Quaternion.identity);
        }
        m_XScaleOrginValue = 0.4f;
        m_Mat = GetComponent<Renderer>().material;
	}

    public void SetTarget(Enemy enemyTarget, float interval)
    {
        m_Target = enemyTarget.transform;
        m_IntervalValue = interval;
    }

    public void TogglePause(bool pause)
    {
        m_Pause = pause;
    }


    void Update()
    {
        if (m_Target != null && m_IntervalValue > -0.2f)
        {
            m_IntervalValue -= Time.deltaTime;

            m_Offset += Time.deltaTime;

            if (m_Offset >= 1)
            {
                m_Offset = 0;
            }

            m_XScaleModTimer += Time.deltaTime;
            if (m_XScaleModTimer >= 0.06f)
            {
                m_XScaleModValue = m_XScaleOrginValue + Random.Range(-0.08f, 0.08f);
                m_XScaleModTimer = 0;
            }

            m_Distance = Vector3.Distance(new Vector3(ShootPos.position.x, ShootPos.position.y, -1), new Vector3(m_Target.transform.position.x, m_Target.transform.position.y, -1)+m_TargetOffsetPoint);

            transform.localScale = new Vector3(m_XScaleModValue, m_Distance, 1);
            transform.position = Vector3.Lerp(new Vector3(ShootPos.position.x, ShootPos.position.y, -1), new Vector3(m_Target.transform.position.x, m_Target.transform.position.y, -1) + m_TargetOffsetPoint, 0.5f);

            m_Mat.mainTextureScale = new Vector2(1, m_Distance);

            if (!m_Pause)
            {
                m_Mat.mainTextureOffset = new Vector2(1, m_Offset * m_RaySpeed);
            }

            m_Difference = (m_Target.transform.position +m_TargetOffsetPoint) - ShootPos.position;
            m_RotationZ = Mathf.Atan2(m_Difference.y, m_Difference.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0, 0, (m_RotationZ + 90));

            m_ImpactArt.transform.position = m_Target.transform.position + m_TargetOffsetPoint;
        }
        else if (m_Distance > 0)
        {
            m_ImpactArt.transform.position = new Vector3(9999999, 9999999, 1);
            m_Distance = 0;
            transform.localScale = new Vector3(1, 0, 1);
        }
    }
}