using System.Collections.Concurrent;

namespace Todo.Api.Hubs;

public class UserConnectionManager : IUserConnectionManager
{
    private static readonly ConcurrentDictionary<Guid, HashSet<string>> UserConnections = new();

    public void AddConnection(Guid userId, string connectionId)
    {
        var connections = UserConnections.GetOrAdd(userId, _ => []);
        
        lock (connections)
        {
            connections.Add(connectionId);
        }
    }

    public void RemoveConnection(string connectionId)
    {
        foreach (var pair in UserConnections)
        {
            if (!pair.Value.Contains(connectionId)) continue;
            
            lock (pair.Value)
            {
                pair.Value.Remove(connectionId);
            }

            if (pair.Value.Count == 0)
            {
                UserConnections.TryRemove(pair.Key, out _);
            }
                
            break;
        }
    }

    public List<string> GetConnections(Guid userId)
    {
        if (UserConnections.TryGetValue(userId, out var connections))
        {
            return connections.ToList();
        }
        
        return [];
    }
}

public interface IUserConnectionManager
{
    void AddConnection(Guid userId, string connectionId);
    void RemoveConnection(string connectionId);
    List<string> GetConnections(Guid userId);
}