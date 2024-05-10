using Chinook.ClientModels;
using Chinook.Models;

namespace Chinook.Repositories
{
    public interface IArtistRepository
    {
        Task<ClientModels.Artist> GetArtistByArtistId(long artistId);
        Task<List<Models.Artist>> GetArtists(string searchName);
        Task<List<Album>> GetAlbumsByArtistId(long artistId);
    }
}
