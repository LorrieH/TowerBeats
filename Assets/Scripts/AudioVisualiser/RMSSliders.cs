using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using DG.Tweening;

public class RMSSliders : MonoBehaviour
{
    /// <summary>
    /// Action for highlighting the slider in the tutorial.
    /// </summary>
    public static Action s_HighlightSlider;

    [SerializeField] private List<RMSSlider> m_Sliders;

    private void Awake()
    {
        s_HighlightSlider += HighlightSlider;
    }

    private void OnDestroy()
    {
        s_HighlightSlider -= HighlightSlider;
    }

    /// <summary>
    /// Finds the slider by instrument.
    /// </summary>
    /// <param name="instrument">Instrument to find slider of</param>
    /// <returns></returns>
    public RMSSlider GetSlider(InstrumentGroup instrument)
    {
        return m_Sliders.Find(x => x.Instrument == instrument);
    }

    /// <summary>
    /// Highlight the slider.
    /// </summary>
    void HighlightSlider()
    {
        transform.DOScale(1.1f, 0.3f).SetLoops(4, LoopType.Yoyo);
    }
}
