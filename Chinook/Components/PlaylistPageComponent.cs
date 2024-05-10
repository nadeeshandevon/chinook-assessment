using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Repositories;
using Microsoft.AspNetCore.Components;
using NuGet.DependencyResolver;

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
                await LoadInitialData();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private async Task LoadInitialData()
        {
            CurrentUserId = await GetUserId();
            await LoadPlaylist();
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
            Tracks = Playlist.Tracks;
        }

        public async Task FavoriteTrack(long trackId)
        {
            try
            {
                await FavoriteTrackByTrackId(trackId);
                await LoadInitialData();
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
                await UnfavoriteTrackByTrackId(trackId);
                await LoadInitialData();
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
