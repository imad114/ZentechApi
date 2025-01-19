using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using ZentechAPI.Models;

namespace Zentech.Repositories
{
    public class SlidesRepository
    {
        private readonly DatabaseContext _context;

        public SlidesRepository(DatabaseContext context)
        {
            _context = context;
        }

        // Method to retrieve all slides
        public List<Slide> GetAllSlides()
        {
            var slideList = new List<Slide>();

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM slides", connection);

                using (var reader = command.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var slide = new Slide
                        {
                            SlideID = reader.GetInt32("SlideID"),
                            Description = reader.GetString("Description"),
                            PicturePath = reader.IsDBNull(reader.GetOrdinal("Picture")) ? "./images/zentech-logo.svg" : reader.GetString("Picture"),
                            EntityType = reader.GetString("EntityType"),
                            EntityID = reader.GetInt32("EntityID"),
                            CreatedBy = reader.GetString("CreatedBy"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedBy = reader.GetString("UpdatedBy"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt")
                        };

                        slideList.Add(slide);
                    }
                }
            }

            return slideList;
        }

        // Method to retrieve a specific slide by its ID
        public Slide GetSlideById(int id)
        {
            Slide slide = null;

            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("SELECT * FROM slides WHERE SlideID = @SlideID", connection);
                command.Parameters.AddWithValue("@SlideID", id);

                using (var reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        slide = new Slide
                        {
                            SlideID = reader.GetInt32("SlideID"),
                            Description = reader.GetString("Description"),
                            PicturePath = reader.IsDBNull(reader.GetOrdinal("Picture")) ? "./images/zentech-logo.svg" : reader.GetString("Picture"),
                            EntityType = reader.GetString("EntityType"),
                            EntityID = reader.GetInt32("EntityID"),
                            CreatedBy = reader.GetString("CreatedBy"),
                            CreatedAt = reader.GetDateTime("CreatedAt"),
                            UpdatedBy = reader.GetString("UpdatedBy"),
                            UpdatedAt = reader.GetDateTime("UpdatedAt")
                        };
                    }
                }
            }

            return slide;
        }

        // Method to add a new slide
        public int AddSlide(Slide Slide)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    INSERT INTO slides (Description, EntityType, EntityID, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt) 
                    VALUES (@Description, @EntityType, @EntityID, @CreatedBy, @CreatedAt, @UpdatedBy, @UpdatedAt); 
                    SELECT LAST_INSERT_ID();", connection);

                command.Parameters.AddWithValue("@Description", Slide.Description);
/*                command.Parameters.AddWithValue("@Picture", Slide.Picture ?? "./images/zentech-logo.svg");
*/                command.Parameters.AddWithValue("@EntityType", Slide.EntityType);
                command.Parameters.AddWithValue("@EntityID", Slide.EntityID);
                command.Parameters.AddWithValue("@CreatedBy", Slide.CreatedBy);
                command.Parameters.AddWithValue("@CreatedAt", Slide.CreatedAt); 
                command.Parameters.AddWithValue("@UpdatedBy", Slide.UpdatedBy);
                command.Parameters.AddWithValue("@UpdatedAt", Slide.UpdatedAt); 

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        // Method to update a slide
        public bool UpdateSlide(Slide Slide)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(@"
                    UPDATE slides 
                    SET Description = @Description, Picture = @Picture, 
                        EntityType = @EntityType, EntityID = @EntityID,
                        UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt
                    WHERE SlideID = @SlideID", connection);

                command.Parameters.AddWithValue("@Description", Slide.Description);
                command.Parameters.AddWithValue("@Picture", Path.Combine("uploads","Slides",Slide.Picture.FileName) ?? "./images/zentech-logo.svg");
               command.Parameters.AddWithValue("@EntityType", Slide.EntityType);
                command.Parameters.AddWithValue("@EntityID", Slide.EntityID);
                command.Parameters.AddWithValue("@UpdatedBy", Slide.UpdatedBy);
                command.Parameters.AddWithValue("@UpdatedAt", Slide.UpdatedAt); 
                command.Parameters.AddWithValue("@SlideID", Slide.SlideID);

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public bool UpdateSlidePicture(int SlideID, string path)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(@"
                    UPDATE slides 
                    SET Picture = @Picture where slideID =  @SlideID", connection);


                command.Parameters.AddWithValue("@Picture", path ?? "./images/zentech-logo.svg");
      
                command.Parameters.AddWithValue("@SlideID", SlideID);

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        // Method to remove a slide
        public bool RemoveSlide(int slideID)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM slides WHERE SlideID = @SlideID", connection);
                command.Parameters.AddWithValue("@SlideID", slideID);

                var rowsAffected = command.ExecuteNonQuery();
                return rowsAffected > 0;
            }
        }

        public void DeleteSlide(int slideId)
        {
        }
    }
}
