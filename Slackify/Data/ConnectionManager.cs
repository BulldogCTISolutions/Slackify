namespace Slackify.Data;

public class ConnectionManager
{
    private readonly Dictionary<string, HashSet<string>> _connections = new();

    public int Count => this._connections.Count;

    public void Add( string key, string connectionId )
    {
        lock( this._connections )
        {
            if( this._connections.TryGetValue( key, out HashSet<string> connection ) == false )
            {
                connection = new HashSet<string>();
                this._connections.Add( key, connection );
            }

            lock( connection )
            {
                _ = connection.Add( connectionId );
            }
        }
    }

    public IEnumerable<string> GetUsers()
    {
        return this._connections.Keys.ToList();
    }

    public string GetConnection( string key )
    {
        HashSet<string> connection = this._connections[key];
        return connection == null ? string.Empty : connection.FirstOrDefault();
    }

    public IEnumerable<string> GetConnections( string key )
    {
        return this._connections.TryGetValue( key, out HashSet<string> connections ) == false ? Enumerable.Empty<string>() : connections;
    }

    public void Remove( string key, string connectionId )
    {
        lock( this._connections )
        {
            if( this._connections.TryGetValue( key, out HashSet<string> connection ) == false )
            {
                return;
            }

            lock( connection )
            {
                _ = connection.Remove( connectionId );
                if( connection.Count == 0 )
                {
                    _ = this._connections.Remove( key );
                }
            }
        }
    }
}
