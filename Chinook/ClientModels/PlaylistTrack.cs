namespace Chinook.ClientModels;

public class PlaylistTrack
{
    public long TrackId { get; set; }
    public string TrackName { get; set; } = null!;
    public string AlbumTitle { get; set; } = null!;
    public string ArtistName { get; set; } = null!;
    public bool IsFavorite { get; set; }

}