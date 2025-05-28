using System.Collections.Concurrent;

namespace Todo.Api.Hubs;

public interface IUserConnectionManager
{
    void AddConnection(Guid userId, string connectionId);
    void RemoveConnection(string connectionId);
    List<string> GetConnections(Guid userId);
}

public class UserConnectionManager : IUserConnectionManager
{
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> _userConnections = new();

    public void AddConnection(Guid userId, string connectionId)
    {
        var connections = _userConnections.GetOrAdd(userId, _ => new HashSet<string>());
        lock (connections) connections.Add(connectionId);
    }

    public void RemoveConnection(string connectionId)
    {
        foreach (var pair in _userConnections)
        {
            if (pair.Value.Contains(connectionId))
            {
                lock (pair.Value) pair.Value.Remove(connectionId);
                if (pair.Value.Count == 0)
                    _userConnections.TryRemove(pair.Key, out _);
                break;
            }
        }
    }

    

    public List<string> GetConnections(Guid userId)
    {
        if (_userConnections.TryGetValue(userId, out var connections))
        {
            return connections.ToList();
        }
        return new List<string>();
    }
}