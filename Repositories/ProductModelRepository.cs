using MySql.Data.MySqlClient;
using System.Text;
using ZentechAPI.Models;

namespace ZentechAPI.Repositories
{
    public class ProductModelRepository
    {
        private readonly DatabaseContext _context;

        public ProductModelRepository(DatabaseContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retrieves all models.
        /// </summary>
        /// <returns>A list of models.</returns>
        public List<ProductModel> GetAllModels(int offset , int limit)
        {
            var modelList = new List<ProductModel>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand($"SELECT * FROM productmodel limit {limit} offset {offset}", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var model = new ProductModel
                        {
                            ProductID = reader.IsDBNull(reader.GetOrdinal("productID")) ? 0 : reader.GetInt32(reader.GetOrdinal("productID")),
                            ModelId = reader.GetInt32(reader.GetOrdinal("ID")),
                            Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? "N/A" : reader.GetString(reader.GetOrdinal("Model")),
                            Displacement = reader.IsDBNull(reader.GetOrdinal("Displacement")) ? "N/A" : reader.GetString(reader.GetOrdinal("Displacement")),
                            CoolingType = reader.IsDBNull(reader.GetOrdinal("CoolingType")) ? "N/A" : reader.GetString(reader.GetOrdinal("CoolingType")),
                            MotorType = reader.IsDBNull(reader.GetOrdinal("MotorType")) ? "N/A" : reader.GetString(reader.GetOrdinal("MotorType")),
                            VoltageFrequency = reader.IsDBNull(reader.GetOrdinal("VoltageFrequency")) ? "N/A" : reader.GetString(reader.GetOrdinal("VoltageFrequency")),
                            CoolingCapacityW = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityW")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityW")),
                            CoolingCapacityBTUPerHour = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityBTUPerHour")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityBTUPerHour")),
                            CoolingCapacityKcal = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityKcal")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityKcal")),
                            COPWW = reader.IsDBNull(reader.GetOrdinal("COPWW")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPWW")),
                            COPBTUPerWH = reader.IsDBNull(reader.GetOrdinal("COPBTUPerWH")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPBTUPerWH")),
                            CreateDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdateDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
                        };

                        modelList.Add(model);
                    }
                }
            }

            return modelList;
        }

        /// <summary>
        /// Retrieves a model by its ID.
        /// </summary>
        /// <param name="id">Model ID.</param>
        /// <returns>The model object or null if not found.</returns>
        public ProductModel GetModelById(int id)
        {
            ProductModel? model = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM productmodel WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new ProductModel
                        {
                            ProductID = reader.IsDBNull(reader.GetOrdinal("productID")) ? 0 : reader.GetInt32(reader.GetOrdinal("productID")),
                            ModelId = reader.GetInt32(reader.GetOrdinal("ID")),
                            Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? "N/A" : reader.GetString(reader.GetOrdinal("Model")),
                            Displacement = reader.IsDBNull(reader.GetOrdinal("Displacement")) ? "N/A" : reader.GetString(reader.GetOrdinal("Displacement")),
                            CoolingType = reader.IsDBNull(reader.GetOrdinal("CoolingType")) ? "N/A" : reader.GetString(reader.GetOrdinal("CoolingType")),
                            MotorType = reader.IsDBNull(reader.GetOrdinal("MotorType")) ? "N/A" : reader.GetString(reader.GetOrdinal("MotorType")),
                            VoltageFrequency = reader.IsDBNull(reader.GetOrdinal("VoltageFrequency")) ? "N/A" : reader.GetString(reader.GetOrdinal("VoltageFrequency")),
                            CoolingCapacityW = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityW")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityW")),
                            CoolingCapacityBTUPerHour = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityBTUPerHour")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityBTUPerHour")),
                            CoolingCapacityKcal = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityKcal")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityKcal")),
                            COPWW = reader.IsDBNull(reader.GetOrdinal("COPWW")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPWW")),
                            COPBTUPerWH = reader.IsDBNull(reader.GetOrdinal("COPBTUPerWH")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPBTUPerWH")),
                            CreateDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdateDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
                        };
                    }
                }
            }

            return model;
        }

        public List<ProductModel> GetModelsByProductId(int ProdId, int offset, int limit)
        {
            List<ProductModel> models = new List<ProductModel>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand($"SELECT * FROM productmodel WHERE productID = @Id  limit {limit} offset {offset}", connection);
                command.Parameters.AddWithValue("@Id", ProdId);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ProductModel model = new ProductModel
                        {
                            ProductID = reader.IsDBNull(reader.GetOrdinal("productID")) ? 0 : reader.GetInt32(reader.GetOrdinal("productID")),
                            ModelId = reader.GetInt32(reader.GetOrdinal("ID")),
                            Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? "N/A" : reader.GetString(reader.GetOrdinal("Model")),
                            Displacement = reader.IsDBNull(reader.GetOrdinal("Displacement")) ? "N/A" : reader.GetString(reader.GetOrdinal("Displacement")),
                            CoolingType = reader.IsDBNull(reader.GetOrdinal("CoolingType")) ? "N/A" : reader.GetString(reader.GetOrdinal("CoolingType")),
                            MotorType = reader.IsDBNull(reader.GetOrdinal("MotorType")) ? "N/A" : reader.GetString(reader.GetOrdinal("MotorType")),
                            VoltageFrequency = reader.IsDBNull(reader.GetOrdinal("VoltageFrequency")) ? "N/A" : reader.GetString(reader.GetOrdinal("VoltageFrequency")),
                            CoolingCapacityW = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityW")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityW")),
                            CoolingCapacityBTUPerHour = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityBTUPerHour")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityBTUPerHour")),
                            CoolingCapacityKcal = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityKcal")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityKcal")),
                            COPWW = reader.IsDBNull(reader.GetOrdinal("COPWW")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPWW")),
                            COPBTUPerWH = reader.IsDBNull(reader.GetOrdinal("COPBTUPerWH")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPBTUPerWH")),
                            CreateDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdateDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
                        };

                        models.Add(model);
                    }
                }
            }

            return models;
        }

        /// <summary>
        /// Adds a new model.
        /// </summary>
        /// <param name="model">Model data.</param>
        /// <param name="createdBy">User who created the model.</param>
        /// <returns>The ID of the newly created model.</returns>
        public int AddModel(ProductModel model, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    INSERT INTO productmodel (Model, Displacement, CoolingType, MotorType, VoltageFrequency, CoolingCapacityW, CoolingCapacityBTUPerHour, CoolingCapacityKcal, COPWW, COPBTUPerWH, CreatedBy, CreatedDate, productID) 
                    VALUES (@Model, @Displacement, @CoolingType, @MotorType, @VoltageFrequency, @CoolingCapacityW, @CoolingCapacityBTUPerHour, @CoolingCapacityKcal, @COPWW, @COPBTUPerWH, @CreatedBy, @CreatedDate, @ProductID); 
                    SELECT LAST_INSERT_ID();", connection);

                command.Parameters.AddWithValue("@Model", model.Model);
                command.Parameters.AddWithValue("@Displacement", model.Displacement);
                command.Parameters.AddWithValue("@CoolingType", model.CoolingType);
                command.Parameters.AddWithValue("@MotorType", model.MotorType);
                command.Parameters.AddWithValue("@VoltageFrequency", model.VoltageFrequency);
                command.Parameters.AddWithValue("@CoolingCapacityW", model.CoolingCapacityW);
                command.Parameters.AddWithValue("@CoolingCapacityBTUPerHour", model.CoolingCapacityBTUPerHour);
                command.Parameters.AddWithValue("@CoolingCapacityKcal", model.CoolingCapacityKcal);
                command.Parameters.AddWithValue("@COPWW", model.COPWW);
                command.Parameters.AddWithValue("@COPBTUPerWH", model.COPBTUPerWH);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
                command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@ProductID", model.ProductID);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }


        /// <summary>
        /// Adds multiple models to the database.
        /// </summary>
        /// <param name="models">List of models to add.</param>
        /// <returns>The number of models successfully added.</returns>
        public async Task<int> AddMultipleProducts(List<ProductModel> models)
        {
            int recordsInserted = 0;

            using (var connection = _context.GetConnection())
            {
                await connection.OpenAsync();

                foreach (var model in models)
                {
                    var command = new MySqlCommand(@"
                INSERT INTO productmodel (Model, Displacement, CoolingType, MotorType, VoltageFrequency, CoolingCapacityW, CoolingCapacityBTUPerHour, CoolingCapacityKcal, COPWW, COPBTUPerWH, CreatedBy, CreatedDate, ProductID) 
                VALUES (@Model, @Displacement, @CoolingType, @MotorType, @VoltageFrequency, @CoolingCapacityW, @CoolingCapacityBTUPerHour, @CoolingCapacityKcal, @COPWW, @COPBTUPerWH, @CreatedBy, @CreatedDate, @ProductID); 
                SELECT LAST_INSERT_ID();", connection);

                    command.Parameters.AddWithValue("@Model", model.Model);
                    command.Parameters.AddWithValue("@Displacement", model.Displacement);
                    command.Parameters.AddWithValue("@CoolingType", model.CoolingType);
                    command.Parameters.AddWithValue("@MotorType", model.MotorType);
                    command.Parameters.AddWithValue("@VoltageFrequency", model.VoltageFrequency);
                    command.Parameters.AddWithValue("@CoolingCapacityW", model.CoolingCapacityW);
                    command.Parameters.AddWithValue("@CoolingCapacityBTUPerHour", model.CoolingCapacityBTUPerHour);
                    command.Parameters.AddWithValue("@CoolingCapacityKcal", model.CoolingCapacityKcal);
                    command.Parameters.AddWithValue("@COPWW", model.COPWW);
                    command.Parameters.AddWithValue("@COPBTUPerWH", model.COPBTUPerWH);
                    command.Parameters.AddWithValue("@CreatedBy", model.CreatedBy);
                    command.Parameters.AddWithValue("@CreatedDate", DateTime.Now);
                    command.Parameters.AddWithValue("@ProductID", model.ProductID);

                    // Execute the insert and get the last inserted ID
                    var result = await command.ExecuteScalarAsync();
                    if (result != null)
                    {
                        recordsInserted++;
                    }
                }
            }

            return recordsInserted;
        }


        /// <summary>
        /// Updates a model's details.
        /// </summary>
        /// <param name="model">Model object with updated data.</param>
        /// <param name="updatedBy">User who updated the model.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public bool UpdateModel(ProductModel model, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var query = @"
                    UPDATE productmodel 
                    SET Model = @Model, Displacement = @Displacement, CoolingType = @CoolingType, MotorType = @MotorType, VoltageFrequency = @VoltageFrequency, CoolingCapacityW = @CoolingCapacityW, 
                        CoolingCapacityBTUPerHour = @CoolingCapacityBTUPerHour, CoolingCapacityKcal = @CoolingCapacityKcal,
                        COPWW = @COPWW, COPBTUPerWH = @COPBTUPerWH, UpdatedBy = @UpdatedBy, UpdatedDate = @UpdatedDate , productID = @productID where ID = @ID";


                var command = new MySqlCommand(query, connection);
                command.Parameters.AddWithValue("@Model", model.Model);
                command.Parameters.AddWithValue("@Displacement", model.Displacement);
                command.Parameters.AddWithValue("@CoolingType", model.CoolingType);
                command.Parameters.AddWithValue("@MotorType", model.MotorType);
                command.Parameters.AddWithValue("@VoltageFrequency", model.VoltageFrequency);
                command.Parameters.AddWithValue("@CoolingCapacityW", model.CoolingCapacityW);
                command.Parameters.AddWithValue("@CoolingCapacityBTUPerHour", model.CoolingCapacityBTUPerHour);
                command.Parameters.AddWithValue("@CoolingCapacityKcal", model.CoolingCapacityKcal);
                command.Parameters.AddWithValue("@COPWW", model.COPWW);
                command.Parameters.AddWithValue("@COPBTUPerWH", model.COPBTUPerWH);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@UpdatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@productID", model.ProductID);
                command.Parameters.AddWithValue("@ID", model.ModelId);


                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Deletes a model by ID.
        /// </summary>
        /// <param name="modelID">Model ID.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveModel(int modelID)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM productmodel WHERE Id = @Id", connection);
                command.Parameters.AddWithValue("@Id", modelID);

                return command.ExecuteNonQuery() > 0;
            }
        }


        /// <summary>
        /// Retrieves a product model by name and product ID.
        /// </summary>
        /// <param name="productId">The ID of the product.</param>
        /// <param name="modelName">The name of the model.</param>
        /// <returns>The product model if found, otherwise null.</returns>
        public ProductModel? GetProductModelByNameAndProductId(int productId, string modelName)
        {
            ProductModel? model = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM productmodel WHERE productID = @ProductId AND Model = @ModelName", connection);
                command.Parameters.AddWithValue("@ProductId", productId);
                command.Parameters.AddWithValue("@ModelName", modelName);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        model = new ProductModel
                        {
                            ProductID = reader.IsDBNull(reader.GetOrdinal("productID")) ? 0 : reader.GetInt32(reader.GetOrdinal("productID")),
                            ModelId = reader.GetInt32(reader.GetOrdinal("ID")),
                            Model = reader.IsDBNull(reader.GetOrdinal("Model")) ? "N/A" : reader.GetString(reader.GetOrdinal("Model")),
                            Displacement = reader.IsDBNull(reader.GetOrdinal("Displacement")) ? "N/A" : reader.GetString(reader.GetOrdinal("Displacement")),
                            CoolingType = reader.IsDBNull(reader.GetOrdinal("CoolingType")) ? "N/A" : reader.GetString(reader.GetOrdinal("CoolingType")),
                            MotorType = reader.IsDBNull(reader.GetOrdinal("MotorType")) ? "N/A" : reader.GetString(reader.GetOrdinal("MotorType")),
                            VoltageFrequency = reader.IsDBNull(reader.GetOrdinal("VoltageFrequency")) ? "N/A" : reader.GetString(reader.GetOrdinal("VoltageFrequency")),
                            CoolingCapacityW = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityW")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityW")),
                            CoolingCapacityBTUPerHour = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityBTUPerHour")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityBTUPerHour")),
                            CoolingCapacityKcal = reader.IsDBNull(reader.GetOrdinal("CoolingCapacityKcal")) ? 0 : reader.GetDouble(reader.GetOrdinal("CoolingCapacityKcal")),
                            COPWW = reader.IsDBNull(reader.GetOrdinal("COPWW")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPWW")),
                            COPBTUPerWH = reader.IsDBNull(reader.GetOrdinal("COPBTUPerWH")) ? 0 : reader.GetDouble(reader.GetOrdinal("COPBTUPerWH")),
                            CreateDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdateDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("UpdatedBy")),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? string.Empty : reader.GetString(reader.GetOrdinal("CreatedBy"))
                        };
                    }
                }
            }

            return model;
        }

    }
}
