using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace Zentech.Repositories
{
    public class SolutionRepository
    {
        private readonly DatabaseContext _context;

        public SolutionRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Method to retrieve all solutions with their photos and associated products (including product photos and specifications)
        public List<Solution> GetAllSolutions()
        {
            var solutions = new List<Solution>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();

                // Retrieve all solutions
                var command = new MySqlCommand(@"SELECT * FROM Solutions", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var solution = new Solution
                        {
                            SolutionID = reader.GetInt32("SolutionID"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            Photos = GetPhotosForEntity(reader.GetInt32("SolutionID"), "Solutions"),
                            SolutionProducts = GetProductsForSolution(reader.GetInt32("SolutionID"))
                        };

                        solutions.Add(solution);
                    }
                }
            }

            return solutions;
        }



        // Method to retrieve a solution by its ID, including photos and associated products
        public Solution GetSolutionById(int solutionId)
        {
            Solution solution = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();

                // Retrieve the solution
                var command = new MySqlCommand(@"SELECT * FROM Solutions WHERE SolutionID = @SolutionID", connection);
                command.Parameters.AddWithValue("@SolutionID", solutionId);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        solution = new Solution
                        {
                            SolutionID = reader.GetInt32("SolutionID"),
                            Title = reader.GetString("Title"),
                            Description = reader.GetString("Description"),
                            CreatedAt = reader.IsDBNull(reader.GetOrdinal("CreatedAt")) ? (DateTime?)null : reader.GetDateTime("CreatedAt"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? (DateTime?)null : reader.GetDateTime("UpdatedAt"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            Photos = GetPhotosForEntity(reader.GetInt32("SolutionID"), "Solutions"),
                            SolutionProducts = GetProductsForSolution(reader.GetInt32("SolutionID"))
                        };
                    }
                }
            }

            return solution;
        }



        // Method to retrieve products associated with a solution (including photos and specifications)
        public List<SolutionProduct> GetProductsForSolution(int solutionId)
        {
            var solutionProducts = new List<SolutionProduct>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(@"
                    SELECT sp.*, p.*, c.Name AS CategoryName, c.Description AS CategoryDescription
                    FROM SolutionProduct sp
                    INNER JOIN Products p ON sp.ProductID = p.ProductID
                    LEFT JOIN Categories c ON p.CategoryID = c.CategoryID
                    WHERE sp.SolutionID = @SolutionID", connection);

                command.Parameters.AddWithValue("@SolutionID", solutionId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var product = new ProductDto
                        {
                            ProductID = reader.GetInt32("ProductID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("Description"),
                            Price = reader.GetDecimal("Price"),
                            CategoryID = reader.GetInt32("CategoryID"),
                            Photos = GetPhotosForEntity(reader.GetInt32("ProductID"), "Products"),
                            Specifications = GetSpecificationsForProduct(reader.GetInt32("ProductID"))
                        };

                        solutionProducts.Add(new SolutionProduct
                        {
                            SolutionProductID = reader.GetInt32("SolutionProductID"),
                            SolutionID = solutionId,
                            ProductID = product.ProductID,
                            Product = product
                        });
                    }
                }
            }

            return solutionProducts;
        }

        // Method to retrieve photos associated with an entity (Solution or Product)
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

        // Method to retrieve specifications for a product
        public List<SpecificationDto> GetSpecificationsForProduct(int productId)
        {
            var specificationsDto = new List<SpecificationDto>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT SpecificationID, `Key`, `Value` FROM Specifications WHERE ProductID = @ProductID", connection);
                command.Parameters.AddWithValue("@ProductID", productId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var specificationDto = new SpecificationDto
                        {
                            SpecificationID = reader.GetInt32("SpecificationID"),
                            Key = reader.GetString("Key"),
                            Value = reader.GetString("Value")
                        };

                        specificationsDto.Add(specificationDto);
                    }
                }
            }

            return specificationsDto;
        }


        // Method to add a new solution (including photos and associated products)

        public int AddSolution(SolutionDto solutionDto, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(@"
                    INSERT INTO Solutions (Title, Description, CreatedAt, CreatedBy) 
                    VALUES (@Title, @Description, @CreatedAt, @CreatedBy); 
                    SELECT LAST_INSERT_ID();", connection);

                command.Parameters.AddWithValue("@Title", solutionDto.Title);
                command.Parameters.AddWithValue("@Description", solutionDto.Description);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);

                var solutionId = Convert.ToInt32(command.ExecuteScalar());

                return solutionId;
            }
        }

        // Method to add a photo to Solution 
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
        
        public void AddProductToSolution(int solutionId, int productId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                // Check if product already exists for the solution
                var checkCommand = new MySqlCommand(@"SELECT COUNT(*) FROM SolutionProduct WHERE SolutionID = @SolutionID AND ProductID = @ProductID", connection);
                checkCommand.Parameters.AddWithValue("@SolutionID", solutionId);
                checkCommand.Parameters.AddWithValue("@ProductID", productId);

                var existingCount = Convert.ToInt32(checkCommand.ExecuteScalar());

                if (existingCount > 0)
                    throw new InvalidOperationException("This product is already associated with the solution.");

                // Add the product to the solution
                var insertCommand = new MySqlCommand(@"INSERT INTO SolutionProduct (SolutionID, ProductID) VALUES (@SolutionID, @ProductID)", connection);
                insertCommand.Parameters.AddWithValue("@SolutionID", solutionId);
                insertCommand.Parameters.AddWithValue("@ProductID", productId);

                insertCommand.ExecuteNonQuery();
            }
        }
        // Method to delete a photo associated with Solution 
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


        // Method to update an existing solution

        public void UpdateSolution(SolutionDto solutionDto, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(@"
            UPDATE Solutions 
            SET Title = @Title, 
                Description = @Description, 
                UpdatedAt = @UpdatedAt, 
                UpdatedBy = @UpdatedBy
            WHERE SolutionID = @SolutionID", connection);

                command.Parameters.AddWithValue("@Title", solutionDto.Title);
                command.Parameters.AddWithValue("@Description", solutionDto.Description);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@SolutionID", solutionDto.SolutionID);

                command.ExecuteNonQuery();
            }
        }


        // Method to delete a solution
        public void DeleteSolution(int solutionId)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                using (var transaction = connection.BeginTransaction())
                {
                    try
                    {
                        // Delete photos associated with the solution
                        var deletePhotosCommand = new MySqlCommand(@"DELETE FROM Photos WHERE EntityID = @EntityID AND EntityType = 'Solutions'", connection, transaction);
                        deletePhotosCommand.Parameters.AddWithValue("@EntityID", solutionId);
                        deletePhotosCommand.ExecuteNonQuery();

                        // Delete products associated with the solution
                        var deleteProductsCommand = new MySqlCommand(@"DELETE FROM SolutionProduct WHERE SolutionID = @SolutionID", connection, transaction);
                        deleteProductsCommand.Parameters.AddWithValue("@SolutionID", solutionId);
                        deleteProductsCommand.ExecuteNonQuery();

                        // Delete the solution itself
                        var deleteSolutionCommand = new MySqlCommand(@"DELETE FROM Solutions WHERE SolutionID = @SolutionID", connection, transaction);
                        deleteSolutionCommand.Parameters.AddWithValue("@SolutionID", solutionId);
                        deleteSolutionCommand.ExecuteNonQuery();

                        transaction.Commit();
                    }
                    catch
                    {
                        transaction.Rollback();
                        throw;
                    }
                }
            }
        }
    }
}