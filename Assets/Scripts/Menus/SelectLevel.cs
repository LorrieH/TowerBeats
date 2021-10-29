using System.Collections.Generic;
using UnityEngine;

public class SelectLevel : MonoBehaviour
{
    /// <summary>
    /// Playlist of songs.
    /// </summary>
    private List<Song> m_Playlist = new List<Song>();

    /// <summary>
    /// Select the currently selected level.
    /// </summary>
    public void SelectTheLevel()
    {
        m_Playlist.Clear();
        Level selectedLevel = ScrollLevels.s_Instance.GetSelectedLevel();

        if (!selectedLevel.Locked)
        {
            for (int i = 0; i < selectedLevel.Songs.Length; i++)
            {
                m_Playlist.Add(new Song(selectedLevel.Songs[i]));
            }
            SongManager.s_Instance.Songs = m_Playlist;
            SoundManager.s_Instance.StopSound(SoundNames.BACKGROUND_MUSIC);
            Sceneloader.s_Instance.LoadGameSceneWithLevel(selectedLevel.MapName);
        }
    }
}