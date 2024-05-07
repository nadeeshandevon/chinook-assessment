namespace Chinook
{
    public class AppState
    {
        private List<ClientModels.Playlist> _userPlaylist = new List<ClientModels.Playlist>();

        public event Action OnChange;

        private readonly object _lock = new object();

        public List<ClientModels.Playlist> UserPlaylist
        {
            get
            {
                lock (_lock)
                {
                    return _userPlaylist;
                }
            }
        }

        public void SetUserPlaylist(List<ClientModels.Playlist> userPlaylist)
        {
            lock (_lock)
            {
                _userPlaylist = userPlaylist;
                NotifyStateChanged();
            }
        }

        private void NotifyStateChanged () => OnChange?.Invoke();
    }
}
