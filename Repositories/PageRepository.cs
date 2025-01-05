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
        public List<Pages> GetAllPages()
        {
            var pageList = new List<Pages>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT * 
                    FROM Pages", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var page = new Pages
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Slug = reader.GetString("Slug"),
                            Content = reader.GetString("Content"),
                            MetaTitle = reader.IsDBNull(reader.GetOrdinal("MetaTitle")) ? null : reader.GetString("MetaTitle"),
                            MetaDescription = reader.IsDBNull(reader.GetOrdinal("MetaDescription")) ? null : reader.GetString("MetaDescription"),
                            LanguageCode = reader.GetString("LanguageCode"),
                            Status = reader.GetString("Status"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            CreatedBy = reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            VisitorCount = reader.GetInt32("VisitorCount"),
                        };

                        pageList.Add(page);
                    }
                }
            }

            return pageList;
        }

        // Method to retrieve a page by Slug
        public Pages GetPageBySlug(string slug)
        {
            Pages page = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
            SELECT * 
            FROM Pages 
            WHERE Slug = @Slug", connection);

                command.Parameters.AddWithValue("@Slug", slug);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        page = new Pages
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Slug = reader.GetString("Slug"),
                            Content = reader.GetString("Content"),
                            MetaTitle = reader.IsDBNull(reader.GetOrdinal("MetaTitle")) ? null : reader.GetString("MetaTitle"),
                            MetaDescription = reader.IsDBNull(reader.GetOrdinal("MetaDescription")) ? null : reader.GetString("MetaDescription"),
                            LanguageCode = reader.GetString("LanguageCode"),
                            Status = reader.GetString("Status"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            CreatedBy = reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            VisitorCount = reader.GetInt32("VisitorCount"),
                        };
                    }
                }
            }

            return page;
        }


        // Method to retrieve a specific page by ID
        public Pages GetPageById(int id)
        {
            Pages page = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT * 
                    FROM Pages 
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        page = new Pages
                        {
                            Id = reader.GetInt32("Id"),
                            Title = reader.GetString("Title"),
                            Slug = reader.GetString("Slug"),
                            Content = reader.GetString("Content"),
                            MetaTitle = reader.IsDBNull(reader.GetOrdinal("MetaTitle")) ? null : reader.GetString("MetaTitle"),
                            MetaDescription = reader.IsDBNull(reader.GetOrdinal("MetaDescription")) ? null : reader.GetString("MetaDescription"),
                            LanguageCode = reader.GetString("LanguageCode"),
                            Status = reader.GetString("Status"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt"),
                            CreatedBy = reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            VisitorCount = reader.GetInt32("VisitorCount"),
                        };
                    }
                }
            }

            return page;
        }

        // Method to add a new page
        public int AddPage(PageDto pageDto, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    INSERT INTO Pages (Title, Slug, Content, MetaTitle, MetaDescription, LanguageCode, Status, CreatedAt, CreatedBy) 
                    VALUES (@Title, @Slug, @Content, @MetaTitle, @MetaDescription, @LanguageCode, @Status, @CreatedAt, @CreatedBy);
                    SELECT LAST_INSERT_ID();", connection);

                command.Parameters.AddWithValue("@Title", pageDto.Title);
                command.Parameters.AddWithValue("@Slug", pageDto.Slug);
                command.Parameters.AddWithValue("@Content", pageDto.Content);
                command.Parameters.AddWithValue("@MetaTitle", pageDto.MetaTitle ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MetaDescription", pageDto.MetaDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LanguageCode", pageDto.LanguageCode);
                command.Parameters.AddWithValue("@Status", pageDto.Status.ToString());
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);

                var pageId = Convert.ToInt32(command.ExecuteScalar());
                return pageId;
            }
        }

        // Method to update a page
        public bool UpdatePage(PageDto pageDto, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var existingPage = GetPageById(pageDto.Id);
                if (existingPage == null)
                {
                    return false;
                }

                var command = new MySqlCommand(@"
                    UPDATE Pages 
                    SET Title = @Title, Slug = @Slug, Content = @Content, 
                        MetaTitle = @MetaTitle, MetaDescription = @MetaDescription, 
                        LanguageCode = @LanguageCode, Status = @Status, 
                        UpdatedAt = @UpdatedAt, UpdatedBy = @UpdatedBy
                    WHERE Id = @Id", connection);

                command.Parameters.AddWithValue("@Title", pageDto.Title);
                command.Parameters.AddWithValue("@Slug", pageDto.Slug);
                command.Parameters.AddWithValue("@Content", pageDto.Content);
                command.Parameters.AddWithValue("@MetaTitle", pageDto.MetaTitle ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@MetaDescription", pageDto.MetaDescription ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@LanguageCode", pageDto.LanguageCode);
                command.Parameters.AddWithValue("@Status", pageDto.Status.ToString());
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@Id", pageDto.Id);

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }
        // Increment Visitor Count 
        public void IncrementVisitorCount(int pageId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"UPDATE Pages SET VisitorCount = VisitorCount + 1 WHERE Id = @PageId", connection);
                command.Parameters.AddWithValue("@PageId", pageId);
                command.ExecuteNonQuery();
            }
        }


        // Method to delete a page
        public void DeletePage(int pageId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand("DELETE FROM Pages WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", pageId);
                command.ExecuteNonQuery();
            }
        }
    }
}
