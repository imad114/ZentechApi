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
                var command = new MySqlCommand("SELECT * FROM News", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var news = new News
                        {
                            NewsID = reader.GetInt32("NewsID"),
                            Title = reader.GetString("Title"),
                            Content = reader.GetString("Content"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            Author = reader.GetString("Author"),
                            Photos = GetPhotosForEntity(reader.GetInt32("NewsID"), "News") // get photos
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
            News news = null;

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
                            Author = reader.GetString("Author"),
                            Photos = GetPhotosForEntity(reader.GetInt32("NewsID"), "News")
                        };
                    }
                }
            }
            return news;
        }

        // Method to add a new news item
        public NewsDto AddNews(NewsDto news)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO News (Title, Content, CreatedAt, Author) VALUES (@Title, @Content, @CreatedAt, @Author); SELECT LAST_INSERT_ID();",
                    connection
                );

                command.Parameters.AddWithValue("@Title", news.Title);
                command.Parameters.AddWithValue("@Content", news.Content);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@Author", news.Author);

                var newsId = Convert.ToInt32(command.ExecuteScalar());
               
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
            }
        }

        // Method to update a news article and its photos
        public bool UpdateNews(NewsDto news)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                //  update a news
                var command = new MySqlCommand(
                    "UPDATE News SET Title = @Title, Content = @Content, Author = @Author WHERE NewsID = @NewsID",
                    connection
                );
                command.Parameters.AddWithValue("@Title", news.Title);
                command.Parameters.AddWithValue("@Content", news.Content);
                command.Parameters.AddWithValue("@Author", news.Author);
                command.Parameters.AddWithValue("@NewsID", news.NewsID);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    return false;

               

               

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
            }
        }

        public List<News> GetNewsByCategoryId(int category_id)
        {
            List<News> news = new List<News>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT NewsID, title, content, author, categoryID  FROM News WHERE categoryID = @CategoryID", connection);
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
                            CategoryID = reader.GetString("categoryID")


                        });
                    }
                }
            }
            return news;
        }

        public List<Category> GetGategories()
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
                            ID = reader.GetString("ID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("description"),

                        });
                    }
                }
            }
            return categories;
        }
    }
}
