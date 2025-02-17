using MySql.Data.MySqlClient;

public class DatabaseContext 
{
    private readonly string _connectionString;

    public DatabaseContext(string configuration)
    {
        //_connectionString = configuration.GetConnectionString("DefaultConnection");
        _connectionString = configuration;
    }

    public MySqlConnection GetConnection()
    {
        return new MySqlConnection(_connectionString);
    }
}
