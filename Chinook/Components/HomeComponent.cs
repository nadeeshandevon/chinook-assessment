using Chinook.Models;
using Chinook.Repositories;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Security.Claims;

namespace Chinook.Components
{
    public class HomeComponent : ComponentBase
    {
        public List<Artist> Artists = new List<Artist>();
        public string SearchText = string.Empty;
        public long ArtistCount = 0;

        [Inject] IArtistRepository ArtistRepository { get; set; }
        [Inject] IPlaylistRepository? PlaylistRepository { get; set; }
        [Inject] AppState? AppState { get; set; }
        [CascadingParameter] private Task<AuthenticationState> AuthenticationState { get; set; }
        protected override async Task OnInitializedAsync()
        {
            await InvokeAsync(StateHasChanged);
            Artists = await GetArtists();
            var userId = await GetUserId();
            var userPlaylist = await PlaylistRepository.GetPlaylistByUserId(userId);
            AppState.SetUserPlaylist(userPlaylist);
        }

        private async Task<string> GetUserId()
        {
            var user = (await AuthenticationState).User;
            var userId = user.FindFirst(u => u.Type.Contains(ClaimTypes.NameIdentifier))?.Value;
            return userId;
        }

        public async Task SearchArtist()
        {
            Artists = await GetArtists();
        }

        public async Task CrealArtist()
        {
            SearchText = string.Empty;
            Artists = await GetArtists();
        }

        public async Task<List<Artist>> GetArtists()
        {
            var artists = await ArtistRepository.GetArtists(SearchText);
            ArtistCount = artists.Count();
            return artists;
        }

        public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        {
            var albums = await ArtistRepository.GetAlbumsByArtistId(artistId);
            return albums;
        }
    }
}
