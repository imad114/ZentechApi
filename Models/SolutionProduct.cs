using ZentechAPI.Dto;

namespace ZentechAPI.Models
{
    public class SolutionProduct
    {
        public int SolutionProductID { get; set; }
        public int SolutionID { get; set; }  // Clé étrangère vers Solution
        public int ProductID { get; set; }  // Clé étrangère vers Product

        public Solution Solution { get; set; }  // Relation avec Solution
        public ProductDto Product { get; set; }  // Relation avec Product
    }
}
