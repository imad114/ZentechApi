using System.Collections.Concurrent;
using Microsoft.AspNetCore.Mvc;
using MySql.Data.MySqlClient;
using ZentechAPI.Models;

namespace ZentechAPI.Repositories
{
    public class OtherCategoriesRepository
    {
        private readonly DatabaseContext _context;
        public OtherCategoriesRepository(DatabaseContext context)
        {
            _context = context;
        }


        #region NewsCategories methods
        public ConcurrentBag<Other_Category> GetOtherCategories(string _Type)
        {
           
            var categoryList = new ConcurrentBag<Other_Category>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM other_categories where CategoryType = @CategoryType ", connection);
                command.Parameters.AddWithValue("@CategoryType", _Type);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var categorie = new Other_Category
                        {
                            CategoryID = reader.GetInt32("CategoryID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("description"),
                            CreatedDate = reader.IsDBNull(reader.GetOrdinal("CreatedDate")) ? (DateTime?)null : reader.GetDateTime("CreatedDate"),
                            UpdatedDate = reader.IsDBNull(reader.GetOrdinal("UpdatedDate")) ? (DateTime?)null : reader.GetDateTime("UpdatedDate"),
                            CreatedBy = reader.IsDBNull(reader.GetOrdinal("CreatedBy")) ? null : reader.GetString("CreatedBy"),
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy")

                        };
                        categoryList.Add(categorie);
                    }
                }
               // connection.Close();
            }
            return categoryList;
        }

        // update by imad 26/01/2025 8:37 modify id to int 
        public Other_Category AddOtherCategory(Other_Category category, string _Type)
        {


            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"insert into other_categories (name, description,categoryType, createdDate, createdBy) 
                                            values(@name,@description, @CategoryType, @createdDate, @createdBy);
                                        SELECT LAST_INSERT_ID();",
                    connection);

                command.Parameters.AddWithValue("@name", category.Name);
                command.Parameters.AddWithValue("@description", category.Description);
                command.Parameters.AddWithValue("@createdDate", DateTime.Now);
                command.Parameters.AddWithValue("@createdBy", category.CreatedBy);
                command.Parameters.AddWithValue("@CategoryType", _Type);



                int categoryId = Convert.ToInt32(command.ExecuteScalar());
                category.CategoryID = categoryId;
                connection.Close();
                return category;

            }
        }
        // update by imad 26/01/2025 8:37 modify id to int 
        public Other_Category UpdateOtherCategory(Other_Category category, string _Type)
        {

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"update other_categories set Name = @Name
                    , description = @Description , updatedDate = @updatedDate , updatedBy = @updatedBy 
                    where  CategoryID = @ID and CategoryType = @CategoryType ;
                     SELECT LAST_INSERT_ID();   ", connection);

                command.Parameters.AddWithValue("@Name", category.Name);
                command.Parameters.AddWithValue("@Description", category.Description);
                command.Parameters.AddWithValue("@ID", category.CategoryID);
                command.Parameters.AddWithValue("@updatedDate", DateTime.Now);
                command.Parameters.AddWithValue("@updatedBy", category.UpdatedBy);
                command.Parameters.AddWithValue("@CategoryType", _Type);



                int result = Convert.ToInt32(command.ExecuteScalar());
                category.CategoryID = result;
                connection.Close();

                return category;

            }
        }
        public int DeleteOtherCategory(string id,string _Type)
        {


            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("delete from other_categories where CategoryID = @ID and CategoryType = @CategoryType", connection);

                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@CategoryType", _Type);


                int result = command.ExecuteNonQuery();
                connection.Close();

                return result;

            }
        }
        #endregion
    }
}
