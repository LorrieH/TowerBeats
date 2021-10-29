using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuAvatar : MonoBehaviour
{
    #region Variables

    [SerializeField] private Image m_EyeLids;
    [SerializeField] private Image m_OpenMouth;

    /// <summary>
    /// Blinking coroutine
    /// </summary>
    private Coroutine m_Blinking;

    #endregion

    #region Monobehaviour Functions

    private void Start()
    {
        // Start the blinking if the avatar isn't blinking already
        if(m_Blinking == null)
            m_Blinking = StartCoroutine(RandomBlink());
    }

    private void OnDestroy()
    {
        // Stop the blinking
        StopCoroutine(m_Blinking);
        m_Blinking = null;
    }

    #endregion

    #region Avatar Features

    /// <summary>
    /// Blinks the avatar's eyes
    /// </summary>
    private IEnumerator RandomBlink()
    {
        m_EyeLids.enabled = true;
        yield return new WaitForSeconds(UnityEngine.Random.Range(0.1f, 0.4f));
        m_EyeLids.enabled = false;
        yield return new WaitForSeconds(UnityEngine.Random.Range(1f, 3f));
        m_Blinking = StartCoroutine(RandomBlink());
    }

    /// <summary>
    /// Sets the mouth state of the avatar
    /// </summary>
    /// <param name="open">Open the mouth?</param>
    public void SetMouthState(bool open)
    {
        if(open)
        {
            StopCoroutine(m_Blinking);
            m_EyeLids.enabled = true;
        }
        else
        {
            m_Blinking = StartCoroutine(RandomBlink());
        }
        m_OpenMouth.enabled = open;
    }

    #endregion
}
