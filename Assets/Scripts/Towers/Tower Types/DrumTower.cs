using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;

public class DrumTower : Tower
{
    private TowerProjectile m_TowerProjectileData;
    private ObjectPool m_Pool;
    private SkeletonAnimation m_Animation;
    [Space]
    [SerializeField] private Transform m_LeftMuzzle;
    [SerializeField] private Transform m_RightMuzzle;

    private bool m_ShotLastWithLeft;
    private bool m_CanShoot;

    public override void Awake()
    {
        base.Awake();
        m_Pool = ObjectPoolManager.s_Instance.GetObjectPool(attackProjectile, 10, 5, 5, 20, false,PooledSubObject.TowerProjectile);
        GetRMS.s_DrumCue += Attack;
        m_Animation = GetComponent<SkeletonAnimation>();
        StartCoroutine(SpawnEffect());
    }

    private IEnumerator SpawnEffect()
    {
        m_CanShoot = false;
        m_Animation.state.SetAnimation(0, "Drum_Turret_SPAWN", false);
        m_Animation.state.AddAnimation(0, "Drum_Turret_IDLE", true, 0);
        yield return new WaitForSeconds(0.5f);
        m_CanShoot = true;
    }

    public override void Attack()
    {
        if (!m_CanShoot) return;

        base.Attack();

        if (m_ReadyToAttack && m_Target != null)
        {
            m_TowerProjectileData = m_Pool.GetFromPool() as TowerProjectile;
            Transform selectedMuzzle;

            if (m_ShotLastWithLeft)
                selectedMuzzle = m_LeftMuzzle;
            else
                selectedMuzzle = m_RightMuzzle;

            m_ShotLastWithLeft = !m_ShotLastWithLeft;
            m_Animation.state.SetAnimation(0, "Drum_Turret_ATTACK_" + (m_ShotLastWithLeft ? "RIGHT" : "LEFT"), false);
            m_Animation.state.AddAnimation(0, "Drum_Turret_IDLE", true, 0);
            m_TowerProjectileData.SetData(new ProjectileData(selectedMuzzle.position, m_Target, 0.5f, TowerData.AttackDamage));
            m_ReadyToAttack = false;
            m_StartedCooldown = false;
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
        m_Animation.state.SetAnimation(0, "Drum_Turret_SELL", false);
        yield return new WaitForSeconds(0.5f);
        if (onComplete != null) onComplete();
    }

    private void OnDestroy()
    {
        GetRMS.s_DrumCue -= Attack;
    }
}