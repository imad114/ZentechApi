using Microsoft.AspNetCore.Mvc;
using Zentech.Models;
using System.Threading.Tasks;
using System.Collections.Generic;
using Zentech.Services;
using Microsoft.AspNetCore.Authorization;
using Swashbuckle.AspNetCore.Annotations;
using ZentechAPI.Dto;


[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly UserService _userService;

    public UserController(UserService userService)
    {
        _userService = userService;
    }


    // Endpoint: User Registration
    /// <summary>
    /// User registration.
    /// </summary>
    /// <remarks>
    /// This endpoint creates a new user by providing a full name, email, and password.
    /// The password is encrypted before being saved to the database.
    /// </remarks>
    /// <param name="user">The information of the user to register.</param>
    /// <returns>Returns the created user with an HTTP 201 status code.</returns>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] UserDto user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            var createdBy = User.Identity.Name;
            var newUser = await _userService.RegisterUserAsync(user, createdBy);
            return CreatedAtAction(nameof(GetUserById), new { id = newUser.UserID }, newUser);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }


    // Endpoint: User Authentication
    /// <summary>
    /// User authentication.
    /// </summary>
    /// <remarks>
    /// This endpoint allows a user to log in by providing their email and password.
    /// The password is compared with the one stored in the database.
    /// </remarks>
    /// <param name="request">The login information of the user.</param>
    /// <returns>Returns a JWT token if authentication is successful.</returns>
    [HttpPost("login")]
    [SwaggerOperation(Summary = "User authentication", Description = "Verifies user credentials and returns a JWT token.")]

    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var token = await _userService.AuthenticateUserAsync(request.Email, request.Password);
        if (token == null)
        {
            return Unauthorized(new { Message = "Invalid email or password." });
        }

        return Ok(new { Token = token }); // Retourne le token JWT
    }


    // Endpoint: Get User by ID
    /// <summary>
    /// Retrieve a user by their ID.
    /// </summary>
    /// <remarks>
    /// This endpoint returns the detailed information of a specific user.
    /// Accessible only by administrators.
    /// </remarks>
    /// <param name="id">The ID of the user.</param>
    /// <returns>The details of the user.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Get a user", Description = "Returns the details of a specific user by their ID.")]

    public async Task<IActionResult> GetUserById(int id)
    {
        var user = await _userService.GetUserByIdAsync(id);
        if (user == null)
        {
            return NotFound(new { Message = "User not found." });
        }

        return Ok(user);
    }

    // Endpoint: Get All Users
    /// <summary>
    /// Retrieve all users.
    /// </summary>
    /// <remarks>
    /// Returns the complete list of users registered in the system.
    /// Accessible only by administrators.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [SwaggerOperation(Summary = "List of users", Description = "Returns the complete list of users.")]
    public async Task<IActionResult> GetAllUsers()
    {
        var users = await _userService.GetAllUsersAsync();
        return Ok(users);
    }

    // Endpoint: Update User
    /// <summary>
    /// Update a user.
    /// </summary>
    /// <remarks>
    /// Allows updating the information of an existing user.
    /// Ensure that the ID matches the one provided in the request body.
    /// Accessible only by administrators.
    /// </remarks>
    /// <param name="id">The ID of the user to update.</param>
    /// <param name="user">The updated information of the user.</param>
    /// <returns>The updated user.</returns>
    [Authorize(Roles = "Admin")]
    [HttpPut("{id}")]
    [SwaggerOperation(Summary = "Update a user", Description = "Modifies the information of an existing user.")]

    public async Task<IActionResult> UpdateUser(int id, [FromBody] User user)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        if (id != user.UserID)
        {
            return BadRequest(new { Message = "User ID mismatch." });
        }

        try
        {
            var updatedBy = User.Identity.Name;
            var updatedUser = await _userService.UpdateUserAsync(user, updatedBy);
            if (updatedUser == null)
            {
                return NotFound(new { Message = "User not found." });
            }

            return Ok(updatedUser);
        }
        catch (System.Exception ex)
        {
            return BadRequest(new { Message = ex.Message });
        }
    }

    // Endpoint: Delete User
    /// <summary>
    /// Delete a user.
    /// </summary>
    /// <remarks>
    /// Deletes an existing user by their ID.
    /// Accessible only by administrators.
    /// </remarks>
    /// <param name="id">The ID of the user to delete.</param>
    /// <returns>Confirmation of deletion.</returns>
    [Authorize(Roles = "Admin")]
    [HttpDelete("{id}")]
    [SwaggerOperation(Summary = "Delete a user", Description = "Deletes a user by their ID.")]
    public async Task<IActionResult> DeleteUser(int id)
    {
        var result = await _userService.DeleteUserAsync(id);
        if (!result)
        {
            return NotFound(new { Message = "User not found." });
        }

        return Ok(new { Message = "User deleted successfully." });
    }

    // Endpoint: Check if a user belongs to a role
    /// <summary>
    /// Check if a user belongs to a role.
    /// </summary>
    /// <remarks>
    /// Checks if a user has a specific role.
    /// Accessible only by administrators.
    /// </remarks>
    /// <param name="id">The ID of the user.</param>
    /// <param name="roleName">The name of the role to check.</param>
    /// <returns>A boolean indicating if the user belongs to the role.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}/role/{roleName}")]
    [SwaggerOperation(Summary = "Check user's role", Description = "Checks if a user has a specific role.")]

    public async Task<IActionResult> IsUserInRole(int id, string roleName)
    {
        var isInRole = await _userService.IsUserInRoleAsync(id, roleName);
        return Ok(new { IsInRole = isInRole });
    }

    // Endpoint: Get All Roles
    /// <summary>
    /// Retrieve all roles.
    /// </summary>
    /// <remarks>
    /// Returns the list of roles available in the system.
    /// Accessible only by administrators.
    /// </remarks>
    [Authorize(Roles = "Admin")]
    [HttpGet("roles")]
    [SwaggerOperation(Summary = "List of roles", Description = "Returns the list of roles defined in the system.")]
    public async Task<IActionResult> GetAllRoles()
    {
        var roles = await _userService.GetAllRolesAsync();
        return Ok(roles);
    }
}

// Classe auxiliaire pour les requêtes de connexion
public class LoginRequest
{
    public string Email { get; set; }
    public string Password { get; set; }
}
