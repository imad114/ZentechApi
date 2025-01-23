using MySql.Data.MySqlClient;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using ZentechAPI.Models;

namespace Zentech.Repositories
{
    public class CategoryRepository
    {
        private readonly DatabaseContext _context;

        public CategoryRepository(DatabaseContext context)
        {
            _context = context;
        }
        #region Sub Category
        // Méthode pour récupérer toutes les catégories
        public ConcurrentBag<Category> GetAllCategories()
        {
            var categoryList = new ConcurrentBag<Category>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT c.*, mc.Name AS MainCategoryName 
                    FROM Categories c
                    LEFT JOIN main_prod_categories mc ON c.MainCategoryID = mc.CategoryID",
                    connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var category = new Category
                        {
                            CategoryID = reader.GetInt32("CategoryID"),
                            MainCategoryID = reader.GetInt32("MainCategoryID"),
                            MainCategoryName = reader.IsDBNull(reader.GetOrdinal("MainCategoryName")) ? null : reader.GetString("MainCategoryName"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy")
                        };
                        categoryList.Add(category);
                    }
                }
            }

            return categoryList;
        }

        // Méthode pour récupérer une catégorie par ID
        public Category GetCategoryById(int id)
        {
            Category category = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT c.*, mc.Name AS MainCategoryName 
                    FROM Categories c
                    LEFT JOIN main_prod_categories mc ON c.MainCategoryID = mc.CategoryID
                    WHERE c.CategoryID = @CategoryID",
                    connection);

                command.Parameters.AddWithValue("@CategoryID", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        category = new Category
                        {
                            CategoryID = reader.GetInt32("CategoryID"),
                            MainCategoryID = reader.GetInt32("MainCategoryID"),
                            MainCategoryName = reader.IsDBNull(reader.GetOrdinal("MainCategoryName")) ? null : reader.GetString("MainCategoryName"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy")
                        };
                    }
                }
            }

            return category;
        }

        // Méthode pour ajouter une nouvelle catégorie
        public Category AddCategory(Category category, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    INSERT INTO Categories (MainCategoryID, Name, Description, CreatedDate, CreatedBy) 
                    VALUES (@MainCategoryID, @Name, @Description, @CreatedDate, @CreatedBy); 
                    SELECT LAST_INSERT_ID();",
                    connection);

                command.Parameters.AddWithValue("@MainCategoryID", category.MainCategoryID);
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Description", category.Description);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);

                var categoryId = Convert.ToInt32(command.ExecuteScalar());
                category.CategoryID = categoryId;
            }

            return category;
        }

        // Méthode pour mettre à jour une catégorie existante
        public bool UpdateCategory(Category category, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    UPDATE Categories 
                    SET MainCategoryID = @MainCategoryID, Name = @Name, Description = @Description, 
                        UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate 
                    WHERE CategoryID = @CategoryID",
                    connection);

                command.Parameters.AddWithValue("@MainCategoryID", category.MainCategoryID);
                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Description", category.Description);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // Méthode pour supprimer une catégorie
        public void DeleteCategory(int categoryId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM Categories WHERE CategoryID = @CategoryID", connection);
                command.Parameters.AddWithValue("@CategoryID", categoryId);
                command.ExecuteNonQuery();
            }
        }
        #endregion

        #region Main Category
        // Récupérer toutes les Main catégories 
        public ConcurrentBag<MainProdCategory> GetAllMainProdCategories()
        {
            var categoryList = new ConcurrentBag<MainProdCategory>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM main_prod_categories", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var category = new MainProdCategory
                        {
                            CategoryID = reader.GetInt32("CategoryID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy")
                        };
                        categoryList.Add(category);
                    }
                }
            }

            return categoryList;
        }

        // Récupérer une catégorie par ID
        public MainProdCategory GetMainProdCategoryById(int id)
        {
            MainProdCategory category = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM main_prod_categories WHERE CategoryID = @CategoryID", connection);
                command.Parameters.AddWithValue("@CategoryID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        category = new MainProdCategory
                        {
                            CategoryID = reader.GetInt32("CategoryID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy")
                        };
                    }
                }
            }
            return category;
        }

        // Ajouter une nouvelle catégorie
        public MainProdCategory AddMainProdCategory(MainProdCategory category, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO main_prod_categories (Name, Description, CreatedDate, CreatedBy) VALUES (@Name, @Description, @CreatedDate, @CreatedBy); SELECT LAST_INSERT_ID();",
                    connection
                );

                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Description", category.Description);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);

                var categoryId = Convert.ToInt32(command.ExecuteScalar());
                category.CategoryID = categoryId;
            }

            return category;
        }

        // Mettre à jour une catégorie
        public bool UpdateMainProdCategory(MainProdCategory category, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(
                    "UPDATE main_prod_categories SET Name = @Name, Description = @Description, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate WHERE CategoryID = @CategoryID",
                    connection
                );

                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Description", category.Description);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@CategoryID", category.CategoryID);
                command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // Supprimer une catégorie
        public void DeleteMainProdCategory(int categoryId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand("DELETE FROM main_prod_categories WHERE CategoryID = @CategoryID", connection);
                command.Parameters.AddWithValue("@CategoryID", categoryId);
                command.ExecuteNonQuery();
            }
        }
        #endregion




    }
}
