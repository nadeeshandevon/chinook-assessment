﻿using Chinook.ClientModels;

namespace Chinook.Repositories
{
    public interface IPlaylistRepository
    {
        Task<bool> AddTrackToPlaylist(AddTrackToPlaylist addTrackToPlaylist);
        Task<bool> AddTrackToFavorite(long trackId, string userId);
        Task<bool> RemoveTrackFromFavorite(long trackId, string userId);
        Task<bool> RemoveTrackFromPlaylist(long trackId, long playlistId, string userId);
        Task<List<PlaylistTrack>> GetPlaylistTracksByArtistId(long artistId, string userId);
        Task<Playlist> GetPlaylistByPlaylistId(long playlistId, string userId);
        Task<List<Playlist>> GetPlaylistByUserId(string userId);
    }
}
