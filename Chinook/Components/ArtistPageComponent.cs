using Chinook.ClientModels;
using Chinook.Exceptions;
using Chinook.Helpers;
using Chinook.Repositories;
using Chinook.Shared.Components;
using Microsoft.AspNetCore.Components;

namespace Chinook.Components
{
    public class ArtistPageComponent : ChinookComponentBase
    {
        [Parameter] public long ArtistId { get; set; }
        [Inject] IPlaylistRepository? PlaylistRepository { get; set; } = default!;
        [Inject] IArtistRepository? ArtistRepository { get; set; } = default!;
        [Inject] AppState? AppState { get; set; } = default!;

        public Modal? PlaylistDialog { get; set; }
        public ClientModels.Artist? Artist;
        public List<PlaylistTrack> Tracks = new();
        public List<ClientModels.Playlist> Playlists = new();
        public PlaylistTrack? SelectedTrack;
        public long SelectedPlaylist = -1;
        public string NewPlaylistName = string.Empty;

        protected override async Task OnInitializedAsync()
        {
            try
            {
                CurrentUserId = await GetUserId();

                Artist = await ArtistRepository!.GetArtistByArtistId(ArtistId);
                Tracks = await PlaylistRepository!.GetPlaylistTracksByArtistId(ArtistId, CurrentUserId);
                Playlists = await PlaylistRepository.GetPlaylistByUserId(CurrentUserId);
                AppState!.SetUserPlaylist(Playlists);
                SelectedPlaylist = -1;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task FavoriteTrack(long trackId)
        {
            try
            {
                var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
                var addTrackToPlaylist = new AddTrackToPlaylist
                {
                    TrackId = trackId,
                    UserId = CurrentUserId,
                    Name = CommonConstants.MyFavoriteTrackPlayListName,
                };

                _ = await PlaylistRepository!.AddTrackToPlaylist(addTrackToPlaylist);

                await OnInitializedAsync();
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
                var track = Tracks.FirstOrDefault(t => t.TrackId == trackId);
                var myFavoritePlaylistId = await PlaylistRepository!.GetMyFavoritePlaylistId(CurrentUserId);

                if (myFavoritePlaylistId == null)
                {
                    throw new CustomValidationException($"My favorite playlist not found");
                }

                _ = await PlaylistRepository.RemoveTrackFromPlaylist(trackId, myFavoritePlaylistId!.Value, CurrentUserId);
                await OnInitializedAsync();
                InfoMessage = $"Track {track!.ArtistName} - {track.AlbumTitle} - {track.TrackName} removed from playlist Favorites.";
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public void OpenPlaylistDialog(long trackId)
        {
            CloseInfoMessage();
            SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistDialog!.Open();
        }

        public async Task AddTrackToPlaylist()
        {
            try
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

                InfoMessage = $"Track {Artist!.Name} - {SelectedTrack.AlbumTitle} - {SelectedTrack.TrackName} added to playlist {NewPlaylistName}.";
                PlaylistDialog!.Close();
            }
            catch (Exception ex)
            {
                NewPlaylistName = string.Empty;
                ErrorMessage = ex.Message;
            }
        }
    }
}
