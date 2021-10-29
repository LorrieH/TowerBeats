/// <summary>
/// Songs class.
/// </summary>
[System.Serializable]
public class Song
{
    /// <summary>
    /// Constructor of the Song
    /// </summary>
    /// <param name="songName">Name of the Song</param>
    public Song(string songName)
    {
        Songname = songName;
    }

    /// <summary>
    /// Song name
    /// </summary>
    public string Songname;
}
