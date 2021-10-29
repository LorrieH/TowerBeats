using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kino;
using UnityEngine.UI;

//Used to identify the type of instrument
public enum InstrumentGroup
{
    Bass,
    Drum,
    Lead
}

public class GetRMS : MonoBehaviour {

    /// <summary>
    /// Audio cues.
    /// </summary>
    public delegate void AudioCueEvent();
    public static AudioCueEvent s_BassCue;
    public static AudioCueEvent s_DrumCue;
    public static AudioCueEvent s_LeadCue;

    /// <summary>
    /// Event which gets triggered if the audio is lost.
    /// </summary>
    public static AudioCueEvent s_OnBassLost;
    public static AudioCueEvent s_OnLeadLost;

    /// <summary>
    /// Bloom
    /// </summary>
    private Bloom m_BloomProfile;
    private float m_BloomIntensity;

    /// <summary>
    /// Timer for resetting animations.
    /// </summary>
    private float m_Timer;

    /// <summary>
    /// Instrument of this object.
    /// </summary>
    public InstrumentGroup Instrument;

    /// <summary>
    /// Slider it is connected with.
    /// </summary>
    public RMSSlider Slider { get; set; }

    private int qSamples = 512;  // array size
    private float rmsValue;   // sound level - RMS

    private float[] samples; // audio samples

    [SerializeField]private AudioSource m_Source;

    private void OnEnable()
    {
        Sceneloader.s_OnSceneLoaded += SetSlider;
        if (Instrument == InstrumentGroup.Lead)
        {
            Sceneloader.s_OnSceneLoaded += GetPostPostProcessingBehaviour;
        }
    }

    void Start()
    {
        samples = new float[qSamples];
    }

    /// <summary>
    /// Gets the volume of the current instrument.
    /// </summary>
    void GetVolume()
    {
        m_Timer += Time.deltaTime;

        m_Source.GetOutputData(samples, 0); // fill array with samples
        int i;
        float sum = 0;

        for (i = 0; i < qSamples; i++)
        {
            sum += samples[i] * samples[i]; // sum squared samples
        }
        rmsValue = Mathf.Sqrt(sum / qSamples); // rms = square root of average

        if(rmsValue >= 0.06)
        {
            switch (Instrument)
            {
                case InstrumentGroup.Bass:
                    m_Timer = 0f;
                    if(s_BassCue != null)
                        s_BassCue();
                    break;
                case InstrumentGroup.Drum:
                    if(s_DrumCue != null)
                        s_DrumCue();
                    break;
                case InstrumentGroup.Lead:
                    if (s_LeadCue != null)
                        s_LeadCue();
                    break;
            }
        }
        else
        {
            switch (Instrument)
            {
                case InstrumentGroup.Bass:
                    if (m_Timer > 0.5f && s_OnBassLost != null) s_OnBassLost();
                    break;

                case InstrumentGroup.Lead:
                    if (s_OnLeadLost != null) s_OnLeadLost();
                    break;
            }
        }
    }

    /// <summary>
    /// Sets camera for bloom.
    /// </summary>
    void GetPostPostProcessingBehaviour()
    {
        m_BloomProfile = Camera.main.GetComponent<Bloom>();
    }

    /// <summary>
    /// Sets the slider and its threshold.
    /// </summary>
    void SetSlider()
    {
        Slider = FindObjectOfType<RMSSliders>().GetSlider(Instrument);
        Slider.SetThreshold(0.06f * 6f);
    }

    void Update()
    {
        GetVolume();
        if (Slider != null)
            Slider.SetFill((rmsValue * 6));
        if(m_BloomProfile != null && Instrument == InstrumentGroup.Lead)
        {
            m_BloomIntensity = Mathf.Clamp(1+(rmsValue * 6), 1f, 2f);
            m_BloomProfile.intensity = m_BloomIntensity;
        }
    }

    private void OnDisable()
    {
        Sceneloader.s_OnSceneLoaded -= SetSlider;
    }
}
