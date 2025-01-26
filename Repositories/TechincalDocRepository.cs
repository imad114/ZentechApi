using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using System.Collections.Concurrent;
using ZentechAPI.Dto;
using ZentechAPI.Models;

namespace ZentechAPI.Repositories
{
    public class TechincalDocRepository
    {
        private readonly DatabaseContext _context;
        private OtherCategoriesRepository _otherCategoriesRepository;

        public TechincalDocRepository(DatabaseContext context)
        {
            _context = context;
            _otherCategoriesRepository = new OtherCategoriesRepository(context);
        }

        // Method to get all news
        public List<TechincalDoc> GetAllTechnicalDocs(int limit)
        {
            var technicalDocList = new List<TechincalDoc>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand($"SELECT * FROM technical_documentations limit {limit}", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        
                            var technicalDoc = new TechincalDoc
                            {
                                TD_ID =  reader.GetString("TD_ID"),
                                Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "NULL" : reader.GetString("name"),
                                filePath = reader.IsDBNull(reader.GetOrdinal("filePath")) ? "NULL" : reader.GetString("filePath"),
                                CreateDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? "NULL" : reader.GetString("CreatedDate"),
                                CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? "NULL" : reader.GetString("CreatedBy"),

                            };
                        
                        technicalDocList.Add(technicalDoc);
                    }
                }
            }
            return technicalDocList;
        }

        // Method to get a specific news item
        public TechincalDoc GetTechnicalDocByID(string id)
        {
            TechincalDoc technicalDoc = new TechincalDoc();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM technical_documentations WHERE TD_ID = @TD_ID", connection);
                command.Parameters.AddWithValue("@TD_ID", id);
                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        technicalDoc = new TechincalDoc
                        {
                            TD_ID = reader.GetString("TD_ID"),
                            Name = reader.GetString("name"),
                            filePath = reader.GetString("filePath"),
                            CreateDate = reader.GetString("UpdatedDate"),
                            CreatedBy = reader.GetString("CreatedBy"),
                            UpdatedBy = reader.GetString("UpdatedBy"),
                            UpdatedDate = reader.GetString("UpdatedDate"),
                        };
                    }
                }
                connection.Close();
            }
            return technicalDoc;
        }

        // Method to add a new news item
        public int AddTechnicalDoc(TechincalDoc technicalDoc,string createdBy)
        {

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(
                    "INSERT INTO technical_documentations (name, filePath,td_category_id, createdBy) VALUES (@Name, @FilePath, @td_category_id, @CreatedBy);" +
                    " SELECT LAST_INSERT_ID();",
                    connection
                );

                command.Parameters.AddWithValue("@Name", technicalDoc.Name);
                command.Parameters.AddWithValue("@FilePath", technicalDoc.filePath);
                command.Parameters.AddWithValue("@td_category_id", technicalDoc.TD_CategoryID);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);

                var TD_Id = Convert.ToInt32(command.ExecuteScalar());
                connection.Close();
                return TD_Id;

            }

        }


        // Method to update a news article and its photos
        public bool UpdateTechnicalDoc(TechincalDoc technicalDoc)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(
                    "UPDATE technical_documentations SET name = @Name, filePath= @file, td_category_id = @catID , updatedDate = @updateDate , updatedBy = @updatedBy WHERE TD_ID = @TD_ID",
                    connection
                );
                command.Parameters.AddWithValue("@TD_ID", technicalDoc.TD_ID);
                command.Parameters.AddWithValue("@Name", technicalDoc.Name);
                command.Parameters.AddWithValue("@file", technicalDoc.filePath);
                command.Parameters.AddWithValue("@catID", technicalDoc.TD_CategoryID);
                command.Parameters.AddWithValue("@updateDate", technicalDoc.UpdatedDate);
                command.Parameters.AddWithValue("@updatedBy", technicalDoc.UpdatedBy);


                var rowsAffected = command.ExecuteNonQuery();
                connection.Close();

                return (rowsAffected != 0);

            }
        }

        /// <summary>
        /// 
        /// Delete technicalDocumentation by ID
        /// </summary>
        /// <param name="ID"></param>
        public void DeleteTechnicalDoc(int ID)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var deleteCMD = new MySqlCommand("DELETE FROM technical_documentations WHERE TD_ID = @ID", connection);
                deleteCMD.Parameters.AddWithValue("@ID", ID);
                deleteCMD.ExecuteNonQuery();

                connection.Close();
            }
        }

        public List<TechincalDoc> GetAllTechnicalDocsWithCategories(int limit)
        {
            var technicalDocList = new List<TechincalDoc>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand($@"WITH RankedDocuments AS (
                SELECT   t.TD_ID as TD_ID, t.Name as 'name', t.filePath as filePath, tc.CategoryID as 'CategoryID', tc.name as 'Category',
                   ROW_NUMBER() OVER(PARTITION BY tc.CategoryID ORDER BY tc.CreatedDate DESC) AS RowNum
                   FROM technical_documentations t LEFT JOIN other_categories tc ON t.td_category_id = tc.CategoryID and tc.CategoryType = 'TD'
                )SELECT* FROM RankedDocuments WHERE RowNum <= {limit} ORDER BY RankedDocuments.CategoryID ASC ;", connection);
                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {

                        var technicalDoc = new TechincalDoc
                        {
                            TD_ID = reader.GetString("TD_ID"),
                            Name = reader.IsDBNull(reader.GetOrdinal("name")) ? "NULL" : reader.GetString("name"),
                            filePath = reader.IsDBNull(reader.GetOrdinal("filePath")) ? "NULL" : reader.GetString("filePath"),
                            TD_CategoryName = reader.IsDBNull(reader.GetOrdinal("Category")) ? "NULL" : reader.GetString("Category"),
                            TD_CategoryID = reader.IsDBNull(reader.GetOrdinal("CategoryID")) ? "NULL" : reader.GetString("CategoryID"),

                        };

                        technicalDocList.Add(technicalDoc);
                    }
                }
            }
            return technicalDocList;
        }


        #region TDCategories methods
        public async Task<ConcurrentBag<Other_Category>> GetTDCategories()
        {
            return await Task.Run(() => _otherCategoriesRepository.GetOtherCategories("TD"));

        }
        
        public Other_Category AddTDCategory(Other_Category category)
        {


            return _otherCategoriesRepository.AddOtherCategory(category, "TD");

        }
        public Other_Category UpdateTDCategory(Other_Category category)
        {

            return _otherCategoriesRepository.UpdateOtherCategory(category, "TD");

        }
        public int DeleteTDCategory(string id)
        {
            return _otherCategoriesRepository.DeleteOtherCategory(id, "TD");
        }
        #endregion
    }
}
