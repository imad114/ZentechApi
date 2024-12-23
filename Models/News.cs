﻿namespace ZentechAPI.Models
{
    public class News
    {
        public int NewsID { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public DateTime? CreatedAt { get; set; }
        public string Author { get; set; }
        public List<String>? Photos { get; set; }
    }

}
