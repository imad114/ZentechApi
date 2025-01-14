using Zentech.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Zentech.Repositories;
using System.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.RegularExpressions;


namespace Zentech.Services
{
    public class UserService
    {
        private readonly UserRepository _userRepository;
        private readonly IConfiguration _configuration;

        public UserService(UserRepository userRepository, IConfiguration configuration)
        {
            _userRepository = userRepository;
            _configuration = configuration;
        }

        // Register a new user with password encryption
        public async Task<User> RegisterUserAsync(User user, string createdBy)
        {
            // Check if the email is already in use
            var existingUser = await _userRepository.GetByEmailAsync(user.Email);
            if (existingUser != null)
            {
                throw new System.Exception("Email already in use.");
            }
            // Check the email format
            if (!IsValidEmail(user.Email))
            {
                throw new Exception("Invalid email format.");
            }

            // Check the password format
            if (!IsValidPassword(user.Password))
            {
                throw new Exception("Password must be at least 8 characters long and contain a mix of letters, numbers, and special characters.");
            }


            // Encrypt the password
            user.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);

            // Add the user
            return await _userRepository.AddAsync(user, createdBy);
        }

        // Method to validate email
        private bool IsValidEmail(string email)
        {
            var emailPattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            return Regex.IsMatch(email, emailPattern);
        }

        // Method to validate password
        private bool IsValidPassword(string password)
        {
            // Example: minimum 8 characters, at least one letter, one number, and one special character
            var passwordPattern = @"^(?=.*[A-Za-z])(?=.*\d)(?=.*[!$%^&*?])[A-Za-z\d!$%^&*?]{8,}$";
            return Regex.IsMatch(password, passwordPattern);
        }



        // Authenticate a user (check email and password)
        public async Task<string> AuthenticateUserAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);


            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                return null;
            }

            // Generate the JWT token
            return GenerateJwtToken(user);
        }


        // Method to generate the JWT token
        private string GenerateJwtToken(User user)
        {
            var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.UserID.ToString()),
            new Claim(ClaimTypes.Name, user.FullName),
            new Claim(ClaimTypes.Email, user.Email),
            new Claim(ClaimTypes.Role, user.Role.RoleName)  // add roles
        };
            // Retrieve the secret key from appsettings.json
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddMinutes(Convert.ToDouble(_configuration["Jwt:ExpirationInMinutes"])),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }



        // Retrieve a user by ID
        public async Task<User?> GetUserByIdAsync(int userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        // Retrieve all users
        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        // Update a user
        public async Task<User?> UpdateUserAsync(User user, string updatedBy)
        {
            // Retrieve the current user
            var existingUser = await _userRepository.GetByIdAsync(user.UserID);
            if (existingUser == null)
            {
                throw new System.Exception("User not found.");
            }

            // Update only the necessary fields
            existingUser.FullName = user.FullName;
            existingUser.Email = user.Email;

            // If a password is provided, encrypt it before updating
            if (!string.IsNullOrWhiteSpace(user.Password))
            {
                existingUser.Password = BCrypt.Net.BCrypt.HashPassword(user.Password);
            }

            existingUser.RoleID = user.RoleID;

            // Update the user in the database
            return await _userRepository.UpdateAsync(existingUser, updatedBy);
        }

        // Delete a user by ID
        public async Task<bool> DeleteUserAsync(int userId)
        {
            return await _userRepository.DeleteAsync(userId);
        }

        // Check if a user belongs to a given role
        public async Task<bool> IsUserInRoleAsync(int userId, string roleName)
        {
            return await _userRepository.IsUserInRoleAsync(userId, roleName);
        }

        // Retrieve all available roles
        public async Task<List<Role>> GetAllRolesAsync()
        {
            return await _userRepository.GetAllRolesAsync();
        }
    }
}
