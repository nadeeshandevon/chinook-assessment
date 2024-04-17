using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Components
{
    public class PlaylistPageComponent : ComponentBase
    {
        [Parameter] public long PlaylistId { get; set; }
        [Inject] IPlaylistRepository UserPlayListRepo { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; }

        public ClientModels.Playlist Playlist;
        public string CurrentUserId = string.Empty;
        public string InfoMessage = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            CurrentUserId = await GetUserId();
        }

        protected override void OnParametersSet()
        {
            LoadPlaylist();
        }

        private void LoadPlaylist()
        {
            Playlist = UserPlayListRepo.GetPlaylistByPlaylistId(PlaylistId, CurrentUserId).ConfigureAwait(false).GetAwaiter().GetResult();
        }
        public async Task<string> GetUserId()
        {
            var user = (await AuthenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        public void FavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            UserPlayListRepo.AddTrackToFavorite(trackId, CurrentUserId).ConfigureAwait(false);
            LoadPlaylist();
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
        }

        public void UnfavoriteTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            UserPlayListRepo.RemoveTrackFromFavorite(trackId, CurrentUserId).ConfigureAwait(false);
            LoadPlaylist();
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
        }

        public void RemoveTrack(long trackId)
        {
            var track = Playlist.Tracks.FirstOrDefault(t => t.TrackId == trackId);
            UserPlayListRepo.RemoveTrackFromPlaylist(trackId, PlaylistId, CurrentUserId).ConfigureAwait(false);
            LoadPlaylist();
            CloseInfoMessage();
        }

        public void CloseInfoMessage()
        {
            InfoMessage = string.Empty;
        }
    }
}
