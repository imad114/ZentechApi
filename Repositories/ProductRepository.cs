using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace Zentech.Repositories
{
    public class ProductRepository
    {
        private readonly DatabaseContext _context;
        public ProductRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Method to retrieve all products with their category

        public List<Product> GetAllProducts()
        {
            var productList = new List<Product>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT p.*, c.CategoryID, c.Name AS CategoryName, c.Description AS CategoryDescription
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryID = c.CategoryID", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            ProductID = reader.GetInt32("ProductID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            Price = reader.GetDecimal("Price"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? 0 : reader.GetInt32("CategoryID"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            Photos = GetPhotosForEntity(reader.GetInt32("ProductID"), "Products"),
                            //Category = new Category
                            //{
                            //    CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? 0 : reader.GetInt32("CategoryID"),
                            //    Name = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString("CategoryName"),
                            //    Description = reader.IsDBNull(reader.GetOrdinal("CategoryDescription")) ? null : reader.GetString("CategoryDescription")
                            //}
                        };
                        productList.Add(product);
                    }
                }
            }

            return productList;
        }

        // Method to retrieve a specific product with its category

        public Product GetProductById(int id)
        {
            Product product = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    SELECT p.*, c.CategoryID, c.Name AS CategoryName, c.Description AS CategoryDescription
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                    WHERE p.ProductID = @ProductID", connection);

                command.Parameters.AddWithValue("@ProductID", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        product = new Product
                        {
                            ProductID = reader.GetInt32("ProductID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            Price = reader.GetDecimal("Price"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CategoryID = reader.GetInt32("CategoryID"),
                            Photos = GetPhotosForEntity(reader.GetInt32("ProductID"), "Products"),
                            //Category = new Category
                            //{
                            //    CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? 0 : reader.GetInt32("CategoryID"),
                            //    Name = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString("CategoryName"),
                            //    Description = reader.IsDBNull(reader.GetOrdinal("CategoryDescription")) ? null : reader.GetString("CategoryDescription")
                            //}
                        };
                    }
                }
            }

            return product;
        }

        // Method to add a new product

        public int AddProduct(ProductDto product, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    INSERT INTO Products (Name, Description, Price, CreatedDate, CreatedBy, CategoryID) 
                    VALUES (@Name, @Description, @Price, @CreatedDate, @CreatedBy, @CategoryID); 
                    SELECT LAST_INSERT_ID();", connection);

                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
                command.Parameters.AddWithValue("@CategoryID", product.CategoryID);

                var productId = Convert.ToInt32(command.ExecuteScalar());
                return productId;
            }

           
        }

        // Method to add a photo to an entity (Product or News)
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

        // Method to get the photos associated with an entity (Product or News)
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

        // Method to delete a photo associated with an entity (Product or News)
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

        // Method to update an existing product

        public bool UpdateProduct(ProductDto product, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    UPDATE Products 
                    SET Name = @Name, Description = @Description, Price = @Price, 
                        UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt, CategoryID = @CategoryID 
                    WHERE ProductID = @ProductID", connection);

                command.Parameters.AddWithValue("@Name", product.Name);
                command.Parameters.AddWithValue("@Description", product.Description);
                command.Parameters.AddWithValue("@Price", product.Price);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@ProductID", product.ProductID);
                command.Parameters.AddWithValue("@CategoryID", product.CategoryID);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                    return false;


                

                return true;
            }
        }

        // Method to delete a product and its associated photos
        public void DeleteProduct(int productId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                // First, delete all the associated photos
                var deletePhotosCommand = new MySqlCommand("DELETE FROM Photos WHERE EntityID = @EntityID AND EntityType = 'Products'", connection);
                deletePhotosCommand.Parameters.AddWithValue("@EntityID", productId);
                deletePhotosCommand.ExecuteNonQuery();

                // Delete the product
                var deleteProductCommand = new MySqlCommand("DELETE FROM Products WHERE ProductID = @ProductID", connection);
                deleteProductCommand.Parameters.AddWithValue("@ProductID", productId);
                deleteProductCommand.ExecuteNonQuery();
            }
        }
    }
}
