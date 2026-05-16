using System.Collections.Concurrent;

namespace WebApi.Services
{
    public class NotificationConnectionTracker
    {
        private readonly ConcurrentDictionary<string, ConcurrentDictionary<string, byte>> _connections = new();

        public void Add(string userId, string connectionId)
        {
            var userConnections = _connections.GetOrAdd(userId, _ => new ConcurrentDictionary<string, byte>());
            userConnections.TryAdd(connectionId, 0);
        }

        public void Remove(string userId, string connectionId)
        {
            if (!_connections.TryGetValue(userId, out var userConnections))
            {
                return;
            }

            userConnections.TryRemove(connectionId, out _);
            if (userConnections.IsEmpty)
            {
                _connections.TryRemove(userId, out _);
            }
        }

        public bool IsOnline(string userId)
        {
            return _connections.TryGetValue(userId, out var userConnections) && !userConnections.IsEmpty;
        }
    }
}
