using MySql.Data.MySqlClient;
using Zentech.Models;
using System.Data;
using System.Collections.Generic;
using System.Threading.Tasks;
using ZentechAPI.Dto;
using System.Text;
namespace Zentech.Repositories
{
    public class UserRepository
    {
        private readonly DatabaseContext _context;

        public UserRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Add a new user

        public async Task<UserDto> AddAsync(UserDto user, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "INSERT INTO users (FullName, Email, Password, RoleID, CreatedBy) VALUES (@FullName, @Email, @Password, @RoleID, @CreatedBy)";
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

        // Retrieve a user by ID

        public async Task<User?> GetByIdAsync(int userId)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT u.*, r.RoleName FROM users u JOIN roles r ON u.RoleID = r.RoleID WHERE u.UserID = @UserID";
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
                                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
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

        // Retrieve a user by email

        public async Task<User?> GetByEmailAsync(string email)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT u.*, r.RoleName FROM users u JOIN roles r ON u.RoleID = r.RoleID WHERE u.Email = @Email";
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

        // Retrieve all users

        public async Task<List<User>> GetAllAsync()
        {
            var users = new List<User>();
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT u.*, r.RoleName FROM users u JOIN roles r ON u.RoleID = r.RoleID";
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
                                CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime("CreatedAt"),
                                UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                                UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
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

        // Update  user

        public async Task<User?> UpdateAsync(User user, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();

                // Construire la requête conditionnellement
                var query = new StringBuilder("UPDATE users SET FullName = @FullName, Email = @Email, RoleID = @RoleID, UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy");
                if (!string.IsNullOrWhiteSpace(user.Password))
                {
                    query.Append(", Password = @Password");
                }
                query.Append(" WHERE UserID = @UserID");

                using (var command = new MySqlCommand(query.ToString(), connection))
                {
                    // Ajouter les paramètres communs
                    command.Parameters.AddWithValue("@FullName", user.FullName);
                    command.Parameters.AddWithValue("@Email", user.Email);
                    command.Parameters.AddWithValue("@RoleID", user.RoleID);
                    command.Parameters.AddWithValue("@UserID", user.UserID);
                    command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                    command.Parameters.AddWithValue("@UpdatedBy", updatedBy);

                    // Ajouter le mot de passe uniquement s'il est fourni
                    if (!string.IsNullOrWhiteSpace(user.Password))
                    {
                        command.Parameters.AddWithValue("@Password", user.Password);
                    }

                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0 ? user : null;
                }
            }
        }


        // Delete User
        public async Task<bool> DeleteAsync(int userId)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "DELETE FROM users WHERE UserID = @UserID";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    var rowsAffected = await command.ExecuteNonQueryAsync();
                    return rowsAffected > 0;
                }
            }
        }

        // Check if a user has a given role

        public async Task<bool> IsUserInRoleAsync(int userId, string roleName)
        {
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT COUNT(*) FROM users u JOIN roles r ON u.RoleID = r.RoleID WHERE u.UserID = @UserID AND r.RoleName = @RoleName";
                using (var command = new MySqlCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@UserID", userId);
                    command.Parameters.AddWithValue("@RoleName", roleName);

                    var count = (long)await command.ExecuteScalarAsync();
                    return count > 0;
                }
            }
        }

        // Retrieve all available roles

        public async Task<List<Role>> GetAllRolesAsync()
        {
            var roles = new List<Role>();
            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();
                var query = "SELECT * FROM roles";
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
