using MySql.Data.MySqlClient;
using Zentech.Models;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
namespace Zentech.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Ajouter un nouvel utilisateur
        public async Task<User> AddAsync(User user, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "INSERT INTO Users (FullName, Email, Password, RoleID, CreatedBy) VALUES (@FullName, @Email, @Password, @RoleID, @CreatedBy)";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    command.Parameters.AddWithValue("@CreatedBy", createdBy);

                    await command.ExecuteNonQueryAsync();
                    user.UserID = (int)command.LastInsertedId;
                }
            }
            return user;
        }

        // Récupérer un utilisateur par ID
        public async Task<User?> GetByIdAsync(int userId)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT u.*, r.RoleName FROM Users u JOIN Roles r ON u.RoleID = r.RoleID WHERE u.UserID = @UserID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32("UserID"),
                                FullName = reader.GetString("FullName"),
                                Email = reader.GetString("Email"),
                                Password = reader.GetString("Password"),
                                RoleID = reader.GetInt32("RoleID"),
                                Role = new Role
                                {
                                    RoleID = reader.GetInt32("RoleID"),
                                    RoleName = reader.GetString("RoleName")
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Récupérer un utilisateur par email
        public async Task<User?> GetByEmailAsync(string email)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT u.*, r.RoleName FROM Users u JOIN Roles r ON u.RoleID = r.RoleID WHERE u.Email = @Email";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@Email", email);
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        if (await reader.ReadAsync())
                        {
                            return new User
                            {
                                UserID = reader.GetInt32("UserID"),
                                FullName = reader.GetString("FullName"),
                                Email = reader.GetString("Email"),
                                Password = reader.GetString("Password"),
                                RoleID = reader.GetInt32("RoleID"),
                                Role = new Role
                                {
                                    RoleID = reader.GetInt32("RoleID"),
                                    RoleName = reader.GetString("RoleName")
                                }
                            };
                        }
                    }
                }
            }
            return null;
        }

        // Récupérer tous les utilisateurs
        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT u.*, r.RoleName FROM Users u JOIN Roles r ON u.RoleID = r.RoleID";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            users.Add(new User
                            {
                                UserID = reader.GetInt32("UserID"),
                                FullName = reader.GetString("FullName"),
                                Email = reader.GetString("Email"),
                                Password = reader.GetString("Password"),
                                RoleID = reader.GetInt32("RoleID"),
                                Role = new Role
                                {
                                    RoleID = reader.GetInt32("RoleID"),
                                    RoleName = reader.GetString("RoleName")
                                }
                            });
                        }
                    }
                }
            }
            return users;
        }

        // Mettre à jour un utilisateur
        public async Task<User?> UpdateAsync(User user, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "UPDATE Users SET FullName = @FullName, Email = @Email, Password = @Password, RoleID = @RoleID, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy WHERE UserID = @UserID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@Password", user.Password);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@UpdatedBy", updatedBy);



                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? user : null;
                }
            }
        }

        // Supprimer un utilisateur
        public async Task<bool> DeleteAsync(int userId)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "DELETE FROM Users WHERE UserID = @UserID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        // Vérifier si un utilisateur a un rôle donné
        public async Task<bool> IsUserInRoleAsync(int userId, string roleName)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT COUNT(*) FROM Users u JOIN Roles r ON u.RoleID = r.RoleID WHERE u.UserID = @UserID AND r.RoleName = @RoleName";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@RoleName", roleName);

                    var count = (long)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        // Récupérer tous les rôles disponibles
        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = new List<Role>();
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM Roles";
                using (var command = new MySqlCommand(query, connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            roles.Add(new Role
                            {
                                RoleID = reader.GetInt32("RoleID"),
                                RoleName = reader.GetString("RoleName")
                            });
                        }
                    }
                }
            }
            return roles;
        }
    }
}
