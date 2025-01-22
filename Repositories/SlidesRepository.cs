using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Zentech.Models;
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

        /// <summary>
        /// Retrieves all slides.
        /// </summary>
        /// <returns>A list of slides.</returns>
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
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime("UpdatedAt")
                        };

                        slideList.Add(slide);
                    }
                }
            }

            return slideList;
        }

        /// <summary>
        /// Retrieves a slide by its ID.
        /// </summary>
        /// <param name="id">Slide ID.</param>
        /// <returns>The slide object or null if not found.</returns>
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
                            UpdatedBy = reader.IsDBNull(reader.GetOrdinal("UpdatedBy")) ? null : reader.GetString("UpdatedBy"),
                            UpdatedAt = reader.IsDBNull(reader.GetOrdinal("UpdatedAt")) ? null : reader.GetDateTime("UpdatedAt")
                        };
                    }
                }
            }

            return slide;
        }

        /// <summary>
        /// Adds a new slide.
        /// </summary>
        /// <param name="slide">Slide data.</param>
        /// <param name="createdBy">User who created the slide.</param>
        /// <returns>The ID of the newly created slide.</returns>
        public int AddSlide(Slide slide, string createdBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand(@"
                    INSERT INTO slides (Description, EntityType, EntityID, CreatedBy, CreatedAt, Picture) 
                    VALUES (@Description, @EntityType, @EntityID, @CreatedBy, @CreatedAt, @Picture); 
                    SELECT LAST_INSERT_ID();", connection);

                command.Parameters.AddWithValue("@Description", slide.Description ?? string.Empty);
                command.Parameters.AddWithValue("@EntityType", slide.EntityType ?? string.Empty);
                command.Parameters.AddWithValue("@EntityID", slide.EntityID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@CreatedBy", createdBy);
                command.Parameters.AddWithValue("@CreatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@Picture", slide.Picture?.FileName ?? "./images/zentech-logo.svg");

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        /// <summary>
        /// Updates a slide's details.
        /// </summary>
        /// <param name="slide">Slide object with updated data.</param>
        /// <param name="updatedBy">User who updated the slide.</param>
        /// <returns>True if the update was successful, false otherwise.</returns>
        public bool UpdateSlide(Slide slide, string updatedBy)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var query = new StringBuilder(@"
                    UPDATE slides 
                    SET Description = @Description, EntityType = @EntityType, EntityID = @EntityID, UpdatedBy = @UpdatedBy, UpdatedAt = @UpdatedAt");

                if (slide.Picture != null)
                {
                    query.Append(", Picture = @Picture");
                }

                query.Append(" WHERE SlideID = @SlideID");

                var command = new MySqlCommand(query.ToString(), connection);
                command.Parameters.AddWithValue("@Description", slide.Description ?? string.Empty);
                command.Parameters.AddWithValue("@EntityType", slide.EntityType ?? string.Empty);
                command.Parameters.AddWithValue("@EntityID", slide.EntityID ?? (object)DBNull.Value);
                command.Parameters.AddWithValue("@UpdatedBy", updatedBy);
                command.Parameters.AddWithValue("@UpdatedAt", DateTime.Now);
                command.Parameters.AddWithValue("@SlideID", slide.SlideID);

                if (slide.Picture != null)
                {
                    command.Parameters.AddWithValue("@Picture", Path.Combine("uploads", "Slides", slide.Picture.FileName));
                }

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Updates the picture path of a slide.
        /// </summary>
        /// <param name="slideID">Slide ID.</param>
        /// <param name="path">New picture path.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool UpdateSlidePicture(int slideID, string path)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();

                var command = new MySqlCommand(@"
                    UPDATE slides 
                    SET Picture = @Picture 
                    WHERE SlideID = @SlideID", connection);

                command.Parameters.AddWithValue("@Picture", path ?? "./images/zentech-logo.svg");
                command.Parameters.AddWithValue("@SlideID", slideID);

                return command.ExecuteNonQuery() > 0;
            }
        }

        /// <summary>
        /// Deletes a slide by ID.
        /// </summary>
        /// <param name="slideID">Slide ID.</param>
        /// <returns>True if successful, false otherwise.</returns>
        public bool RemoveSlide(int slideID)
        {
            using (var connection = _context.GetConnection())
            {
                connection.Open();
                var command = new MySqlCommand("DELETE FROM slides WHERE SlideID = @SlideID", connection);
                command.Parameters.AddWithValue("@SlideID", slideID);

                return command.ExecuteNonQuery() > 0;
            }
        }
    }
}
