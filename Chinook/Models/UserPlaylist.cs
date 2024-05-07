namespace Chinook.Models;

public class UserPlaylist
{
    public string UserId { get; set; } = null!;
    public long PlaylistId { get; set; }
    public ChinookUser User { get; set; } = null!;
    public Playlist Playlist { get; set; } = null!;
}
