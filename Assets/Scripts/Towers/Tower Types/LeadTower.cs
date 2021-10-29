using Spine.Unity;
using System.Collections;
using UnityEngine;

public class LeadTower : Tower
{

    private GameObject m_Laser;
    private TowerLaser m_LaserData;

    public Transform laserOrgin;

    private SkeletonAnimation m_Animation;
    private bool m_CanShoot;
    private bool m_IsShooting;

    public override void Awake()
    {
        base.Awake();
        if(m_Laser == null)
        {
            m_Laser = Instantiate(attackProjectile);
        }
        m_LaserData = m_Laser.GetComponent<TowerLaser>();
        m_Laser.transform.position = new Vector3(-50, -50, 0);
        m_LaserData.ShootPos = laserOrgin;
        GetRMS.s_LeadCue += Attack;
        GetRMS.s_OnLeadLost += Idle;

        m_Animation = GetComponent<SkeletonAnimation>();
        StartCoroutine(SpawnEffect());
    }

    private IEnumerator SpawnEffect()
    {
        m_CanShoot = false;
        m_Animation.state.SetAnimation(0, "Lead_Turret_SPAWN", false);
        m_Animation.state.AddAnimation(0, "Lead_Turret_IDLE", true, 0);
        yield return new WaitForSeconds(0.45f);
        m_CanShoot = true;
    }

    public override void Attack()
    {
        if (!m_CanShoot) return;

        base.Attack();
        
        if(m_Target != null)
        {
            if (m_ReadyToAttack)
            {
                if (!m_IsShooting)
                {
                    m_Animation.state.SetAnimation(0, "Lead_Turret_ATTACK", true);
                    m_IsShooting = true;
                }
                m_LaserData.SetTarget(m_Target, TowerData.AttackInterval);

                m_Target.TakeDamage(TowerData.AttackDamage, "Lead");
                m_ReadyToAttack = false;
                m_StartedCooldown = false;
            }
        }
        else
        {
            Idle();
        }


    }

    private void Idle()
    {
        if (m_IsShooting)
        {
            m_Animation.state.SetAnimation(0, "Lead_Turret_IDLE", true);
            m_IsShooting = false;
        }
    }

    public override void Sell()
    {
        StartCoroutine(SellAnimation(() => {
            base.Sell();
        }));
    }

    private IEnumerator SellAnimation(System.Action onComplete = null)
    {
        m_CanShoot = false;
        m_Animation.state.SetAnimation(0, "Lead_Turret_SELL", false);
        yield return new WaitForSeconds(0.45f);
        if (onComplete != null) onComplete();
    }

    private void OnDestroy()
    {
        GetRMS.s_LeadCue -= Attack;
        GetRMS.s_OnLeadLost -= Idle;
    }
}