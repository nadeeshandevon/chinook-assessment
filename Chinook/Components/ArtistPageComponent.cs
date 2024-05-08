using Chinook.Repositories;
using Microsoft.AspNetCore.Components;

namespace Chinook.Components
{
    public class ArtistPageComponent : ChinookComponentBase
    {
        [Parameter] public long ArtistId { get; set; }
        [Inject] IPlaylistRepository? PlaylistRepository { get; set; } = default!;
        [Inject] IArtistRepository? ArtistRepository { get; set; } = default!;
        [Inject] AppState? AppState { get; set; } = default!;
        public List<ClientModels.Playlist> Playlists = new();
        

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
                await FavoriteTrackByTrackId(trackId);
                await OnInitializedAsync();
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
                await OnInitializedAsync();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task AddTrackToPlaylist()
        {
            try
            {
                await AddSelectedTrackToPlaylist();
                CloseInfoMessage();
                await OnInitializedAsync();
                PlaylistDialog!.Close();
            }
            catch (Exception ex)
            {
                NewPlaylistName = string.Empty;
                ErrorMessage = ex.Message;
            }
        }

        public void OpenPlaylistDialog(long trackId)
        {
            CloseInfoMessage();
            SelectedTrack = Tracks.FirstOrDefault(t => t.TrackId == trackId);
            PlaylistDialog!.Open();
        }
    }
}
