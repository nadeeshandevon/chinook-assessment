using Chinook.ClientModels;
using Chinook.Exceptions;
using Chinook.Helpers;
using Chinook.Models;
using Microsoft.EntityFrameworkCore;

namespace Chinook.Repositories
{
    public class PlaylistRepository : IPlaylistRepository
    {
        private readonly ChinookContext DbContext;
        public PlaylistRepository(ChinookContext context)
        {
            DbContext = context;
        }

        public async Task<bool> AddTrackToPlaylist(AddTrackToPlaylist addTrackToPlaylist)
        {
            if (addTrackToPlaylist == null)
            {
                throw new ArgumentNullException($"{nameof(addTrackToPlaylist)} cannot be empty");
            }

            if (addTrackToPlaylist.TrackId <= 0)
            {
                throw new ArgumentNullException($"{nameof(addTrackToPlaylist.TrackId)} cannot be empty");
            }

            if (string.IsNullOrWhiteSpace(addTrackToPlaylist.UserId))
            {
                throw new ArgumentNullException($"{nameof(addTrackToPlaylist.UserId)} cannot be empty");
            }

            Models.Playlist? playlist = null;
            long playlistId = 0;

            using var transaction = await DbContext.Database.BeginTransactionAsync();

            try
            {
                if (!string.IsNullOrWhiteSpace(addTrackToPlaylist.Name) &&
                addTrackToPlaylist.Name.Equals(CommonConstants.MyFavoriteTrackPlayListName, StringComparison.InvariantCultureIgnoreCase))
                {
                    addTrackToPlaylist.PlaylistId = await GetMyFavoritePlaylistId(addTrackToPlaylist.UserId);
                }

                if (addTrackToPlaylist.PlaylistId.HasValue)
                {
                    playlist = await DbContext.Playlists.Include(t => t.Tracks)
                    .FirstOrDefaultAsync(t => t.PlaylistId == addTrackToPlaylist.PlaylistId.Value);

                    if (playlist!.Tracks.Any(t => t.TrackId == addTrackToPlaylist.TrackId))
                    {
                        throw new CustomValidationException($"This track is already in {playlist.Name} playlist");
                    }

                    playlistId = addTrackToPlaylist.PlaylistId.Value;
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(addTrackToPlaylist.Name))
                    {
                        throw new ArgumentNullException($"{nameof(addTrackToPlaylist.Name)} cannot be empty");
                    }

                    playlist = await DbContext.Playlists.Include(t => t.Tracks).Include(t => t.UserPlaylists)
                                .Where(t => t.UserPlaylists.Any(u => u.UserId == addTrackToPlaylist.UserId)
                                && t.Name.Trim()
                                         .ToLower()
                                         .Equals(addTrackToPlaylist.Name.Trim().ToLower()))
                                .FirstOrDefaultAsync();

                    if (playlist != null)
                    {
                        throw new CustomValidationException($"Playlist already exist");
                    }

                    var nextPlaylistid = await DbContext.Playlists.MaxAsync(t => t.PlaylistId) + 1;
                    var nextSortOrder = await DbContext.Playlists
                                        .Where(t => t.UserPlaylists.Any(u => u.UserId == addTrackToPlaylist.UserId))
                                        .MaxAsync(t => t.SortOrder) + 1;

                    playlist = new Models.Playlist
                    {
                        PlaylistId = nextPlaylistid,
                        Name = addTrackToPlaylist.Name,
                        SortOrder = nextSortOrder
                    };

                    _ = await DbContext.AddAsync(playlist);
                    _ = await DbContext.SaveChangesAsync();

                    playlistId = nextPlaylistid;
                }

                var track = await DbContext.Tracks.FirstOrDefaultAsync(t => t.TrackId == addTrackToPlaylist.TrackId);

                playlist.Tracks.Add(track!);
                DbContext.Attach(playlist);

                var userPlaylist = await DbContext.UserPlaylists
                    .FirstOrDefaultAsync(t => t.PlaylistId == playlistId && t.UserId == addTrackToPlaylist.UserId);

                if (userPlaylist == null)
                {
                    var userPlayList = new UserPlaylist
                    {
                        PlaylistId = playlistId,
                        UserId = addTrackToPlaylist.UserId
                    };

                    _ = await DbContext.AddAsync(userPlayList);
                }

                _ = await DbContext.SaveChangesAsync();
                await transaction.CommitAsync();

            }
            catch (Exception)
            {
                await transaction.RollbackAsync();
                throw;
            }

            return true;
        }

