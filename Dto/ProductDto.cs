﻿using ZentechAPI.Models;

namespace ZentechAPI.Dto
{
    public class ProductDto
    {
        public int ProductID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal? Price { get; set; }
        public int CategoryID { get; set; }
        public string? CategoryName { get; set; }
        public List<string>? Photos { get; set; }
        public List<SpecificationDto>? Specifications { get; set; }

        public string? MainPicture { get; set; }

    }
}
