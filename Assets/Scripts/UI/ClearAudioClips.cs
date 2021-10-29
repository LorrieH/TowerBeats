using UnityEngine;

public class ClearAudioClips : MonoBehaviour {

    /// <summary>
    /// Delegate whichs goes off when audio gets cleared.
    /// </summary>
    public delegate void ClearAudioEvent();
    public static ClearAudioEvent s_OnClearAudio;

    /// <summary>
    /// Executes the OnClearAudio delegate.
    /// </summary>
    public void ClearAudio()
    {
        if(s_OnClearAudio != null)
        {
            s_OnClearAudio();
        }
    }
}
