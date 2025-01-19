
namespace ZentechAPI.Models
{
    // Slides class added By Aymen 16/01/2025
    public class Slide
    {
        public int SlideID { get; set; } 
        public string Description { get; set; }
        public IFormFile Picture { get; set; } 
        public string EntityType { get; set; }
        public int EntityID { get; set; }
        public string CreatedBy { get;  set; }
        public DateTime CreatedAt { get;  set; }
        public string UpdatedBy { get;  set; }
        public DateTime UpdatedAt { get;  set; }
    }
}
