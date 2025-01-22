namespace ZentechAPI.Models
{
    public class Solution
    {
        public int SolutionID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public string? CreatedBy { get; set; }
        public string? UpdatedBy { get; set; }

        public string? MainPicture { get; set; }



        public List<string> Photos { get; set; } 
        public List<SolutionProduct> SolutionProducts { get; set; }  // Lien vers les produits d'application
    }
}
