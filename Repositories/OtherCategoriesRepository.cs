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
        public List<Other_Category> GetOtherCategories(string _Type)
        {
            List<Other_Category> categories = new List<Other_Category>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT CategoryID, Name, description  FROM other_categories where CategoryType = @CategoryType ", connection);
                command.Parameters.AddWithValue("@CategoryType", _Type);


                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        categories.Add(new Other_Category()
                        {
                            CategoryID = reader.GetString("CategoryID"),
                            Name = reader.GetString("Name"),
                            Description = reader.GetString("description"),

                        });
                    }
                }
                connection.Close();
            }
            return categories;
        }
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
                category.CategoryID = categoryId.ToString();
                connection.Close();
                return category;

            }
        }
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
                category.CategoryID = result.ToString();
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
