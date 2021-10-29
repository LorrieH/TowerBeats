using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Effect types.
/// </summary>
public enum EffectType
{
    ENEMY_SPAWN,
    ENEMY_HIT,
    BassTurretFX_Attack,
    LASER_TURRET_ATTACK,
    SHOOT_TURRET_ATTACK,
    BassTurretFX_Spawn,
    BassTurretFX_Disappear,
    TURRET_SPAWN,
	LASERBEAM_HIT,
    EMPTY
}

public class EffectsManager : MonoBehaviour
{
    /// <summary>
    /// Instance of this script.
    /// </summary>
    public static EffectsManager s_Instance;

    /// <summary>
    /// This effect.
    /// </summary>
    [SerializeField] private VisualEffect m_EffectPrefab;

    /// <summary>
    /// Pool of all the effects.
    /// </summary>
    private List<VisualEffect> m_EffectsPool = new List<VisualEffect>();

    private void Awake()
    {
        if (s_Instance == null)
            s_Instance = this;
        else
            Destroy(gameObject);

        VisualEffect.s_OnEffectCompleted += OnEffectCompleted;

        InitEffectsPool(10);
    }

    /// <summary>
    /// Add each effect to the pool.
    /// </summary>
    /// <param name="amountOfEffects">Amount of total effects</param>
    private void InitEffectsPool(int amountOfEffects)
    {
        for (int i = 0; i < amountOfEffects; i++)
            AddEffectToPool();
    }

    /// <summary>
    /// Adds an effect to the pool.
    /// </summary>
    /// <returns>Returns an effect.</returns>
    private VisualEffect AddEffectToPool()
    {
        VisualEffect effect = Instantiate(m_EffectPrefab, transform, false);
        effect.gameObject.SetActive(false);
        m_EffectsPool.Add(effect);
        return effect;
    }

    private void OnDestroy()
    {
        VisualEffect.s_OnEffectCompleted -= OnEffectCompleted;
    }

    /// <summary>
    /// Searches for an effect.
    /// </summary>
    /// <param name="type">Effect to spawn</param>
    /// <param name="loop">Looping?</param>
    /// <param name="position">Position to spawn on</param>
    public void SpawnEffect(EffectType type, bool loop, Vector2 position)
    {
        for (int i = 0; i < m_EffectsPool.Count; i++)
        {
            if(!m_EffectsPool[i].InUse)
            {
                InitEffect(m_EffectsPool[i], type, loop, position);
                return;
            }
        }
        VisualEffect effect = AddEffectToPool();
        InitEffect(effect, type, loop, position);
    }

    /// <summary>
    /// Spawns an effect.
    /// </summary>
    /// <param name="effect"></param>
    /// <param name="type">Effect to spawn</param>
    /// <param name="loop">looping?</param>
    /// <param name="position">Position to spawn effect on</param>
    private void InitEffect(VisualEffect effect, EffectType type, bool loop, Vector2 position)
    {
        effect.gameObject.SetActive(true);
        effect.Init(type, loop);
        effect.transform.position = position;
    }

    /// <summary>
    /// On effect completed callback.
    /// </summary>
    /// <param name="effect"></param>
    private void OnEffectCompleted(VisualEffect effect)
    {
        for (int i = 0; i < m_EffectsPool.Count; i++)
        {
            if (effect == m_EffectsPool[i])
            {
                m_EffectsPool[i].gameObject.SetActive(false);
                m_EffectsPool[i].InUse = false;
            }
        }
    }
}