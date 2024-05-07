namespace Chinook.ClientModels;

public class Playlist
{
    public Playlist()
    {
        Tracks = new List<PlaylistTrack>();
    }
    public long PlaylistId { get; set; }
    public string? Name { get; set; } = null!;
    public List<PlaylistTrack> Tracks { get; set; }
}