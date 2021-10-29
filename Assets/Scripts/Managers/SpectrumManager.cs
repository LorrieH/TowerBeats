using UnityEngine;

public enum BeatTypes
{
    BASS,
    KICK
}

public class SpectrumManager : MonoBehaviour {

    public static SpectrumManager s_Instance;
    public delegate BeatsInfo GetSpectrumInfo(BeatTypes currentBeatType, float currentSpectrumValue);
    public static GetSpectrumInfo GetCurrentBeatInfo;

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
        else
            Destroy(gameObject);

    }
}

public struct BeatsInfo
{
    public BeatTypes BeatType;
    public float spectrumValue;
}