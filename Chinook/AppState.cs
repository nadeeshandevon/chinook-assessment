namespace Chinook
{
    public class AppState
    {
        public List<ClientModels.Playlist> UserPlaylist { get; private set; } = new List<ClientModels.Playlist>();

        public event Action OnChange;

        public void SetUserPlaylist(List<ClientModels.Playlist> userPlaylist)
        {
            UserPlaylist = userPlaylist;
            NotifyStateChanged();
        }

        private void NotifyStateChanged() => OnChange?.Invoke();
    }
}
