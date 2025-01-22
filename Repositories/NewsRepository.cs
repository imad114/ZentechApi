using Microsoft.AspNetCore.Http.HttpResults;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace Zentech.Repositories
{
    public class NewsRepository
    {
        private readonly DatabaseContext _context;
        public NewsRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Method to get all news
        public List<News> GetAllNews()
        {
            var newsList = new List<News>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM news", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var news = new News
                        {
                            NewsID = reader.IsDBNull(reader.GetOrdinal("NewsID"))?0: reader.GetInt32("NewsID"),
                            Title = reader.IsDBNull(reader.GetOrdinal("Title")) ? "": reader.GetString("Title"),
                            Content = reader.IsDBNull(reader.GetOrdinal("Content")) ? "" : reader.GetString("Content"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            Author = reader.IsDBNull(reader.GetOrdinal("Author")) ? "" : reader.GetString("Author"),
                            mainPicture = reader.IsDBNull(reader.GetOrdinal("mainPicture")) ? "" : reader.GetString("mainPicture"),
                            Photos = GetPhotosForEntity(reader.IsDBNull(reader.GetOrdinal("NewsID")) ? 0 : reader.GetInt32("NewsID"), "News") // get photos
                        };
                        newsList.Add(news);
                    }
                }
            }
            return newsList;
        }

        // Method to get a specific news item
        public News GetNewsById(int id)
        {
            News news = new News();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM News WHERE NewsID = @NewsID", connection);
                command.Parameters.AddWithValue("@NewsID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        news = new News
                        {
                            NewsID = reader.GetInt32("NewsID"),
                            Title = reader.GetString("Title"),
                            Content = reader.GetString("Content"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            Author = reader.GetString("Author"),
                            mainPicture = reader.IsDBNull(reader.GetOrdinal("mainPicture")) ? "" : reader.GetString("mainPicture"),
                            Photos = GetPhotosForEntity(reader.GetInt32("NewsID"), "News")
                        };
                    }
                }
                connection.Close();
            }
            return news;
        }

        // Method to add a new news item
        public NewsDto AddNews(NewsDto news, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO News (Title, Content, CreatedAt, Author, CreatedBy, mainPicture) VALUES (@Title, @Content, @CreatedAt, @Author, @CreatedBy, @mainPicture); SELECT LAST_INSERT_ID();",
                    connection
                );
               
                command.Parameters.AddWithValue("@Title", news.Title);
                command.Parameters.AddWithValue("@Content", news.Content);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
                command.Parameters.AddWithValue("@Author", news.Author);
                command.Parameters.AddWithValue("@mainPicture", news.mainPicture);

                var newsId = Convert.ToInt32(command.ExecuteScalar());
                news.NewsID = newsId;
                connection.Close();
            }

            return news;
        }

        // Method to add a photo to an entity (News or Product)
        public void AddPhoto(int entityId, string entityType, string photoUrl)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("INSERT INTO Photos (EntityID, EntityType, Url) VALUES (@EntityID, @EntityType, @Url)", connection);
                command.Parameters.AddWithValue("@EntityID", entityId);
                command.Parameters.AddWithValue("@EntityType", entityType);
                command.Parameters.AddWithValue("@Url", photoUrl);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        // Method to get the photos associated with an entity (News or Product)
        public List<string> GetPhotosForEntity(int entityId, string entityType)
        {
            var photos = new List<string>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT Url FROM Photos WHERE EntityID = @EntityID AND EntityType = @EntityType", connection);
                command.Parameters.AddWithValue("@EntityID", entityId);
                command.Parameters.AddWithValue("@EntityType", entityType);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        photos.Add(reader.GetString("Url"));
                    }
                }
                connection.Close();
            }

            return photos;
        }

        // Method to delete a photo associated with an entity (News or Product)
        public void DeletePhoto(string photoUrl)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM Photos WHERE Url = @Url", connection);
                command.Parameters.AddWithValue("@Url", photoUrl);
                command.ExecuteNonQuery();
                connection.Close();
            }
        }

        // Method to update a news article and its photos
        public bool UpdateNews(NewsDto news, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                // update a news
                var command = new MySqlCommand(
                    "UPDATE News SET Title = @Title, Content = @Content, Author = @Author, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt, mainPicture = @mainPicture  WHERE NewsID = @NewsID",
                    connection
                );
                command.Parameters.AddWithValue("@Title", news.Title);
                command.Parameters.AddWithValue("@Content", news.Content);
                command.Parameters.AddWithValue("@Author", news.Author);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@NewsID", news.NewsID);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@mainPicture", news.mainPicture);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    return false;

               

               connection.Close(); 

                return true;

            }
        }

        // Method to delete a news article and its associated photos
        public void DeleteNews(int newsId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                // First, delete all photos associated with the news article
                var deletePhotosCommand = new MySqlCommand("DELETE FROM Photos WHERE EntityID = @EntityID AND EntityType = 'News'", connection);
                deletePhotosCommand.Parameters.AddWithValue("@EntityID", newsId);
                deletePhotosCommand.ExecuteNonQuery();

                // Delete the news article
                var deleteNewsCommand = new MySqlCommand("DELETE FROM News WHERE NewsID = @NewsID", connection);
                deleteNewsCommand.Parameters.AddWithValue("@NewsID", newsId);
                deleteNewsCommand.ExecuteNonQuery();

                connection.Close();
            }
        }

        public List<News> GetNewsByCategoryId(int category_id)
        {
            List<News> news = new List<News>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT NewsID, title, content, author, categoryID,mainPicture  FROM News WHERE categoryID = @CategoryID", connection);
                command.Parameters.AddWithValue("@CategoryID", category_id);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        news.Add(new News()
                        {
                            Author = reader.GetString("author"),
                            Content = reader.GetString("content"),
                            NewsID = int.Parse(reader.GetString("NewsID")),
                            Title = reader.GetString("title"),
                            CategoryID = reader.GetString("categoryID"),
                            mainPicture = reader.GetString("mainPicture")


                        });
                    }
                }
                connection.Close();
            }
            return news;
        }

        public List<Category> GetNewsCategories()
        {
            List<Category> categories = new List<Category>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT ID, Name, description  FROM News_categories", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Category()
                        {
                            NewsID = reader.GetString("ID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("description"),

                        });
                    }
                }
                connection.Close();
            }
            return categories;
        }
    }
}
