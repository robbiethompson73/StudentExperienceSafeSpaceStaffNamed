namespace MvcUi.Services
{
    public class UserTrackerService
    {

        private readonly HashSet<string> _activeUsers = new();
        private readonly object _lock = new();

        public void AddUser(string userId)
        {
            lock (_lock)
            {
                _activeUsers.Add(userId);
            }
        }

        public void RemoveUser(string userId)
        {
            lock (_lock)
            {
                _activeUsers.Remove(userId);
            }
        }

        public List<string> GetActiveUsers()
        {
            lock (_lock)
            {
                return _activeUsers.ToList();
            }
        }

        public int GetUserCount()
        {
            lock (_lock)
            {
                return _activeUsers.Count;
            }
        }

    }
}