        public async Task<long?> GetMyFavoritePlaylistId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException($"{nameof(userId)} cannot be empty");
            }

            var favoritePlaylist = await DbContext.Playlists
            .Where(t => t.Name == CommonConstants.MyFavoriteTrackPlayListName && t.UserPlaylists.Any(p => p.UserId == userId))
            .FirstOrDefaultAsync();

            if(favoritePlaylist != null)
            {
                return favoritePlaylist!.PlaylistId;
            }

            return null;
        }

        public async Task<bool> RemoveTrackFromPlaylist(long trackId, long playlistId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException($"{nameof(userId)} cannot be empty");
            }

            if (playlistId < 0)
            {
                throw new ArgumentNullException($"{nameof(playlistId)} cannot be empty");
            }

            if (trackId <= 0)
            {
                throw new ArgumentNullException($"{nameof(trackId)} cannot be empty");
            }

            var playlist = await DbContext.Playlists.Include(t => t.Tracks)
                .FirstOrDefaultAsync(t => t.PlaylistId == playlistId);

            var track = playlist!.Tracks.FirstOrDefault(t => t.TrackId == trackId);

            playlist.Tracks.Remove(track!);
            DbContext.Attach(playlist);

            _ = await DbContext.SaveChangesAsync();

            return true;
        }

        public async Task<List<PlaylistTrack>> GetPlaylistTracksByArtistId(long artistId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException($"{nameof(userId)} cannot be empty");
            }

            if (artistId <= 0)
            {
                throw new ArgumentNullException($"{nameof(artistId)} cannot be empty");
            }

            var tracks = await DbContext.Tracks.Include(t => t.Album)
                .AsNoTracking()
                .Where(a => a.Album!.ArtistId == artistId)
                .Include(a => a.Album)
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = (t.Album == null ? "-" : t.Album.Title),
                    TrackId = t.TrackId,
                    TrackName = t.Name,
                    IsFavorite = t.Playlists.Where(p => p.UserPlaylists
                    .Any(up => up.UserId == userId && up.Playlist.Name == CommonConstants.MyFavoriteTrackPlayListName)).Any()
                }).ToListAsync();

            return tracks;
        }

        public async Task<ClientModels.Playlist> GetPlaylistByPlaylistId(long playlistId, string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
            {
                throw new ArgumentNullException($"{nameof(userId)} cannot be empty");
            }

            if (playlistId < 0)
            {
                throw new ArgumentNullException($"{nameof(playlistId)} cannot be empty");
            }

            var playlist = await DbContext.Playlists.AsNoTracking()
                .Include(a => a.Tracks)
                .ThenInclude(a => a.Album)
                .ThenInclude(a => a.Artist)
                .Include(a => a.UserPlaylists)
                .Where(p => p.PlaylistId == playlistId && p.UserPlaylists.Any(u => u.UserId == userId))
                .Select(p => new
                {
                    p.Name,
                    Tracks = p.Tracks.Select(t => new
                    {
                        AlbumTitle = t.Album != null ? t.Album.Title : string.Empty,
                        ArtistName = t.Album != null ? t.Album.Artist.Name : string.Empty,
                        t.TrackId,
                        TrackName = t.Name,
                        IsFavorite = t.Playlists.Where(p => p.UserPlaylists
                        .Any(up => up.UserId == userId && up.Playlist.Name == CommonConstants.MyFavoriteTrackPlayListName)).Any()
                    })
                })
                .FirstOrDefaultAsync();

            var returnPlaylist = new ClientModels.Playlist
            {
                Name = !string.IsNullOrWhiteSpace(playlist!.Name) ? playlist.Name : string.Empty,
                Tracks = playlist.Tracks
                .Select(t => new PlaylistTrack()
                {
                    AlbumTitle = t.AlbumTitle,
                    ArtistName = !string.IsNullOrWhiteSpace(t.ArtistName) ? t.ArtistName : string.Empty,
                    TrackId = t.TrackId,
                    TrackName = t.TrackName,
                    IsFavorite = t.IsFavorite
                }
                ).ToList()
            };

            return returnPlaylist;
        }

        public async Task<List<ClientModels.Playlist>> GetPlaylistByUserId(string userId)
        {
            var playlists = await DbContext.Playlists.Include(t => t.UserPlaylists)
                .AsNoTracking()
                .Where(t => t.UserPlaylists.Any(p => p.UserId == userId))
                .OrderBy(t => t.Name == CommonConstants.MyFavoriteTrackPlayListName ? 0 : 1).ThenBy(t => t.SortOrder)
                .Select(p => new ClientModels.Playlist()
                {
                    PlaylistId = p.PlaylistId,
                    Name = !string.IsNullOrWhiteSpace(p.Name) ? p.Name : string.Empty,
                })
                .ToListAsync();

            return playlists;
        }
    }
}
