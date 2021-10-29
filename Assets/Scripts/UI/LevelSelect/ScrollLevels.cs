using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class ScrollLevels : MonoBehaviour {

    [SerializeField] private Transform[] m_LevelPositions;
    [SerializeField] private List<GameObject> m_Levels = new List<GameObject>();

    private bool m_IsAnimating;

    public static ScrollLevels s_Instance;

    private void Awake()
    {
        if (s_Instance == null)
        {
            s_Instance = this;
        }
        else
            Destroy(gameObject);

        StartCoroutine(LateStart());

        // Play background music if the background music isn't playing yet
        if (!SoundManager.s_Instance.IsSoundPlaying(SoundNames.BACKGROUND_MUSIC))
            SoundManager.s_Instance.PlaySound(SoundNames.BACKGROUND_MUSIC, true);
    }

    /// <summary>
    /// Wait for a frame before this starts running
    /// </summary>
    /// <returns></returns>
    IEnumerator LateStart()
    {
        yield return new WaitForEndOfFrame();

        if(MapInfo.s_OnLevelChange != null)
            MapInfo.s_OnLevelChange(GetSelectedLevel());
    }

    /// <summary>
    /// Selects the next level in the list
    /// </summary>
    public void NextLevel()
    {
        RepositionLevels(-1);
    }

    /// <summary>
    /// Selects the previous level in the list
    /// </summary>
    public void PreviousLevel()
    {
        RepositionLevels(1);
    }

    /// <summary>
    /// Wait for tween to finish
    /// </summary>
    /// <returns></returns>
    IEnumerator WaitForTween()
    {
        m_IsAnimating = true;
        yield return new WaitForSeconds(0.1f);
        m_IsAnimating = false;
    }

    /// <summary>
    /// Reposition the level buttons in the wheel in a certain direction
    /// </summary>
    /// <param name="direction">Direction the level buttons move in</param>
    void RepositionLevels(int direction)
    {
        if (!m_IsAnimating)
        {
            SoundManager.s_Instance.PlaySound(SoundNames.LEVELSELECTSCROLLSOUND);

            StartCoroutine(WaitForTween());

            if (direction == 1)
            {
                for (int i = 0; i < m_Levels.Count; i++)
                {
                    if (i <= 0)
                        m_Levels[i].transform.position = m_LevelPositions[m_LevelPositions.Length - 1].transform.position;
                    else
                        m_Levels[i].transform.DOMove(m_LevelPositions[i - 1].transform.position, 0.1f);
                }
            }
            else
            {
                for (int i = 0; i < m_Levels.Count; i++)
                {
                    if (i >= m_Levels.Count - 1)
                        m_Levels[i].transform.position = m_LevelPositions[0].transform.position;
                    else
                        m_Levels[i].transform.DOMove(m_LevelPositions[i + 1].transform.position, 0.1f);
                }
            }

            RepositionList(direction);
        }
    }

    /// <summary>
    /// Reorders the list.
    /// </summary>
    /// <param name="direction"></param>
    public void RepositionList(int direction)
    {
        List<GameObject> tmpList = new List<GameObject>();

        for (int i = 0; i < m_Levels.Count; i++)
        {
            tmpList.Add(m_Levels[i]);
        }

        //Clear old list
        m_Levels.Clear();

        if(direction == 1)
        {
            int lastIndex = tmpList.Count - 1;
            //Reorder the list so that everything moves down 1 slot.

            for (int i = 0; i < tmpList.Count; i++)
            {
                if (i != lastIndex)
                {
                    m_Levels.Add(tmpList[i + 1]);
                }
                else
                {
                    m_Levels.Add(tmpList[0]);
                }
            }
        }
        else
        {
            m_Levels.Add(tmpList[tmpList.Count - 1]);
            tmpList.RemoveAt(tmpList.Count - 1);

            for (int i = 0; i < tmpList.Count; i++)
            {
                m_Levels.Add(tmpList[i]);
            }
        }

        MapInfo.s_OnLevelChange(GetSelectedLevel());
    }

    /// <summary>
    /// Get the level data from the currently selected level
    /// </summary>
    /// <returns></returns>
    public Level GetSelectedLevel()
    {
        SetColors();
        Level selectedLevel = m_Levels[3].GetComponent<Level>();
        return selectedLevel;
    }

    /// <summary>
    /// Sets the color of the level image
    /// </summary>
    private void SetColors()
    {
        for (int i = 0; i < m_Levels.Count; i++)
        {
            if(i != 3)
            {
                Image[] images = m_Levels[i].GetComponentsInChildren<Image>();

                foreach (Image image in images)
                {
                   if(image != m_Levels[i].GetComponent<Image>())
                    {
                        image.color = Color.gray;
                    } 
                }
            }
            else
            {
                Image[] images = m_Levels[i].GetComponentsInChildren<Image>();

                foreach (Image image in images)
                {
                    if (image != m_Levels[i].GetComponent<Image>())
                    {
                        image.color = Color.white;
                    }
                }
            }
        }
    }

    /// <summary>
    /// Resets the color of the level buttons
    /// </summary>
    private void ResetButtonColors()
    {
        for (int i = 0; i < m_Levels.Count; i++)
        {
            m_Levels[i].GetComponent<Button>().image.color = Color.white;
        }
    }
}
