using Chinook.Models;
using Chinook.Repositories;
using Microsoft.AspNetCore.Components;

namespace Chinook.Components
{
    public class HomeComponent : ChinookComponentBase
    {
        public List<Artist> Artists = new List<Artist>();
        public string SearchText = string.Empty;
        public long ArtistCount = 0;
        [Inject] IArtistRepository? ArtistRepository { get; set; }
        [Inject] IPlaylistRepository? PlaylistRepository { get; set; }
        [Inject] AppState? AppState { get; set; }

        protected override async Task OnInitializedAsync()
        {
            try
            {
                Artists = await GetArtists();
                var userId = await GetUserId();
                var userPlaylist = await PlaylistRepository!.GetPlaylistByUserId(userId);
                AppState!.SetUserPlaylist(userPlaylist);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task SearchArtist()
        {
            try
            {
                Artists = await GetArtists();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task ClearArtist()
        {
            try
            {
                SearchText = string.Empty;
                Artists = await GetArtists();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        public async Task<List<Artist>> GetArtists()
        {
            try
            {
                var artists = await ArtistRepository!.GetArtists(SearchText);
                ArtistCount = artists.Count();
                return artists;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return new List<Artist>();
            }
        }

        public async Task<List<Album>> GetAlbumsForArtist(int artistId)
        {
            try
            {
                var albums = await ArtistRepository!.GetAlbumsByArtistId(artistId);
                return albums;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return new List<Album>();
            }
        }
    }
}
