using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestSongButtons : MonoBehaviour {

    /// <summary>
    /// Skips the current song.
    /// </summary>
	public void SkipSong()
    {
        SongManager.s_Instance.SkipSong();
    }
}
