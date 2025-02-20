using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Zentech.Models;
using Swashbuckle.AspNetCore.Annotations;
using ZentechAPI.Services;
using ZentechAPI.Models;


[ApiController]
[Route("api/[controller]")]
public class ContactController : ControllerBase
{
    private readonly ContactService _contactService;
    private readonly EmailService _emailService;

    public ContactController(ContactService contactService, EmailService emailService)
    {
        _contactService = contactService;
        _emailService = emailService;
    }


   

    [HttpPost("send-custom-email")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> SendCustomEmail([FromBody] CustomEmail customEmail)
    {
        if (string.IsNullOrEmpty(customEmail.RecipientEmail) || string.IsNullOrEmpty(customEmail.Subject) || string.IsNullOrEmpty(customEmail.Body))
            return BadRequest("Tous les champs sont obligatoires.");

        try
        {
            await _emailService.SendEmailAsync(customEmail.RecipientEmail, customEmail.Subject, customEmail.Body);
            return Ok("Email envoyé avec succès.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Erreur d'envoi email : {ex.Message}");
            return StatusCode(500, "Erreur lors de l'envoi de l'email.");
        }
    }


    // Endpoint to retrieve all contact requests

    /// <summary>
    /// Retrieve all contact requests.
    /// </summary>
    /// <remarks>
    /// This endpoint returns the complete list of contact requests.
    /// Accessible only to administrators.
    /// </remarks>
    /// <returns>A list of all contact requests.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet]
    [SwaggerOperation(Summary = "Retrieve all contact requests",
                      Description = "Returns the complete list of contact requests.")]
    public async Task<IActionResult> GetAllContacts()
    {
        var contacts = await _contactService.GetAllContactsAsync();
        return Ok(contacts);
    }

    // Endpoint to retrieve a contact request by ID

    /// <summary>
    /// Retrieve a contact request by ID.
    /// </summary>
    /// <param name="id">The ID of the contact request.</param>
    /// <returns>The details of the contact request.</returns>
    [Authorize(Roles = "Admin")]
    [HttpGet("{id}")]
    [SwaggerOperation(Summary = "Retrieve a contact request by ID",
                      Description = "Returns the details of a specific contact request.")]
    public async Task<IActionResult> GetContactById(int id)
    {
        var contact = await _contactService.GetContactByIdAsync(id);
        if (contact == null)
            return NotFound();
        return Ok(contact);
    }

    // Endpoint to create a new contact request

    /// <summary>
    /// Create a new contact request.
    /// </summary>
    /// <param name="contactMessage">Object containing the details of the contact request.</param>
    /// <returns>The created contact request.</returns>
    [HttpPost]
    [SwaggerOperation(Summary = "Create a new contact request",
                      Description = "Creates a new contact request with the provided information.")]
    public async Task<IActionResult> CreateContact([FromBody] ContactMessage contactMessage)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        // Capture the PC information
        contactMessage.IPAddress = HttpContext.Connection.RemoteIpAddress?.ToString();
        contactMessage.UserAgent = Request.Headers["User-Agent"];
        contactMessage.MachineName = Environment.MachineName;  // Get the machine name from the environment
        var createdContact = await _contactService.CreateContactAsync(contactMessage);
        // Envoyer un email automatique
        string subject = "Merci de nous avoir contactés!";
        string body = $"Bonjour {contactMessage.FirstName},<br><br>Merci pour votre message. Notre équipe vous répondra dans les plus brefs délais.<br><br>Bien cordialement,<br>L'équipe Zentech.";
        try
        {
            await _emailService.SendEmailAsync(contactMessage.Email, subject, body);
        }
        catch (Exception ex)
        {
            // Gérer l'erreur sans interrompre le processus
            Console.WriteLine($"Erreur d'envoi email : {ex.Message}");
        }

        return CreatedAtAction(nameof(GetContactById), new { id = createdContact.ContactID }, createdContact);
    }

    // Endpoint to update an existing contact request

    /// <summary>
    /// Update an existing contact request.
    /// </summary>
    /// <param name="id">The ID of the contact request to be updated.</param>
    /// <param name="contactMessage">Object containing the updated information of the contact request.</param>
    /// <returns>The updated contact request.</returns>
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Update a contact request",
                      Description = "Updates an existing contact request with new information.")]
    public async Task<IActionResult> UpdateContact(int id, [FromBody] ContactMessage contactMessage)
    {
        if (id != contactMessage.ContactID)
            return BadRequest("Contact ID mismatch.");

        var updatedContact = await _contactService.UpdateContactAsync(contactMessage);
        if (updatedContact == null)
            return NotFound();

        return Ok(updatedContact);
    }

    // Endpoint to delete a contact request

    /// <summary>
    /// Delete a contact request.
    /// </summary>
    /// <param name="id">The ID of the contact request to be deleted.</param>
    /// <returns>Confirmation of deletion.</returns>
    [HttpDelete("{id}")]
    [Authorize(Roles = "Admin")]
    [SwaggerOperation(Summary = "Delete a contact request",
                      Description = "Deletes a specific contact request by its ID.")]
    public async Task<IActionResult> DeleteContact(int id)
    {
        var deleted = await _contactService.DeleteContactAsync(id);
        if (!deleted)
            return NotFound();

        return NoContent();
    }
}
