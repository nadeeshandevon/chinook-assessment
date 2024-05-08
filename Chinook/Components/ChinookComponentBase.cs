using Chinook.ClientModels;
using Chinook.Exceptions;
using Chinook.Helpers;
using Chinook.Models;
using Chinook.Repositories;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Components
{
    public class ChinookComponentBase: ComponentBase
    {
        [Inject] IPlaylistRepository? PlaylistRepository { get; set; } = default!;
        [CascadingParameter] private Task<AuthenticationState>? AuthenticationState { get; set; } = default!;       
        public string InfoMessage = string.Empty;
        public string ErrorMessage = string.Empty;
        public string CurrentUserId = string.Empty;
        public PlaylistTrack? SelectedTrack;
        public long SelectedPlaylist = -1;
        public string NewPlaylistName = string.Empty;
        public Modal? PlaylistDialog { get; set; }
        public ClientModels.Artist? Artist;
        public List<PlaylistTrack> Tracks = new();

        public async Task<string> GetUserId()
        {
            var user = (await AuthenticationState!).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return CurrentUserId = userId!;
        }

        public void CloseInfoMessage()
        {
            InfoMessage = string.Empty;
        }

        public void CloseErrorMessage()
        {
            ErrorMessage = string.Empty;
        }

        public async Task FavoriteTrackByTrackId(long trackId)
        {
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            var addTrackToPlaylist = new AddTrackToPlaylist
            {
                TrackId = trackId,
                UserId = CurrentUserId,
                Name = CommonConstants.MyFavoriteTrackPlayListName,
            };

            _ = await PlaylistRepository!.AddTrackToPlaylist(addTrackToPlaylist);
            InfoMessage = $"Track {track!.ArtistName} - {track!.AlbumTitle} - {track!.TrackName} added to playlist Favorites.";
        }

        public async Task UnfavoriteTrackByTrackId(long trackId)
        {
            
            var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            var myFavoritePlaylistId = await PlaylistRepository!.GetMyFavoritePlaylistId(CurrentUserId);

            if (myFavoritePlaylistId == null)
            {
                throw new CustomValidationException($"My favorite playlist not found");
            }

            _ = await PlaylistRepository.RemoveTrackFromPlaylist(trackId, myFavoritePlaylistId!.Value, CurrentUserId);
            InfoMessage = $"Track {track!.ArtistName} - {track!.AlbumTitle} - {track!.TrackName} removed from playlist Favorites.";
        }

        public async Task AddSelectedTrackToPlaylist()
        {
            var addTrackToPlaylist = new AddTrackToPlaylist
            {
                TrackId = SelectedTrack!.TrackId,
                UserId = CurrentUserId,
                PlaylistId = SelectedPlaylist != -1 ? SelectedPlaylist : null,
                Name = SelectedPlaylist == -1 ? NewPlaylistName : null,
            };

            _ = await PlaylistRepository!.AddTrackToPlaylist(addTrackToPlaylist);
            CloseInfoMessage();
            await OnInitializedAsync();

            NewPlaylistName = string.Empty;

            InfoMessage = $"Track {Artist!.Name} - {SelectedTrack!.AlbumTitle} - {SelectedTrack!.TrackName} added to playlist {NewPlaylistName!}.";
            PlaylistDialog!.Close();
        }
    }
}
