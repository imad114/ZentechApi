using Microsoft.AspNetCore.Mvc.RazorPages;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace Zentech.Repositories
{
    public class PageRepository
    {
        private readonly DatabaseContext _context;

        public PageRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Method to retrieve all pages
        public List<AboutUs> GetAllPages()
        {
            var pageList = new List<AboutUs>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT * 
                    FROM aboutus", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                          var aboutAs = new AboutUs
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Content = reader.GetString("Content"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),

                        };

                        pageList.Add(aboutAs);
                    }
                }
            }

            return pageList;
        }

     

        // Method to retrieve a specific page by ID
        public AboutUs GetPageById(int id)
        {
            AboutUs aboutAs = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT * 
                    FROM aboutus 
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        aboutAs = new AboutUs
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Content = reader.GetString("Content"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            
                        };
                    }
                }
            }

            return aboutAs;
        }

        // Method to add a new page
        public int AddPage(AboutUs aboutAs, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    INSERT INTO aboutus (Title, Content,CreatedAt) 
                    VALUES (@Title,@Content, @CreatedAt);
                    SELECT LAST_INSERT_ID();", connection);

                command.Parameters.AddWithValue("@Title", aboutAs.Title);
                command.Parameters.AddWithValue("@Content", aboutAs.Content);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);

                var pageId = Convert.ToInt32(command.ExecuteScalar());
                aboutAs.Id = pageId;
                return pageId;
            }
        }

        // Method to update a page
        public bool UpdatePage(AboutUs aboutAs, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var existingPage = GetPageById(aboutAs.Id);
                if (existingPage == null)
                {
                    return false;
                }

                var command = new MySqlCommand(@"
                    UPDATE aboutus 
                    SET Title = @Title,Content = @Content, 
                        UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Title", aboutAs.Title);
                command.Parameters.AddWithValue("@Content", aboutAs.Content);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@Id", aboutAs.Id);

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        // Method to delete a page
        public void DeletePage(int pageId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand("DELETE FROM aboutus WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", pageId);
                command.ExecuteNonQuery();
            }
        }
    }
}
