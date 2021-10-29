public class GameOverMenu : Menu {

    /// <summary>
    /// Resets the playlist back to the first song.
    /// </summary>
	public void ResetPlaylist()
    {
        SongManager.s_OnPlaylistComplete();
    }
}