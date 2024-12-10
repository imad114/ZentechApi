using MySql.Data.MySqlClient;
using System.Data;
using Zentech.Models;
namespace Zentech.Repositories
{
    public class RoleRepository
    {
        private readonly DatabaseContext _context;

        public RoleRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Récupérer tous les rôles
        public async Task<List<Role>> GetAllAsync()
        {
            var roles = new List<Role>();
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Roles";
                using (var command = new MySqlCommand(query, connection))
                using (var reader = await command.ExecuteReaderAsync())
                {
                    while (await reader.ReadAsync())
                    {
                        var role = new Role
                        {
                            RoleID = reader.GetInt32("RoleID"),
                            RoleName = reader.GetString("RoleName")
                        };
                        roles.Add(role);
                    }
                }
            }
            return roles;
        }



        // Ajouter un nouveau rôle
        public async Task<Role> AddRoleAsync(Role role)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "INSERT INTO Roles (RoleName) VALUES (@RoleName)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@RoleName", role.RoleName);
                    await command.ExecuteNonQueryAsync();
                    role.RoleID = (int)command.LastInsertedId;
                }
            }
            return role;
        }
    }
}
