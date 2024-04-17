namespace Chinook.ClientModels;

public class AddTrackToPlaylist
{
    public long? PlaylistId { get; set; }
    public long TrackId { get; set; }
    public string Name { get; set; }
    public string UserId { get; set; }
}