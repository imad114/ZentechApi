using System;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ZentechAPI.Models;

namespace ZentechAPI.Repositories
{
    public class CompanyInformationRepository
    {
        private readonly DatabaseContext _context;

        public CompanyInformationRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Method to Retrieve All Companies
        public List<CompanyInformation> GetAll()
        {
            var companies = new List<CompanyInformation>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM company_information", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        companies.Add(MapReaderToCompany(reader));
                    }
                }
            }

            return companies;
        }

        // Method to Retrieve a Company by ID
        public CompanyInformation GetById(int id)
        {
            CompanyInformation company = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM company_information WHERE id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        company = MapReaderToCompany(reader);
                    }
                }
            }

            return company;
        }

        // Method to Insert a New Company
        public void Add(CompanyInformation company )
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    @"INSERT INTO company_information 
                    (company_name, company_logo_url, company_address, postal_code, city, country, 
                     contact_person_name, contact_person_position, phone_number, fax_number, email, 
                     business_license_code, facebook_url, twitter_url, linkedin_url, instagram_url, youtube_url)
                    VALUES 
                    (@CompanyName, @CompanyLogoUrl, @CompanyAddress, @PostalCode, @City, @Country, 
                     @ContactPersonName, @ContactPersonPosition, @PhoneNumber, @FaxNumber, @Email, 
                     @BusinessLicenseCode, @FacebookUrl, @TwitterUrl, @LinkedInUrl, @InstagramUrl, @YoutubeUrl)",
                    connection);

                AddParameters(command, company);

                command.ExecuteNonQuery();
            }
        }

        //Method to Update an Existing Company
        public void Update(CompanyInformation company)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    @"UPDATE company_information SET
                        company_name = @CompanyName,
                        company_logo_url = @CompanyLogoUrl,
                        company_address = @CompanyAddress,
                        postal_code = @PostalCode,
                        city = @City,
                        country = @Country,
                        contact_person_name = @ContactPersonName,
                        contact_person_position = @ContactPersonPosition,
                        phone_number = @PhoneNumber,
                        fax_number = @FaxNumber,
                        email = @Email,
                        business_license_code = @BusinessLicenseCode,
                        facebook_url = @FacebookUrl,
                        twitter_url = @TwitterUrl,
                        linkedin_url = @LinkedInUrl,
                        instagram_url = @InstagramUrl,
                        youtube_url = @YoutubeUrl,
                        updated_at = NOW()
                    WHERE id = @Id",
                    connection);

                AddParameters(command, company);
                command.Parameters.AddWithValue("@Id", company.Id);

                command.ExecuteNonQuery();
            }
        }

       
        private CompanyInformation MapReaderToCompany(MySqlDataReader reader)
        {
            return new CompanyInformation
            {
                Id = Convert.ToInt32(reader["id"]),
                CompanyName = reader["company_name"].ToString(),
                CompanyLogoUrl = reader["company_logo_url"]?.ToString(),
                CompanyAddress = reader["company_address"].ToString(),
                PostalCode = reader["postal_code"]?.ToString(),
                City = reader["city"]?.ToString(),
                Country = reader["country"]?.ToString(),
                ContactPersonName = reader["contact_person_name"]?.ToString(),
                ContactPersonPosition = reader["contact_person_position"]?.ToString(),
                PhoneNumber = reader["phone_number"]?.ToString(),
                FaxNumber = reader["fax_number"]?.ToString(),
                Email = reader["email"].ToString(),
                BusinessLicenseCode = reader["business_license_code"]?.ToString(),
                FacebookUrl = reader["facebook_url"]?.ToString(),
                TwitterUrl = reader["twitter_url"]?.ToString(),
                LinkedInUrl = reader["linkedin_url"]?.ToString(),
                InstagramUrl = reader["instagram_url"]?.ToString(),
                YoutubeUrl = reader["youtube_url"]?.ToString(),
                CreatedAt = Convert.ToDateTime(reader["created_at"]),
                UpdatedAt = Convert.ToDateTime(reader["updated_at"])
            };
        }

        
        private void AddParameters(MySqlCommand command, CompanyInformation company)
        {
            command.Parameters.AddWithValue("@CompanyName", company.CompanyName);
            command.Parameters.AddWithValue("@CompanyLogoUrl", company.CompanyLogoUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@CompanyAddress", company.CompanyAddress);
            command.Parameters.AddWithValue("@PostalCode", company.PostalCode ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@City", company.City ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Country", company.Country ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ContactPersonName", company.ContactPersonName ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@ContactPersonPosition", company.ContactPersonPosition ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@PhoneNumber", company.PhoneNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FaxNumber", company.FaxNumber ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@Email", company.Email);
            command.Parameters.AddWithValue("@BusinessLicenseCode", company.BusinessLicenseCode ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@FacebookUrl", company.FacebookUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@TwitterUrl", company.TwitterUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@LinkedInUrl", company.LinkedInUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@InstagramUrl", company.InstagramUrl ?? (object)DBNull.Value);
            command.Parameters.AddWithValue("@YoutubeUrl", company.YoutubeUrl ?? (object)DBNull.Value);
        }
    }
}
