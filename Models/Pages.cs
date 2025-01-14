using System;

namespace ZentechAPI.Models
{
    public class Pages
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Content { get; set; }
        public string MetaTitle { get; set; }
        public string MetaDescription { get; set; }
        public string LanguageCode { get; set; }
        public string Status { get; set; } // 'Draft', 'Published', 'Archived'
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public string CreatedBy { get; set; }
        public string UpdatedBy { get; set; }
        public int VisitorCount { get; set; }
    }
}