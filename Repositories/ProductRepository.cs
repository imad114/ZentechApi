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

        // Method to retrieve all products with their category and specifications
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
                            CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? null : reader.GetString("CategoryName"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? 0 : reader.GetInt32("CategoryID"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            Photos = GetPhotosForEntity(reader.GetInt32("ProductID"), "Products"),
                            Specifications = GetSpecificationsForProduct(reader.GetInt32("ProductID")) // Récupérer les spécifications
                        };

                        productList.Add(product);
                    }
                }
            }

            return productList;
        }

        public List<Product> GetProductsWithCategories(int limit)
        {
            var productList = new List<Product>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@$"
                WITH RankedProducts AS (
                    SELECT 
                        p.ProductID,             
                        p.Name as ProductName,
                        p.Price,
                        p.CreatedDate,
                        p.CreatedBy,
                        p.UpdatedBy,
                        p.UpdatedAt,
                        c.CategoryID AS SubCategoryID,
                        c.Name AS CategoryName,
                        c.Description AS CategoryDescription,
                        mc.CategoryID AS MainCategoryID,
                        mc.Name AS MainCategoryName,
                        ROW_NUMBER() OVER (PARTITION BY c.CategoryID ORDER BY p.CreatedDate DESC) AS RowNum
                    FROM Products p
                    LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                    LEFT JOIN main_prod_categories mc ON mc.CategoryID = c.mainCategoryID
                )
                SELECT *
                FROM RankedProducts
                WHERE RowNum <= @Limit
                ORDER BY RankedProducts.ProductName;", connection);

                command.Parameters.AddWithValue("@Limit", limit);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new Product
                        {
                            ProductID = reader.GetInt32("ProductID"),
                            Name = reader.GetString("ProductName"),
                            Description = reader.IsDBNull(reader.GetOrdinal("CategoryDescription")) ? "null" : reader.GetString("CategoryDescription"),
                            Price = reader.IsDBNull(reader.GetOrdinal("Price"))?0: reader.GetDecimal("Price"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?) null : reader.GetDateTime("CreatedDate"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CategoryID = reader.IsDBNull(reader.GetOrdinal("SubCategoryID")) ? 0 : reader.GetInt32("SubCategoryID"),
                            CategoryName = reader.IsDBNull(reader.GetOrdinal("CategoryName")) ? "null": reader.GetString("CategoryName"),
                            MainCategoryID = reader.IsDBNull(reader.GetOrdinal("MainCategoryID")) ? 0 : reader.GetInt32("MainCategoryID"),
                            MainCategoryName = reader.IsDBNull(reader.GetOrdinal("MainCategoryName")) ? "" : reader.GetString("MainCategoryName"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            Photos = GetPhotosForEntity(reader.GetInt32("ProductID"), "Products"),
                            Specifications = GetSpecificationsForProduct(reader.GetInt32("ProductID"))
                        };

                        productList.Add(product);
                    }
                }

            }

            return productList;
        }
        // Method to retrieve a specific product with its category and specifications
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
                            Price = reader.IsDBNull(reader.GetOrdinal("Price"))?0: reader.GetDecimal("Price"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CategoryID = reader.GetInt32("CategoryID"),
                            Photos = GetPhotosForEntity(reader.GetInt32("ProductID"), "Products"),
                            Specifications = GetSpecificationsForProduct(reader.GetInt32("ProductID")) // Récupérer les spécifications
                        };
                    }
                }
            }

            return product;
        }


        // Method to add a new product s
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

                // Ajout des spécifications
                AddSpecificationsForProduct(productId, product.Specifications);

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

        public bool UpdateProduct(ProductDto productDto, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                
                var existingProduct = GetProductById(productDto.ProductID);
                if (existingProduct == null)
                {
                    return false; 
                }

                var existingSpecifications = GetSpecificationsForProduct(productDto.ProductID);

                
                var command = new MySqlCommand(@"
            UPDATE Products 
            SET Name = @Name, Description = @Description, Price = @Price, 
                UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt, CategoryID = @CategoryID 
            WHERE ProductID = @ProductID", connection);

                command.Parameters.AddWithValue("@Name", productDto.Name);
                command.Parameters.AddWithValue("@Description", productDto.Description);
                command.Parameters.AddWithValue("@Price", productDto.Price);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@ProductID", productDto.ProductID);
                command.Parameters.AddWithValue("@CategoryID", productDto.CategoryID);

                var rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return false; 
                }

               
                var specificationsToDelete = existingSpecifications
                    .Where(existingSpec => !productDto.Specifications.Any(newSpec => newSpec.Key == existingSpec.Key))
                    .ToList();

                var specificationsToAdd = productDto.Specifications
                    .Where(newSpec => !existingSpecifications.Any(existingSpec => existingSpec.Key == newSpec.Key))
                    .ToList();

                var specificationsToUpdate = productDto.Specifications
                    .Where(newSpec => existingSpecifications.Any(existingSpec => existingSpec.Key == newSpec.Key))
                    .ToList();

                
                foreach (var specToDelete in specificationsToDelete)
                {
                    var deleteCommand = new MySqlCommand("DELETE FROM Specifications WHERE SpecificationID = @SpecificationID", connection);
                    deleteCommand.Parameters.AddWithValue("@SpecificationID", specToDelete.SpecificationID);
                    deleteCommand.ExecuteNonQuery();
                }

               
                foreach (var specToAdd in specificationsToAdd)
                {
                    var insertCommand = new MySqlCommand("INSERT INTO Specifications (`ProductId`, `Key`, `Value`) VALUES (@ProductID, @Key, @Value)", connection);
                    insertCommand.Parameters.AddWithValue("@ProductID", productDto.ProductID);
                    insertCommand.Parameters.AddWithValue("@Key", specToAdd.Key);
                    insertCommand.Parameters.AddWithValue("@Value", specToAdd.Value);
                    insertCommand.ExecuteNonQuery();
                }

              
                foreach (var specToUpdate in specificationsToUpdate)
                {
                    var updateCommand = new MySqlCommand("UPDATE Specifications SET `Value` = @Value WHERE ProductID = @ProductID AND `Key` = @Key", connection);
                    updateCommand.Parameters.AddWithValue("@ProductID", productDto.ProductID);
                    updateCommand.Parameters.AddWithValue("@Key", specToUpdate.Key);
                    updateCommand.Parameters.AddWithValue("@Value", specToUpdate.Value);
                    updateCommand.ExecuteNonQuery();
                }

                return true;
            }
        }

        // Method to retrieve specifications for a product
        public List<Specification> GetSpecificationsForProduct(int productId)
        {
            var specifications = new List<Specification>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT SpecificationID, `Key`, `Value` FROM Specifications WHERE ProductID = @ProductID", connection);
                command.Parameters.AddWithValue("@ProductID", productId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var specification = new Specification
                        {
                            SpecificationID = reader.GetInt32("SpecificationID"),
                            Key = reader.GetString("Key"),
                            Value = reader.GetString("Value")
                        };
                        specifications.Add(specification);
                    }
                }
            }

            return specifications;
        }

        // Method to add specifications for a product
        public void AddSpecificationsForProduct(int productId, List<SpecificationDto> specifications)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                foreach (var spec in specifications)
                {
                    var command = new MySqlCommand("INSERT INTO Specifications (`ProductId`, `Key`, `Value`) VALUES (@ProductID, @Key, @Value)", connection);
                    command.Parameters.AddWithValue("@ProductID", productId);
                    command.Parameters.AddWithValue("@Key", spec.Key);
                    command.Parameters.AddWithValue("@Value", spec.Value);
                    command.ExecuteNonQuery();
                }
            }
        }

        // Method to update specifications for a product
        public void UpdateSpecificationsForProduct(int productId, List<SpecificationDto> specifications)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                
                var deleteSpecsCommand = new MySqlCommand("DELETE FROM Specifications WHERE ProductID = @ProductID", connection);
                deleteSpecsCommand.Parameters.AddWithValue("@ProductID", productId);
                deleteSpecsCommand.ExecuteNonQuery();

                
                AddSpecificationsForProduct(productId, specifications);
            }
        }
        // Method to delete a product and its associated photos and specifications
        public void DeleteProduct(int productId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

               
                var deleteSpecsCommand = new MySqlCommand("DELETE FROM Specifications WHERE ProductID = @ProductID", connection);
                deleteSpecsCommand.Parameters.AddWithValue("@ProductID", productId);
                deleteSpecsCommand.ExecuteNonQuery();

               
                var deletePhotosCommand = new MySqlCommand("DELETE FROM Photos WHERE EntityID = @EntityID AND EntityType = 'Products'", connection);
                deletePhotosCommand.Parameters.AddWithValue("@EntityID", productId);
                deletePhotosCommand.ExecuteNonQuery();

               
                var deleteProductCommand = new MySqlCommand("DELETE FROM Products WHERE ProductID = @ProductID", connection);
                deleteProductCommand.Parameters.AddWithValue("@ProductID", productId);
                deleteProductCommand.ExecuteNonQuery();
            }
        }

    }
}
