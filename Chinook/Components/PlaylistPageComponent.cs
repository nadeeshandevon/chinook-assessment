using Chinook.ClientModels;
using Chinook.Exceptions;
using Chinook.Helpers;
using Chinook.Models;
using Chinook.Repositories;
using Microsoft.AspNetCore.Components;

namespace Chinook.Components
{
    public class PlaylistPageComponent : ChinookComponentBase
    {
        [Parameter] public long PlaylistId { get; set; }
        [Inject] IPlaylistRepository? PlaylistRepository { get; set; } = default!;

        public ClientModels.Playlist Playlist = new();

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CurrentUserId = await GetUserId();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        protected override async Task OnParametersSetAsync()
        {
            try
            {
                await LoadPlaylist();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadPlaylist()
        {
            Playlist =  await PlaylistRepository!.GetPlaylistByPlaylistId(PlaylistId, CurrentUserId);
        }

        public async Task FavoriteTrack(long trackId)
        {
            try
            {
                var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
                var addTrackToPlaylist = new AddTrackToPlaylist
                {
                    TrackId = trackId,
                    UserId = CurrentUserId,
                    Name = CommonConstants.MyFavoriteTrackPlayListName,
                };

                _ = await PlaylistRepository!.AddTrackToPlaylist(addTrackToPlaylist);
                await LoadPlaylist();
                InfoMessage = $"Track {track!.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task UnfavoriteTrack(long trackId)
        {
            try
            {
                var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
                var myFavoritePlaylistId = await PlaylistRepository!.GetMyFavoritePlaylistId(CurrentUserId);

                if (myFavoritePlaylistId == null)
                {
                    throw new CustomValidationException($"My favorite playlist not found");
                }

                _ = await PlaylistRepository.RemoveTrackFromPlaylist(trackId, myFavoritePlaylistId!.Value, CurrentUserId);
                await LoadPlaylist();
                InfoMessage = $"Track {track!.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task RemoveTrack(long trackId)
        {
            try
            {
                var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
                _ = await PlaylistRepository!.RemoveTrackFromPlaylist(trackId, PlaylistId, CurrentUserId);
                await LoadPlaylist();
                CloseInfoMessage();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
