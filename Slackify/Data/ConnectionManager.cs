namespace Slackify.Data;

public class ConnectionManager
{
    private readonly Dictionary<string, HashSet<string>> _connections = new Dictionary<string, HashSet<string>>();

    public int Count => _connections.Count;

    public void Add( string key, string connectionId )
    {
        lock( _connections )
        {
            HashSet<string> connection;
            if( _connections.TryGetValue( key, out connection ) == false )
            {
                connection = new HashSet<string>();
                _connections.Add( key, connection );
            }

            lock( connection )
            {
                connection.Add( connectionId );
            }
        }
    }

    public Dictionary<string, HashSet<string>> GetUsers()
    {
        return _connections;
    }

    public string GetConnection( string key )
    {
        HashSet<string> connection = _connections[key];
        if( connection == null )
        {
            return string.Empty;
        }
        return connection.FirstOrDefault();
    }

    public IEnumerable<string> GetConnections( string key )
    {
        HashSet<string> connections;
        if( _connections.TryGetValue( key, out connections ) == false )
        {
            return Enumerable.Empty<string>();
        }
        return connections;
    }

    public void Remove( string key, string connectionId )
    {
        lock( _connections )
        {
            HashSet<string> connection;
            if( _connections.TryGetValue( key, out connection ) == false )
            {
                return;
            }

            lock( connection )
            {
                connection.Remove( connectionId );
                if( connection.Count == 0 )
                {
                    _connections.Remove( key );
                }
            }
        }
    }
}
