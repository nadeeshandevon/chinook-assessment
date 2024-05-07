using Chinook.Exceptions;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Repositories
{
    public class ArtistRepository : IArtistRepository
    {
        private readonly ChinookContext DbContext;
        public ArtistRepository(ChinookContext context)
        {
            DbContext = context;
        }

        public async Task<List<Album>> GetAlbumsByArtistId(long artistId)
        {
            if (artistId <= 0)
            {
                throw new ArgumentNullException($"{nameof(artistId)} cannot be empty");
            }

            var albums = await DbContext.Albums.AsNoTracking()
                .Where(a => a.ArtistId == artistId).ToListAsync();

            if (!albums.Any())
            {
                throw new CustomValidationException($"Artist could not found");
            }

            return albums;
        }

        public async Task<ClientModels.Artist> GetArtistByArtistId(long artistId)
        {
            if (artistId <= 0)
            {
                throw new ArgumentNullException($"{nameof(artistId)} cannot be empty");
            }

            var artist = await DbContext.Artists
                .AsNoTracking()
                .Select(t => new ClientModels.Artist()
                {
                    ArtistId = t.ArtistId,
                    Name = !string.IsNullOrWhiteSpace(t.Name)? t.Name : string.Empty
                })
                .FirstOrDefaultAsync(t => t.ArtistId == artistId);

            if (artist == null)
            {
                throw new CustomValidationException($"Artist could not found");
            }

            return artist;
        }

        public async Task<List<Artist>> GetArtists(string searchName)
        {
            var result = new List<Artist>();

            if (string.IsNullOrWhiteSpace(searchName))
            {
                result = await DbContext.Artists.Include(t => t.Albums)
                    .AsNoTracking()
                    .ToListAsync();
            }
            else
            {
                result = await DbContext.Artists.Include(t => t.Albums)
                    .AsNoTracking()
                    .Where(t => t.Name != null && t.Name.Trim().ToLower()
                    .Contains(searchName.Trim().ToLower())).ToListAsync();
            }

            return result;
        }
    }
}
