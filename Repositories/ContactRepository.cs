using System.Data;
using MySql.Data.MySqlClient;
using Zentech.Models;

public class ContactRepository
{
    private readonly DatabaseContext _context;

    public ContactRepository(DatabaseContext context)
    {
        _context = context;
    }

    // Method to retrieve all contact requests
    public async Task<List<ContactMessage>> GetAllAsync()
    {
        var contacts = new List<ContactMessage>();
        using (var connection = _context.GetConnection())
        {
            await connection.OpenAsync();
            var query = "SELECT * FROM contactmessages";
            using (var command = new MySqlCommand(query, connection))
            using (var reader = await command.ExecuteReaderAsync())
            {
                while (await reader.ReadAsync())
                {
                    contacts.Add(new ContactMessage
                    {
                        ContactID = reader.GetInt32("ContactID"),
                        FullName = reader.GetString("FullName"),
                        Email = reader.GetString("Email"),
                        Message = reader.GetString("Message"),
                        CreatedAt = reader.GetDateTime("CreatedAt")
                    });
                }
            }
        }
        return contacts;
    }

    // Method to retrieve a contact request by ID
    public async Task<ContactMessage?> GetByIdAsync(int id)
    {
        using (var connection = _context.GetConnection())
        {
            await connection.OpenAsync();
            var query = "SELECT * FROM ContactMessages WHERE ContactID = @ContactID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ContactID", id);
                using (var reader = await command.ExecuteReaderAsync())
                {
                    if (await reader.ReadAsync())
                    {
                        return new ContactMessage
                        {
                            ContactID = reader.GetInt32("ContactID"),
                            FullName = reader.GetString("FullName"),
                            Email = reader.GetString("Email"),
                            Message = reader.GetString("Message"),
                            CreatedAt = reader.GetDateTime("CreatedAt")
                        };
                    }
                }
            }
        }
        return null;
    }

    // Method to add a new contact request
    public async Task<ContactMessage> AddAsync(ContactMessage contactMessage)
    {
        using (var connection = _context.GetConnection())
        {
            await connection.OpenAsync();
            var query = "INSERT INTO ContactMessages (FullName, Email, Message) VALUES (@FullName, @Email, @Message)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", contactMessage.FullName);
                command.Parameters.AddWithValue("@Email", contactMessage.Email);
                command.Parameters.AddWithValue("@Message", contactMessage.Message);
                await command.ExecuteNonQueryAsync();
                contactMessage.ContactID = (int)command.LastInsertedId;
            }
        }
        return contactMessage;
    }

    // Method to update an existing contact request
    public async Task<ContactMessage?> UpdateAsync(ContactMessage contactMessage)
    {
        using (var connection = _context.GetConnection())
        {
            await connection.OpenAsync();
            var query = "UPDATE ContactMessages SET FullName = @FullName, Email = @Email, Message = @Message WHERE ContactID = @ContactID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FullName", contactMessage.FullName);
                command.Parameters.AddWithValue("@Email", contactMessage.Email);
                command.Parameters.AddWithValue("@Message", contactMessage.Message);
                command.Parameters.AddWithValue("@ContactID", contactMessage.ContactID);
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0 ? contactMessage : null;
            }
        }
    }

    // Method to delete a contact request
    public async Task<bool> DeleteAsync(int id)
    {
        using (var connection = _context.GetConnection())
        {
            await connection.OpenAsync();
            var query = "DELETE FROM ContactMessages WHERE ContactID = @ContactID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@ContactID", id);
                var rowsAffected = await command.ExecuteNonQueryAsync();
                return rowsAffected > 0;
            }
        }
    }
}
