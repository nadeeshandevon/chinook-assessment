using Chinook.ClientModels;
using Chinook.Models;
using Chinook.Repositories;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Components
{
    public class ArtistPageComponent : ComponentBase
    {
        [Parameter] public long ArtistId { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; }
        [Inject] IPlaylistRepository? PlaylistRepository { get; set; }
        [Inject] IArtistRepository? ArtistRepository { get; set; }
        [Inject] AppState? AppState { get; set; }
        public Modal PlaylistDialog { get; set; }
        public ClientModels.Artist? Artist;
        public List<PlaylistTrack> Tracks = new();
        public List<ClientModels.Playlist> Playlists = new();
        public PlaylistTrack? SelectedTrack;
        public string InfoMessage = string.Empty;
        public string CurrentUserId = string.Empty;
        public long SelectedPlaylist;
        public string NewPlaylistName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            CurrentUserId = await GetUserId();

            Artist = await ArtistRepository.GetArtistByArtistId(ArtistId);
            Tracks = await PlaylistRepository.GetPlaylistTracksByArtistId(ArtistId, CurrentUserId);
            Playlists = await PlaylistRepository.GetPlaylistByUserId(CurrentUserId);
            AppState.SetUserPlaylist(Playlists);
            SelectedPlaylist = -1;
        }

        private async Task<string> GetUserId()
        {
            var user = (await AuthenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        public void FavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistRepository.AddTrackToFavorite(trackId, CurrentUserId).ConfigureAwait(false);
            OnInitializedAsync().ConfigureAwait(false);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} added to playlist Favorites.";
        }

        public void UnfavoriteTrack(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistRepository.RemoveTrackFromFavorite(trackId, CurrentUserId).ConfigureAwait(false);
            OnInitializedAsync().ConfigureAwait(false);
            InfoMessage = $"Track {track.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
        }

        public void OpenPlaylistDialog(long trackId)
        {
            CloseInfoMessage();
            SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistDialog.Open();
        }

        public void AddTrackToPlaylist()
        {
            try
            {
                var addTrackToPlaylist = new AddTrackToPlaylist
                {
                    TrackId = SelectedTrack.TrackId,
                    UserId = CurrentUserId,
                    PlaylistId = SelectedPlaylist != -1 ? SelectedPlaylist : null,
                    Name = SelectedPlaylist == -1 ? NewPlaylistName : null,
                };

                PlaylistRepository.AddTrackToPlaylist(addTrackToPlaylist).ConfigureAwait(false);
                CloseInfoMessage();
                OnInitializedAsync().ConfigureAwait(false);

                NewPlaylistName = string.Empty;

                InfoMessage = $"Track {Artist.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {NewPlaylistName}.";
                PlaylistDialog.Close();
            }
            catch (Exception ex)
            {
                InfoMessage = ex.Message;
            }
        }

        public void CloseInfoMessage()
        {
            InfoMessage = "";
        }
    }
}
