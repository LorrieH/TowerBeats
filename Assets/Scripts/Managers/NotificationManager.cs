using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public struct Notification
{
    public Notification(string Text, float Duration, Color Color)
    {
        this.Text = Text;
        this.Duration = Duration;
        this.Color = Color;
    }

    public string Text;
    public float Duration;
    public Color Color;
}

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager s_Instance;
    [SerializeField] private Text m_NotificationText;
    [SerializeField] private CanvasGroup m_NotificationTextAlpha;
    [SerializeField] private Image m_Background;
    [SerializeField] private Image m_DonDiablo;

    private List<Notification> m_NotificationQueue = new List<Notification>();
    private bool m_NotificationQueueActive;
    private bool m_ShowingNotification;

    private Coroutine m_Queue;

    private void Awake()
    {
        Init();
    }

    /// <summary>
    /// Creates a instance of this object, if there is an instance already delete the new one
    /// </summary>
    private void Init()
    {
        if (s_Instance == null)
            s_Instance = this;
        else
            Destroy(gameObject);
    }

    public void ShowDon()
    {
        m_DonDiablo.color = new Color(m_DonDiablo.color.r, m_DonDiablo.color.g, m_DonDiablo.color.b, 0);
        m_DonDiablo.rectTransform.DOAnchorPosX(0, 0.33f).SetEase(Ease.OutExpo);
        m_DonDiablo.DOFade(1, 0.2f);
    }

    public void HideDon()
    {
        m_DonDiablo.color = new Color(m_DonDiablo.color.r, m_DonDiablo.color.g, m_DonDiablo.color.b, 1);
        m_DonDiablo.rectTransform.DOAnchorPosX(-100, 0.33f).SetEase(Ease.InExpo);
        m_DonDiablo.DOFade(0, 0.2f).SetDelay(0.13f);
    }

    /// <summary>
    /// Places a notification in the notification queue
    /// </summary>
    /// <param name="text">Notification text to show</param>
    /// <param name="duration">The duration of how long the notification is shown</param>
    public void EnqueueNotification(string text, float duration)
    {
        if (NotificationAlreadyExists(text)) return;
        m_NotificationQueue.Add(new Notification(text, duration, Color.white));

        StartQueue();
    }

    /// <summary>
    /// Places a notification in the notification queue and gives the text a color
    /// </summary>
    /// <param name="text">Notification text to show</param>
    /// <param name="duration">The duration of how long the notification is shown</param>
    /// <param name="color">Color of the notification text</param>
    public void EnqueueNotification(string text, float duration, Color color)
    {
        if (NotificationAlreadyExists(text)) return;
        m_NotificationQueue.Add(new Notification(text, duration, color));

        StartQueue();
    }

    /// <summary>
    /// Starts playing the notifications in the queue
    /// </summary>
    private void StartQueue()
    {
        if (!m_NotificationQueueActive)
            m_Queue = StartCoroutine(NotificationQueue());
    }

    /// <summary>
    /// Checks if the notification already exists in the queue
    /// </summary>
    /// <param name="text">Notification to check</param>
    /// <returns></returns>
    private bool NotificationAlreadyExists(string text)
    {
        return m_NotificationQueue.Contains(m_NotificationQueue.Find(x => x.Text == text));
    }

    /// <summary>
    /// Loops through a queue of notifications until it's empty
    /// </summary>
    /// <returns></returns>
    private IEnumerator NotificationQueue()
    {
        if (m_NotificationQueueActive) yield break;

        m_NotificationQueueActive = true;

        while(m_NotificationQueue.Count > 0)
        {
            if (!m_NotificationQueueActive) yield break;

            yield return new WaitWhile(() => m_ShowingNotification);
            ShowAnimation();
            m_NotificationText.text = "<color=#40FFFF>Don Diablo: </color>" + m_NotificationQueue[0].Text;
            m_NotificationText.color = m_NotificationQueue[0].Color;
            yield return new WaitForSeconds(m_NotificationQueue[0].Duration);
            m_NotificationQueue.RemoveAt(0);
            HideAnimation();
        }
        yield return new WaitWhile(() => m_ShowingNotification);

        ClearQueue();
    }

    /// <summary>
    /// Plays the animation for the notification queue
    /// </summary>
    private void ShowAnimation()
    {
        m_ShowingNotification = true;

        //Just to be sure
        m_Background.color = new Color(m_Background.color.r, m_Background.color.g, m_Background.color.b, 0);
        m_NotificationTextAlpha.alpha = 0;

        //Animation
        Sequence showSequence = DOTween.Sequence();
        showSequence.Append(m_Background.DOFade(0.5f, 0.2f).SetEase(Ease.OutQuad));
        showSequence.Join(m_NotificationTextAlpha.DOFade(1, 0.33f).SetEase(Ease.OutQuad));
    }

    /// <summary>
    /// Stops and hides the animation for the notification queue
    /// </summary>
    private void HideAnimation()
    {
        //Just to be sure
        m_Background.color = new Color(m_Background.color.r, m_Background.color.g, m_Background.color.b, 0.5f);
        m_NotificationTextAlpha.alpha = 1;

        //Animation
        Sequence hideSequence = DOTween.Sequence();
        hideSequence.Append(m_Background.DOFade(0f, 0.2f).SetEase(Ease.InQuad));
        hideSequence.Join(m_NotificationTextAlpha.DOFade(0f, 0.33f).SetEase(Ease.InQuad)).OnComplete(() => m_ShowingNotification = false);
    }


    /// <summary>
    /// Removes all the messages from the notification queue
    /// </summary>
    public void ClearQueue()
    {
        if (m_ShowingNotification)
            HideAnimation();

        m_NotificationQueueActive = false;
        m_ShowingNotification = false;
        m_NotificationQueue.Clear();

        if(m_Queue != null)
            StopCoroutine(m_Queue);
    }
}
