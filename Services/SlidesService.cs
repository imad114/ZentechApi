using Zentech.Repositories;
using ZentechAPI.Models;
using System;
using System.Collections.Generic;

public class SlidesService
{
    private readonly SlidesRepository _slidesRepository;

    public SlidesService(SlidesRepository slidesRepository)
    {
        _slidesRepository = slidesRepository ?? throw new ArgumentNullException(nameof(slidesRepository));
    }

    /// <summary>
    /// Retrieves all slides.
    /// </summary>
    /// <returns>List of slides.</returns>
    public List<Slide> GetAllSlides()
    {
        try
        {
            return _slidesRepository.GetAllSlides();
        }
        catch (Exception ex)
        {
            // Log the exception (add a logger if available)
            throw new Exception("An error occurred while retrieving slides.", ex);
        }
    }

    /// <summary>
    /// Retrieves a slide by its ID.
    /// </summary>
    /// <param name="slideId">Slide ID.</param>
    /// <returns>Slide object or null if not found.</returns>
    public Slide GetSlideById(int slideId)
    {
        if (slideId <= 0)
        {
            throw new ArgumentException("Slide ID must be greater than zero.", nameof(slideId));
        }

        try
        {
            return _slidesRepository.GetSlideById(slideId) ?? throw new KeyNotFoundException("Slide not found.");
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"An error occurred while retrieving the slide with ID {slideId}.", ex);
        }
    }

    /// <summary>
    /// Adds a new slide.
    /// </summary>
    /// <param name="slide">Slide data.</param>
    /// <param name="createdBy">User who creates the slide.</param>
    /// <returns>ID of the newly created slide.</returns>
    public int AddSlide(Slide slide, string createdBy)
    {
        if (slide == null)
        {
            throw new ArgumentNullException(nameof(slide), "Slide data cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(createdBy))
        {
            throw new ArgumentException("CreatedBy cannot be null or empty.", nameof(createdBy));
        }

        try
        {
            return _slidesRepository.AddSlide(slide, createdBy);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception("An error occurred while adding the slide.", ex);
        }
    }

    /// <summary>
    /// Updates an existing slide.
    /// </summary>
    /// <param name="slide">Updated slide data.</param>
    /// <param name="updatedBy">User who updates the slide.</param>
    /// <returns>True if the update was successful, false otherwise.</returns>
    public bool UpdateSlide(Slide slide, string updatedBy)
    {
        if (slide == null)
        {
            throw new ArgumentNullException(nameof(slide), "Slide data cannot be null.");
        }

        if (string.IsNullOrWhiteSpace(updatedBy))
        {
            throw new ArgumentException("UpdatedBy cannot be null or empty.", nameof(updatedBy));
        }

        try
        {
            return _slidesRepository.UpdateSlide(slide, updatedBy);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"An error occurred while updating the slide with ID {slide.SlideID}.", ex);
        }
    }

    /// <summary>
    /// Updates the picture of a slide.
    /// </summary>
    /// <param name="slideID">Slide ID.</param>
    /// <param name="picturePath">New picture path.</param>
    /// <returns>True if successful, false otherwise.</returns>
    public bool UpdateSlidePicture(int slideID, string picturePath)
    {
        if (slideID <= 0)
        {
            throw new ArgumentException("Slide ID must be greater than zero.", nameof(slideID));
        }

        if (string.IsNullOrWhiteSpace(picturePath))
        {
            throw new ArgumentException("Picture path cannot be null or empty.", nameof(picturePath));
        }

        try
        {
            return _slidesRepository.UpdateSlidePicture(slideID, picturePath);
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"An error occurred while updating the picture for slide ID {slideID}.", ex);
        }
    }

    /// <summary>
    /// Deletes a slide by its ID.
    /// </summary>
    /// <param name="slideId">Slide ID.</param>
    public bool DeleteSlide(int slideId)
    {
        if (slideId <= 0)
        {
            throw new ArgumentException("Slide ID must be greater than zero.", nameof(slideId));
        }

        try
        {
            var success = _slidesRepository.RemoveSlide(slideId);
            return success;
            if (!success)
            {
                throw new KeyNotFoundException("Slide not found or could not be deleted.");
            }
        }
        catch (Exception ex)
        {
            // Log the exception
            throw new Exception($"An error occurred while deleting the slide with ID {slideId}.", ex);
        }
    }
}
