using System.Data;
using MySql.Data.MySqlClient;
using Zentech.Models;
using System.Text.RegularExpressions;

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
                        FirstName = reader.GetString("FirstName"),
                        LastName = reader.GetString("LastName"),
                        Email = reader.GetString("Email"),
                        PhoneNumbre = reader.GetString("PhoneNumbre"),
                        Message = reader.GetString("Message"),
                        CreatedAt = reader.GetDateTime("CreatedAt"),
                        Country = reader.GetString("Country"),
                        Topic = reader.IsDBNull(reader.GetOrdinal("Topic")) ? null : reader.GetString("Topic"),
                        Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString("Role")

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
                            FirstName = reader.GetString("FirstName"),
                            LastName = reader.GetString("LastName"),
                            Email = reader.GetString("Email"),
                            PhoneNumbre = reader.GetString("PhoneNumbre"),
                            Message = reader.GetString("Message"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            Country = reader.GetString("Country"),
                            Topic = reader.IsDBNull(reader.GetOrdinal("Topic")) ? null : reader.GetString("Topic"),
                            Role = reader.IsDBNull(reader.GetOrdinal("Role")) ? null : reader.GetString("Role")
                        };
                    }
                }
            }
        }
        return null;
    }

    public bool IsValidEmail(string email)
    {
        var emailPattern = @"^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+\.[a-zA-Z0-9-.]+$";
        return Regex.IsMatch(email, emailPattern);
    }

    public bool IsValidPhoneNumber(string phoneNumber)
    {
        var phonePattern = @"^\+?[0-9]{1,4}[-\s]?[0-9]{1,15}$";
        return Regex.IsMatch(phoneNumber, phonePattern);
    }

    // Method to add a new contact request
    public async Task<ContactMessage> AddAsync(ContactMessage contactMessage)
    {

        if (!IsValidEmail(contactMessage.Email))
        {
            throw new ArgumentException("Invalid email format.");
        }

        if (!IsValidPhoneNumber(contactMessage.PhoneNumbre))
        {
            throw new ArgumentException("Invalid phone number format.");
        }

        using (var connection = _context.GetConnection())
        {
            await connection.OpenAsync();
            var query = "INSERT INTO ContactMessages (FirstName,LastName,Email,PhoneNumbre,Message,Country,Topic, Role) VALUES (@FirstName,@LastName,@Email,@PhoneNumbre,@Message,@Country,@Topic,@Role)";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FirstName", contactMessage.FirstName);
                command.Parameters.AddWithValue("@LastName", contactMessage.LastName);
                command.Parameters.AddWithValue("@Email", contactMessage.Email);
                command.Parameters.AddWithValue("@PhoneNumbre", contactMessage.PhoneNumbre);
                command.Parameters.AddWithValue("@Message", contactMessage.Message);
                command.Parameters.AddWithValue("@Country", contactMessage.Country ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Topic", contactMessage.Topic ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@Role", contactMessage.Role ?? (object)DBNull.Value);
                await command.ExecuteNonQueryAsync();
                contactMessage.ContactID = (int)command.LastInsertedId;
            }
        }
        return contactMessage;
    }

    // Method to update an existing contact request
    public async Task<ContactMessage?> UpdateAsync(ContactMessage contactMessage)
    {
        if (!IsValidEmail(contactMessage.Email))
        {
            throw new ArgumentException("Invalid email format.");
        }

        if (!IsValidPhoneNumber(contactMessage.PhoneNumbre))
        {
            throw new ArgumentException("Invalid phone number format.");
        }


        using (var connection = _context.GetConnection())
        {
            await connection.OpenAsync();
            var query = "UPDATE ContactMessages SET FirstName = @FirstName ,LastName = @LastName , Email = @Email,PhoneNumbre = @PhoneNumbre, Message = @Message , Country = @Country WHERE ContactID = @ContactID";
            using (var command = new MySqlCommand(query, connection))
            {
                command.Parameters.AddWithValue("@FirstName", contactMessage.FirstName);
                command.Parameters.AddWithValue("@LastName", contactMessage.LastName);
                command.Parameters.AddWithValue("@Email", contactMessage.Email);
                command.Parameters.AddWithValue("@PhoneNumbre", contactMessage.PhoneNumbre);
                command.Parameters.AddWithValue("@Message", contactMessage.Message);
                command.Parameters.AddWithValue("@Country", contactMessage.Country);
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
