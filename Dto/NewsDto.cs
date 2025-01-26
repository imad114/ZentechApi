namespace ZentechAPI.Dto
{
    public class NewsDto
    {
        public int NewsID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string? Author { get; set; }
        public string? mainPicture { get; set; }
        public int? categoryId { get; set; }
    }
}
