namespace Chinook.Models
{
    public partial class Artist
    {
        public Artist()
        {
            Albums = new HashSet<Album>();
        }

        public long ArtistId { get; set; }
        public string? Name { get; set; } = null!;

        public virtual ICollection<Album> Albums { get; set; }
    }
}
