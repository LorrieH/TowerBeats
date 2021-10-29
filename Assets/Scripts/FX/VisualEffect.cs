using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spine.Unity;
using Spine;
using System;

public class VisualEffect : MonoBehaviour
{
    /// <summary>
    /// Skeleton animation for this effect.
    /// </summary>
    private SkeletonAnimation m_Animation;

    /// <summary>
    /// Meshrenderer of this effect.
    /// </summary>
    private MeshRenderer m_Renderer;

    /// <summary>
    /// Delegate for when the effect is completed.
    /// </summary>
    /// <param name="effect"></param>
    public delegate void EffectCompleted(VisualEffect effect);
    public static EffectCompleted s_OnEffectCompleted;

    /// <summary>
    /// Bool to check if the effect is in use.
    /// </summary>
    public bool InUse { get; set; }

    private void Awake()
    {
        m_Animation = GetComponent<SkeletonAnimation>();
        m_Renderer = GetComponent<MeshRenderer>();
    }

    /// <summary>
    /// Set the animation.
    /// </summary>
    /// <param name="type">Effect which gets called</param>
    /// <param name="loop">looping?</param>
    public void Init(EffectType type, bool loop)
    {
        InUse = true;

        switch (type)
        {
            case EffectType.ENEMY_SPAWN:
                m_Animation.state.SetAnimation(0,"Spawn_Enemy FX",loop);
                break;
            case EffectType.ENEMY_HIT:
                m_Animation.state.SetAnimation(0,"Hit FX",loop);
                break;
            case EffectType.BassTurretFX_Spawn:
                m_Animation.state.SetAnimation(0, "BassTurretFX_Spawn",loop);
                break;
            case EffectType.BassTurretFX_Attack:
                m_Animation.state.SetAnimation(0, "BassTurretFX_Attack", loop);
                break;
            case EffectType.BassTurretFX_Disappear:
                m_Animation.state.SetAnimation(0, "BassTurretFX_Disappear", loop);
                break;
            case EffectType.TURRET_SPAWN:
                m_Animation.state.SetAnimation(0, "Spawn_Turret FX",loop);
                break;
            case EffectType.EMPTY:
            default:
                m_Animation.state.SetAnimation(0, "1_Empty", loop);
                break;
        }

        m_Animation.loop = loop;
        m_Animation.AnimationState.Complete += OnEffectComplete;
    }

    /// <summary>
    /// Set the layer of the effect.
    /// </summary>
    /// <param name="layer">Layer number</param>
    public void SetLayer(int layer)
    {
        m_Renderer.sortingOrder = layer;
    }

    /// <summary>
    /// Stop this animation.
    /// </summary>
    public void Stop()
    {
        m_Animation.state.ClearTracks();
    }

    /// <summary>
    /// Callback when the effect is completed.
    /// </summary>
    /// <param name="trackEntry"></param>
    private void OnEffectComplete(TrackEntry trackEntry)
    {
        m_Animation.AnimationState.Complete -= OnEffectComplete;
        if (s_OnEffectCompleted != null) s_OnEffectCompleted(this);
    }
}
