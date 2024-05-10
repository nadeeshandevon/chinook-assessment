namespace Chinook.ClientModels;

public class AddTrackToPlaylist
{
    public long? PlaylistId { get; set; } = null!;
    public long TrackId { get; set; }
    public string Name { get; set; } = null!;
    public string UserId { get; set; } = null!;
}