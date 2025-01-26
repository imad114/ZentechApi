namespace ZentechAPI.Models
{
    public class News
    {
        public string? mainPicture { get; set; }

        public int NewsID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }
        public string? Author { get; set; }
        public List<String> Photos { get; set; }
        public int? categoryId { get; set; }
    }

}
