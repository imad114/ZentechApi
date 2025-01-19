using Zentech.Repositories;
using ZentechAPI.Models;

public class SlidesService
{
    private readonly SlidesRepository _slidesRepository;

    public SlidesService(SlidesRepository slidesRepository)
    {
        _slidesRepository = slidesRepository;
    }

    // Method to retrieve all slides
    public List<Slide> GetAllSlides()
    {
        return _slidesRepository.GetAllSlides();
    }

    // Method to retrieve a slide by its ID
    public Slide GetSlideById(int slideId)
    {
        return _slidesRepository.GetSlideById(slideId);
    }

    // Method to add a new slide
    public int AddSlide(Slide slide)
    {
        return _slidesRepository.AddSlide(slide);
    }

    // Method to update an existing slide
    public bool UpdateSlide(Slide slide)
    {
        return _slidesRepository.UpdateSlide(slide);
    }

    public bool UpdateSlidePicture(int slideID, string picturePath)
    {
        return _slidesRepository.UpdateSlidePicture(slideID, picturePath);
    }

    // Method to delete a slide
    public void DeleteSlide(int slideId)
    {
        _slidesRepository.DeleteSlide(slideId);
    }

}
